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
using Microsoft.AspNetCore.Http;

namespace AzureIoTPortal.Web.Controllers
{
    [AbpMvcAuthorize]
    public class IoTController : AzureIoTPortalControllerBase
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


        public IoTController(IDeviceAppService deviceAppServcie,ISystemConfigAppService systemConfigAppService
            ,SMSDbContext dbSMS
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

        public class UnitActivityJson
        {
            public string y { get; set; }
            public decimal Invoice { get; set; }
            public decimal Receipt { get; set; }
        }

        public class BankBalanceChart
        {
            public string label { get; set; }
            public string fillColor { get; set; }
            public string strokeColor { get; set; }
            public string pointColor { get; set; }
            public string pointStrokeColor { get; set; }
            public string pointHighlightFill { get; set; }
            public string pointHighlightStroke { get; set; }
            public decimal[] data { get; set; }
        }
        public ActionResult Index()
        {

            //var devices = _DeviceAppService.GetDevicesWithLastEvent(20);
            //DeviceInfo deviceInfo = new DeviceInfo();
            //deviceInfo.DeviceCount = devices.Count;
            //deviceInfo.DeviceConectedCount = devices.Where(x => x.connection_state == "Connected").Count();
            //deviceInfo.SetDevices(devices);
            //deviceInfo.DisplayName = "Devices";
            //ViewBag.Devices = JsonConvert.SerializeObject(deviceInfo);



            //two ways to get data

            //settings in Startup
            //Bodycorp bodycorp = _dbSMS.Bodycorps.Where(x => x.bodycorp_id == 10).FirstOrDefault();

            var user_dto = _UserAppService.GetCurrentUserSMSInfo(conn_iot, conn_sms);
            //string unit_id = user_dto.Result.UnitId[0];
            

            Data.AdFunction.conn_sms = conn_sms;
            Data.AdFunction.conn_iot = conn_iot;
            Data.Odbc o_sms = new Data.Odbc(conn_sms);

            //ViewBag.bc_code_name = user_dto.Result.firstAddress;
            ViewBag.bc_code_name = "BC " + user_dto.Result.BodycorpCode[0];
            ViewBag.unit_code = user_dto.Result.UnitCode[0];

            decimal unit_balance = Data.AdFunction.GetUnitBalance(user_dto.Result.UnitId[0], new DateTime(2010,1,1), DateTime.Now, o_sms);
            ViewBag.unit_balance = unit_balance + 
                (unit_balance > 0 ? " dr" : "cr");

            //current bank balance
            ViewBag.bank_balance = Data.AdFunction.GetBCBalance(int.Parse(user_dto.Result.BodycorpId[0]), new DateTime(1990, 1, 1), DateTime.Now, null, o_sms);

            //get another 9 month of bank balance for the graph
            BankBalanceChart ubc = new BankBalanceChart();
            ubc.label = "Bank Balance";

            //grey
            //ubc.fillColor = "rgba(210, 214, 222, 1)";
            //ubc.strokeColor = "rgba(210, 214, 222, 1)";
            //ubc.pointColor = "rgba(210, 214, 222, 1)";
            //ubc.pointStrokeColor = "#c1c7d1";
            //ubc.pointHighlightFill = "#fff";
            //ubc.pointHighlightStroke = "rgba(220,220,220,1)";

            //blue
            ubc.fillColor = "rgba(60,141,188,0.9)";
            ubc.strokeColor = "rgba(60,141,188,0.8)";
            ubc.pointColor = "#3b8bba";
            ubc.pointStrokeColor = "rgba(60,141,188,1)";
            ubc.pointHighlightFill = "#fff";
            ubc.pointHighlightStroke = "rgba(60,141,188,1)";

            decimal[] ubc_data = new decimal[10];
            string[] ubc_label = new string[10];
            ubc_data[9] = ViewBag.bank_balance;
            ubc_label[9] = DateTime.Now.ToString("yyyy-MM");
            for (int i = 1; i<=9; i++)
            {
                ubc_data[9 - i] = Data.AdFunction.GetBCBalance(int.Parse(user_dto.Result.BodycorpId[0]), new DateTime(1990, 1, 1), DateTime.Now.AddMonths(-i), null, o_sms);
                ubc_label[9 - i] = DateTime.Now.AddMonths(-i).ToString("yyyy-MM");
            }
            ubc.data = ubc_data;
            ViewBag.BankBalanceChart = JsonConvert.SerializeObject(ubc);
            ViewBag.ubc_label = JsonConvert.SerializeObject(ubc_label);

            if (user_dto.Result.AGMDate.HasValue)
            {
                ViewBag.agm_date = user_dto.Result.AGMDate.Value.ToString("dd/MM/yyyy");
                if (user_dto.Result.AGMTime.HasValue)
                {
                    ViewBag.agm_time = new DateTime(user_dto.Result.AGMTime.Value.Ticks).ToString("HH:mm");
                }
            }

            var dt_activity = Data.AdFunction.GetUnitActivity(
                user_dto.Result.BodycorpId[0], new DateTime(2010,1,1), DateTime.Now, 
                "All", "DueDate", int.Parse(user_dto.Result.UnitId[0]), o_sms);
            List<UnitActivityJson> activityList = new List<UnitActivityJson>();
            foreach(DataRow dr in dt_activity.Rows)
            {
                DateTime period = DateTime.ParseExact(
                    dr["InvDate"].ToString(), "dd/MM/yyyy",
                    CultureInfo.InvariantCulture
                    );
                if(period < new DateTime(2010, 1, 1))
                {
                    continue;
                }
                string month = period.ToString("yyyy-MM");
                decimal inv = 0;
                decimal rcp = 0;
                decimal.TryParse(dr["Invoice"].ToString(), out inv);
                decimal.TryParse(dr["Receipt"].ToString(), out rcp);
                bool monthExist = false;
                foreach (UnitActivityJson eob in activityList)
                {
                    if(eob.y == month)
                    {
                        eob.Invoice += inv;
                        eob.Receipt += rcp;
                        monthExist = true;
                        break;
                    }
                }
                if (!monthExist)
                {
                    UnitActivityJson ob = new UnitActivityJson
                    {
                        y = month,
                        Invoice = inv,
                        Receipt = rcp
                    };
                    activityList.Add(ob);
                }
            }
            ViewBag.unit_activity = JsonConvert.SerializeObject(activityList);


            var dt_donut = Data.AdFunction.GetDonut(
                user_dto.Result.BodycorpId[0], new DateTime(2010, 1, 1), DateTime.Now,
                "All", "DueDate", int.Parse(user_dto.Result.UnitId[0]), o_sms);
            List<Object> donutList = new List<object>();
            foreach (DataRow dr in dt_donut.Rows)
            {
                decimal cc = 0;
                decimal.TryParse(dr["cc"].ToString(), out cc);
                Object ob = new
                {
                    label = dr["chart_master_name"].ToString(),
                    value = cc
                };

                donutList.Add(ob);
            }
            ViewBag.donut = JsonConvert.SerializeObject(donutList);


            //calendar
            string sql = "select * from rental_work_order where rental_work_order_schedule_date_time is not null and rental_work_order_schedule_end_date_time is not null order by rental_work_order_schedule_date_time ";





            return View();
        }


        public class PortalDocument
        {
            public string Description { get; set; }
            public string Date { set; get; }
            public string Ref { get; set; }
            public decimal Invoice { get; set; }
            public decimal Receipt { get; set; }
            public decimal Balance { get; set; }
        }
        public class PortalFolder
        {
            public string Name { get; set; }
            public List<PortalDocument> documents { get; set; }
        }
        public ActionResult Levies()
        {
            Odbc o = new Odbc(conn_sms);
            SMS.system s = new SMS.system(conn_sms);
            s.SetOdbc(o);
            s.LoadData("FILEFOLDER");
            string fileFolder = s.system_value;
            var user_dto = _UserAppService.GetCurrentUserSMSInfo(conn_iot, conn_sms);

            List<PortalFolder> folders = new List<PortalFolder>();

            PortalFolder pf = new PortalFolder();
            pf.Name = "Activities";

            List<PortalDocument> docs = new List<PortalDocument>();

            Data.Odbc o_sms = new Data.Odbc(conn_sms);

            var dt_activity = Data.AdFunction.GetUnitActivity(
                user_dto.Result.BodycorpId[0], new DateTime(2010, 1, 1), DateTime.Now,
                "All", "DueDate", int.Parse(user_dto.Result.UnitId[0]), o_sms);
            List<UnitActivityJson> activityList = new List<UnitActivityJson>();
            foreach (DataRow dr in dt_activity.Rows)
            {
                DateTime period = DateTime.ParseExact(
                    dr["InvDate"].ToString(), "dd/MM/yyyy",
                    CultureInfo.InvariantCulture
                    );
                if (period < new DateTime(2010, 1, 1))
                {
                    continue;
                }
                string month = period.ToString("yyyy-MM");
                decimal inv = 0;
                decimal rcp = 0;
                decimal.TryParse(dr["Invoice"].ToString(), out inv);
                decimal.TryParse(dr["Receipt"].ToString(), out rcp);
                bool monthExist = false;
                foreach (UnitActivityJson eob in activityList)
                {
                    if (eob.y == month)
                    {
                        eob.Invoice += inv;
                        eob.Receipt += rcp;
                        monthExist = true;
                        break;
                    }
                }
                if (!monthExist)
                {
                    UnitActivityJson ob = new UnitActivityJson
                    {
                        y = month,
                        Invoice = inv,
                        Receipt = rcp
                    };
                    activityList.Add(ob);
                }
            }

            foreach (DataRow dr in dt_activity.Rows)
            {
                decimal inv = 0;
                decimal rcp = 0;
                decimal b = 0;
                decimal.TryParse(dr["Invoice"].ToString(), out inv);
                decimal.TryParse(dr["Receipt"].ToString(), out rcp);
                decimal.TryParse(dr["Balance"].ToString(), out b);
                docs.Add(
                    new PortalDocument
                    {
                        Date = dr["InvDate"].ToString(),
                        Description = dr["Description"].ToString(),
                        Ref = dr["Ref"].ToString(),
                        Invoice = inv,
                        Receipt = rcp,
                        Balance = b
                    }
                );
            }

            pf.documents = docs;
            folders.Add(pf);
            ViewBag.folders = folders;
            return View();
        }




        public ActionResult Devices()
        {
            var devices = _DeviceAppService.GetDevicesWithLastEvent(20, false);
            DeviceInfo deviceInfo = new DeviceInfo();
            deviceInfo.DeviceCount = devices.Count;
            deviceInfo.DeviceConectedCount = devices.Where(x => x.connection_state == "Connected").Count();
            deviceInfo.SetDevices(devices);
            deviceInfo.DisplayName = "Devices";
            ViewBag.Devices = JsonConvert.SerializeObject(deviceInfo);

            return View();
        }
        public ActionResult FloorPlan()
        {
            var devices = _DeviceAppService.GetDevicesWithLastEvent(20);
            DeviceInfo deviceInfo = new DeviceInfo();
            deviceInfo.DeviceCount = devices.Count;
            deviceInfo.DeviceConectedCount = devices.Where(x => x.connection_state == "Connected").Count();
            deviceInfo.SetDevices(devices);
            deviceInfo.DisplayName = "Devices";
            ViewBag.Devices = JsonConvert.SerializeObject(deviceInfo);
            return View();
        }
        public ActionResult Alarms()
        {
            return View();
        }
        public ActionResult Rules()
        {
            return View();
        }

        [AbpAuthorize(PermissionNames.Pages_Audit)]
        public ActionResult Audit()
        {
            return View();
        }


        [AbpMvcAuthorize(PermissionNames.Pages_Reports)]
        public ActionResult Reports()
        {
            return View();
        }
        public ActionResult Settings()
        {
            return View();
        }


        public string GetEvents()
        {
            bool success = true;
            string info = "";
            try
            {
                var devices = _DeviceAppService.GetDevicesWithLastEvent(20);
                DeviceInfo deviceInfo = new DeviceInfo();
                deviceInfo.DeviceCount = devices.Count;
                deviceInfo.DeviceConectedCount = devices.Where(x => x.connection_state == "Connected").Count();
                deviceInfo.SetDevices(devices);
                deviceInfo.DisplayName = "Devices";
                
                info = JsonConvert.SerializeObject(deviceInfo);

            }
            catch (Exception ex)
            {
                success = false;
                info = ex.Message;
            }
            Object result = new
            {
                success = success,
                info = info
            };
             
            return JsonConvert.SerializeObject(result);
        }

        public string SendMessage([FromBody]dynamic data)
        {
            bool success = true;
            
            string info = "";
            try
            {
                string _DeviceID = data.DeviceID;
                string _DeviceOn = data.DeviceOn;
                string _Temperature = data.Temperature;
                if (string.IsNullOrWhiteSpace(_DeviceID))
                    throw new Exception("Invalid Device ID.");
                string connectionString = _SystemConfigAppService.GetConfig("IoTConnectionString");
                Microsoft.Azure.Devices.Message message = new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes("Cloud to device message."));
                message.MessageId = Guid.NewGuid().ToString();
                if (!string.IsNullOrWhiteSpace(_DeviceOn) && (_DeviceOn == "on" || _DeviceOn == "off"))
                    message.Properties.Add("on", _DeviceOn);
                if (!string.IsNullOrWhiteSpace(_Temperature))
                    message.Properties.Add("temperature", _Temperature);
                message.Ack = DeliveryAcknowledgement.Full;
                EventHub.SendMessageToDeviceAsync(connectionString,_DeviceID, message);

            }
            catch (Exception ex)
            {
                success = false;
                info = ex.Message;
            }
            Object result = new
            {
                success = success,
                info = info
            };

            return JsonConvert.SerializeObject(result);
        }
    }
}
