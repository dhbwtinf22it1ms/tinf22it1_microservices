use std::borrow::Cow;

use axum::{
    extract::{Path, State},
    http::StatusCode,
    routing::get,
    Json, Router,
};
use serde::de::Expected;
use tracing::warn;

use crate::{
    database::{self, DatabaseError},
    models::{Comment, Thesis, ThesisId, ThesisSummary, UserId},
    result::ApiResult,
    AppState,
};

pub fn build_router() -> Router<AppState> {
    Router::new()
        .route("/", get(list_theses).post(create_thesis))
        .route("/mine", get(get_my_thesis).put(update_my_thesis))
        .route("/{id}", get(get_thesis_by_id).put(update_thesis_by_id))
        .route("/{id}/comments", get(list_comments).post(create_comment))
}

async fn list_theses(State(state): State<AppState>) -> ApiResult<Vec<ThesisSummary>> {
    let mut database_connection = state.database_pool.get().unwrap();

    let theses =
        database::list_theses(&mut database_connection).expect("listing all theses not to fail");

    let thesis_summaries = theses.into_iter().map(ThesisSummary::from).collect();

    ApiResult::new_success(StatusCode::OK, thesis_summaries)
}

async fn create_thesis(
    State(state): State<AppState>,
    Json(mut payload): Json<Thesis>,
) -> ApiResult<Thesis> {
    let mut database_connection = state.database_pool.get().unwrap();

    // TODO retrieve student from current JWT and perform authorization
    payload.student.id = Some(UserId(999));
    warn!("No authentication yet, using student id 999 for new thesis");

    let created_thesis = match database::create_thesis(&mut database_connection, payload) {
        Ok(t) => t,
        Err(database::DatabaseError::UniqueKeyViolation) => {
            // TODO this is unreachable right now because the student id is not set as a PK in the database
            return ApiResult::new_error(
                StatusCode::CONFLICT,
                "thesis already exists for current user",
            );
        }
        Err(other) => panic!("expected database query not to fail: {other}"),
    };

    ApiResult::new_success(StatusCode::OK, created_thesis)
}

async fn get_my_thesis(State(state): State<AppState>) -> ApiResult<Thesis> {
    warn!("No authentication yet, using student id 999 for get_my_thesis");
    let student_id = UserId(999);

    let mut connection = state.database_pool.get().unwrap();

    match database::get_thesis_by_student_id(&mut connection, student_id) {
        Ok(thesis) => ApiResult::new_success(StatusCode::OK, thesis),
        Err(DatabaseError::NotFound) => return ApiResult::new_error(StatusCode::NOT_FOUND, "you do not have a thesis yet"),
        Err(other)  => panic!("unexpected database error: {other}"),
    }
}

async fn update_my_thesis(
    State(state): State<AppState>,
    Json(mut payload): Json<Thesis>,
) -> ApiResult<Thesis> {
    warn!("No authentication yet, using student id 999 for get_my_thesis");
    let student_id = UserId(999);
    let mut connection = state.database_pool.get().unwrap();

    if payload.id.is_some() {
        return ApiResult::new_error(
            StatusCode::UNPROCESSABLE_ENTITY,
            "payload must not contains a thesis id",
        );
    }

    if payload.student.id.is_some() {
        return ApiResult::new_error(
            StatusCode::UNPROCESSABLE_ENTITY,
            "payload must not contains a student id",
        );
    }

    let existing_thesis = match database::get_thesis_by_student_id(&mut connection, student_id) {
        Ok(existing_thesis) => existing_thesis,
        Err(DatabaseError::NotFound) => 
        return ApiResult::new_error(
            StatusCode::NOT_FOUND,
            "you do not have a thesis yet. create one first before updating it",
        ),
        Err(other) => panic!("unexpected database error: {other}"),
    };

    payload.id = existing_thesis.id;
    payload.student.id = existing_thesis.student.id;

    let updated_thesis =
        database::update_thesis(&mut connection, payload).expect("database call to not fail");

    ApiResult::new_success(StatusCode::OK, updated_thesis)
}

async fn get_thesis_by_id(
    State(state): State<AppState>,
    Path(thesis_id): Path<ThesisId>,
) -> ApiResult<Thesis> {
    let mut connection = state.database_pool.get().unwrap();

    match database::get_thesis_by_id(&mut connection, thesis_id) {
        Ok(thesis) => ApiResult::new_success(StatusCode::OK, thesis),
        Err(DatabaseError::NotFound) => ApiResult::new_error(StatusCode::NOT_FOUND, format!("a thesis with given id={} does not exist", thesis_id.0)),
        Err(other) => panic!("unexpected database error: {other}"),
    }
}

async fn update_thesis_by_id(
    State(state): State<AppState>,
    Path(thesis_id): Path<ThesisId>,
    Json(mut payload): Json<Thesis>,
) -> ApiResult<Thesis> {
    let mut connection = state.database_pool.get().unwrap();

    if payload.id.is_some() {
        return ApiResult::new_error(
            StatusCode::UNPROCESSABLE_ENTITY,
            "payload must not contains a thesis id",
        );
    }

    if payload.student.id.is_some() {
        return ApiResult::new_error(
            StatusCode::UNPROCESSABLE_ENTITY,
            "payload must not contains a student id",
        );
    }

    let existing_thesis = match database::get_thesis_by_id(&mut connection, thesis_id) {
        Ok(existing_thesis) => existing_thesis,
        Err(DatabaseError::NotFound) => {
            return ApiResult::new_error(
                StatusCode::NOT_FOUND,
                "you do not have a thesis yet. create one first before updating it",
            );
        }
        Err(err) => panic!("unexpected database error: {err}"),
    };

    payload.id = existing_thesis.id;
    payload.student.id = existing_thesis.student.id;

    let updated_thesis =
        database::update_thesis(&mut connection, payload).expect("database call to not fail");

    ApiResult::new_success(StatusCode::OK, updated_thesis)
}

async fn create_comment(
    State(_state): State<AppState>,
    Path(_thesis_id): Path<ThesisId>,
    Json(_payload): Json<Comment>,
) -> (StatusCode, Json<Comment>) {
    todo!()
}

async fn list_comments(
    State(_state): State<AppState>,
    Path(_thesis_id): Path<ThesisId>,
) -> (StatusCode, Json<Vec<Comment>>) {
    todo!()
}
