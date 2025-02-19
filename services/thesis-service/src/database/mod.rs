use anyhow::Context;
use diesel::{
    r2d2::{self, ConnectionManager},
    result::DatabaseErrorKind,
    PgConnection,
};

// mod schema;

pub fn create_connection_pool(
    database_url: &str,
) -> anyhow::Result<r2d2::Pool<ConnectionManager<PgConnection>>> {
    let manager = ConnectionManager::new(database_url);

    r2d2::Pool::builder()
        .test_on_check_out(true)
        .build(manager)
        .context("Failed to initialize database connection pool")
}

/// A helper for returing database errors to the application layer.
/// This does not contain any `diesel`-specific types on purpose.
#[derive(thiserror::Error, Debug)]
pub enum DatabaseError {
    #[error("row(s) could not be inserted due to a unique key violation")]
    UniqueKeyViolation,
    #[error("no row could be found")]
    NotFound,
    #[error("unknown error {0}")]
    Other(Box<dyn std::error::Error>),
}

impl From<diesel::result::Error> for DatabaseError {
    fn from(value: diesel::result::Error) -> Self {
        use diesel::result::Error::*;

        match value {
            DatabaseError(DatabaseErrorKind::UniqueViolation, _) => Self::UniqueKeyViolation,
            NotFound => Self::NotFound,
            other_err => Self::Other(Box::new(other_err)),
        }
    }
}
