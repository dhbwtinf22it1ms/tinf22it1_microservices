use crate::models::*;
use crate::result::UserApiResult;
use axum::{
    http::StatusCode,
    response::{IntoResponse, Response},
    Json,
};

pub async fn get_all_users() -> UserApiResult<Vec<User>> {
    UserApiResult::success(
        StatusCode::OK,
        vec![User {
            id: Some(UserId(10)),
            registration_status: Some(RegistrationStatus::Completed),
            first_name: String::from("Test"),
            last_name: String::from("User"),
            email: String::from("test@test.de"),
            role: Role::Supervisor,
        }],
    )
}

pub async fn create_users() -> UserApiResult<Vec<User>> {
    todo!();
}

pub async fn get_user_information() -> UserApiResult<User> {
    todo!();
}

pub async fn update_user() -> UserApiResult<User> {
    todo!();
}

pub async fn delete_user() -> UserApiResult<()> {
    todo!();
}

pub async fn regenerate_registration_token() -> UserApiResult<RegistrationToken> {
    todo!();
}
