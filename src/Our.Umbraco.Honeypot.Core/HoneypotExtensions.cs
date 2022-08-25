using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Our.Umbraco.Honeypot.Core
{
    public static class HoneypotExtensions
    {

        /// <summary>
        /// IsHoneypotTrapped
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static bool IsHoneypotTrapped(this HttpContext httpContext)
        {
            HoneypotService service = httpContext.RequestServices.GetRequiredService<HoneypotService>();

            return service.IsTrapped(httpContext);
        }
    }
}
