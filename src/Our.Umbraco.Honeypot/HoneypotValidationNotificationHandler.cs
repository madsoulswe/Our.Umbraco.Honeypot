using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Our.Umbraco.Honeypot.Core;
using System;
using Umbraco.Cms.Core.Events;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services.Notifications;

namespace Our.Umbraco.Honeypot
{

    public class HoneypotValidationNotificationHandler : INotificationHandler<FormValidateNotification>
    {
        public HoneypotValidationNotificationHandler(IOptions<HoneypotOptions> options)
        {
            Options = options.Value;
        }

        private HoneypotOptions Options { get; }
        
        public void Handle(FormValidateNotification notification)
        {
            if (!Options.HoneypotEnableFieldCheck && !Options.HoneypotEnableTimeCheck)
                return;

            if(notification.Context.IsHoneypotTrapped())
            {
                notification.ModelState.AddModelError("error", Options.HoneypotMessage);
            }
        }
    }
}
