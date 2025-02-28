// @generated automatically by Diesel CLI.

diesel::table! {
    comments (id) {
        #[sql_name = "commentId"]
        id -> Int4,
        #[sql_name = "thesisId"]
        thesis_id -> Int4,
        #[sql_name = "author"]
        author -> Int4,
        #[sql_name = "msg"]
        msg -> Text,
        #[sql_name = "createdAt"]
        created_at -> Time,
    }
}

diesel::table! {
    company (id) {
        #[sql_name = "companyId"]
        id -> Int4,
        #[sql_name = "street"]
        street -> Varchar,
        #[sql_name = "zipcode"]
        zipcode -> Int4,
        #[sql_name = "city"]
        city -> Varchar,
        #[sql_name = "name"]
        name -> Varchar,
        #[sql_name = "country"]
        country -> Varchar,
    }
}

diesel::table! {
    supervisorsExclusion (thesisId, companyId) {
        thesisId -> Int4,
        companyId -> Int4,
    }
}

diesel::table! {
    thesis (id) {
        #[sql_name = "thesisId"]
        id -> Int4,
        topic -> Varchar,
        #[sql_name = "studentId"]
        student_id -> Int4,
        #[sql_name = "companyId"]
        company_id -> Int4,
        #[sql_name = "operationalLocationDepartment"]
        operational_location_department -> Int4,
        #[sql_name = "operationalLocationStreet"]
        operational_location_street -> Varchar,
        #[sql_name = "operationalLocationZipcode"]
        operational_location_zipcode -> Int4,
        #[sql_name = "operationalLocationCity"]
        operational_location_city -> Varchar,
        #[sql_name = "operationalLocationCountry"]
        operational_location_country -> Varchar,
        #[sql_name = "inCompanySupervisorTitle"]
        in_company_supervisor_title -> Nullable<Varchar>,
        #[sql_name = "inCompanySupervisorAcademicTitle"]
        in_company_supervisor_academic_title -> Nullable<Varchar>,
        #[sql_name = "inCompanySupervisorFirstName"]
        in_company_supervisor_first_name -> Varchar,
        #[sql_name = "inCompanySupervisorLastName"]
        in_company_supervisor_last_name -> Varchar,
        #[sql_name = "inCompanySupervisorEmail"]
        in_company_supervisor_email -> Varchar,
        #[sql_name = "inCompanySupervisorPhoneNumber"]
        in_company_supervisor_phone_number -> Varchar,
        #[sql_name = "inCompanySupervisorAcademicDegree"]
        in_company_supervisor_academic_degree -> Varchar,
        #[sql_name = "preparationPeriodBegin"]
        preparation_period_begin -> Date,
        #[sql_name = "preparationPeriodEnd"]
        preparation_period_end -> Date,
    }
}

diesel::joinable!(comments -> thesis (id));
diesel::joinable!(supervisorsExclusion -> company (companyId));
diesel::joinable!(supervisorsExclusion -> thesis (thesisId));
diesel::joinable!(thesis -> company (id));

diesel::allow_tables_to_appear_in_same_query!(comments, company, supervisorsExclusion, thesis,);
