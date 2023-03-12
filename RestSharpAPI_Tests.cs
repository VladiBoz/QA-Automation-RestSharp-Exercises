using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using RestSharp;
using System;
using System.Net;
using System.Text.Json;

namespace RestSharpAPITests
{


    public class RestSharpAPI_Tests
    {
        private RestClient client;
        private const string baseUrl = "https://exam-app.vladislavbozvel.repl.co";

        [SetUp]
        public void Setup()
        {
            this.client = new RestClient(baseUrl);
        }

        [Test]
        public void ListAllContacts()
        {
            //Arrange
            var request = new RestRequest("/api/contacts", Method.Get);

            //Act
            var response = this.client.Execute(request);

            //Assert
            var contact = JsonSerializer.Deserialize<List<contact>>(response.Content);

            Assert.That(contact[0].firstName, Is.EqualTo("Steve"));
            Assert.That(contact[0].lastName, Is.EqualTo("Jobs"));


        }
        [Test]
        public void ListContactByKeyword()
        {
            //Arrange
            var request = new RestRequest("/api/contacts/search/alber", Method.Get);

            //Act
            var response = this.client.Execute(request);

            //Assert
            var contact = JsonSerializer.Deserialize<List<contact>>(response.Content);

            Assert.That(contact[0].lastName, Is.EqualTo("Einstein"));

        }
        [Test]
        public void GetContactByInvalidKeyword()
        {
            //Arrange
            var request = new RestRequest("/api/contacts/search/missing" + DateTime.Now.Ticks, Method.Get);

            //Act
            var response = this.client.Execute(request);

            //Assert
            Assert.That(response.Content, Is.EqualTo("[]"));

        }
        [Test]
        public void CreateContactWithInvalidData()
        {
            //Arrange
            var request = new RestRequest("/api/contacts", Method.Post);
            var reqBody = new
            {
                lastName = "Curie",
                email = "marie67@gmail.com",
                phone = "+1 800 200 300",
                comments = "Old friend"
            };
            request.AddBody(reqBody);

            //Act
            var response = this.client.Execute(request);

            //Assert

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Is.EqualTo("{\"errMsg\":\"First name cannot be empty!\"}"));


        }
        [Test]
        public void CreateContactWithValidData()
        {
            //Arrange
            var request = new RestRequest("/api/contacts/", Method.Post);
            var reqBody = new
            {
                firstName = "Some name" + DateTime.Now.Ticks,
                lastName = "Curie",
                email = "marie67@gmail.com",
                phone = "+1 800 200 300",
                comments = "Old friend"
            };
            request.AddBody(reqBody);

            //Act
            var response = this.client.Execute(request);

            //Assert

            var contactObject = JsonSerializer.Deserialize<contactObject>(response.Content);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(contactObject.msg, Is.EqualTo("Contact added."));
            Assert.That(contactObject.contact.id, Is.GreaterThan(0));
            Assert.That(contactObject.contact.firstName, Is.EqualTo(reqBody.firstName));
            Assert.That(contactObject.contact.lastName, Is.EqualTo(reqBody.lastName));
            Assert.That(contactObject.contact.email, Is.EqualTo("marie67@gmail.com"));
            Assert.That(contactObject.contact.phone, Is.EqualTo("+1 800 200 300"));
            Assert.That(contactObject.contact.dateCreated, Is.Not.Empty);
            Assert.That(contactObject.contact.comments, Is.EqualTo("Old friend"));
        }





    }
}