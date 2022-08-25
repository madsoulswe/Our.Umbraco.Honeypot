using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.Honeypot.Core
{
    public class HoneypotAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            bool isTrapped = context.HttpContext.IsHoneypotTrapped();

            if (isTrapped == true)
            {
                context.Result = new ContentResult() { Content = "bot detection", ContentType = "text/plain", StatusCode = (int)HttpStatusCode.OK };
            }
        }
    }
}
