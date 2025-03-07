use anyhow::anyhow;
use serde::{Deserialize, Serialize};
use strum::Display;

#[derive(Debug, Serialize, Deserialize, Clone, Display)]
#[serde(rename_all = "camelCase")]
pub enum Role {
    #[strum(to_string = "student")]
    Student,
    #[strum(to_string = "supervisor")]
    Supervisor,
    #[strum(to_string = "secretary")]
    Secretary,
    #[strum(to_string = "administrator")]
    Administrator,
}

impl TryFrom<&str> for Role {
    type Error = anyhow::Error;

    fn try_from(val: &str) -> Result<Role, Self::Error> {
        match val {
            "student" => Ok(Role::Student),
            "supervisor" => Ok(Role::Supervisor),
            "secretary" => Ok(Role::Secretary),
            "administrator" => Ok(Role::Administrator),
            _ => Err(anyhow!("Invalid role")),
        }
    }
}

#[derive(Debug, Serialize, Deserialize, Clone)]
pub struct UserId(pub String);

#[derive(Debug, Serialize, Deserialize, PartialEq, Clone)]
pub struct RegistrationToken(pub String);

#[derive(Debug, Serialize, Deserialize, PartialEq, Clone)]
#[serde(rename_all = "camelCase")]
pub enum RegistrationStatus {
    Pending(RegistrationToken),
    Completed,
}

#[derive(Debug, Serialize, Deserialize, Clone)]
#[serde(rename_all = "camelCase")]
pub struct User {
    #[serde(default, skip_deserializing)]
    pub id: Option<UserId>,
    #[serde(default, skip_deserializing)]
    pub registration_status: Option<RegistrationStatus>,
    pub first_name: String,
    pub last_name: String,
    pub email: String,
    pub role: Role,
}
