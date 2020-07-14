using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureIoTPortal
{
    internal interface IDtoMapping
    {
        void CreateMapping(IMapperConfigurationExpression mapperConfig);
    }
}
