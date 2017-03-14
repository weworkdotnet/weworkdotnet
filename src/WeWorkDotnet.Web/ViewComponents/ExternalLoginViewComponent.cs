using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WeWorkDotnet.Web.Models;

namespace WeWorkDotnet.Web.ViewComponents
{
    public class ExternalLoginViewComponent : ViewComponent
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ExternalLoginViewComponent(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string displayText)
        {
            ViewData["DisplayText"] = displayText;
            var loginProviders = _signInManager.GetExternalAuthenticationSchemes().ToList();
            return View(loginProviders);
        }
    }
}
