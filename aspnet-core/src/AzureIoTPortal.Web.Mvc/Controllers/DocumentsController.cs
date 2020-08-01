
using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using AzureIoTPortal.Controllers;
using AzureIoTPortal.AzureIoT;
using AzureIoTPortal.SystemConfig;
using AzureIoTPortal.Web.AzureIoTCore;
using Newtonsoft.Json;
using System;
using AzureIoTPortal.Web.Models.IoT;
using System.Linq;
using System.Text;
using Microsoft.Azure.Devices;
using AzureIoTPortal.Authorization;
using Abp.Authorization;
using AzureIoTPortal.EntityFrameworkCore;
using Abp.Domain.Repositories;
using AzureIoTPortal.SMS;
using Microsoft.AspNetCore.Hosting;
using AzureIoTPortal.Configuration;
using Microsoft.Extensions.Configuration;
using AzureIoTPortal.Users;
using System.Collections.Generic;
using AzureIoTPortal.Data;
using System.Data;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Net;
using System.Configuration;

namespace AzureIoTPortal.Web.Controllers
{
    [AbpMvcAuthorize]
    public class DocumentsController : AzureIoTPortalControllerBase
    {
        private readonly IDeviceAppService _DeviceAppService;
        private readonly ISystemConfigAppService _SystemConfigAppService;


        //private readonly SMSDbContext _dbSMS;
        //private readonly IRepository<Bodycorp> _BodycorpRepository;
        private readonly IBodycorpAppService _BodycorpAppService;

        private readonly IConfigurationRoot _appConfiguration;
        private readonly string conn_sms;
        private readonly string conn_iot;

        private readonly IUserAppService _UserAppService;


        public DocumentsController(IDeviceAppService deviceAppServcie, ISystemConfigAppService systemConfigAppService
            //, SMSDbContext dbSMS
            //,IRepository<Bodycorp> BodycorpRepository
            , IBodycorpAppService bodycorpAppService
            , IHostingEnvironment env
            , IUserAppService UserAppService
            )
        {
            _DeviceAppService = deviceAppServcie;
            _SystemConfigAppService = systemConfigAppService;
            _BodycorpAppService = bodycorpAppService;
            _UserAppService = UserAppService;


            //_dbSMS = dbSMS;
            //_BodycorpRepository = BodycorpRepository;

            _appConfiguration = env.GetAppConfiguration();
            conn_sms = _appConfiguration.GetConnectionString("SMS_ODBC");
            conn_iot = _appConfiguration.GetConnectionString("IOT_ODBC");

        }

        public class PortalDocument
        {
            public string Name { get; set; }
            public string Date { set; get; }
            public bool New { get; set; }
            public string By { get; set; }
            public string Status { get; set; }
        }
        public class PortalFolder
        {
            public string Name { get; set; }
            public List<PortalDocument> documents { get; set; }
        }

        public ActionResult Index()
        {
            Odbc o = new Odbc(conn_sms);
            SMS.system s = new SMS.system(conn_sms);
            s.SetOdbc(o);
            s.LoadData("FILEFOLDER");
            string fileFolder = s.system_value;
            var user_dto = _UserAppService.GetCurrentUserSMSInfo(conn_iot, conn_sms);
            string bankFolder = Path.Combine(
                fileFolder,
                //   AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                "SmartTransaction", "bank"
                );

            Directory.CreateDirectory(bankFolder);
            DirectoryInfo bcfolderinfo = new DirectoryInfo(bankFolder); // C:/SMS/SmartTransaction/bank
            DirectoryInfo[] subFolders = bcfolderinfo.GetDirectories().OrderBy(p => p.Name).ToArray();
            List<PortalFolder> folders = new List<PortalFolder>();
            foreach(DirectoryInfo di in subFolders) // C:/SMS/SmartTransaction/bank/Westpac
            {
                PortalFolder pf = new PortalFolder();
                pf.Name = di.Name;
                List<PortalDocument> docs = new List<PortalDocument>(); 
                FileInfo[] files = di.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
                foreach (FileInfo file in files) // C:/SMS/SmartTransaction/bank/Westpac/xxxxxxx.zip
                {
                    docs.Add(
                        new PortalDocument
                        {
                            Name = file.Name,
                            Date = file.CreationTime.ToString("dd/MM/yyyy HH:mm"),
                            New = (DateTime.Now - file.CreationTime).TotalDays < 7,
                            By = file.GetAccessControl().GetOwner(typeof(System.Security.Principal.NTAccount)).ToString()
                        }
                    );
                }

                //try to find unit folder
                string unitFolder = Path.Combine(
                    di.FullName,
                    AdFunction.GetCleanCode(user_dto.Result.UnitCode[0])
                );//C:/SMS/BC_CODE/Invoice/Unit_Code
                if (Directory.Exists(unitFolder))
                {
                    DirectoryInfo di2 = new DirectoryInfo(unitFolder);
                    FileInfo[] files2 = di2.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
                    foreach (FileInfo file in files2)
                    {
                        docs.Add(
                            new PortalDocument
                            {
                                Name = file.Name,
                                Date = file.CreationTime.ToString("dd/MM/yyyy HH:mm"),
                                New = (DateTime.Now - file.CreationTime).TotalDays < 7,
                                By = file.GetAccessControl().GetOwner(typeof(System.Security.Principal.NTAccount)).ToString()
                            }
                        );
                    }
                }

                pf.documents = docs;
                folders.Add(pf);
            }

            ViewBag.folders = folders;
            return View();
        }

