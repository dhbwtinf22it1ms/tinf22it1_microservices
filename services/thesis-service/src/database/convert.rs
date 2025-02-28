use std::net::Incoming;

use chrono::{DateTime, Utc, NaiveDate};

use crate::database::models as db;
use crate::models as api;

impl From<db::Thesis> for api::Thesis {
    fn from(thesis: db::Thesis) -> Self {
        let naive_date_to_utc_datetime = |naive_date: NaiveDate| {
            DateTime::from_naive_utc_and_offset(naive_date.and_hms_opt(0, 0, 0).unwrap(), Utc)
        };
        let preparation_period = api::ThesisPreparationPeriod {
            from: naive_date_to_utc_datetime(thesis.preparation_period_begin),
            to: naive_date_to_utc_datetime(thesis.preparation_period_end),
        };

        Self {
            id: Some(api::ThesisId(thesis.id as u32)),
            topic: thesis.topic,
            student: api::ThesisStudent {
                id: Some(api::UserId(thesis.student_id as u32)),
                title: "".to_owned(),
                first_name: "".to_owned(),
                last_name: "".to_owned(),
                registration_number: "".to_owned(),
                course: "".to_owned(),
            },
            preparation_period,
            partner_company: api::ThesisPartnerCompany {
                name: "".to_owned(),
                address: api::ThesisPartnerCompanyAddress {
                    street: "".to_owned(),
                    zip_code: 1234567,
                    city: "".to_owned(),
                },
            },
            operational_location: api::ThesisOperationalLocation {
                company_name: "".to_owned(),
                department: "".to_owned(),
                address: api::ThesisOperationalLocationAddress {
                    street: "".to_owned(),
                    zip_code: 1234567,
                    city: "".to_owned(),
                    country: "".to_owned(),
                },
            },
            in_company_supervisor: api::ThesisInCompanySupervisor {
                title: "".to_owned(),
                academic_title: "".to_owned(),
                first_name: "".to_owned(),
                last_name: "".to_owned(),
                phone_number: "".to_owned(),
                email: "".to_owned(),
                academic_degree: "".to_owned(),
            },
            exclude_supervisors_from_companies: Vec::new(),
        }
    }
}

impl From<api::Thesis> for db::Thesis {
    fn from(thesis: api::Thesis) -> Self {
        db::Thesis {
            id: thesis.id.unwrap().0 as i32,
            topic: thesis.topic,
            student_id: thesis.student.id.unwrap().0 as i32,
            company_id: 0,
            operational_location_department: 0,
            operational_location_street: thesis.operational_location.address.street,
            operational_location_zipcode: thesis.operational_location.address.zip_code as i32,
            operational_location_city: thesis.operational_location.address.city,
            operational_location_country: thesis.operational_location.address.country,
            in_company_supervisor_title: Some(thesis.in_company_supervisor.title),
            in_company_supervisor_academic_title: Some(thesis.in_company_supervisor.academic_title),
            in_company_supervisor_first_name: thesis.in_company_supervisor.first_name,
            in_company_supervisor_last_name: thesis.in_company_supervisor.last_name,
            in_company_supervisor_email: thesis.in_company_supervisor.email,
            in_company_supervisor_phone_number: thesis.in_company_supervisor.phone_number,
            in_company_supervisor_academic_degree: thesis.in_company_supervisor.academic_degree,
            preparation_period_begin: thesis.preparation_period.from.date_naive(),
            preparation_period_end: thesis.preparation_period.to.date_naive(),
        }
    }
}