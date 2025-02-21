use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
pub enum Role {
    #[serde(rename = "student")]
    Student,
    #[serde(rename = "supervisor")]
    Supervisor,
    #[serde(rename = "secretary")]
    Secretary,
    #[serde(rename = "administrator")]
    Administrator,
}

#[derive(Serialize, Deserialize)]
pub struct UserId(pub i64);

#[derive(Serialize, Deserialize)]
pub struct RegistrationToken(pub String);

#[derive(Serialize, Deserialize)]
pub enum RegistrationStatus {
    #[serde(rename = "pending")]
    Pending(RegistrationToken),
    #[serde(rename = "completed")]
    Completed,
}

#[derive(Serialize, Deserialize)]
pub struct User {
    #[serde(default, skip_deserializing)]
    pub id: Option<UserId>,
    #[serde(rename = "registrationStatus", default, skip_deserializing)]
    pub registration_status: Option<RegistrationStatus>,
    #[serde(rename = "firstName")]
    pub first_name: String,
    #[serde(rename = "lastName")]
    pub last_name: String,
    pub email: String,
    pub role: Role,
}
