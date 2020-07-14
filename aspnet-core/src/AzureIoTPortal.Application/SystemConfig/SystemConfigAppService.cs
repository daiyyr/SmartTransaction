using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AzureIoTPortal.SystemConfig
{
    public class SystemConfigAppService : AzureIoTPortalAppServiceBase, ISystemConfigAppService
    {
        private IRepository<SystemConfig> _SystemConfigRepository;
        public SystemConfigAppService(IRepository<SystemConfig> SystemConfigRepostory)
        {
            _SystemConfigRepository = SystemConfigRepostory;
        }
        public string GetConfig(string systemCode)
        {
            var sConfig = _SystemConfigRepository.FirstOrDefault(x => x.system_code == systemCode);
            string value = "";
            if (sConfig != null)
                value = sConfig.system_value;
            return value;
        }
    }
}
