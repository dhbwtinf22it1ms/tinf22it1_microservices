use futures::future::join_all;
use keycloak::{
    types::{TypeVec, UserRepresentation},
    KeycloakAdmin,
};
use std::sync::Arc;
use uuid::Uuid;

use super::error::KeycloakExtError::{self, *};
use crate::models::{RegistrationStatus, RegistrationToken, User};

pub fn get_registration_status(
    user: &UserRepresentation,
) -> Result<RegistrationStatus, KeycloakExtError> {
    let attributes = user.attributes.as_ref().ok_or(EmptyField("attributes"))?;

    let registration_complete: bool = attributes
        .get(&"registration_complete".to_owned())
        .ok_or(EmptyField("registration_complete"))?
        .get(0)
        .ok_or(InvalidField("registration_complete is empty"))?
        .as_str()
        .parse::<bool>()
        .map_err(|_| InvalidField("registration_complete is not a valid boolean"))?;

    if registration_complete {
        return Ok(RegistrationStatus::Completed);
    };

    let registration_token = RegistrationToken(
        attributes
            .get(&"registration_token".to_owned())
            .ok_or(EmptyField("registration_token"))?
            .get(0)
            .ok_or(InvalidField("registration_token is empty"))?
            .to_string(),
    );

    Ok(RegistrationStatus::Pending(registration_token))
}

pub async fn get_users(
    keycloak_admin: &Arc<KeycloakAdmin>,
    realm: &str,
) -> Result<Vec<User>, KeycloakExtError> {
    let users = keycloak_admin
        .realm_users_get(
            realm, None, None, None, None, None, None, None, None, None, None, None, None, None,
            None,
        )
        .await?;

    let users = users.into_iter().map(|user_repr| async move {
        let user_id = user_repr.id.clone().ok_or(EmptyField("user_id"))?;

        get_user(keycloak_admin, realm, &user_id).await
    });

    let users = join_all(users).await;

    println!("{:?}", users);

    Ok(users.into_iter().filter_map(|user| user.ok()).collect())
}

pub async fn get_user(
    keycloak_admin: &Arc<KeycloakAdmin>,
    realm: &str,
    user_id: &str,
) -> Result<User, KeycloakExtError> {
    let mut user_repr = keycloak_admin
        .realm_users_with_user_id_get(realm, user_id, None)
        .await?;

    let groups = keycloak_admin
        .realm_users_with_user_id_groups_get(realm, &user_id, None, None, Some(1), None)
        .await?
        .into_iter()
        .map(|group_repr| group_repr.name.unwrap())
        .collect::<TypeVec<String>>();

    user_repr.groups = Some(groups);

    User::try_from(user_repr)
}

pub async fn regenerate_registration_token(
    keycloak_admin: &Arc<KeycloakAdmin>,
    realm: &str,
    user_id: &str,
) -> Result<RegistrationToken, KeycloakExtError> {
    let mut user = get_user(keycloak_admin, realm, user_id).await?;

    if user.registration_status.clone().unwrap() == RegistrationStatus::Completed {
        return Err(UserAlreadyRegistered);
    }

    let new_token = Uuid::new_v4().to_string();

    if let Some(RegistrationStatus::Pending(_)) = user.registration_status {
        user.registration_status = Some(RegistrationStatus::Pending(RegistrationToken(
            new_token.clone(),
        )));
    }

    let user_repr = UserRepresentation::try_from(user)?;

    keycloak_admin
        .realm_users_with_user_id_put(realm, user_id, user_repr)
        .await?;

    Ok(RegistrationToken(new_token))
}

pub async fn update_user(
    keycloak_admin: &Arc<KeycloakAdmin>,
    realm: &str,
    user_id: &str,
    user: &User,
) -> Result<(), KeycloakExtError> {
    let user_repr = UserRepresentation::try_from(user.clone())?;

    keycloak_admin
        .realm_users_with_user_id_put(realm, user_id, user_repr)
        .await?;

    Ok(())
}

pub async fn delete_user(
    keycloak_admin: &Arc<KeycloakAdmin>,
    realm: &str,
    user_id: &str,
) -> Result<User, KeycloakExtError> {
    let user = get_user(keycloak_admin, realm, user_id).await?;
    keycloak_admin
        .realm_users_with_user_id_delete(realm, user_id)
        .await?;

    Ok(user)
}

async fn create_user(
    keycloak_admin: &Arc<KeycloakAdmin>,
    realm: &str,
    user: &User,
) -> Result<String, KeycloakExtError> {
    let mut new_user = user.clone();
    let new_registration_token = Uuid::new_v4().to_string();

    new_user.registration_status = Some(RegistrationStatus::Pending(RegistrationToken(
        new_registration_token,
    )));

    let mut user_repr = UserRepresentation::try_from(new_user)?;
    user_repr.username = Some(Uuid::new_v4().to_string());
    user_repr.enabled = Some(true);

    let created_user_id = keycloak_admin.realm_users_post(realm, user_repr).await?;

    Ok(created_user_id.unwrap())
}

pub async fn create_users(
    keycloak_admin: &Arc<KeycloakAdmin>,
    realm: &str,
    users: &Vec<User>,
) -> Result<Vec<String>, KeycloakExtError> {
    let mut user_ids: Vec<String> = Vec::new();

    for user in users {
        let created_id = create_user(keycloak_admin, realm, user).await?;

        user_ids.push(created_id)
    }

    Ok(user_ids)
}
