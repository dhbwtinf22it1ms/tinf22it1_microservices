import connexion
import six

from swagger_server.models.comment import Comment  # noqa: E501
from swagger_server.models.error import Error  # noqa: E501
from swagger_server.models.inline_response2003 import InlineResponse2003  # noqa: E501
from swagger_server.models.inline_response2011 import InlineResponse2011  # noqa: E501
from swagger_server.models.thesis_id import ThesisId  # noqa: E501
from swagger_server import util


def theses_id_comments_get(id):  # noqa: E501
    """List all comments for specific thesis

     # noqa: E501

    :param id: The thesis id
    :type id: dict | bytes

    :rtype: InlineResponse2003
    """
    if connexion.request.is_json:
        id = ThesisId.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def theses_id_comments_post(id, body=None):  # noqa: E501
    """Post a new comment for a specific thesis

     # noqa: E501

    :param id: The thesis id
    :type id: dict | bytes
    :param body: 
    :type body: dict | bytes

    :rtype: InlineResponse2011
    """
    if connexion.request.is_json:
        id = ThesisId.from_dict(connexion.request.get_json())  # noqa: E501
    if connexion.request.is_json:
        body = Comment.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'
