using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.Honeypot.Core
{
    public class HoneypotFieldTagHelper : TagHelper
    {
        public HoneypotFieldTagHelper(IOptions<HoneypotOptions> options)
        {
            Name = null;
            Type = "text";
            Options = options.Value;
        }
        public HoneypotOptions Options { get; }
        
        public string Name { get; set; }

        public string Type { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";

            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = Options.HoneypotFieldNames[new Random().Next(0, Options.HoneypotFieldNames.Length)];
            }

            string fieldName = Options.HoneypotGetFieldName(Name);


            output.Attributes.Add("class", $"{Options.HoneypotFieldClass} {fieldName}");
            output.Attributes.Add("style", Options.HoneypotFieldStyles);

            output.Content.AppendHtml($"<input type=\"{Type}\" name=\"{fieldName}\" id=\"{fieldName}\" />");
        }
    }
}
