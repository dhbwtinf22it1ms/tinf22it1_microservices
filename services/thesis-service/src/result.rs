use std::borrow::Cow;

use axum::{http::StatusCode, response::IntoResponse, Json};
use serde::Serialize;

pub struct ApiResult<T: Serialize>(
    Result<(StatusCode, Json<OkWrapper<T>>), (StatusCode, Json<ErrWrapper>)>,
);

impl<T: Serialize> ApiResult<T> {
    pub fn new_success(status_code: StatusCode, payload: T) -> Self {
        Ok::<_, (StatusCode, String)>((status_code, payload)).into()
    }

    pub fn new_error(
        status_code: StatusCode,
        message: impl Into<Cow<'static, str>>,
    ) -> ApiResult<T> {
        Err::<(StatusCode, T), _>((status_code, message)).into()
    }
}

impl<T: Serialize> IntoResponse for ApiResult<T> {
    fn into_response(self) -> axum::response::Response {
        self.0.into_response()
    }
}

impl<S: Into<Cow<'static, str>>, T: Serialize> From<Result<(StatusCode, T), (StatusCode, S)>>
    for ApiResult<T>
{
    fn from(value: Result<(StatusCode, T), (StatusCode, S)>) -> Self {
        Self(
            value
                .map(|(ok_status, ok_payload)| (ok_status, Json(OkWrapper { ok: ok_payload })))
                .map_err(|(err_status, err_payload)| {
                    (
                        err_status,
                        Json(ErrWrapper {
                            error: err_payload.into(),
                        }),
                    )
                }),
        )
    }
}

#[derive(Serialize)]
struct ErrWrapper {
    error: Cow<'static, str>,
}

#[derive(Serialize)]
struct OkWrapper<T: Serialize> {
    ok: T,
}
