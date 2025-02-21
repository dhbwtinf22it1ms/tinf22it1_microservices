use crate::keycloak_extension::error::KeycloakExtError::*;
use crate::keycloak_extension::{self, user};
use crate::models::*;
use crate::result::UserApiResult;
use crate::AppState;
use anyhow::anyhow;
use axum::{
    extract::{Json, Path, State},
    http::StatusCode,
    response::{IntoResponse, Response},
};
use axum_macros::debug_handler;
use keycloak::KeycloakError::*;
use serde_json::{self, json};
use uuid::Uuid;

pub async fn get_all_users(State(state): State<AppState>) -> UserApiResult<Vec<User>> {
    let user_result =
        keycloak_extension::user::get_users(&state.keycloak_admin, &state.config.keycloak_realm)
            .await;

    UserApiResult::new_success(StatusCode::OK, user_result.unwrap_or(vec![]))
}

pub async fn create_users(
    State(state): State<AppState>,
    Json(users): Json<Vec<User>>,
) -> UserApiResult<Vec<String>> {
    let creation_result =
        user::create_users(&state.keycloak_admin, &state.config.keycloak_realm, &users).await;

    match creation_result {
        Ok(user_ids) => UserApiResult::new_success(StatusCode::OK, user_ids),
        Err(UserCreationFailed(first_name, last_name)) => UserApiResult::new_error(
            StatusCode::UNPROCESSABLE_ENTITY,
            format!("The user {first_name} {last_name} could not be created"),
        ),
        Err(other) => panic!("Keycloak call failed: {other:?}"),
    }
}

#[debug_handler]
pub async fn get_user_information(
    Path(id): Path<String>,
    State(state): State<AppState>,
) -> UserApiResult<User> {
    let user_result = keycloak_extension::user::get_user(
        &state.keycloak_admin,
        &state.config.keycloak_realm,
        &id.to_string(),
    )
    .await;

    match user_result {
        Ok(user) => UserApiResult::new_success(StatusCode::OK, user),
        Err(KeycloakError(HttpFailure { status, .. })) if status == 404 => {
            UserApiResult::new_error(StatusCode::NOT_FOUND, "User was not found".to_owned())
        }
        Err(other) => panic!("Keycloak call failed: {other}"),
    }
}

pub async fn update_user(
    Path(id): Path<String>,
    State(state): State<AppState>,
    Json(user): Json<User>,
) -> UserApiResult<User> {
    dbg!(&user);
    let update_result = user::update_user(
        &state.keycloak_admin,
        &state.config.keycloak_realm,
        &id,
        &user,
    )
    .await;

    match update_result {
        Ok(()) => UserApiResult::new_success(StatusCode::OK, user),
        Err(KeycloakError(HttpFailure { status, .. })) if status == 404 => {
            UserApiResult::new_error(StatusCode::NOT_FOUND, "User was not found")
        }
        Err(other) => panic!("Keycloak call failed: {other}"),
    }
}

pub async fn delete_user(
    Path(id): Path<String>,
    State(state): State<AppState>,
) -> UserApiResult<serde_json::Value> {
    let deletion_result =
        user::delete_user(&state.keycloak_admin, &state.config.keycloak_realm, &id).await;

    match deletion_result {
        Ok(_) => UserApiResult::new_success(StatusCode::NO_CONTENT, json!({})),
        Err(KeycloakError(HttpFailure { status, .. })) if status == 404 => {
            UserApiResult::new_error(StatusCode::NOT_FOUND, "User was not found")
        }
        Err(other) => panic!("Keycloak call failed: {other}"),
    }
}

pub async fn regenerate_registration_token(
    Path(id): Path<String>,
    State(state): State<AppState>,
) -> UserApiResult<RegistrationToken> {
    let new_token_result = user::regenerate_registration_token(
        &state.keycloak_admin,
        &state.config.keycloak_realm,
        &id,
    )
    .await;

    match new_token_result {
        Ok(new_token) => UserApiResult::new_success(StatusCode::OK, new_token),
        Err(KeycloakError(HttpFailure { status, .. })) if status == 404 => {
            UserApiResult::new_error(StatusCode::NOT_FOUND, "User was not found".to_owned())
        }
        Err(err) => panic!("Keycloak call failed: {err}"),
    }
}
