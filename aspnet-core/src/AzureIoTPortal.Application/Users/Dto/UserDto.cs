using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using AzureIoTPortal.Authorization.Users;

namespace AzureIoTPortal.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserDto : EntityDto<long>
    {
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public bool IsActive { get; set; }

        public string FullName { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public DateTime CreationTime { get; set; }

        public string[] RoleNames { get; set; }


        public string DebtorId { get; set; }
        public List<string> BodycorpId { get; set; }
        public List<string> BodycorpCode { get; set; }
        public List<string> UnitId { get; set; }
        public List<string> UnitCode { get; set; }
        public string firstAddress { get; set; }
        public DateTime? AGMDate { get; set; }
        public TimeSpan? AGMTime { get; set; }

    }
}
