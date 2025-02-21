use futures::io::Empty;
use keycloak::types::UserRepresentation;

use super::error::KeycloakExtError::{self, *};
use super::user::get_registration_status;
use crate::models::{RegistrationStatus, RegistrationToken, Role, User, UserId};
use std::collections::HashMap;

impl TryFrom<UserRepresentation> for User {
    type Error = KeycloakExtError;

    fn try_from(user: UserRepresentation) -> Result<User, Self::Error> {
        println!("{:?}", user);
        let groups = user.groups.as_ref().ok_or(EmptyField("groups"))?;

        if groups.len() < 1 {
            return Err(InvalidField("groups does not have enough entries"));
        }

        let registration_status = get_registration_status(&user)?;

        Ok(User {
            id: Some(UserId(user.id.ok_or(EmptyField("user_id"))?)),
            registration_status: Some(registration_status),
            first_name: user.first_name.ok_or(EmptyField("first_name"))?,
            last_name: user.last_name.ok_or(EmptyField("last_name"))?,
            email: user.email.ok_or(EmptyField("email"))?,
            role: Role::try_from(groups[0].as_str())
                .map_err(|_| InvalidField("role is invalid"))?,
        })
    }
}

impl TryFrom<User> for UserRepresentation {
    type Error = KeycloakExtError;

    fn try_from(user: User) -> Result<UserRepresentation, Self::Error> {
        let mut user_repr = UserRepresentation::default();

        let registration_complete = match user.registration_status {
            Some(RegistrationStatus::Completed) => true,
            _ => false,
        };

        let mut attributes: HashMap<String, Vec<String>> = HashMap::from([(
            "registration_complete".to_owned(),
            vec![registration_complete.to_string()],
        )]);

        if let Some(RegistrationStatus::Pending(token)) = user.registration_status.clone() {
            attributes
                .entry("registration_token".to_owned())
                .or_insert(vec![token.0]);
        }

        user_repr.id = user.id.map(|s| s.0);
        user_repr.first_name = Some(user.first_name);
        user_repr.last_name = Some(user.last_name);
        user_repr.email = Some(user.email);
        user_repr.attributes = user.registration_status.map(|_| attributes);
        user_repr.groups = Some(vec![user.role.to_string()]);

        Ok(user_repr)
    }
}
