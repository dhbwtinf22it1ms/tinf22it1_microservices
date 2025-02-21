use keycloak::KeycloakError;
use thiserror::Error;

#[derive(Error, Debug)]
pub enum KeycloakExtError {
    #[error("The field {0} is empty")]
    EmptyField(&'static str),
    #[error("The field {0}")]
    InvalidField(&'static str),
    #[error("The user with the Username could not be found")]
    UserNotFound(String),
    #[error("The user is already registered")]
    UserAlreadyRegistered,
    #[error("The user {0} {1} could not be created")]
    UserCreationFailed(String, String),
    #[error("Keycloak raised an error")]
    KeycloakError(#[from] KeycloakError),
}
