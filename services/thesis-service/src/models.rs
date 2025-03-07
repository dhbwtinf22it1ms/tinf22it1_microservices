use chrono::{DateTime, Utc};
use serde::{Deserialize, Serialize};
use uuid::Uuid;

#[derive(Copy, Clone, PartialEq, Eq, Debug, Serialize, Deserialize)]
pub struct UserId(pub Uuid);

#[derive(Copy, Clone, PartialEq, Eq, Debug, Serialize, Deserialize)]
pub struct ThesisId(pub u32);

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Thesis {
    #[serde(default, skip_deserializing)]
    pub id: Option<ThesisId>,
    pub topic: String,
    pub student: ThesisStudent,
    pub preparation_period: ThesisPreparationPeriod,
    pub partner_company: ThesisPartnerCompany,
    pub operational_location: ThesisOperationalLocation,
    pub in_company_supervisor: ThesisInCompanySupervisor,
    pub exclude_supervisors_from_companies: String,
}

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ThesisStudent {
    #[serde(default, skip_deserializing)]
    pub id: Option<UserId>,
    pub title: String,
    pub first_name: String,
    pub last_name: String,
    pub registration_number: String,
    pub course: String,
}

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ThesisPreparationPeriod {
    #[serde(with = "iso_8601_format")]
    pub from: DateTime<Utc>,
    #[serde(with = "iso_8601_format")]
    pub to: DateTime<Utc>,
}

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ThesisPartnerCompany {
    pub name: String,
    pub address: ThesisPartnerCompanyAddress,
}

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ThesisPartnerCompanyAddress {
    pub street: String,
    pub zip_code: u32,
    pub city: String,
}

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ThesisOperationalLocation {
    pub company_name: String,
    pub department: String,
    pub address: ThesisOperationalLocationAddress,
}

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ThesisOperationalLocationAddress {
    pub street: String,
    pub zip_code: u32,
    pub city: String,
    pub country: String,
}

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ThesisInCompanySupervisor {
    pub title: String,
    pub academic_title: String,
    pub first_name: String,
    pub last_name: String,
    pub phone_number: String,
    pub email: String,
    pub academic_degree: String,
}

#[derive(Debug, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct ThesisSummary {
    pub id: ThesisId,
    pub title: String,
    pub student_first_name: String,
    pub student_last_name: String,
}

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
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

impl From<Thesis> for ThesisSummary {
    fn from(value: Thesis) -> Self {
        Self {
            id: value.id.expect("the id to be Some because thesis summaries can only be created for theses that already exist"),
            title: value.topic,
            student_first_name: value.student.first_name,
            student_last_name: value.student.last_name,
        }
    }
}