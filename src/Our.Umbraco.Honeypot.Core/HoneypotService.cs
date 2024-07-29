
using System.Web;

#if NETFRAMEWORK

#else
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
#endif

using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;

namespace Our.Umbraco.Honeypot.Core
{
    public class HoneypotService
    {
        public const string HttpContextItemName = "Our.Umbraco.Honeypot.IsHoneypotTrapped";

        private HoneypotOptions Options { get; }


        public bool IsTrapped(HttpContext httpContext)
        {
            return IsTrapped(httpContext);
        }

#if NETFRAMEWORK
            public HoneypotService(HoneypotOptions options)
        {
            Options = options;
        }

        public bool IsTrapped(HttpContext httpContext, out bool fieldTrap, out bool timeTrap)
        {
            fieldTrap = false;
            timeTrap = false;

            if (httpContext.Request.ContentType != "application/x-www-form-urlencoded" && httpContext.Request.ContentType != "multipart/form-data")
            {
                return false;
            }
            
            if (!httpContext.Items.Contains(HttpContextItemName) || (httpContext.Items[HttpContextItemName] is bool value) == false)
            {
                
                bool trapped = false;

                if (Options.HoneypotEnableFieldCheck)
                {
                    //check fields
                    foreach(var inputKey in httpContext.Request.Form.AllKeys)
                    {
                        if (Options.HoneypotIsFieldName(inputKey) && !string.IsNullOrEmpty(httpContext.Request.Form[inputKey]))
                        {
                            fieldTrap = true;
                            trapped = true;
                            break;
                        }
                    }
                }

                if (Options.HoneypotEnableTimeCheck && !trapped)
                {
                    //check time
                    if (httpContext.Request.Form[Options.HoneypotTimeFieldName] is string timeValue)
                    {
                        TimeSpan diff = DateTime.UtcNow - new DateTime(long.Parse(timeValue), DateTimeKind.Utc);

                        timeTrap = true;
                        trapped = diff < Options.HoneypotMinTimeDuration;
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

#else
        public HoneypotService(IOptions<HoneypotOptions> options)
        {
            Options = options.Value;
        }

        public bool IsTrapped(HttpContext httpContext, out bool fieldTrap, out bool timeTrap)
        {
            fieldTrap = false;
            timeTrap = false;

            if (!httpContext.Request.HasFormContentType)
            {
                //Fallback when Request.Form is missing (Umbraco Forms API)
                return false;
            }

            if (httpContext.Items.TryGetValue(HttpContextItemName, out object? value) == false)
            {

                bool trapped = false;

                if (Options.HoneypotEnableFieldCheck)
                {
                    //check fields
                    fieldTrap = true;
                    trapped = httpContext.Request.Form.Any(x => Options.HoneypotIsFieldName(x.Key) && x.Value.Any(v => !string.IsNullOrEmpty(v)));
                }

                if (Options.HoneypotEnableTimeCheck && !trapped)
                {
                    //check time
                    if (httpContext.Request.Form.TryGetValue(Options.HoneypotTimeFieldName, out StringValues timeValues))
                    {
                        if (timeValues.Any())
                        {
                            TimeSpan diff = DateTime.UtcNow - new DateTime(long.Parse(timeValues.First()), DateTimeKind.Utc);

                            timeTrap = true;
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
#endif
    }
}
