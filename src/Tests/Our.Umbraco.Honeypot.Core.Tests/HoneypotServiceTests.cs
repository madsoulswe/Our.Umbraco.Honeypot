using NUnit.Framework;
using System.Web;
using System.Collections.Specialized;
using Our.Umbraco.Honeypot.Core;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Reflection;

#if NETFRAMEWORK
using HttpContext = System.Web.HttpContext;
using HttpRequest = System.Web.HttpRequest;
using HttpResponse = System.Web.HttpResponse;
#else
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;
#endif

namespace Our.Umbraco.Honeypot.Tests
{
    [TestFixture]
    public class HoneypotServiceTests
    {
        private HoneypotService _honeypotService;
        private HoneypotOptions _options;

        [SetUp]
        public void SetUp()
        {
            _options = new HoneypotOptions
            {
                HoneypotEnableFieldCheck = true,
                HoneypotEnableTimeCheck = true,
                HoneypotTimeFieldName = "honeypotTime",
                HoneypotMinTimeDuration = TimeSpan.FromSeconds(5),
            };

#if NETFRAMEWORK
            _honeypotService = new HoneypotService(_options);
#else
            var options = Options.Create(_options);
            _honeypotService = new HoneypotService(options);
#endif
        }

#if NETFRAMEWORK
        private void SetForm(HttpRequest request, NameValueCollection form)
        {
            var httpValueCollectionType = typeof(HttpRequest).Assembly.GetType("System.Web.HttpValueCollection");
            var formInstance = (NameValueCollection)Activator.CreateInstance(httpValueCollectionType, true);
            foreach (string key in form)
            {
                formInstance.Add(key, form[key]);
            }

            var formField = typeof(HttpRequest).GetField("_form", BindingFlags.NonPublic | BindingFlags.Instance);
            formField.SetValue(request, formInstance);
        }

        [Test]
        public void IsTrapped_ShouldReturnTrue_WhenFieldTrapIsTriggered_NETFramework()
        {
            // Arrange
            var context = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(null)
            );
            var form = new NameValueCollection
            {
                { $"{_options.HoneypotPrefixFieldName}Field", "notEmpty" }
            };
            SetForm(context.Request, form);

            // Act
            var result = _honeypotService.IsTrapped(context, out bool fieldTrap, out bool timeTrap);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(fieldTrap);
            Assert.IsFalse(timeTrap);
        }

        [Test]
        public void IsTrapped_ShouldReturnTrue_WhenTimeTrapIsTriggered_NETFramework()
        {
            // Arrange
            var context = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(null)
            );
            var timeValue = DateTime.UtcNow.AddSeconds(-1).Ticks.ToString();
            var form = new NameValueCollection
            {
                { _options.HoneypotTimeFieldName, timeValue }
            };
            SetForm(context.Request, form);

            // Act
            var result = _honeypotService.IsTrapped(context, out bool fieldTrap, out bool timeTrap);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(fieldTrap);
            Assert.IsTrue(timeTrap);
        }

        [Test]
        public void IsTrapped_ShouldReturnFalse_WhenNoTrapIsTriggered_NETFramework()
        {
            // Arrange
            var context = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(null)
            );
            var timeValue = DateTime.UtcNow.AddSeconds(-10).Ticks.ToString();
            var form = new NameValueCollection
            {
                { _options.HoneypotTimeFieldName, timeValue }
            };
            SetForm(context.Request, form);

            // Act
            var result = _honeypotService.IsTrapped(context, out bool fieldTrap, out bool timeTrap);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(fieldTrap);
            Assert.IsFalse(timeTrap);
        }
#endif

#if !NETFRAMEWORK
        [Test]
        public void IsTrapped_ShouldReturnTrue_WhenFieldTrapIsTriggered()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { $"{_options.HoneypotPrefixFieldName}Field", "notEmpty" }
            });

            // Act
            var result = _honeypotService.IsTrapped(context, out bool fieldTrap, out bool timeTrap);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(fieldTrap);
            Assert.IsFalse(timeTrap);
        }

        [Test]
        public void IsTrapped_ShouldReturnTrue_WhenTimeTrapIsTriggered()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var timeValue = DateTime.UtcNow.AddSeconds(-1).Ticks.ToString();
            context.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { _options.HoneypotTimeFieldName, timeValue }
            });

            // Act
            var result = _honeypotService.IsTrapped(context, out bool fieldTrap, out bool timeTrap);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(fieldTrap);
            Assert.IsTrue(timeTrap);
        }

        [Test]
        public void IsTrapped_ShouldReturnFalse_WhenNoTrapIsTriggered()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var timeValue = DateTime.UtcNow.AddSeconds(-10).Ticks.ToString();
            context.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { _options.HoneypotTimeFieldName, timeValue }
            });

            // Act
            var result = _honeypotService.IsTrapped(context, out bool fieldTrap, out bool timeTrap);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(fieldTrap);
            Assert.IsFalse(timeTrap);
        }
#endif
    }
}
