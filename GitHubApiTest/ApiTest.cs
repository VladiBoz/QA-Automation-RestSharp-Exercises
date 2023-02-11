using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using RestSharp;
using RestSharp.Authenticators;
using System.Collections.Immutable;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace GitHubApiTest
{
    public class ApiTest
    {
        private RestClient client;
        private const string baseUrl = "https://api.github.com";
        private const string partialUrl = "repos/VladiBoz/Repo-for-exercises-from-Postman/issues";
        private const string username = "VladiBoz";
        private const string password = "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";

        [SetUp]

        public void Setup()
        {
            this.client = new RestClient(baseUrl);
            this.client.Authenticator = new HttpBasicAuthenticator(username, password);

        }



        [Test]
        public void TestGetSignleIssue()
        {
            var request = new RestRequest($"{partialUrl}/1", Method.Get);
            var response = this.client.Execute(request);



            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var issue = JsonSerializer.Deserialize<Issue>(response.Content);


            Assert.That(issue.title, Is.EqualTo("Issue created from Postman"));
            Assert.That(issue.number, Is.EqualTo(1));

        }
        [Test]
        public void TestGetAllIsues()
        {
            var request = new RestRequest($"{partialUrl}", Method.Get);
            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var issues = JsonSerializer.Deserialize<List<Issue>>(response.Content);

            foreach (var issue in issues)
            {

                Assert.That(issue.title, Is.Not.Empty);
                Assert.That(issue.number, Is.GreaterThan(0));
            }
        }
        [Test]
        public void TestCreateNewIssue()

        {
            var request = new RestRequest(partialUrl, Method.Post);

            var issueBody = new
            {
                title = "Test Issue from RestSharp",
                body = "Some body for my issue",
                labels = new string[] { "bug", "critical" }
            };
        
             request.AddBody(issueBody);


            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var issue = JsonSerializer.Deserialize<Issue>(response.Content);
            Assert.That(issue.title, Is.Not.Empty);
            Assert.That(issue.number, Is.GreaterThan(0));
            Assert.That(issue.title, Is.EqualTo(issueBody.title));
            Assert.That(issue.body, Is.EqualTo(issueBody.body));



            }





        [TestCase("US","90210","United States")]
        [TestCase("BG", "8600", "Bulgaria")]
        [TestCase("CA", "M5S", "Canada")]
        [TestCase("DE", "01067","Germany")]
          

        public void Test_Zuppotamus_DD(string countryCode, string zipCode, string expetedCountry)
        {
            var restClient = new RestClient("http://api.zippopotam.us");
            var request = new RestRequest (countryCode + "/" + zipCode, Method.Get);

            var response = restClient.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var location = JsonSerializer.Deserialize<Locations>(response.Content);
            Assert.That(location.country, Is.EqualTo(expetedCountry));


        }



    }
    }
