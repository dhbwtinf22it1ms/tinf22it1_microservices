# coding: utf-8

from __future__ import absolute_import

from flask import json
from six import BytesIO

from swagger_server.models.acceptnotify_body import AcceptnotifyBody  # noqa: E501
from swagger_server.models.changenotify_body import ChangenotifyBody  # noqa: E501
from swagger_server.models.deadlinereached_body import DeadlinereachedBody  # noqa: E501
from swagger_server.models.generic_body import GenericBody  # noqa: E501
from swagger_server.models.registerpnotify_body import RegisterpnotifyBody  # noqa: E501
from swagger_server.models.rejectnotify_body import RejectnotifyBody  # noqa: E501
from swagger_server.models.submitnotify_body import SubmitnotifyBody  # noqa: E501
from swagger_server.test import BaseTestCase


class TestDefaultController(BaseTestCase):
    """DefaultController integration test stubs"""

    def test_acceptnotify_post(self):
        """Test case for acceptnotify_post

        Trigger notification for proposal acceptance
        """
        body = AcceptnotifyBody()
        response = self.client.open(
            '/acceptnotify',
            method='POST',
            data=json.dumps(body),
            content_type='application/json')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_changenotify_post(self):
        """Test case for changenotify_post

        Trigger notification for changes in a proposal
        """
        body = ChangenotifyBody()
        response = self.client.open(
            '/changenotify',
            method='POST',
            data=json.dumps(body),
            content_type='application/json')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_deadlinereached_post(self):
        """Test case for deadlinereached_post

        Trigger notification for deadline reached
        """
        body = DeadlinereachedBody()
        response = self.client.open(
            '/deadlinereached',
            method='POST',
            data=json.dumps(body),
            content_type='application/json')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_generic_post(self):
        """Test case for generic_post

        Trigger a generic notification
        """
        body = GenericBody()
        response = self.client.open(
            '/generic',
            method='POST',
            data=json.dumps(body),
            content_type='application/json')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_registerpnotify_post(self):
        """Test case for registerpnotify_post

        Trigger notification for sign-up with login details
        """
        body = RegisterpnotifyBody()
        response = self.client.open(
            '/registerpnotify',
            method='POST',
            data=json.dumps(body),
            content_type='application/json')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_rejectnotify_post(self):
        """Test case for rejectnotify_post

        Trigger notification for proposal rejection
        """
        body = RejectnotifyBody()
        response = self.client.open(
            '/rejectnotify',
            method='POST',
            data=json.dumps(body),
            content_type='application/json')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_submitnotify_post(self):
        """Test case for submitnotify_post

        Trigger notification for successful proposal upload
        """
        body = SubmitnotifyBody()
        response = self.client.open(
            '/submitnotify',
            method='POST',
            data=json.dumps(body),
            content_type='application/json')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))


if __name__ == '__main__':
    import unittest
    unittest.main()
