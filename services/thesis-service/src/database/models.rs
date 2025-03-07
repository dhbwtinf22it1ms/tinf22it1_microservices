
use diesel::{prelude::AsChangeset, Queryable, Selectable};
use uuid::Uuid;

#[derive(Selectable, Queryable, AsChangeset)]
#[diesel(table_name = super::schema::thesis)]
#[diesel(check_for_backend(diesel::pg::Pg))]
#[allow(non_camel_case_types)]
pub struct Thesis {
    pub id: i32,
    pub topic: String,
    pub studentId: Uuid,
    pub operationalLocationDepartment: String,
    pub operationalLocationStreet: String,
    pub operationalLocationZipcode: i32,
    pub operationalLocationCity: String,
    pub operationalLocationCountry: String,
    pub inCompanySupervisorTitle: Option<String>,
    pub inCompanySupervisorAcademicTitle: Option<String>,
    pub inCompanySupervisorFirstName: String,
    pub inCompanySupervisorLastName: String,
    pub inCompanySupervisorEmail: String,
    pub inCompanySupervisorPhoneNumber: String,
    pub inCompanySupervisorAcademicDegree: String,
    pub preparationPeriodBegin: chrono::NaiveDate,
    pub preparationPeriodEnd: chrono::NaiveDate,
}
