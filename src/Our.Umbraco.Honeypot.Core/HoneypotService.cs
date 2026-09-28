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

        public HoneypotService(HoneypotOptions options)
        {
            Options = options;
        }

#if !NETFRAMEWORK
        public HoneypotService(IOptions<HoneypotOptions> options)
        {
            Options = options.Value;
        }
#endif

        public bool IsTrapped(HttpContext httpContext)
        {
            return IsTrapped(httpContext, out _, out _);
        }

        public bool IsTrapped(HttpContext httpContext, out bool fieldTrap, out bool timeTrap)
        {
            fieldTrap = false;
            timeTrap = false;

#if NETFRAMEWORK

            if (httpContext.Request.ContentType != "application/x-www-form-urlencoded" && httpContext.Request.ContentType != "multipart/form-data")
            {
                return false;
            }
#else
            if (!httpContext.Request.HasFormContentType)
            {
                return false;
            }
#endif

#if NETFRAMEWORK
            if (!httpContext.Items.Contains(HttpContextItemName) || (httpContext.Items[HttpContextItemName] is bool value) == false)
#else
            if (httpContext.Items.TryGetValue(HttpContextItemName, out object? value) == false)
#endif
            {
                bool trapped = false;

                if (Options.HoneypotEnableFieldCheck)
                {
#if NETFRAMEWORK
                    foreach (var inputKey in httpContext.Request.Form.AllKeys)
                    {
                        if (IsFieldTrapTriggered(inputKey, httpContext.Request.Form[inputKey]))
                        {
                            fieldTrap = true;
                            trapped = true;
                            break;
                        }
                    }
#else
                    if (httpContext.Request.Form.Any(x => IsFieldTrapTriggered(x.Key, x.Value)))
                    {
                        fieldTrap = true;
                        trapped = true;
                    }
#endif
                }

                if (Options.HoneypotEnableTimeCheck && !trapped)
                {
#if NETFRAMEWORK
                    if (httpContext.Request.Form[Options.HoneypotTimeFieldName] is string timeValue)
                    {
                        if (IsTimeTrapTriggered(timeValue))
                        {
                            timeTrap = true;
                            trapped = true;
                        }
                    }
#else
                    if (httpContext.Request.Form.TryGetValue(Options.HoneypotTimeFieldName, out StringValues timeValues) && timeValues.Any())
                    {
                        if (IsTimeTrapTriggered(timeValues.First()))
                        {
                            timeTrap = true;
                            trapped = true;
                        }
                    }
#endif
                }

                httpContext.Items.Add(HttpContextItemName, trapped);

                return trapped;
            }
            else
            {
                return (bool)value;
            }
        }

        public bool IsFieldTrapTriggered(string key, string value)
        {
            return Options.HoneypotIsFieldName(key) && !string.IsNullOrEmpty(value);
        }

#if !NETFRAMEWORK
        public bool IsFieldTrapTriggered(string key, StringValues values)
        {
            return Options.HoneypotIsFieldName(key) && values.Any(v => !string.IsNullOrEmpty(v));
        }
#endif

        public bool IsTimeTrapTriggered(string timeValue)
        {
            TimeSpan diff = DateTime.UtcNow - new DateTime(long.Parse(timeValue), DateTimeKind.Utc);
            return diff < Options.HoneypotMinTimeDuration;
        }
    }
}