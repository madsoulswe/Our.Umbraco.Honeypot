using Microsoft.Extensions.DependencyInjection;


#if NETFRAMEWORK
using Umbraco.Core;
using Umbraco.Core.Composing;
#else
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
#endif

namespace Our.Umbraco.Honeypot.Core
{
    
    
    #if NETFRAMEWORK
    public class Composer : IComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<HoneypotOptions>(Lifetime.Scope);
            composition.Register<HoneypotService>(Lifetime.Scope);
        }
    }

    #else
    public class Composer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            
            builder.Services.Configure<HoneypotOptions>(builder.Config.GetSection("Our.Umbraco.Honeypot"));
            builder.Services.AddTransient<HoneypotService>();
        }
    }

    #endif
}
