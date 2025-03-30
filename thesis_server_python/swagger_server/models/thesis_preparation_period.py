# coding: utf-8

from __future__ import absolute_import
from datetime import date, datetime  # noqa: F401

from typing import List, Dict  # noqa: F401

from swagger_server.models.base_model_ import Model
from swagger_server import util


class ThesisPreparationPeriod(Model):
    """NOTE: This class is auto generated by the swagger code generator program.

    Do not edit the class manually.
    """
    def __init__(self, _from: str=None, to: str=None):  # noqa: E501
        """ThesisPreparationPeriod - a model defined in Swagger

        :param _from: The _from of this ThesisPreparationPeriod.  # noqa: E501
        :type _from: str
        :param to: The to of this ThesisPreparationPeriod.  # noqa: E501
        :type to: str
        """
        self.swagger_types = {
            '_from': str,
            'to': str
        }

        self.attribute_map = {
            '_from': 'from',
            'to': 'to'
        }
        self.__from = _from
        self._to = to

    @classmethod
    def from_dict(cls, dikt) -> 'ThesisPreparationPeriod':
        """Returns the dict as a model

        :param dikt: A dict.
        :type: dict
        :return: The Thesis_preparationPeriod of this ThesisPreparationPeriod.  # noqa: E501
        :rtype: ThesisPreparationPeriod
        """
        return util.deserialize_model(dikt, cls)

    @property
    def _from(self) -> str:
        """Gets the _from of this ThesisPreparationPeriod.

        ISO-8601 timestamp  # noqa: E501

        :return: The _from of this ThesisPreparationPeriod.
        :rtype: str
        """
        return self.__from

    @_from.setter
    def _from(self, _from: str):
        """Sets the _from of this ThesisPreparationPeriod.

        ISO-8601 timestamp  # noqa: E501

        :param _from: The _from of this ThesisPreparationPeriod.
        :type _from: str
        """

        self.__from = _from

    @property
    def to(self) -> str:
        """Gets the to of this ThesisPreparationPeriod.

        ISO-8601 timestamp  # noqa: E501

        :return: The to of this ThesisPreparationPeriod.
        :rtype: str
        """
        return self._to

    @to.setter
    def to(self, to: str):
        """Sets the to of this ThesisPreparationPeriod.

        ISO-8601 timestamp  # noqa: E501

        :param to: The to of this ThesisPreparationPeriod.
        :type to: str
        """

        self._to = to
