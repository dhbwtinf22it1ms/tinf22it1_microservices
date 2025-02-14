import connexion
import six

from swagger_server.models.inline_response200 import InlineResponse200  # noqa: E501
from swagger_server.models.sendemail_body import SendemailBody  # noqa: E501
from swagger_server import util

import os
from swagger_server.services.smtp_example import mail_client 
from flask import jsonify, request

smtp_host = os.getenv('SMTP_SERVER_HOST', 'localhost')
smtp_port = os.getenv('SMTP_SERVER_PORT', 2525)
print(smtp_host)
smtp_client = mail_client(smtp_server="localhost", smtp_port=2525)

def send_email(body):  # noqa: E501
    """Send an email

    Triggers an email to be sent using the provided details. # noqa: E501

    :param body: 
    :type body: dict | bytes

    :rtype: InlineResponse200
    """
    if connexion.request.is_json:
        body = SendemailBody.from_dict(connexion.request.get_json())  # noqa: E501
        sender = body.sender
        recipients = body.recipients
        subject = body.subject
        message_body = body.body
        username = body.username
        password = body.password

        print(username)
        print(password)

        if not sender or not recipients:
            return jsonify({"error": "Sender and recipients are required!"}), 400 # TODO I think that swagger_server already supports something like this
        try:
            smtp_client.send_mail(sender, recipients, subject, message_body, username, password)
            return jsonify({"message": "Email sent successfully!"}), 200
        except Exception as e:
            return jsonify({"error": str(e)}), 500

    return 'do some magic!'
