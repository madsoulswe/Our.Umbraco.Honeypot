using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Our.Umbraco.Honeypot.Core
{
    public class HoneypotOptions
    {
        #if NETFRAMEWORK
        public HoneypotOptions()
        {
            HoneypotEnableFieldCheck = Convert.ToBoolean(ConfigurationManager.AppSettings["HoneypotEnableFieldCheck"] ?? "true");
            HoneypotEnableTimeCheck = Convert.ToBoolean(ConfigurationManager.AppSettings["HoneypotEnableTimeCheck"] ?? "true"); ;
            HoneypotPrefixFieldName = ConfigurationManager.AppSettings["HoneypotPrefixFieldName"] ?? "hp_";
            HoneypotSuffixFieldName = ConfigurationManager.AppSettings["HoneypotSuffixFieldName"] ?? "";
            HoneypotTimeFieldName = ConfigurationManager.AppSettings["HoneypotTimeFieldName"] ?? "__time";
            HoneypotMinTimeDuration = TimeSpan.Parse(ConfigurationManager.AppSettings["HoneypotMinTimeDuration"] ?? "00:00:02");
            HoneypotFieldStyles = ConfigurationManager.AppSettings["HoneypotFieldStyles"] ?? "display: none !important; position: absolute !important; left: -9000px !important;";
            HoneypotFieldClass = ConfigurationManager.AppSettings["HoneypotFieldClass"] ?? "hp-field";
            HoneypotFieldNames = ConfigurationManager.AppSettings["HoneypotFieldNames"]?.Split(',') ?? new string[] { "Name", "Phone", "Comment", "Message", "Email", "Website" };
            HoneypotMessage = ConfigurationManager.AppSettings["HoneypotMessage"] ?? "Something went wrong (HP)";
        }
        #endif

        #if NET5_0_OR_GREATER
        public HoneypotOptions()
        {
            HoneypotEnableFieldCheck = true;
            HoneypotEnableTimeCheck = true;
            HoneypotPrefixFieldName = "hp_";
            HoneypotSuffixFieldName = "";
            HoneypotTimeFieldName = "__time";
            HoneypotMinTimeDuration = TimeSpan.FromSeconds(2);
            HoneypotFieldStyles = "display: none !important; position: absolute !important; left: -9000px !important;";
            HoneypotFieldClass = "hp-field";
            HoneypotFieldNames = new string[] { "Name", "Phone", "Comment", "Message", "Email", "Website" };
            HoneypotMessage = "Something went wrong (HP)";

        }
        #endif

        public bool HoneypotEnableFieldCheck { get; set; }

        public string HoneypotMessage { get; set; }

        public string HoneypotFieldStyles { get; set; }

        public string HoneypotFieldClass { get; set; }

        public string[] HoneypotFieldNames { get; set; }
        
        public bool HoneypotEnableTimeCheck { get; set; }
        
        public string HoneypotPrefixFieldName { get; set; }

        #if NETFRAMEWORK
        public string HoneypotSuffixFieldName { get; set; }
        #else
        public string? HoneypotSuffixFieldName { get; set; }
        #endif

        public string HoneypotTimeFieldName { get; set; }
        
        public TimeSpan HoneypotMinTimeDuration { get; set; }

        internal bool HoneypotIsFieldName(string name)
        {
            if(!string.IsNullOrWhiteSpace(HoneypotPrefixFieldName))
                return name.StartsWith($"{HoneypotPrefixFieldName}");

            if (!string.IsNullOrWhiteSpace(HoneypotSuffixFieldName))
                return name.EndsWith($"{HoneypotSuffixFieldName}");

            return false;
        }

        internal string HoneypotGetFieldName(string name)
        {
            return $"{HoneypotPrefixFieldName}{name}";
        }

        public string RandomName()
        {
            return HoneypotFieldNames[new Random().Next(0, HoneypotFieldNames.Length)];
        }

    }
}
