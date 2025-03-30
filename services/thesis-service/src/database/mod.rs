use anyhow::Context;
use chrono::NaiveDate;
use diesel::{
    query_dsl::methods::{FilterDsl, SelectDsl},
    r2d2::{self, ConnectionManager},
    result::DatabaseErrorKind,
    ExpressionMethods, Insertable, PgConnection, RunQueryDsl, SelectableHelper,
};
use models::Thesis;
use uuid::Uuid;

use crate::models::{Thesis as ApiThesis, ThesisId, UserId};

mod convert;
mod models;
#[allow(non_snake_case)]
mod schema;

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

type Result<T> = std::result::Result<T, DatabaseError>;

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

pub fn list_theses(connection: &mut PgConnection) -> Result<Vec<ApiThesis>> {
    use schema::thesis::dsl::*;

    let db_results = thesis
        .select(models::Thesis::as_select())
        .load(connection)?;

    let results = db_results.into_iter().map(ApiThesis::from).collect();

    Ok(results)
}

pub fn create_thesis(connection: &mut PgConnection, thesis: ApiThesis) -> Result<ApiThesis> {

    #[allow(non_camel_case_types)]
    #[derive(Insertable)]
    #[diesel(table_name = schema::thesis)]
    pub struct NewThesis<'a> {
        pub topic: &'a str,
        pub studentId: Uuid,
        pub operationalLocationDepartment: &'a str,
        pub operationalLocationStreet: &'a str,
        pub operationalLocationZipcode: i32,
        pub operationalLocationCity: &'a str,
        pub operationalLocationCountry: &'a str,
        pub inCompanySupervisorTitle: &'a str,
        pub inCompanySupervisorAcademicTitle: &'a str,
        pub inCompanySupervisorFirstName: &'a str,
        pub inCompanySupervisorLastName: &'a str,
        pub inCompanySupervisorEmail: &'a str,
        pub inCompanySupervisorPhoneNumber: &'a str,
        pub inCompanySupervisorAcademicDegree: &'a str,
        preparationPeriodBegin: chrono::NaiveDate,
        preparationPeriodEnd: chrono::NaiveDate,
    }

    let new_thesis = NewThesis {
        topic: &thesis.topic,
        studentId: thesis
            .student
            .id
            .expect("stduent id to be set for new theses")
            .0,
        operationalLocationDepartment: "",
        operationalLocationStreet: "",
        operationalLocationZipcode: 123456,
        operationalLocationCity: "",
        operationalLocationCountry: "",
        inCompanySupervisorTitle: "",
        inCompanySupervisorAcademicTitle: "",
        inCompanySupervisorFirstName: "",
        inCompanySupervisorLastName: "",
        inCompanySupervisorEmail: "",
        inCompanySupervisorPhoneNumber: "",
        inCompanySupervisorAcademicDegree: "",
        preparationPeriodBegin: NaiveDate::from_isoywd(2025, 2, chrono::Weekday::Sun),
        preparationPeriodEnd: NaiveDate::from_isoywd(2025, 3, chrono::Weekday::Sun),
    };

    let db_result = diesel::insert_into(schema::thesis::dsl::thesis)
        .values(new_thesis)
        .returning(models::Thesis::as_returning())
        .get_result(connection)?;

    let result = ApiThesis::from(db_result);

    Ok(result)
}

pub fn get_thesis_by_id(connection: &mut PgConnection, id: ThesisId) -> Result<ApiThesis> {
    use schema::thesis::dsl;

    let db_result = dsl::thesis
        .filter(dsl::id.eq(id.0 as i32))
        .select(models::Thesis::as_select())
        .first(connection)?;

    let result = ApiThesis::from(db_result);

    Ok(result)
}

pub fn get_thesis_by_student_id(
    connection: &mut PgConnection,
    user_id: UserId,
) -> Result<ApiThesis> {
    use schema::thesis::dsl;

    let db_result = dsl::thesis
        .filter(dsl::studentId.eq(user_id.0))
        .select(models::Thesis::as_select())
        .first(connection)?;

    let result = ApiThesis::from(db_result);

    Ok(result)
}

pub fn update_thesis(connection: &mut PgConnection, thesis: ApiThesis) -> Result<ApiThesis> {
    use schema::thesis::dsl;

    let db_thesis = Thesis::from(thesis);

    let db_thesis = diesel::update(dsl::thesis)
        .set(db_thesis)
        .returning(Thesis::as_returning())
        .get_result(connection)?;

    let thesis = ApiThesis::from(db_thesis);

    Ok(thesis)
}
