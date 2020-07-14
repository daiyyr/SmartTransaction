using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using AzureIoTPortal.Controllers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using AzureIoTPortal.Configuration;
using AzureIoTPortal.Data;
using AzureIoTPortal.Users;
using System.IO;
using System.Linq;

namespace AzureIoTPortal.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : AzureIoTPortalControllerBase
    {
        public HomeController(
            )
        {
        }
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Documents");
            //return RedirectToAction("Index", "IoT");
            //return View();
        }

    }
}
