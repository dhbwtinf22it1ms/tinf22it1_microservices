import connexion

from swagger_server.models.error import Error  # noqa: E501
from swagger_server.models.inline_response201 import InlineResponse201  # noqa: E501
from swagger_server.models.thesis import Thesis  # noqa: E501
from swagger_server.models.thesis_id import ThesisId  # noqa: E501
from swagger_server.models.thesis_summary import ThesisSummary  # noqa: E501
from swagger_server import util
from swagger_server.DatabaseConnection.PostgreSQL import ThesisDB, getSession

def theses_get():  # noqa: E501
    """List theses summaries

    Note: This endpoint might allow for filters in the future. # noqa: E501


    :rtype: List[ThesisSummary]
    """
    session = getSession()
    results = session.query(ThesisDB).all()
    return [ThesisSummary()]


def theses_id_get(id):  # noqa: E501
    """Get information for a specific thesis

     # noqa: E501

    :param id: The thesis id
    :type id: dict | bytes

    :rtype: InlineResponse201
    """
    if connexion.request.is_json:
        id = ThesisId.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def theses_id_put(id, body=None):  # noqa: E501
    """Update information for a specific thesis

     # noqa: E501

    :param id: The thesis id
    :type id: dict | bytes
    :param body: 
    :type body: dict | bytes

    :rtype: InlineResponse201
    """
    if connexion.request.is_json:
        id = ThesisId.from_dict(connexion.request.get_json())  # noqa: E501
    if connexion.request.is_json:
        body = Thesis.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def theses_mine_get():  # noqa: E501
    """Get information for the thesis of the current user

     # noqa: E501


    :rtype: InlineResponse201
    """
    return 'do some magic!'


def theses_mine_put(body=None):  # noqa: E501
    """Update information for the thesis of the current user

     # noqa: E501

    :param body: 
    :type body: dict | bytes

    :rtype: InlineResponse201
    """
    if connexion.request.is_json:
        body = Thesis.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def theses_post(body=None):  # noqa: E501
    """Create a new thesis for current user

     # noqa: E501

    :param body: 
    :type body: dict | bytes

    :rtype: InlineResponse201
    """
    if connexion.request.is_json:
        body = Thesis.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'