        public FileResult Download(string foldername, string documentname)
        {
            Odbc o = new Odbc(conn_sms);
            SMS.system s = new SMS.system(conn_sms);
            s.SetOdbc(o);
            s.LoadData("FILEFOLDER");
            string fileFolder = s.system_value;

            //var user_dto = _UserAppService.GetCurrentUserSMSInfo(conn_iot, conn_sms);
            o.Close();

            string bankFolder = Path.Combine(
                fileFolder,
                //   AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                "SmartTransaction", "bank"
                );

            string file = Path.Combine(
                bankFolder,
                foldername, //Westpac or ASB
                documentname
                );

            FileInfo fi = new FileInfo(file);

            //try path without unit code
            if (!fi.Exists)
            {
                
            }

            if (!fi.Exists)
            {
                throw new Exception("File does not exist. Please contact Admin.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(file);
            string fileName = documentname;
            string filetype = System.Net.Mime.MediaTypeNames.Application.Zip;
            if (!fileName.ToLower().EndsWith("zip"))
            {
                filetype = System.Net.Mime.MediaTypeNames.Application.Zip;
            }
            return File(fileBytes, filetype, fileName);
        }


        public FileResult DownloadProcessed(string foldername, string documentname)
        {
            Odbc o = new Odbc(conn_sms);
            SMS.system s = new SMS.system(conn_sms);
            s.SetOdbc(o);
            s.LoadData("FILEFOLDER");
            string fileFolder = s.system_value;

            //var user_dto = _UserAppService.GetCurrentUserSMSInfo(conn_iot, conn_sms);
            o.Close();

            string bankFolder = Path.Combine(
                fileFolder,
                //   AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                "SmartTransaction", "processed"
                );

            string file = Path.Combine(
                bankFolder,
                foldername, //Westpac or ASB
                documentname
                );

            FileInfo fi = new FileInfo(file);

            if (!fi.Exists)
            {
                bankFolder = Path.Combine(
                fileFolder,
                "SmartTransaction", "confirmed"
                ); 
                file = Path.Combine(
                 bankFolder,
                 foldername, //Westpac or ASB
                 documentname
                 );
                fi = new FileInfo(file);
            }

            if (!fi.Exists)
            {
                throw new Exception("File does not exist. Please contact Admin.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(file);
            string fileName = documentname;
            string filetype = System.Net.Mime.MediaTypeNames.Application.Zip;
            if (!fileName.ToLower().EndsWith("zip"))
            {
                filetype = System.Net.Mime.MediaTypeNames.Application.Zip;
            }
            return File(fileBytes, filetype, fileName);
        }

        public ActionResult Process(string foldername, string documentname)
        {
            Odbc o = new Odbc(conn_sms);
            SMS.system s = new SMS.system(conn_sms);
            s.SetOdbc(o);
            s.LoadData("FILEFOLDER");
            string fileFolder = s.system_value;

            string processedFolder = Path.Combine(
                fileFolder,
                //   AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                "SmartTransaction", "processed"
                );
            string bankFolder = Path.Combine(
                fileFolder,
                //   AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                "SmartTransaction", "bank"
                );
            string source = Path.Combine(
                bankFolder,
                foldername, //Westpac or ASB
                documentname
                );
            string target = Path.Combine(
                processedFolder,
                foldername, //Westpac or ASB
                documentname
                );

            #region trigger SMS
            int maxRetry = 5;
            int retry = 0;
            CookieCollection cookie = new CookieCollection();
            Spider sp = new Spider();

            string smsHost = _appConfiguration.GetConnectionString("smsHost");
            string smsLoginPostFirstLine = _appConfiguration.GetConnectionString("smsLoginPostFirstLine");
            string smsUserName = _appConfiguration.GetConnectionString("smsUserName");
            string smsUserPassword = _appConfiguration.GetConnectionString("smsUserPassword");
            string smsLoginSelectedBcId = _appConfiguration.GetConnectionString("smsLoginSelectedBcId");

            if (smsHost.Contains("/"))      //      localhost/sapp_sms2
            {
                int charLocation = smsHost.IndexOf("/", StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    Spider.gHost = smsHost.Substring(0, charLocation);
                }
            }
            else
            {
                Spider.gHost = smsHost;     //          localhost:32553 
            }
            FileInfo fi = new FileInfo(target);
            string filenameP = Path.GetFileNameWithoutExtension(fi.Name);
            string date = filenameP.Substring(filenameP.Length - 8);

            string test = sp.sendRequest("http://" + smsHost + "/login.aspx",
                "POST",
                "",
                true,
                smsLoginPostFirstLine
                + smsUserName + "&TextBoxPassword="
                + smsUserPassword + "&bcSearchBox=&DropDownList1=" + smsLoginSelectedBcId + "&ButtonLogin=Login",
                ref cookie,
                Spider.gHost,
                true
                );

            test = sp.sendRequest("http://" + smsHost + "/cinvoicebatch.aspx?smarttransaction=commit&bank="
                     + foldername + "&date=" + date,
                 "GET",
                 "",
                 true,
                 @"",
                 ref cookie,
                 Spider.gHost,
                 true
                 );
            #endregion

            #region move file
            Directory.CreateDirectory(Path.Combine(processedFolder, foldername));
            if (System.IO.File.Exists(target))
            {
                System.IO.File.Delete(target);
            }
            System.IO.File.Move(source, target);
            
            #endregion
            

            return RedirectToAction("Index", "Documents");
        }

        public ActionResult ProcessedTransaction()
        {
            Odbc o = new Odbc(conn_sms);
            SMS.system s = new SMS.system(conn_sms);
            s.SetOdbc(o);
            s.LoadData("FILEFOLDER");
            string fileFolder = s.system_value;
            var user_dto = _UserAppService.GetCurrentUserSMSInfo(conn_iot, conn_sms);
            string processedFolder = Path.Combine(
                fileFolder,
                //   AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                "SmartTransaction", "processed"
                );

            DirectoryInfo bcfolderinfo = new DirectoryInfo(processedFolder); // C:/SMS/SmartTransaction/processed
            Directory.CreateDirectory(processedFolder);
            DirectoryInfo[] subFolders = bcfolderinfo.GetDirectories().OrderBy(p => p.Name).ToArray();
            List<PortalFolder> folders = new List<PortalFolder>();
            foreach (DirectoryInfo di in subFolders) // C:/SMS/SmartTransaction/processed/ASB07364
            {
                PortalFolder pf = new PortalFolder();
                pf.Name = di.Name;
                List<PortalDocument> docs = new List<PortalDocument>();
                FileInfo[] files = di.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
                foreach (FileInfo file in files) // C:/SMS/SmartTransaction/processed/ASB07364/xxxxxxx.csv
                {
                    docs.Add(
                        new PortalDocument
                        {
                            Name = file.Name,
                            Date = file.CreationTime.ToString("dd/MM/yyyy HH:mm"),
                            New = (DateTime.Now - file.CreationTime).TotalDays < 7,
                            By = file.GetAccessControl().GetOwner(typeof(System.Security.Principal.NTAccount)).ToString(),
                            Status = "Uploaded to bank"
                        }
                    );
                }

                string subFolderUnderConfirmed = Path.Combine(
                fileFolder,
                "SmartTransaction", "confirmed", di.Name
                );
                Directory.CreateDirectory(subFolderUnderConfirmed);
                DirectoryInfo di2 = new DirectoryInfo(subFolderUnderConfirmed);
                files = di2.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
                foreach (FileInfo file in files) // C:/SMS/SmartTransaction/confirmed/ASB07364/xxxxxxx.csv
                {
                    docs.Add(
                        new PortalDocument
                        {
                            Name = file.Name,
                            Date = file.CreationTime.ToString("dd/MM/yyyy HH:mm"),
                            New = (DateTime.Now - file.CreationTime).TotalDays < 7,
                            By = file.GetAccessControl().GetOwner(typeof(System.Security.Principal.NTAccount)).ToString(),
                            Status = "Confirmed"
                        }
                    );
                }

                pf.documents = docs;
                folders.Add(pf);
            }
            
            ViewBag.folders = folders;
            return View();
        }

        //this function is not in use
        public ActionResult Approve(string foldername, string documentname)
        {
            Odbc o = new Odbc(conn_sms);
            SMS.system s = new SMS.system(conn_sms);
            s.SetOdbc(o);
            s.LoadData("FILEFOLDER");
            string fileFolder = s.system_value;

            string dataFolder = Path.Combine(
                fileFolder,
                //   AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                "SmartTransaction", "data"
                );
            string bankFolder = Path.Combine(
                fileFolder,
                //   AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                "SmartTransaction", "bank"
                );

            FileInfo fi = new FileInfo(documentname);
            string filenameP = Path.GetFileNameWithoutExtension(fi.Name);
            string date = filenameP.Substring(filenameP.Length - 8);

            string target = Path.Combine(
                dataFolder,
                foldername + "PaymentApproval" + date + ".csv" //WestpacPaymentApproval20200719.csv
                );

            if(Request.Form.Files.Count == 0)
            {
                return RedirectToAction("ProcessedTransaction", "Documents");
            }

            
            var file = Request.Form.Files[0];
            using (var fileStream = new FileStream(target, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            #region trigger SMS
            int maxRetry = 5;
            int retry = 0;
            CookieCollection cookie = new CookieCollection();
            Spider sp = new Spider();

            string smsHost = _appConfiguration.GetConnectionString("smsHost");
            string smsLoginPostFirstLine = _appConfiguration.GetConnectionString("smsLoginPostFirstLine");
            string smsUserName = _appConfiguration.GetConnectionString("smsUserName");
            string smsUserPassword = _appConfiguration.GetConnectionString("smsUserPassword");
            string smsLoginSelectedBcId = _appConfiguration.GetConnectionString("smsLoginSelectedBcId");

            if (smsHost.Contains("/"))      //      localhost/sapp_sms2
            {
                int charLocation = smsHost.IndexOf("/", StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    Spider.gHost = smsHost.Substring(0, charLocation);
                }
            }
            else
            {
                Spider.gHost = smsHost;     //          localhost:32553 
            }

            string test = sp.sendRequest("http://" + smsHost + "/login.aspx",
                "POST",
                "",
                true,
                smsLoginPostFirstLine
                + smsUserName + "&TextBoxPassword="
                + smsUserPassword + "&bcSearchBox=&DropDownList1=" + smsLoginSelectedBcId + "&ButtonLogin=Login",
                ref cookie,
                Spider.gHost,
                true
                );

            test = sp.sendRequest("http://" + smsHost + "/cinvoicebatch.aspx?smarttransaction=approve&bank="
                     + foldername + "&date=" + date,
                 "GET",
                 "",
                 true,
                 @"",
                 ref cookie,
                 Spider.gHost,
                 true
                 );
            #endregion


            #region move file
            string processedFolder = Path.Combine(
                fileFolder,
                //   AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                "SmartTransaction", "processed"
                );
            string confirmedFolder = Path.Combine(
                            fileFolder,
                            //   AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                            "SmartTransaction", "confirmed"
                            );
            Directory.CreateDirectory(Path.Combine(processedFolder, foldername));
            Directory.CreateDirectory(Path.Combine(confirmedFolder, foldername));
            string source = Path.Combine(
                processedFolder,
                foldername, //ASB3244
                documentname
                );
            target = Path.Combine(
                confirmedFolder,
                foldername, //ASB3244
                documentname
                );
            if (System.IO.File.Exists(target))
            {
                System.IO.File.Delete(target);
            }
            System.IO.File.Move(source, target);

            #endregion


            return RedirectToAction("ProcessedTransaction", "Documents");
        }

        [HttpPost]
        public async Task<IActionResult> Approve(Microsoft.AspNetCore.Http.IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                TempData["Message"] = string.Format("No file selected");
                return RedirectToAction("ProcessedTransaction");
            }

            Odbc o = new Odbc(conn_sms);
            SMS.system s = new SMS.system(conn_sms);
            s.SetOdbc(o);
            s.LoadData("FILEFOLDER");
            string fileFolder = s.system_value;
            o.Close();

            string dataFolder = Path.Combine(
                fileFolder,
                //   AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                "SmartTransaction", "data"
                );
            string bankFolder = Path.Combine(
                fileFolder,
                //   AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                "SmartTransaction", "bank"
                );

            string foldername = Request.Form["HiddenFoldername"];
            string documentname = Request.Form["HiddenDocumentname"];
            
            FileInfo fi = new FileInfo(documentname);
            string filenameP = Path.GetFileNameWithoutExtension(fi.Name);
            string date = filenameP.Substring(filenameP.Length - 8);

            string target = Path.Combine(
                dataFolder,
                foldername + "PaymentApproval" + date + ".csv" //WestpacPaymentApproval20200719.csv
                );

            using (var stream = new FileStream(target, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            #region trigger SMS
            int maxRetry = 5;
            int retry = 0;
            CookieCollection cookie = new CookieCollection();
            Spider sp = new Spider();

            string smsHost = _appConfiguration.GetConnectionString("smsHost");
            string smsLoginPostFirstLine = _appConfiguration.GetConnectionString("smsLoginPostFirstLine");
            string smsUserName = _appConfiguration.GetConnectionString("smsUserName");
            string smsUserPassword = _appConfiguration.GetConnectionString("smsUserPassword");
            string smsLoginSelectedBcId = _appConfiguration.GetConnectionString("smsLoginSelectedBcId");
            string ButtonLogin = _appConfiguration.GetConnectionString("ButtonLogin");

            if (smsHost.Contains("/"))      //      localhost/sapp_sms2
            {
                int charLocation = smsHost.IndexOf("/", StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    Spider.gHost = smsHost.Substring(0, charLocation);
                }
            }
            else
            {
                Spider.gHost = smsHost;     //          localhost:32553 
            }

            string test = sp.sendRequest("http://" + smsHost + "/login.aspx",
                "POST",
                "",
                true,
                smsLoginPostFirstLine
                + smsUserName + "&TextBoxPassword="
                + smsUserPassword + "&bcSearchBox=&DropDownList1=" + smsLoginSelectedBcId + "&ButtonLogin=" + ButtonLogin,
                ref cookie,
                Spider.gHost,
                true
                );

            test = sp.sendRequest("http://" + smsHost + "/cinvoicebatch.aspx?smarttransaction=approve&bank="
                     + foldername + "&date=" + date,
                 "GET",
                 "",
                 true,
                 @"",
                 ref cookie,
                 Spider.gHost,
                 true
                 );
            #endregion

            #region move file
            string processedFolder = Path.Combine(
                fileFolder,
                //   AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                "SmartTransaction", "processed"
                );
            string confirmedFolder = Path.Combine(
                            fileFolder,
                            //   AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                            "SmartTransaction", "confirmed"
                            );
            Directory.CreateDirectory(Path.Combine(processedFolder, foldername));
            Directory.CreateDirectory(Path.Combine(confirmedFolder, foldername));
            string source = Path.Combine(
                processedFolder,
                foldername, //ASB3244
                documentname
                );
            target = Path.Combine(
                confirmedFolder,
                foldername, //ASB3244
                documentname
                );
            if (System.IO.File.Exists(target))
            {
                System.IO.File.Delete(target);
            }
            System.IO.File.Move(source, target);

            #endregion


            TempData["Message"] = string.Format("All done. Succeed to update transactions!");
            return RedirectToAction("ProcessedTransaction");
        }

    }
}