# coding: utf-8

from __future__ import absolute_import
from datetime import date, datetime  # noqa: F401

from typing import List, Dict  # noqa: F401

from swagger_server.models.base_model_ import Model
from swagger_server.models.thesis import Thesis  # noqa: F401,E501
from swagger_server import util


class InlineResponse201(Model):
    """NOTE: This class is auto generated by the swagger code generator program.

    Do not edit the class manually.
    """
    def __init__(self, ok: Thesis=None):  # noqa: E501
        """InlineResponse201 - a model defined in Swagger

        :param ok: The ok of this InlineResponse201.  # noqa: E501
        :type ok: Thesis
        """
        self.swagger_types = {
            'ok': Thesis
        }

        self.attribute_map = {
            'ok': 'ok'
        }
        self._ok = ok

    @classmethod
    def from_dict(cls, dikt) -> 'InlineResponse201':
        """Returns the dict as a model

        :param dikt: A dict.
        :type: dict
        :return: The inline_response_201 of this InlineResponse201.  # noqa: E501
        :rtype: InlineResponse201
        """
        return util.deserialize_model(dikt, cls)

    @property
    def ok(self) -> Thesis:
        """Gets the ok of this InlineResponse201.


        :return: The ok of this InlineResponse201.
        :rtype: Thesis
        """
        return self._ok

    @ok.setter
    def ok(self, ok: Thesis):
        """Sets the ok of this InlineResponse201.


        :param ok: The ok of this InlineResponse201.
        :type ok: Thesis
        """

        self._ok = ok
