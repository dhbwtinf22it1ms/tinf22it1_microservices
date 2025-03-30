#!/usr/bin/env python3

import connexion
import os
from swagger_server import encoder


def main():
    options = {"swagger_ui": True, 'swagger_url': '/ui'}
    app = connexion.FlaskApp(__name__, specification_dir='./swagger/', options=options)
    app.app.json_encoder = encoder.JSONEncoder
    app.add_api('swagger.yaml', arguments={'title': 'Thesis Management Software'}, pythonic_params=True)
    app.run(port=8080)


if __name__ == '__main__':
    main()
