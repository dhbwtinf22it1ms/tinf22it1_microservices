use anyhow::Context;
use chrono::{DateTime, NaiveDate};
use diesel::{
    query_dsl::methods::{FilterDsl, FindDsl, SelectDsl}, r2d2::{self, ConnectionManager}, result::DatabaseErrorKind, ExpressionMethods, Insertable, PgConnection, RunQueryDsl, SelectableHelper
};
use models::Thesis;

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
    #[derive(Insertable)]
    #[diesel(table_name = schema::thesis)]
    pub struct NewThesis<'a> {
        pub topic: &'a str,
        pub student_id: i32,
        pub company_id: i32,
        pub operational_location_department: i32,
        pub operational_location_street: &'a str,
        pub operational_location_zipcode: i32,
        pub operational_location_city: &'a str,
        pub operational_location_country: &'a str,
        pub in_company_supervisor_title: &'a str,
        pub in_company_supervisor_academic_title: &'a str,
        pub in_company_supervisor_first_name: &'a str,
        pub in_company_supervisor_last_name: &'a str,
        pub in_company_supervisor_email: &'a str,
        pub in_company_supervisor_phone_number: &'a str,
        pub in_company_supervisor_academic_degree: &'a str,
        preparation_period_begin: chrono::NaiveDate,
        preparation_period_end: chrono::NaiveDate,
    }

    let new_thesis = NewThesis {
        topic: &thesis.topic,
        student_id: thesis
            .student
            .id
            .expect("student id to be set for new theses")
            .0 as i32,
        company_id: 1,
        operational_location_department: 0,
        operational_location_street: "",
        operational_location_zipcode: 1234567,
        operational_location_city: "",
        operational_location_country: "",
        in_company_supervisor_title: "",
        in_company_supervisor_academic_title: "",
        in_company_supervisor_first_name: "",
        in_company_supervisor_last_name: "",
        in_company_supervisor_email: "",
        in_company_supervisor_phone_number: "",
        in_company_supervisor_academic_degree: "",
        preparation_period_begin: NaiveDate::from_isoywd(2025, 2, chrono::Weekday::Sun),
        preparation_period_end: NaiveDate::from_isoywd(2025, 3, chrono::Weekday::Sun),
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

pub fn get_thesis_by_student_id(connection: &mut PgConnection, user_id: UserId) -> Result<ApiThesis> {
    use schema::thesis::dsl;

    let db_result = dsl::thesis
        .filter(dsl::student_id.eq(user_id.0 as i32))
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