use std::collections::HashMap;

use chrono::{naive::serde::ts_microseconds::deserialize, DateTime, Utc};
use serde::{de::DeserializeOwned, Deserialize, Deserializer, Serialize, Serializer};
use serde::ser::Error;

#[derive(Serialize, Deserialize)]
pub struct UserId(pub u32);

#[derive(Serialize, Deserialize)]
pub struct ThesisId(pub u32);

#[derive(Serialize, Deserialize)]
pub struct Thesis {
    #[serde(default, skip_deserializing)]
    pub id: Option<ThesisId>,
    pub topic: String,
    pub student: ThesisStudent,
    // pub preparation_period: ThesisPreparationPeriod,
    // pub partner_company: ThesisPartnerCompany,
    // pub operational_location: ThesisOperationalLocation,
    // pub in_company_supervisor: ThesisInCompanySupervisor,
    // pub exclude_supervisors_from_companies: Vec<String>,
}

#[derive(Serialize, Deserialize)]
pub struct ThesisStudent {
    #[serde(default, skip_deserializing)]
    pub id: Option<UserId>,
    pub title: String,
    pub first_name: String,
    pub last_name: String,
    pub registration_number: String,
    pub course: String,
}

#[derive(Serialize, Deserialize)]
pub struct ThesisPreparationPeriod {
    #[serde(with = "iso_8601_format")]
    pub from: DateTime<Utc>,
    #[serde(with = "iso_8601_format")]
    pub to: DateTime<Utc>,
}

#[derive(Serialize, Deserialize)]
pub struct ThesisPartnerCompany {
    pub name: String,
    pub address: ThesisPartnerCompanyAddress,
}

#[derive(Serialize, Deserialize)]
pub struct ThesisPartnerCompanyAddress {
    pub street: String,
    pub zip_code: u32,
    pub city: String,
}

#[derive(Serialize, Deserialize)]
pub struct ThesisOperationalLocation {
    pub company_name: String,
    pub department: String,
    pub address: ThesisOperationalLocationAddress,
}

#[derive(Serialize, Deserialize)]
pub struct ThesisOperationalLocationAddress {
    pub street: String,
    pub zip_code: u32,
    pub city: String,
    pub country: String,
}

#[derive(Serialize, Deserialize)]
pub struct ThesisInCompanySupervisor {
    pub title: String,
    pub academic_title: String,
    pub first_name: String,
    pub last_name: String,
    pub phone_number: String,
    pub email: String,
    pub academic_degree: String,
}

#[derive(Serialize)]
pub struct ThesisSummary {
    pub id: ThesisId,
    pub title: String,
    pub student_first_name: String,
    pub student_last_name: String,
}


#[derive(Serialize, Deserialize)]
pub struct Comment {
    #[serde(default, skip_deserializing)]
    pub author: Option<UserId>,
    pub message: String,
}

mod iso_8601_format {
    use serde::{self, Deserialize, Deserializer, Serializer};

    pub fn serialize<S>(
        date: &chrono::DateTime<chrono::Utc>,
        serializer: S,
    ) -> Result<S::Ok, S::Error>
    where
        S: Serializer,
    {
        let s = date.to_rfc3339();
        serializer.serialize_str(&s)
    }

    pub fn deserialize<'de, D>(deserializer: D) -> Result<chrono::DateTime<chrono::Utc>, D::Error>
    where
        D: Deserializer<'de>,
    {
        let s = String::deserialize(deserializer)?;
        let dt = iso8601::datetime(&s).map_err(serde::de::Error::custom)?;
        let naive = dt.into_naive().ok_or(serde::de::Error::custom(
            "Failed to convert datetime to naive datetime",
        ))?;

        Ok(naive.and_utc())
    }
}
