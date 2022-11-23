using Our.Umbraco.Honeypot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Core.Composing;
using Umbraco.Web.Composing;
using Umbraco.Core;

namespace Our.Umbraco.Honeypot
{
    public class HoneypotComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            global::Umbraco.Forms.Web.Controllers.UmbracoFormsController.FormValidate += FormsController_FormValidate;
        }
        
        private void FormsController_FormValidate(object sender, global::Umbraco.Forms.Mvc.FormValidationEventArgs e)
        {
            var options = global::Umbraco.Web.Composing.Current.Factory.GetInstance<HoneypotOptions>();
            
            if (!options.HoneypotEnableFieldCheck && !options.HoneypotEnableTimeCheck)
                return;

            var controller = sender as global::Umbraco.Forms.Web.Controllers.UmbracoFormsController;

            if (e.Context.IsHoneypotTrapped())
            {
                controller.ModelState.AddModelError("error", options.HoneypotMessage);
            }
        }
    }
}
