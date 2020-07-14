using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using AzureIoTPortal.Controllers;

namespace AzureIoTPortal.Web.Controllers
{
    [AbpMvcAuthorize]
    public class AboutController : AzureIoTPortalControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}
