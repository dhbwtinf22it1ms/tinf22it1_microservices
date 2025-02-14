# Configuration
Configuration per (Micro-)Service can be done based on a configuration file or through environment variables that are passed to containers.
We have decided to use environment variables, because...
- it is easy to read env variables from every programming language.
- the services are not required to ship with YML/JSON/TOML parsers for the configuration file.

For now the central management for environment variables is done directly in the top-level `docker-compose.yml` file.

In the future we should use `.env` and `.env.local` files and configure docker-compose to load these on startup using its [`env_file` attribute](https://docs.docker.com/compose/how-tos/environment-variables/set-environment-variables/#use-the-env_file-attribute).

## See also
Envvar configuraiton with top-level `.env` file: https://github.com/open-telemetry/opentelemetry-demo/blob/main/.env