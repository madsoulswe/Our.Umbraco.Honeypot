using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System;

namespace Our.Umbraco.Honeypot.Core
{
    public class HoneypotTimeTagHelper : TagHelper
    {
        public HoneypotTimeTagHelper(IOptions<HoneypotOptions> options)
        {
            Options = options.Value;
        }

        /// <summary>
        /// Options
        /// </summary>
        public HoneypotOptions Options { get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (!Options.HoneypotEnableTimeCheck)
            {
                return;
            }
                

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";

            output.Attributes.Add("class", $"{Options.HoneypotFieldClass} hp-{Options.HoneypotTimeFieldName}");
            output.Attributes.Add("style", Options.HoneypotFieldStyles);

            output.Content.AppendHtml($"<input type=\"hidden\" value=\"{DateTime.UtcNow.Ticks}\" name=\"{Options.HoneypotTimeFieldName}\" id=\"{Options.HoneypotTimeFieldName}\" />");
        }
    }
}
