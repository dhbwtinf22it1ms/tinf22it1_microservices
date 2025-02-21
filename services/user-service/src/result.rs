use axum::{
    http::StatusCode,
    response::{IntoResponse, Response},
    Json,
};
use serde::Serialize;

pub struct UserApiResult<S: Serialize>(Result<(StatusCode, Json<S>), (StatusCode, String)>);

impl<S: Serialize> UserApiResult<S> {
    pub fn success(status_code: StatusCode, val: S) -> Self {
        UserApiResult {
            0: Ok((status_code, Json(val))),
        }
    }
}

impl<S: Serialize> IntoResponse for UserApiResult<S> {
    fn into_response(self) -> Response {
        self.0.into_response()
    }
}
