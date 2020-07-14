
using AzureIoTPortal.AzureIoT;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureIoTPortal.Web.Models.IoT
{
    public class DeviceInfo
    {
        public int DeviceCount { get; set; }
        public int DeviceConectedCount { get; set; }
        public string DisplayName { get; set; }

        public List<DeviceState> DeviceState { get; set; }
        public List<Device> Devices { get; set; }
        public void SetDevices(List<Device> devices)
        {
            this.Devices= devices;

            DeviceState = new List<DeviceState>();
            foreach(var device in devices)
            {
                if (device.events !=null && device.events.Count > 0)
                {
                    if(device.type == "AirCondition")
                    {
                        JObject jo = JObject.Parse(device.events.FirstOrDefault().data);
                        string temperature = jo["temperature"].ToString();
                        string humidity = jo["humidity"].ToString();
                        string controlTemperature = jo["controlTemperature"].ToString();
                        string DeviceOn = jo["airConditionOn"].ToString();
                        DeviceState deviceState = new DeviceState
                        {
                            Connection = device.connection_state,
                            DeviceName = device.iot_id,
                            StateTime = device.events.FirstOrDefault().iot_event_time.Value.ToString("dd MMMM yyyy, HH:mm:ss tt"),
                            Temperature = double.Parse(temperature).ToString("0.0") + "C",
                            DeviceControlTemperture = double.Parse(controlTemperature).ToString("0.0") + "C",
                            Humidity = double.Parse(humidity).ToString("0.0") + "%",
                            DeviceType = device.type,
                            DeviceOn = DeviceOn
                        };
                        deviceState.Events = new List<Event>();
                        foreach (var eventT in device.events)
                        {
                            JObject joT = JObject.Parse(eventT.data);
                            string temperatureT = joT["temperature"].ToString();
                            string humidityT = joT["humidity"].ToString();
                            deviceState.Events.Add(new Event
                            {
                                DeviceOn = DeviceOn=="on",
                                Temperature = double.Parse(double.Parse(temperatureT).ToString("0.0")),
                                Humidity = double.Parse(double.Parse(humidityT).ToString("0.0")),
                                Time = eventT.iot_event_time.Value.ToString("yyyy-MM-dd HH:mm:ss")

                            });



                        }
                        this.DeviceState.Add(deviceState);
                    }
                    if (device.type == "Light")
                    {
                        JObject jo = JObject.Parse(device.events.FirstOrDefault().data);
                        string lightOn = jo["lightOn"].ToString();
                        
                        DeviceState deviceState = new DeviceState
                        {
                            Connection = device.connection_state,
                            DeviceName = device.iot_id,
                            StateTime = device.events.FirstOrDefault().iot_event_time.Value.ToString("dd MMMM yyyy, HH:mm:ss tt"),
                            DeviceOn = lightOn,
                            DeviceType = device.type
                        };
                        deviceState.Events = new List<Event>();
                        foreach (var eventT in device.events)
                        {
                            
                            deviceState.Events.Add(new Event
                            {

                                DeviceOn = lightOn=="on",
                                
                                Time = eventT.iot_event_time.Value.ToString("yyyy-MM-dd HH:mm:ss")

                            });



                        }
                        this.DeviceState.Add(deviceState);
                    }
                    if (device.type == "Meter")
                    {
                        JObject jo = JObject.Parse(device.events.FirstOrDefault().data);
                        string value = double.Parse(jo["value"].ToString()).ToString("0.00");
                        if (device.iot_id.ToLower().Contains("gas-"))
                        {
                            value += " cubic metres";
                        }
                        else if (device.iot_id.ToLower().Contains("water-"))
                        {
                            value += " cubic metres";
                        }
                        else if (device.iot_id.ToLower().Contains("electricity-"))
                        {
                            value += " KW/h";
                        }
                        DeviceState deviceState = new DeviceState
                        {
                            Connection = device.connection_state,
                            DeviceName = device.iot_id,
                            StateTime = device.events.FirstOrDefault().iot_event_time.Value.ToString("dd MMMM yyyy, HH:mm:ss tt"),
                            Value = value,
                            DeviceType = device.type
                        };
                        deviceState.Events = new List<Event>();
                        foreach (var eventT in device.events)
                        {
                            JObject joT = JObject.Parse(eventT.data);
                            string valueT = joT["value"].ToString();

                            deviceState.Events.Add(new Event
                            {

                                Value =double.Parse(double.Parse(valueT).ToString("0.00")),

                                Time = eventT.iot_event_time.Value.ToString("yyyy-MM-dd HH:mm:ss")

                            });



                        }
                        this.DeviceState.Add(deviceState);
                    }
                }
                

            }
            
        }
    }
   
    public class DeviceState
    {
        public string Connection { get; set; }
        public string DeviceName { get; set; }
        public string StateTime { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public string DeviceType { get; set; }
        public string DeviceOn { get; set; }
        public string DeviceControlTemperture { get; set; }
        public List<Event> Events { get; set; }
        public string Value { get; set; }
        public DeviceState()
        {
            Connection = "";
            DeviceName = "";
            StateTime = "";
            DeviceOn = "";
            Temperature = "";
            Humidity = "";
            Value = "";
            DeviceControlTemperture = "";
        }
    }
    
    public class Event
    {
        public bool DeviceOn { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Value { get; set; }
        public string Time { get; set; }
    }

}
