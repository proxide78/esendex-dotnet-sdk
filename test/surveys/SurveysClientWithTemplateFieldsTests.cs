﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.esendex.sdk.authenticators;
using com.esendex.sdk.surveys;
using com.esendex.sdk.test.mockapi;
using com.esendex.sdk.test.models.requests;
using Newtonsoft.Json;
using NUnit.Framework;

namespace com.esendex.sdk.test.surveys
{
    [TestFixture]
    public class SurveysClientWithTemplateFieldsTests
    {
        private readonly Version _version = Assembly.GetAssembly(typeof(SurveysClient)).GetName().Version;
        private Guid _surveyId;
        private string _recipient;
        private Dictionary<string, string> _templateFields;
        private mockapi.Request _request;

        [TestFixtureSetUp]
        public void Given()
        {
            const string username = "user@example.com";
            const string password = "heythiscantbeguessed";
            _recipient = "44123456789";
            _surveyId = Guid.NewGuid();

            MockApi.SetEndpoint(new MockEndpoint(200, "", "text/plain"));

            var surveysClient = new SurveysClient(MockApi.Url, new BasicAuthenticator(username, password));
            _templateFields = new Dictionary<string, string> { {"KEK", "Toppest"}};

            surveysClient.AddRecipient(_surveyId, _recipient , _templateFields);
            _request = MockApi.LastRequest;
        }

        [Test]
        public void ThenTheExpectedRequestIsMade()
        {
            Assert.That(_request.Method, Is.EqualTo("POST"));
            Assert.That(_request.Url, Is.EqualTo(string.Format("/v1.0/surveys/{0}/send", _surveyId)));
        }

        [Test]
        public void ThenTheRequestHasTheExpectedBody()
        {
            var body = JsonConvert.DeserializeObject<SurveyAddRecipientsRequest>(_request.Body);
            var recipient = body.Recipients.First();

            Assert.That(body.Recipients.Count, Is.EqualTo(1));
            Assert.That(recipient.PhoneNumber, Is.EqualTo(_recipient));
            CollectionAssert.AreEquivalent(new Dictionary<string, string> { { "KEK", "Toppest" } }, recipient.TemplateFields);
        }

        [Test]
        public void ThenTheRequestHasTheExpectedHeaders()
        {
            Assert.That(_request.Headers["Accept"], Is.EqualTo("application/json; charset=utf-8"));
            Assert.That(_request.Headers["Authorization"], Is.EqualTo("Basic dXNlckBleGFtcGxlLmNvbTpoZXl0aGlzY2FudGJlZ3Vlc3NlZA=="));
            Assert.That(_request.Headers["Content-Type"], Is.EqualTo("application/json; charset=utf-8"));
            Assert.That(_request.Headers["User-Agent"], Is.EqualTo(string.Format("Esendex .NET SDK v{0}.{1}.{2}", _version.Major, _version.Minor, _version.Build)));
        }
    }
}