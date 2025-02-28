use std::{net::Ipv4Addr, sync::Arc};

use axum::Router;
use diesel::{
    r2d2::{self, ConnectionManager},
    PgConnection,
};
use envconfig::Envconfig;
use tower_http::catch_panic::CatchPanicLayer;
use tracing::level_filters::LevelFilter;

mod database;
mod models;
mod result;
mod routes;

#[derive(Envconfig)]
pub struct Config {
    #[envconfig(from = "SERVER_ADDRESS", default = "0.0.0.0")]
    pub server_addr: Ipv4Addr,

    #[envconfig(from = "SERVER_PORT", default = "3000")]
    pub server_port: u16,

    #[envconfig(from = "RUST_LOG", default = "INFO")]
    pub log_level: LevelFilter,

    #[envconfig(from = "DATABASE_URL")]
    pub database_url: String,
}

#[derive(Clone)]
pub struct AppState {
    database_pool: Arc<r2d2::Pool<ConnectionManager<PgConnection>>>,
}

// #[tokio::main]
#[tokio::main(flavor = "current_thread")]
async fn main() -> anyhow::Result<()> {
    // For development/testing only
    let _ = dotenv::dotenv();

    let config = Config::init_from_env()?;

    tracing_subscriber::FmtSubscriber::builder()
        .with_max_level(config.log_level)
        .init();

    let state = AppState {
        database_pool: Arc::new(database::create_connection_pool(&config.database_url)?),
    };

    let router = build_top_level_router(state);

    let listener = tokio::net::TcpListener::bind((config.server_addr, config.server_port)).await?;

    tracing::info!("Serving at http://{:?}", listener.local_addr()?);

    axum::serve(listener, router).await?;

    Ok(())
}

fn build_top_level_router(state: AppState) -> Router<()> {
    Router::new()
        .nest("/api/v0/theses", routes::build_router())
        .layer(CatchPanicLayer::new())
        .with_state(state)
}
