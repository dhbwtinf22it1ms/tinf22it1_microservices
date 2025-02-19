use axum::{
    extract::{Path, State},
    http::StatusCode,
    routing::get,
    Json, Router,
};

use crate::{
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

async fn list_theses(State(_state): State<AppState>) -> ApiResult<Vec<ThesisSummary>> {
    todo!()
}

async fn create_thesis(
    State(_state): State<AppState>,
    Json(mut payload): Json<Thesis>,
) -> ApiResult<Thesis> {

    // Example
    payload.id = Some(ThesisId(14));
    payload.student.id = Some(UserId(6));


    ApiResult::new_success(StatusCode::OK, payload)
}

async fn get_my_thesis(
    State(_state): State<AppState>,
    Path(_thesis_id): Path<ThesisId>,
) -> (StatusCode, Json<Thesis>) {
    todo!()
}

async fn update_my_thesis(
    State(_state): State<AppState>,
    Path(_thesis_id): Path<ThesisId>,
    Json(_payload): Json<Thesis>,
) -> (StatusCode, Json<Thesis>) {
    todo!()
}

async fn get_thesis_by_id(
    State(_state): State<AppState>,
    Path(_thesis_id): Path<ThesisId>,
) -> (StatusCode, Json<Thesis>) {
    todo!()
}

async fn update_thesis_by_id(
    State(_state): State<AppState>,
    Path(_thesis_id): Path<ThesisId>,
    Json(_payload): Json<Thesis>,
) -> (StatusCode, Json<Thesis>) {
    todo!()
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
