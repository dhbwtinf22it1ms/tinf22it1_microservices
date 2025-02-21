use axum::{
    http::StatusCode,
    response::{IntoResponse, Response},
    Json,
};
use serde::Serialize;
use serde_json::json;
use std::borrow::Cow;

use crate::models::User;

pub struct UserApiResult<S: Serialize>(
    Result<(StatusCode, Json<OkWrapper<S>>), (StatusCode, Json<ErrWrapper>)>,
);

#[derive(Serialize)]
struct ErrWrapper {
    error: Cow<'static, str>,
}

#[derive(Serialize)]
struct OkWrapper<T: Serialize> {
    ok: T,
}

impl<S: Serialize> UserApiResult<S> {
    pub fn new_success(status_code: StatusCode, val: S) -> Self {
        UserApiResult {
            0: Ok((status_code, Json(OkWrapper { ok: val }))),
        }
    }

    pub fn new_error(status_code: StatusCode, val: impl Into<Cow<'static, str>>) -> Self {
        UserApiResult {
            0: Err((status_code, Json(ErrWrapper { error: val.into() }))),
        }
    }
}

impl<S: Serialize> IntoResponse for UserApiResult<S> {
    fn into_response(self) -> Response {
        self.0.into_response()
    }
}
