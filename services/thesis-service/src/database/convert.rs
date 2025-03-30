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
                title: thesis.studentTitle,
                first_name: thesis.studentFirstName,
                last_name: thesis.studentLastName,
                registration_number: thesis.studentRegistrationNumber,
                course: thesis.studentCourse,
            },
            preparation_period,
            partner_company: api::ThesisPartnerCompany {
                name: thesis.companyName,
                address: api::ThesisPartnerCompanyAddress {
                    street: thesis.companyStreet,
                    zip_code: thesis.companyZipcode as u32,
                    city: thesis.companyCity,
                },
            },
            operational_location: api::ThesisOperationalLocation {
                company_name: todo!(),
                department: thesis.operationalLocationDepartment,
                address: api::ThesisOperationalLocationAddress {
                    street: thesis.operationalLocationStreet,
                    zip_code: thesis.operationalLocationZipcode as u32,
                    city: thesis.operationalLocationCity,
                    country: thesis.operationalLocationCountry,
                },
            },
            in_company_supervisor: api::ThesisInCompanySupervisor {
                title: thesis.inCompanySupervisorTitle.unwrap_or_default(),
                academic_title: thesis.inCompanySupervisorAcademicTitle.unwrap_or_default(),
                first_name: thesis.inCompanySupervisorFirstName,
                last_name: thesis.inCompanySupervisorLastName,
                phone_number: thesis.inCompanySupervisorPhoneNumber,
                email: thesis.inCompanySupervisorEmail,
                academic_degree: thesis.inCompanySupervisorAcademicDegree,
            },
            exclude_supervisors_from_companies: thesis.excludedCompanies,
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
            studentFirstName: thesis.student.first_name,
            studentLastName: thesis.student.last_name,
            studentTitle: thesis.student.title,
            studentRegistrationNumber: thesis.student.registration_number,
            studentCourse: thesis.student.course,
            companyStreet: thesis.partner_company.address.street,
            companyZipcode: thesis.partner_company.address.zip_code as i32,
            companyCity: thesis.partner_company.address.city,
            companyName: thesis.partner_company.name,
            companyCountry: todo!(),
            excludedCompanies: thesis.exclude_supervisors_from_companies,
        }
    }
}