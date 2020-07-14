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

namespace AzureIoTPortal.Web.Controllers
{
    [AbpMvcAuthorize]
    public class IoTController : AzureIoTPortalControllerBase
    {
        private readonly IDeviceAppService _DeviceAppService;
        private readonly ISystemConfigAppService _SystemConfigAppService;

        public IoTController(IDeviceAppService deviceAppServcie,ISystemConfigAppService systemConfigAppService)
        {
            _DeviceAppService = deviceAppServcie;
            _SystemConfigAppService = systemConfigAppService;
        }
        public ActionResult Index()
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

        public ActionResult Devices()
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
