using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.Honeypot.Core
{
    public class HoneypotOptions
    {
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
        
        public bool HoneypotEnableFieldCheck { get; set; }

        public string HoneypotMessage { get; set; }

        public string HoneypotFieldStyles { get; set; }

        public string HoneypotFieldClass { get; set; }

        public string[] HoneypotFieldNames { get; set; }
        
        public bool HoneypotEnableTimeCheck { get; set; }
        
        public string HoneypotPrefixFieldName { get; set; }
        public string? HoneypotSuffixFieldName { get; set; }

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

    }
}
