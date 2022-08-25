using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Services.Notifications;

namespace Our.Umbraco.Honeypot
{
    public class HoneypotComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<FormValidateNotification, HoneypotValidationNotificationHandler>();
        }
    }
}
