using NUnit.Framework;
using Our.Umbraco.Honeypot.Core;
using System;
using System.Configuration;

namespace Our.Umbraco.Honeypot.Tests
{
    [TestFixture]
    public class HoneypotOptionsTests
    {
        [Test]
        public void HoneypotOptions_ShouldInitializeWithDefaultValues_NETFramework()
        {
#if NETFRAMEWORK
            var options = new HoneypotOptions();

            Assert.IsTrue(options.HoneypotEnableFieldCheck);
            Assert.IsTrue(options.HoneypotEnableTimeCheck);
            Assert.AreEqual("hp_", options.HoneypotPrefixFieldName);
            Assert.AreEqual("", options.HoneypotSuffixFieldName);
            Assert.AreEqual("__time", options.HoneypotTimeFieldName);
            Assert.AreEqual(TimeSpan.FromSeconds(2), options.HoneypotMinTimeDuration);
            Assert.AreEqual("display: none !important; position: absolute !important; left: -9000px !important;", options.HoneypotFieldStyles);
            Assert.AreEqual("hp-field", options.HoneypotFieldClass);
            CollectionAssert.AreEqual(new string[] { "Name", "Phone", "Comment", "Message", "Email", "Website" }, options.HoneypotFieldNames);
            Assert.AreEqual("Something went wrong (HP)", options.HoneypotMessage);
#endif
        }

        [Test]
        public void HoneypotOptions_ShouldInitializeWithDefaultValues_NET5_0_OR_GREATER()
        {
#if NET5_0_OR_GREATER
            var options = new HoneypotOptions();

            Assert.IsTrue(options.HoneypotEnableFieldCheck);
            Assert.IsTrue(options.HoneypotEnableTimeCheck);
            Assert.AreEqual("hp_", options.HoneypotPrefixFieldName);
            Assert.AreEqual("", options.HoneypotSuffixFieldName);
            Assert.AreEqual("__time", options.HoneypotTimeFieldName);
            Assert.AreEqual(TimeSpan.FromSeconds(2), options.HoneypotMinTimeDuration);
            Assert.AreEqual("display: none !important; position: absolute !important; left: -9000px !important;", options.HoneypotFieldStyles);
            Assert.AreEqual("hp-field", options.HoneypotFieldClass);
            CollectionAssert.AreEqual(new string[] { "Name", "Phone", "Comment", "Message", "Email", "Website" }, options.HoneypotFieldNames);
            Assert.AreEqual("Something went wrong (HP)", options.HoneypotMessage);
#endif
        }

        [Test]
        public void HoneypotIsFieldName_ShouldReturnTrue_WhenPrefixMatches()
        {
            var options = new HoneypotOptions
            {
                HoneypotPrefixFieldName = "hp_"
            };

            Assert.IsTrue(options.HoneypotIsFieldName("hp_test"));
        }

        [Test]
        public void HoneypotIsFieldName_ShouldReturnTrue_WhenSuffixMatches()
        {
            var options = new HoneypotOptions
            {
                HoneypotSuffixFieldName = "_hp"
            };

            Assert.IsTrue(options.HoneypotIsFieldName("test_hp"));
        }

        [Test]
        public void HoneypotIsFieldName_ShouldReturnFalse_WhenNoMatch()
        {
            var options = new HoneypotOptions
            {
                HoneypotPrefixFieldName = "hp_",
                HoneypotSuffixFieldName = "_hp"
            };

            Assert.IsFalse(options.HoneypotIsFieldName("test"));
        }

        [Test]
        public void HoneypotGetFieldName_ShouldReturnFieldNameWithPrefix()
        {
            var options = new HoneypotOptions
            {
                HoneypotPrefixFieldName = "hp_"
            };

            Assert.AreEqual("hp_test", options.HoneypotGetFieldName("test"));
        }

        [Test]
        public void RandomName_ShouldReturnOneOfTheFieldNames()
        {
            var options = new HoneypotOptions
            {
                HoneypotFieldNames = new string[] { "Name", "Phone", "Comment" }
            };

            var randomName = options.RandomName();
            CollectionAssert.Contains(options.HoneypotFieldNames, randomName);
        }
    }
}
