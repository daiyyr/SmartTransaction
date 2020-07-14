using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using AzureIoTPortal.AzureIoT;
using AzureIoTPortal.SystemConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureIoTPortal.Web.AzureIoTCore;
using System.Text;

namespace AzureIoTPortal.Web.BackgroundWorker
{
    public class BackgroundWork : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private IRepository<Device> _DeviceRepository;
        private IRepository<Event> _EventRepository;
        private ISystemConfigAppService _SystemConfigAppService;

        private static int nUpdateDeviceSecs = 60;
        private static int nCheckEventSecs = 10;
        private static int nTimerCount = 0;
        public BackgroundWork(AbpTimer abpTimer, IRepository<Device> deviceRepository,ISystemConfigAppService systemConfigAppService, IRepository<Event> eventRepository) : base(abpTimer)
        {
            _DeviceRepository = deviceRepository;
            _SystemConfigAppService = systemConfigAppService;
            _EventRepository = eventRepository;
            Timer.Period = 1000;
            nUpdateDeviceSecs = 20;
        }

        protected override void DoWork()
        {
            nTimerCount++;
            if (nTimerCount % nUpdateDeviceSecs == 0)
                 CheckDeviceAsync();
            if (nTimerCount % nCheckEventSecs == 0)
                CheckEvent();
        }

        private async Task CheckDeviceAsync()
        {
            string connectionString = _SystemConfigAppService.GetConfig("IoTConnectionString");
            DevicesProcessor devicesProcessor = new DevicesProcessor(connectionString, 10, "");
            var devicesList = await devicesProcessor.GetDevices();
            foreach(var device in devicesList)
            {
                var deviceT = _DeviceRepository.FirstOrDefault(x => x.iot_id == device.Id);
                if(deviceT == null)
                {
                    deviceT = new Device();
                    deviceT.iot_id = device.Id;
                    if (device.Id.ToLower().Contains("light"))
                        deviceT.type = "Light";
                    else if (device.Id.ToLower().Contains("conditioning"))
                        deviceT.type = "AirCondition";
                    else if (device.Id.ToLower().Contains("meter"))
                        deviceT.type = "Meter";
                    _DeviceRepository.Insert(deviceT);
                }
                deviceT.state = device.State;
                deviceT.last_activity_time = device.LastActivityTime;
                deviceT.last_conection_state_update_time = device.LastConnectionStateUpdatedTime;
                deviceT.conneciton_string = device.ConnectionString;

                if ((device.LastActivityTime.HasValue && (DateTime.UtcNow - device.LastActivityTime.Value).TotalMinutes < 15)|| device.ConnectionState == "Connected")
                    deviceT.connection_state = "Connected";
                else
                    deviceT.connection_state = "Disconnected";


                _DeviceRepository.Update(deviceT);
            }

        }

        private async void CheckEvent()
        {
            string connectionString = _SystemConfigAppService.GetConfig("EventHubConnectionString");
            string eventHubPartition = _SystemConfigAppService.GetConfig("EventHubPartition");

            try
            {
                var devices = _DeviceRepository.GetAllList();
                foreach(var device in devices)
                {

                    Event lastEvent = _EventRepository.GetAllList().Where(x=>x.deviceId == device.Id).OrderByDescending(x=>x.iot_event_time).FirstOrDefault();
                    DateTime lastInfo = DateTime.Now.AddDays(-1);
                    if (lastEvent != null && lastEvent.iot_event_time.HasValue)
                        lastInfo =lastEvent.iot_event_time.Value.ToUniversalTime();
                    
                    TimeSpan ts = DateTime.Now - DateTime.UtcNow.AddSeconds(-1);
                    lastInfo = lastInfo.AddHours(ts.Hours).AddSeconds(1);

                    var eventDatas = await EventHub.ReceiveMessagesAsync(connectionString, device.iot_id,lastInfo,0,eventHubPartition);
                    if (eventDatas != null)
                    {
                        foreach (var eventData in eventDatas)
                        {
                            string body = Encoding.UTF8.GetString(eventData.Body);
                           
                            string deviceId = eventData.SystemProperties["iothub-connection-device-id"].ToString();
                            if(deviceId == device.iot_id)
                            {
                                DateTime enqueuedTime = (DateTime)eventData.SystemProperties["iothub-enqueuedtime"];
                                long queueNumber = (long)eventData.SystemProperties["x-opt-sequence-number"];
                                Event newEvent = new Event();
                                newEvent.deviceId = device.Id;
                                newEvent.iot_event_time = enqueuedTime;
                                newEvent.queue_number = queueNumber;
                                newEvent.data = body;
                                _EventRepository.Insert(newEvent);
                            }
                          
                        }
                      
                    }
                    var msgCount = _EventRepository.Count(x => x.deviceId == device.Id);
                    if(device.message_count!=msgCount)
                    {
                        device.message_count = msgCount;
                        _DeviceRepository.Update(device);
                    }
                   


                }
               

            }
            catch(Exception ex)
            {
                Logger.Error("EventHub", ex);
            }
        }
    }
}
