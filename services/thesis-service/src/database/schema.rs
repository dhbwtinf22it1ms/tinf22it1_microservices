// @generated automatically by Diesel CLI.

diesel::table! {
    comments (id) {
        #[sql_name = "commentId"]
        id -> Int4,
        thesisId -> Int4,
        author -> Int4,
        msg -> Text,
        createdAt -> Time,
    }
}

diesel::table! {
    thesis (thesisId) {
        thesisId -> Int4,
        topic -> Varchar,
        studentId -> Uuid,
        studentFirstName -> Varchar,
        studentLastName -> Varchar,
        studentTitle -> Varchar,
        studentRegistrationNumber -> Varchar,
        studentCourse -> Varchar,
        companyStreet -> Varchar,
        companyZipcode -> Int4,
        companyCity -> Varchar,
        companyName -> Varchar,
        companyCountry -> Varchar,
        operationalLocationDepartment -> Varchar,
        operationalLocationStreet -> Varchar,
        operationalLocationZipcode -> Int4,
        operationalLocationCity -> Varchar,
        operationalLocationCountry -> Varchar,
        inCompanySupervisorAcademicTitle -> Nullable<Varchar>,
        inCompanySupervisorTitle -> Nullable<Varchar>,
        inCompanySupervisorFirstName -> Varchar,
        inCompanySupervisorLastName -> Varchar,
        inCompanySupervisorEmail -> Varchar,
        inCompanySupervisorPhoneNumber -> Varchar,
        inCompanySupervisorAcademicDegree -> Varchar,
        preparationPeriodBegin -> Date,
        preparationPeriodEnd -> Date,
        excludedCompanies -> Varchar,
    }
}

diesel::joinable!(comments -> thesis (thesisId));

diesel::allow_tables_to_appear_in_same_query!(
    comments,
    thesis,
);
