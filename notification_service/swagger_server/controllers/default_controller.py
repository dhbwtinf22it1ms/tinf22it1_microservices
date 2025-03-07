import connexion
import six

from swagger_server.models.acceptnotify_body import AcceptnotifyBody  # noqa: E501
from swagger_server.models.changenotify_body import ChangenotifyBody  # noqa: E501
from swagger_server.models.deadlinereached_body import DeadlinereachedBody  # noqa: E501
from swagger_server.models.generic_body import GenericBody  # noqa: E501
from swagger_server.models.registerpnotify_body import RegisterpnotifyBody  # noqa: E501
from swagger_server.models.rejectnotify_body import RejectnotifyBody  # noqa: E501
from swagger_server.models.submitnotify_body import SubmitnotifyBody  # noqa: E501
from swagger_server import util
from flask import jsonify, request
from swagger_server.services.smtp_actions import mail_client 
import os

smtp_host = os.getenv('SMTP_HOST', 'localhost')
smtp_port = os.getenv('SMTP_PORT', 2525)
print(smtp_host)
print(smtp_port)
smtp_client = mail_client(smtp_server=smtp_host, smtp_port=smtp_port)


def acceptnotify_post(body):  # noqa: E501
    """Trigger notification for proposal acceptance

    Send a mail to inform a student that their proposal was accepted # noqa: E501

    :param body: 
    :type body: dict | bytes

    :rtype: None
    """
    if connexion.request.is_json:
        body = AcceptnotifyBody.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def changenotify_post(body):  # noqa: E501
    """Trigger notification for changes in a proposal

    Send a mail to inform a user about changes in a proposal # noqa: E501

    :param body: 
    :type body: dict | bytes

    :rtype: None
    """
    if connexion.request.is_json:
        body = ChangenotifyBody.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def deadlinereached_post(body):  # noqa: E501
    """Trigger notification for deadline reached

    Send a notification to inform that all thesis proposals are online # noqa: E501

    :param body: 
    :type body: dict | bytes

    :rtype: None
    """
    if connexion.request.is_json:
        body = DeadlinereachedBody.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def generic_post(body):  # noqa: E501
    """Trigger a generic notification

    Send a custom notification with specified subject and message # noqa: E501

    :param body: 
    :type body: dict | bytes

    :rtype: None
    """
    if connexion.request.is_json:
        body = GenericBody.from_dict(connexion.request.get_json())  # noqa: E501
        sender = "yourmom@mail.com"
        recipients = body.mail_addresses
        subject = body.mail_subject
        message_body = body.mail_text


        if not sender or not recipients:
            return jsonify({"error": "Sender and recipients are required!"}), 400 # TODO I think that swagger_server already supports something like this
        try:
            smtp_client.send_mail(sender, recipients, subject, message_body)
            return jsonify({"message": "Email sent successfully!"}), 200
        except Exception as e:
            return jsonify({"error": str(e)}), 500
    

    return 'do some magic!'


def registerpnotify_post(body):  # noqa: E501
    """Trigger notification for sign-up with login details

    Send a notification with login details to a list of users # noqa: E501

    :param body: 
    :type body: dict | bytes

    :rtype: None
    """
    if connexion.request.is_json:
        body = RegisterpnotifyBody.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def rejectnotify_post(body):  # noqa: E501
    """Trigger notification for proposal rejection

    Send a mail to inform a student that their proposal was rejected # noqa: E501

    :param body: 
    :type body: dict | bytes

    :rtype: None
    """
    if connexion.request.is_json:
        body = RejectnotifyBody.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def submitnotify_post(body):  # noqa: E501
    """Trigger notification for successful proposal upload

    Send a mail to inform a student that their proposal was uploaded successfully # noqa: E501

    :param body: 
    :type body: dict | bytes

    :rtype: None
    """
    if connexion.request.is_json:
        body = SubmitnotifyBody.from_dict(connexion.request.get_json())  # noqa: E501
    return 'wtf'
