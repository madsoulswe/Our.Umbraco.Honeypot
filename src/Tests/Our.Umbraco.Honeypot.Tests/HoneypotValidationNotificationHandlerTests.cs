using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Our.Umbraco.Honeypot;
using Our.Umbraco.Honeypot.Core;
using System.Collections.Generic;
using Umbraco.Cms.Core.Events;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services.Notifications;

namespace Our.Umbraco.Honeypot.Tests
{
    [TestFixture]
    public class HoneypotValidationNotificationHandlerTests
    {
        private HoneypotOptions _honeypotOptions;
        private HoneypotValidationNotificationHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _honeypotOptions = new HoneypotOptions
            {
                HoneypotEnableFieldCheck = true,
                HoneypotEnableTimeCheck = true,
                HoneypotMessage = "Something went wrong (HP)"
            };

            var options = Options.Create(_honeypotOptions);
            var honeypotService = new HoneypotService(_honeypotOptions);

            _handler = new HoneypotValidationNotificationHandler(honeypotService, options);
        }

        [Test]
        public void Handle_ShouldAddModelError_WhenHoneypotIsTrapped()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "honeypotField", "some value" }
            });
            httpContext.Request.Form = formCollection;

            var modelState = new ModelStateDictionary();
            var form = new Form();
            var messages = new EventMessages();
            var notification = new FormValidateNotification(form, messages, httpContext, modelState);

            // Act
            _handler.Handle(notification);

            // Assert
            Assert.IsTrue(notification.ModelState.ContainsKey("error"));
            Assert.AreEqual(_honeypotOptions.HoneypotMessage, notification.ModelState["error"].Errors[0].ErrorMessage);
        }

        [Test]
        public void Handle_ShouldNotAddModelError_WhenHoneypotIsNotTrapped()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            httpContext.Request.Form = formCollection;

            var modelState = new ModelStateDictionary();
            var form = new Form();
            var messages = new EventMessages();
            var notification = new FormValidateNotification(form, messages, httpContext, modelState);

            // Act
            _handler.Handle(notification);

            // Assert
            Assert.IsFalse(notification.ModelState.ContainsKey("error"));
        }

    }
}