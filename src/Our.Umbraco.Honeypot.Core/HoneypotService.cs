using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Net.Http;

namespace Our.Umbraco.Honeypot.Core
{
    public class HoneypotService
    {
        public const string HttpContextItemName = "Our.Umbraco.Honeypot.IsHoneypotTrapped";

        public HoneypotService(IOptions<HoneypotOptions> options)
        {
            Options = options.Value;
        }
        
        private HoneypotOptions Options { get; }
        
        public bool IsTrapped(HttpContext httpContext)
        {
            if (httpContext.Items.TryGetValue(HttpContextItemName, out object? value) == false)
            {
                bool trapped = false;

                if (Options.HoneypotEnableFieldCheck)
                {
                    //check fields
                    trapped = httpContext.Request.Form.Any(x => Options.HoneypotIsFieldName(x.Key) && x.Value.Any(v => string.IsNullOrEmpty(v) == false));
                }

                if (Options.HoneypotEnableTimeCheck && !trapped)
                {
                    //check time
                    if (httpContext.Request.Form.TryGetValue(Options.HoneypotTimeFieldName, out StringValues timeValues))
                    {
                        if (timeValues.Any())
                        {
                            TimeSpan diff = DateTime.UtcNow - new DateTime(long.Parse(timeValues.First()), DateTimeKind.Utc);

                            trapped = diff < Options.HoneypotMinTimeDuration;
                        }
                    }
                }

                httpContext.Items.Add(HttpContextItemName, trapped);

                return trapped;
            }
            else
            {
                return (bool)value;
            }
        }
    }
}
