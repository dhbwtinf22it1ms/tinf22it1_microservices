use anyhow::Result;
use axum::{
    http::StatusCode,
    routing::{delete, get, post, put},
    Json, Router,
};
use envconfig::Envconfig;
use routes::*;
use serde::{Deserialize, Serialize};
use std::net::Ipv4Addr;

mod models;
mod result;
mod routes;

#[derive(Envconfig)]
struct Config {
    #[envconfig(from = "SERVER_ADDRESS", default = "0.0.0.0")]
    server_address: Ipv4Addr,
    #[envconfig(from = "SERVER_PORT", default = "8081")]
    server_port: u16,
}

#[tokio::main]
async fn main() -> Result<()> {
    let config = Config::init_from_env()?;

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
        );

    let listener =
        tokio::net::TcpListener::bind((config.server_address, config.server_port)).await?;

    axum::serve(listener, router).await?;

    Ok(())
}
