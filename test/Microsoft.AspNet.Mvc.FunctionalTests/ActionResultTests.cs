// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ActionResultsWebSite;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.TestHost;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class ActionResultTests
    {
        private readonly IServiceProvider _provider = TestHelper.CreateServices("ActionResultsWebSite");
        private readonly Action<IApplicationBuilder> _app = new Startup().Configure;

        [Theory]
        [InlineData("application/json;charset=utf-8",
            "{\"test.SampleInt\":[\"The field SampleInt must be between 10 and 100.\"]," +
            "\"test.SampleString\":" +
            "[\"The field SampleString must be a string or array type with a minimum length of '15'.\"]}")]
        [InlineData("application/xml;charset=utf-8",
            "<Error><test.SampleInt>The field SampleInt must be between 10 and 100.</test.SampleInt>" +
            "<test.SampleString>The field SampleString must be a string or array type with a minimum length of '15'." +
            "</test.SampleString></Error>")]
        public async Task SerializableErrorIsReturnedInExpectedFormat(string outputFormat, string output)
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();

            var input = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<DummyClass xmlns=\"http://schemas.datacontract.org/2004/07/ActionResultsWebSite\">" +
                "<SampleInt>2</SampleInt><SampleString>foo</SampleString></DummyClass>";
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/Home/Index");
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(outputFormat));
            request.Content = new StringContent(input, Encoding.UTF8, "application/xml");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(output, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task SerializableError_CanSerializeNormalObjects()
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();

            var input = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<DummyClass xmlns=\"http://schemas.datacontract.org/2004/07/ActionResultsWebSite\">" +
                "<SampleInt>2</SampleInt><SampleString>foo</SampleString></DummyClass>";
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/Home/GetCustomErrorObject");
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;charset=utf-8"));
            request.Content = new StringContent(input, Encoding.UTF8, "application/xml");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("[\"Something went wrong with the model.\"]",
                await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task SerializableError_CanReturnEmptyBadRequestResult()
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/Home/GetBadResult");
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json;charset=utf-8"));

            // Act
            var response = await client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task SerializableError_ReadTheReturnedXml()
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();

            var input = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<DummyClass xmlns=\"http://schemas.datacontract.org/2004/07/ActionResultsWebSite\">" +
                "<SampleInt>20</SampleInt><SampleString>foo</SampleString></DummyClass>";

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/Home/Index");
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/xml;charset=utf-8"));
            request.Content = new StringContent(input, Encoding.UTF8, "application/xml");

            // Act
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Deserializing Xml content
            var serializer = new XmlSerializer(typeof(SerializableError));
            var errors = (SerializableError)serializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(responseContent)));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(
                "<Error><test.SampleString>The field SampleString must be a string or array " +
                "type with a minimum length of '15'.</test.SampleString></Error>",
                responseContent);
            Assert.Equal("The field SampleString must be a string or array " +
                "type with a minimum length of '15'.", errors["test.SampleString"]);
        }
    }
}