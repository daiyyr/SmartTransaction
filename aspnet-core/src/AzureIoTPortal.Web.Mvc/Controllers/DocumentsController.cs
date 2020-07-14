
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

namespace AzureIoTPortal.Web.Controllers
{
    [AbpMvcAuthorize]
    public class DocumentsController : AzureIoTPortalControllerBase
    {
        private readonly IDeviceAppService _DeviceAppService;
        private readonly ISystemConfigAppService _SystemConfigAppService;


        private readonly SMSDbContext _dbSMS;
        //private readonly IRepository<Bodycorp> _BodycorpRepository;
        private readonly IBodycorpAppService _BodycorpAppService;

        private readonly IConfigurationRoot _appConfiguration;
        private readonly string conn_sms;
        private readonly string conn_iot;

        private readonly IUserAppService _UserAppService;


        public DocumentsController(IDeviceAppService deviceAppServcie, ISystemConfigAppService systemConfigAppService
            , SMSDbContext dbSMS
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


            _dbSMS = dbSMS;
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
            string bcFolder = Path.Combine(
                fileFolder,
                AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                );

            DirectoryInfo bcfolderinfo = new DirectoryInfo(bcFolder); // C:/SMS/BC_CODE
            DirectoryInfo[] subFolders = bcfolderinfo.GetDirectories().OrderBy(p => p.Name).ToArray();
            List<PortalFolder> folders = new List<PortalFolder>();
            foreach(DirectoryInfo di in subFolders) // C:/SMS/BC_CODE/Invoice
            {
                PortalFolder pf = new PortalFolder();
                pf.Name = di.Name;
                List<PortalDocument> docs = new List<PortalDocument>(); 
                FileInfo[] files = di.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
                foreach (FileInfo file in files) // C:/SMS/BC_CODE/Invoice/INV0728.pdf
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


        public ActionResult ProcessedTransaction()
        {
            Odbc o = new Odbc(conn_sms);
            SMS.system s = new SMS.system(conn_sms);
            s.SetOdbc(o);
            s.LoadData("FILEFOLDER");
            string fileFolder = s.system_value;
            var user_dto = _UserAppService.GetCurrentUserSMSInfo(conn_iot, conn_sms);
            string bcFolder = Path.Combine(
                fileFolder,
                AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0])
                );

            DirectoryInfo bcfolderinfo = new DirectoryInfo(bcFolder); // C:/SMS/BC_CODE
            DirectoryInfo[] subFolders = bcfolderinfo.GetDirectories().OrderBy(p => p.Name).ToArray();
            List<PortalFolder> folders = new List<PortalFolder>();
            foreach (DirectoryInfo di in subFolders) // C:/SMS/BC_CODE/Invoice
            {
                PortalFolder pf = new PortalFolder();
                pf.Name = di.Name;
                List<PortalDocument> docs = new List<PortalDocument>();
                FileInfo[] files = di.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
                foreach (FileInfo file in files) // C:/SMS/BC_CODE/Invoice/INV0728.pdf
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

            var user_dto = _UserAppService.GetCurrentUserSMSInfo(conn_iot, conn_sms);
            string file = Path.Combine(
                fileFolder, 
                AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0]),
                foldername, //document type
                AdFunction.GetCleanCode(user_dto.Result.UnitCode[0]),
                documentname
                );

            FileInfo fi = new FileInfo(file);

            //try path without unit code
            if (!fi.Exists)
            {
                file = Path.Combine(
                fileFolder,
                AdFunction.GetCleanCode(user_dto.Result.BodycorpCode[0]),
                foldername, //document type
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
            string filetype = System.Net.Mime.MediaTypeNames.Application.Pdf;
            if (!fileName.ToLower().EndsWith("pdf"))
            {
                filetype = System.Net.Mime.MediaTypeNames.Application.Zip;
            }
            return File(fileBytes, filetype, fileName);
        }
    }
}