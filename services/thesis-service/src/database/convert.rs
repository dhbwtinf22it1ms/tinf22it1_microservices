use chrono::{DateTime, Utc, NaiveDate};

use crate::database::models as db;
use crate::models as api;

impl From<db::Thesis> for api::Thesis {
    fn from(thesis: db::Thesis) -> Self {
        let naive_date_to_utc_datetime = |naive_date: NaiveDate| {
            DateTime::from_naive_utc_and_offset(naive_date.and_hms_opt(0, 0, 0).unwrap(), Utc)
        };
        let preparation_period = api::ThesisPreparationPeriod {
            from: naive_date_to_utc_datetime(thesis.preparationPeriodBegin),
            to: naive_date_to_utc_datetime(thesis.preparationPeriodEnd),
        };

        Self {
            id: Some(api::ThesisId(thesis.id as u32)),
            topic: thesis.topic,
            student: api::ThesisStudent {
                id: Some(api::UserId(thesis.studentId)),
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
            studentId: thesis.student.id.unwrap().0,
            operationalLocationDepartment: thesis.operational_location.department,
            operationalLocationStreet: thesis.operational_location.address.street,
            operationalLocationZipcode: thesis.operational_location.address.zip_code as i32,
            operationalLocationCity: thesis.operational_location.address.city,
            operationalLocationCountry: thesis.operational_location.address.country,
            inCompanySupervisorTitle: Some(thesis.in_company_supervisor.title),
            inCompanySupervisorAcademicTitle: Some(thesis.in_company_supervisor.academic_title),
            inCompanySupervisorFirstName: thesis.in_company_supervisor.first_name,
            inCompanySupervisorLastName: thesis.in_company_supervisor.last_name,
            inCompanySupervisorEmail: thesis.in_company_supervisor.email,
            inCompanySupervisorPhoneNumber: thesis.in_company_supervisor.phone_number,
            inCompanySupervisorAcademicDegree: thesis.in_company_supervisor.academic_degree,
            preparationPeriodBegin: thesis.preparation_period.from.date_naive(),
            preparationPeriodEnd: thesis.preparation_period.to.date_naive(),
        }
    }
}