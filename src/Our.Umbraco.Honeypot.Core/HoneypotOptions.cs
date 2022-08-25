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
            HoneypotTimeFieldName = "__time";
            HoneypotMinTimeDuration = TimeSpan.FromSeconds(2);
            HoneypotFieldStyles = "display: none !important; position: absolute !important; left: -9000px !important;";
            HoneypotFieldClass = "hp-field";
            HoneypotFieldNames = new string[] { "Name", "Phone", "Comment", "Message", "Email", "Website" };
        }

        /// <summary>
        /// EnableFieldCheck
        /// </summary>
        public bool HoneypotEnableFieldCheck { get; set; }

        public string HoneypotFieldStyles { get; set; }

        public string HoneypotFieldClass { get; set; }

        public string[] HoneypotFieldNames { get; set; }

        /// <summary>
        /// EnableTimeCheck
        /// </summary>
        public bool HoneypotEnableTimeCheck { get; set; }

        /// <summary>
        /// PrefixFieldName
        /// </summary>
        public string HoneypotPrefixFieldName { get; set; }

        /// <summary>
        /// TimeFieldName
        /// </summary>
        public string HoneypotTimeFieldName { get; set; }

        /// <summary>
        /// MinTimeDuration
        /// </summary>
        public TimeSpan HoneypotMinTimeDuration { get; set; }

        internal bool HoneypotIsFieldName(string name)
        {
            return name.StartsWith($"{HoneypotPrefixFieldName}");
        }

        internal string HoneypotGetFieldName(string name)
        {
            return $"{HoneypotPrefixFieldName}{name}";
        }

    }
}
