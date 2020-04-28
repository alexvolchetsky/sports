﻿using Sports.Common.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;
using Yandex.Alice.Sdk.Models;
using Yandex.Alice.Sdk.Tests.TestsInfrastructure;

namespace Yandex.Alice.Sdk.Tests.Models
{
    public class AliceResponseTests : BaseTests
    {
        public AliceResponseTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void AliceResponse()
        {
            string json = File.ReadAllText(TestsConstants.Assets.AliceResponseFilePath);
            var aliceResponse = JsonSerializer.Deserialize<AliceResponse>(json);
            Assert.NotNull(aliceResponse);
            Assert.NotEmpty(aliceResponse.Version);
            Assert.NotNull(aliceResponse.SessionState);
            Assert.NotNull(aliceResponse.UserStateUpdate);
            Assert.NotNull(aliceResponse.Response);
            Assert.NotEmpty(aliceResponse.Response.Text);
            Assert.NotEmpty(aliceResponse.Response.Tts);
            Assert.True(aliceResponse.Response.EndSession);
            Assert.NotNull(aliceResponse.Response.Buttons);
            Assert.NotEmpty(aliceResponse.Response.Buttons);
            foreach (var button in aliceResponse.Response.Buttons)
            {
                Assert.NotEmpty(button.Title);
                Assert.NotNull(button.Payload);
                Assert.NotNull(button.Url);
                Assert.True(button.Hide);
            }
            WritePrettyJson(aliceResponse);
        }

        [Fact]
        public void AliceImageResponse()
        {
            string json = File.ReadAllText(TestsConstants.Assets.AliceImageResponseFilePath);
            var aliceResponse = JsonSerializer.Deserialize<AliceImageResponse>(json);
            Assert.NotNull(aliceResponse);
            Assert.NotEmpty(aliceResponse.Version);
            Assert.NotNull(aliceResponse.SessionState);
            Assert.NotNull(aliceResponse.UserStateUpdate);
            Assert.NotNull(aliceResponse.Response);
            Assert.NotEmpty(aliceResponse.Response.Text);
            Assert.NotEmpty(aliceResponse.Response.Tts);
            Assert.NotNull(aliceResponse.Response.Card);
            Assert.Equal(AliceCardType.BigImage, aliceResponse.Response.Card.Type);
            Assert.NotEmpty(aliceResponse.Response.Card.ImageId);
            Assert.NotEmpty(aliceResponse.Response.Card.Title);
            Assert.NotEmpty(aliceResponse.Response.Card.Description);
            Assert.NotNull(aliceResponse.Response.Card.Button);
            Assert.NotEmpty(aliceResponse.Response.Card.Button.Text);
            Assert.NotNull(aliceResponse.Response.Card.Button.Url);
            Assert.NotNull(aliceResponse.Response.Card.Button.Payload);
            Assert.True(aliceResponse.Response.EndSession);
            Assert.NotNull(aliceResponse.Response.Buttons);
            Assert.NotEmpty(aliceResponse.Response.Buttons);
            foreach (var button in aliceResponse.Response.Buttons)
            {
                Assert.NotEmpty(button.Title);
                Assert.NotNull(button.Payload);
                Assert.NotNull(button.Url);
                Assert.True(button.Hide);
            }
            WritePrettyJson(aliceResponse);
        }

        [Fact]
        public void AliceGalleryResponse()
        {
            string json = File.ReadAllText(TestsConstants.Assets.AliceGalleryResponseFilePath);
            var aliceResponse = JsonSerializer.Deserialize<AliceGalleryResponse>(json);
            Assert.NotNull(aliceResponse);
            Assert.NotEmpty(aliceResponse.Version);
            Assert.NotNull(aliceResponse.SessionState);
            Assert.NotNull(aliceResponse.UserStateUpdate);
            Assert.NotNull(aliceResponse.Response);
            Assert.NotEmpty(aliceResponse.Response.Text);
            Assert.NotEmpty(aliceResponse.Response.Tts);
            Assert.NotNull(aliceResponse.Response.Card);
            Assert.Equal(AliceCardType.ItemsList, aliceResponse.Response.Card.Type);
            Assert.NotNull(aliceResponse.Response.Card.Header);
            Assert.NotEmpty(aliceResponse.Response.Card.Header.Text);
            Assert.NotNull(aliceResponse.Response.Card.Items);
            Assert.NotEmpty(aliceResponse.Response.Card.Items);
            foreach (var item in aliceResponse.Response.Card.Items)
            {
                Assert.NotEmpty(item.ImageId);
                Assert.NotEmpty(item.Title);
                Assert.NotEmpty(item.Description);
                Assert.NotNull(item.Button);
                Assert.NotEmpty(item.Button.Text);
                Assert.NotNull(item.Button.Url);
                Assert.NotNull(item.Button.Payload);
            }
            Assert.NotNull(aliceResponse.Response.Card.Footer);
            Assert.NotEmpty(aliceResponse.Response.Card.Footer.Text);
            Assert.NotNull(aliceResponse.Response.Card.Footer.Button);
            Assert.NotEmpty(aliceResponse.Response.Card.Footer.Button.Text);
            Assert.NotNull(aliceResponse.Response.Card.Footer.Button.Url);
            Assert.NotNull(aliceResponse.Response.Card.Footer.Button.Payload);
            Assert.True(aliceResponse.Response.EndSession);
            Assert.NotNull(aliceResponse.Response.Buttons);
            Assert.NotEmpty(aliceResponse.Response.Buttons);
            foreach (var button in aliceResponse.Response.Buttons)
            {
                Assert.NotEmpty(button.Title);
                Assert.NotNull(button.Payload);
                Assert.NotNull(button.Url);
                Assert.True(button.Hide);
            }
            WritePrettyJson(aliceResponse);
        }
    }
}
