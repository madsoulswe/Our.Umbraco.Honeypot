using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Our.Umbraco.Honeypot.Core
{
    public class Composer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.Configure<HoneypotOptions>(builder.Config.GetSection("Our.Umbraco.Honeypot"));
            builder.Services.AddTransient<HoneypotService>();
        }
    }
}
