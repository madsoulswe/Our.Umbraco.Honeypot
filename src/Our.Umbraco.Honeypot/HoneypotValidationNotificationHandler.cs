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
        private readonly HoneypotService _honeypotService;
        private readonly HoneypotOptions _options;

        public HoneypotValidationNotificationHandler(HoneypotService honeypotService, IOptions<HoneypotOptions> options)
        {
            _honeypotService = honeypotService;
            _options = options.Value;
        }


        public void Handle(FormValidateNotification notification)
        {
            if (!_options.HoneypotEnableFieldCheck && !_options.HoneypotEnableTimeCheck)
                return;

            if(notification.Context.IsHoneypotTrapped())
            {
                notification.ModelState.AddModelError("error", _options.HoneypotMessage);
            }
        }
    }
}
