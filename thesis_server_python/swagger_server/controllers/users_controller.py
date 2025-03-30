import connexion
import six

from swagger_server.models.error import Error  # noqa: E501
from swagger_server.models.inline_response200 import InlineResponse200  # noqa: E501
from swagger_server.models.inline_response2001 import InlineResponse2001  # noqa: E501
from swagger_server.models.inline_response2002 import InlineResponse2002  # noqa: E501
from swagger_server.models.inline_response204 import InlineResponse204  # noqa: E501
from swagger_server.models.inline_response2041 import InlineResponse2041  # noqa: E501
from swagger_server.models.registration_token import RegistrationToken  # noqa: E501
from swagger_server.models.user import User  # noqa: E501
from swagger_server.models.user_id import UserId  # noqa: E501
from swagger_server.models.users_body import UsersBody  # noqa: E501
from swagger_server import util


def users_get():  # noqa: E501
    """List all users

     # noqa: E501


    :rtype: InlineResponse200
    """
    return 'do some magic!'


def users_post(body=None):  # noqa: E501
    """Create one or more users

    Calling this endpoint will also generate a registration token resource for every user. Each registration token will be emailed to the respective user automatically. # noqa: E501

    :param body: 
    :type body: list | bytes

    :rtype: InlineResponse204
    """
    if connexion.request.is_json:
        body = [UsersBody.from_dict(d) for d in connexion.request.get_json()]  # noqa: E501
    return 'do some magic!'


def users_user_id_delete(user_id):  # noqa: E501
    """Delete a specific user

     # noqa: E501

    :param user_id: The user id
    :type user_id: dict | bytes

    :rtype: InlineResponse2041
    """
    if connexion.request.is_json:
        user_id = UserId.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def users_user_id_get(user_id, registration_token=None):  # noqa: E501
    """Get information about a specific user

     # noqa: E501

    :param user_id: The user id
    :type user_id: dict | bytes
    :param registration_token: A user&#x27;s registration token. This allows requests to this endpoint for a user with the same registration_token without any authentication.
    :type registration_token: dict | bytes

    :rtype: InlineResponse2001
    """
    if connexion.request.is_json:
        user_id = UserId.from_dict(connexion.request.get_json())  # noqa: E501
    if connexion.request.is_json:
        registration_token = RegistrationToken.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def users_user_id_put(user_id, body=None):  # noqa: E501
    """Update information for a specific user

     # noqa: E501

    :param user_id: The user id
    :type user_id: dict | bytes
    :param body: 
    :type body: dict | bytes

    :rtype: InlineResponse2001
    """
    if connexion.request.is_json:
        user_id = UserId.from_dict(connexion.request.get_json())  # noqa: E501
    if connexion.request.is_json:
        body = User.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def users_user_id_regenerate_registration_token_post(user_id):  # noqa: E501
    """Regenerate a specific user&#x27;s registration token

    This endpoint is used to regenerate a new registration token for a specific user that exists already. Calling this endpoint also causes the user to receive an email notification with the new registration token. # noqa: E501

    :param user_id: The user id
    :type user_id: dict | bytes

    :rtype: InlineResponse2002
    """
    if connexion.request.is_json:
        user_id = UserId.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'
