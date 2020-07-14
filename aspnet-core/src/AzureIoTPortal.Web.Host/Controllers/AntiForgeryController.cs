using Microsoft.AspNetCore.Antiforgery;
using AzureIoTPortal.Controllers;

namespace AzureIoTPortal.Web.Host.Controllers
{
    public class AntiForgeryController : AzureIoTPortalControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
