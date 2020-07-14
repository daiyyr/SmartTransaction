using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Domain.Repositories;
using AutoMapper;
using AzureIoTPortal.AzureIoT.Dto;
using AzureIoTPortal.SMS;

namespace AzureIoTPortal.AzureIoT
{
    public class BodycorpAppService : AzureIoTPortalAppServiceBase, IBodycorpAppService
    {
        //private readonly IRepository<Bodycorp> _BodycorpRepository;
        public BodycorpAppService(
            //IRepository<Bodycorp> BodycorpRepository
            )
        {
            //_BodycorpRepository = BodycorpRepository;
        }
        public List<Bodycorp> GetBodycorps()
        {
            //return _BodycorpRepository.GetAll().ToList();
            return null;

        }

    }
}
