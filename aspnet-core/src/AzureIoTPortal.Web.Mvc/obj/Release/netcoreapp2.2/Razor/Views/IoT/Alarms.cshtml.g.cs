#pragma checksum "C:\Users\Teemo\Documents\Visual Studio 2015\Projects\SmartTransaction\aspnet-core\src\AzureIoTPortal.Web.Mvc\Views\IoT\Alarms.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "15a0347ee3843e048ae26f97a26ace26f3bfeecf"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_IoT_Alarms), @"mvc.1.0.view", @"/Views/IoT/Alarms.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/IoT/Alarms.cshtml", typeof(AspNetCore.Views_IoT_Alarms))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\Teemo\Documents\Visual Studio 2015\Projects\SmartTransaction\aspnet-core\src\AzureIoTPortal.Web.Mvc\Views\_ViewImports.cshtml"
using Abp.Localization;

#line default
#line hidden
#line 1 "C:\Users\Teemo\Documents\Visual Studio 2015\Projects\SmartTransaction\aspnet-core\src\AzureIoTPortal.Web.Mvc\Views\IoT\Alarms.cshtml"
using AzureIoTPortal.Web.Startup;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"15a0347ee3843e048ae26f97a26ace26f3bfeecf", @"/Views/IoT/Alarms.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"ce343e6a069753839c90283688a3deb27c70db97", @"/Views/_ViewImports.cshtml")]
    public class Views_IoT_Alarms : AzureIoTPortal.Web.Views.AzureIoTPortalRazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 2 "C:\Users\Teemo\Documents\Visual Studio 2015\Projects\SmartTransaction\aspnet-core\src\AzureIoTPortal.Web.Mvc\Views\IoT\Alarms.cshtml"
  
    ViewBag.CurrentPageName = PageNames.Home; // The menu item will be active for this page.

#line default
#line hidden
            DefineSection("styles", async() => {
                BeginContext(149, 6, true);
                WriteLiteral("\n    \n");
                EndContext();
            }
            );
            DefineSection("scripts", async() => {
                BeginContext(175, 7, true);
                WriteLiteral("\n     \n");
                EndContext();
            }
            );
            BeginContext(184, 65, true);
            WriteLiteral("<div class=\"block-header\">\n    <h2>SAMPLE DASHBOARD</h2>\n</div>\n ");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
