use anyhow::Result;
use axum::{
    http::StatusCode,
    routing::{delete, get, post, put},
    Json, Router,
};
use envconfig::Envconfig;
use keycloak::{types::*, KeycloakAdmin, KeycloakAdminToken};
use routes::*;
use serde::{Deserialize, Serialize};
use std::{net::Ipv4Addr, sync::Arc};
use tower_http::catch_panic::CatchPanicLayer;

mod keycloak_extension;
mod models;
mod result;
mod routes;

#[derive(Envconfig)]
struct Config {
    #[envconfig(from = "SERVER_ADDRESS", default = "0.0.0.0")]
    server_address: Ipv4Addr,
    #[envconfig(from = "SERVER_PORT", default = "8081")]
    server_port: u16,
    #[envconfig(from = "KEYCLOAK_URL", default = "http://172.25.116.166:8080")]
    keycloak_url: String,
    #[envconfig(from = "KEYCLOAK_REALM", default = "thesis-management")]
    keycloak_realm: String,
    #[envconfig(from = "KC_BOOTSTRAP_ADMIN_USERNAME", default = "admin")]
    keycloak_admin_user: String,
    #[envconfig(from = "KC_BOOTSTRAP_ADMIN_PASSWORD", default = "admin")]
    keycloak_admin_password: String,
}

#[derive(Clone)]
pub struct AppState {
    pub keycloak_admin: Arc<KeycloakAdmin>,
    pub config: Arc<Config>,
}

#[tokio::main]
async fn main() -> Result<()> {
    let config = Arc::new(Config::init_from_env()?);

    let client = reqwest::Client::new();
    let keycloak_admin_token = KeycloakAdminToken::acquire(
        &config.keycloak_url,
        &config.keycloak_admin_user,
        &config.keycloak_admin_password,
        &client,
    )
    .await?;

    let keycloak_admin = Arc::new(KeycloakAdmin::new(
        &config.keycloak_url,
        keycloak_admin_token,
        client,
    ));

    let state = AppState {
        keycloak_admin: Arc::clone(&keycloak_admin),
        config: Arc::clone(&config),
    };

    let users = keycloak_admin
        .realm_users_get(
            "thesis-management",
            None,
            None,
            None,
            None,
            None,
            None,
            None,
            None,
            None,
            None,
            None,
            None,
            None,
            None,
        )
        .await?;

    println!("{}", serde_json::to_string_pretty(&users).unwrap());

    let router = Router::new()
        .route("/users", get(get_all_users).post(create_users))
        .route(
            "/users/{id}",
            get(get_user_information)
                .put(update_user)
                .delete(delete_user),
        )
        .route(
            "/users/{id}/regenerate_registration_token",
            post(regenerate_registration_token),
        )
        .layer(CatchPanicLayer::new())
        .with_state(state);

    let listener =
        tokio::net::TcpListener::bind((config.server_address, config.server_port)).await?;

    axum::serve(listener, router).await?;

    Ok(())
}
