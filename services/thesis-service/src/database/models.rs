use diesel::{prelude::AsChangeset, Queryable, Selectable};

#[derive(Selectable, Queryable, AsChangeset)]
#[diesel(table_name = super::schema::thesis)]
#[diesel(check_for_backend(diesel::pg::Pg))]
pub struct Thesis {
    pub id: i32,
    pub topic: String,
    pub student_id: i32,
    pub company_id: i32,
    pub operational_location_department: i32,
    pub operational_location_street: String,
    pub operational_location_zipcode: i32,
    pub operational_location_city: String,
    pub operational_location_country: String,
    pub in_company_supervisor_title: Option<String>,
    pub in_company_supervisor_academic_title: Option<String>,
    pub in_company_supervisor_first_name: String,
    pub in_company_supervisor_last_name: String,
    pub in_company_supervisor_email: String,
    pub in_company_supervisor_phone_number: String,
    pub in_company_supervisor_academic_degree: String,
    pub preparation_period_begin: chrono::NaiveDate,
    pub preparation_period_end: chrono::NaiveDate,
}
