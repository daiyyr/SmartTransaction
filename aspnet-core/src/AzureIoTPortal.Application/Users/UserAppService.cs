using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using AzureIoTPortal.Authorization;
using AzureIoTPortal.Authorization.Accounts;
using AzureIoTPortal.Authorization.Roles;
using AzureIoTPortal.Authorization.Users;
using AzureIoTPortal.Data;
using AzureIoTPortal.Roles.Dto;
using AzureIoTPortal.Users.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AzureIoTPortal.Users
{
    //[AbpAuthorize(PermissionNames.Pages_Users)]
    public class UserAppService : AsyncCrudAppService<User, UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAbpSession _abpSession;
        private readonly LogInManager _logInManager;

        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IRepository<Role> roleRepository,
            IPasswordHasher<User> passwordHasher,
            IAbpSession abpSession,
            LogInManager logInManager)
            : base(repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _abpSession = abpSession;
            _logInManager = logInManager;
        }

        public override async Task<UserDto> Create(CreateUserDto input)
        {
            CheckCreatePermission();

            var user = ObjectMapper.Map<User>(input);

            user.TenantId = AbpSession.TenantId;
            user.IsEmailConfirmed = true;

            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

            CheckErrors(await _userManager.CreateAsync(user, input.Password));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(user);
        }

        public override async Task<UserDto> Update(UserDto input)
        {
            CheckUpdatePermission();

            var user = await _userManager.GetUserByIdAsync(input.Id);

            MapToEntity(input, user);

            CheckErrors(await _userManager.UpdateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            return await Get(input);
        }

        public override async Task Delete(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            await _userManager.DeleteAsync(user);
        }

        public async Task<ListResultDto<RoleDto>> GetRoles()
        {
            var roles = await _roleRepository.GetAllListAsync();
            return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
        }

        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            user.SetNormalizedNames();
            return user;
        }

        protected override void MapToEntity(UserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }

        protected override UserDto MapToEntityDto(User user)
        {
            var roles = _roleManager.Roles.Where(r => user.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => r.NormalizedName);
            var userDto = base.MapToEntityDto(user);
            userDto.RoleNames = roles.ToArray();
            return userDto;
        }

        protected override IQueryable<User> CreateFilteredQuery(PagedUserResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Roles)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.UserName.Contains(input.Keyword) || x.Name.Contains(input.Keyword) || x.EmailAddress.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(User), id);
            }

            return user;
        }

        protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedUserResultRequestDto input)
        {
            return query.OrderBy(r => r.UserName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public async Task<bool> ChangePassword(ChangePasswordDto input)
        {
            if (_abpSession.UserId == null)
            {
                throw new UserFriendlyException("Please log in before attemping to change password.");
            }
            long userId = _abpSession.UserId.Value;
            var user = await _userManager.GetUserByIdAsync(userId);
            var loginAsync = await _logInManager.LoginAsync(user.UserName, input.CurrentPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException("Your 'Existing Password' did not match the one on record.  Please try again or contact an administrator for assistance in resetting your password.");
            }
            if (!new Regex(AccountAppService.PasswordRegex).IsMatch(input.NewPassword))
            {
                throw new UserFriendlyException("Passwords must be at least 8 characters, contain a lowercase, uppercase, and number.");
            }
            user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
            CurrentUnitOfWork.SaveChanges();
            return true;
        }

        public async Task<bool> ResetPassword(ResetPasswordDto input)
        {
            if (_abpSession.UserId == null)
            {
                throw new UserFriendlyException("Please log in before attemping to reset password.");
            }
            long currentUserId = _abpSession.UserId.Value;
            var currentUser = await _userManager.GetUserByIdAsync(currentUserId);
            var loginAsync = await _logInManager.LoginAsync(currentUser.UserName, input.AdminPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException("Your 'Admin Password' did not match the one on record.  Please try again.");
            }
            if (currentUser.IsDeleted || !currentUser.IsActive)
            {
                return false;
            }
            var roles = await _userManager.GetRolesAsync(currentUser);
            if (!roles.Contains(StaticRoleNames.Tenants.Admin))
            {
                throw new UserFriendlyException("Only administrators may reset passwords.");
            }

            var user = await _userManager.GetUserByIdAsync(input.UserId);
            if (user != null)
            {
                user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
                CurrentUnitOfWork.SaveChanges();
            }

            return true;
        }

        public async Task<UserDto> GetCurrentUserSMSInfo(string connIOT, string connSMS)
        {
            if (_abpSession.UserId == null)
            {
                throw new UserFriendlyException("Please log in");
            }
            long currentUserId = _abpSession.UserId.Value;
            var currentUser = await _userManager.GetUserByIdAsync(currentUserId);
            var userDto = base.MapToEntityDto(currentUser);

            Odbc o1 = new Odbc(connIOT);
            Odbc o2 = new Odbc(connSMS);
            var t1 = o1.ReturnTable("select * from user_debtor where user_debtor_user_id = " + currentUserId, "user");
            if(t1.Rows.Count == 0)
            {
                throw new System.Exception("Current User Id Error");
            }
            var t2 = o2.ReturnTable(@"
select * from debtor_master
join unit_master on unit_master_debtor_id = debtor_master_id
join property_master on property_master_id = unit_master_property_id
join bodycorps on bodycorp_id = property_master_bodycorp_id
where debtor_master_id = " + t1.Rows[0]["user_debtor_debtor_id"].ToString()
, "debtor_info");
            foreach(DataRow dr in t2.Rows)
            {
                if(string.IsNullOrEmpty(userDto.DebtorId))
                {
                    userDto.DebtorId = dr["debtor_master_id"].ToString();
                }
                if (string.IsNullOrEmpty(userDto.firstAddress))
                {
                    userDto.firstAddress = 
                        "BC" + " " + dr["bodycorp_code"].ToString() 
                        + " | " + dr["bodycorp_name"].ToString();
                }
                if (userDto.BodycorpId == null)
                {
                    userDto.BodycorpId = new List<string>();
                }
                if (userDto.BodycorpCode == null)
                {
                    userDto.BodycorpCode = new List<string>();
                }
                if (userDto.UnitId == null)
                {
                    userDto.UnitId = new List<string>();
                }
                if (userDto.UnitCode == null)
                {
                    userDto.UnitCode = new List<string>();
                }
                if (!userDto.BodycorpId.Contains(dr["bodycorp_id"].ToString()))
                {
                    userDto.BodycorpId.Add(dr["bodycorp_id"].ToString());
                }
                if (!userDto.UnitId.Contains(dr["unit_master_id"].ToString()))
                {
                    userDto.UnitId.Add(dr["unit_master_id"].ToString());
                }
                if (!userDto.UnitId.Contains(dr["unit_master_code"].ToString()))
                {
                    userDto.UnitCode.Add(dr["unit_master_code"].ToString());
                }
                if (!userDto.BodycorpCode.Contains(dr["bodycorp_code"].ToString()))
                {
                    userDto.BodycorpCode.Add(dr["bodycorp_code"].ToString());
                }
                if (!userDto.AGMDate.HasValue && !string.IsNullOrEmpty(dr["bodycorp_agm_date"].ToString()))
                {
                    userDto.AGMDate = DBSafeUtils.DBDateToDateN(dr["bodycorp_agm_date"]);
                }
                if (!userDto.AGMTime.HasValue && !string.IsNullOrEmpty(dr["bodycorp_agm_time"].ToString()))
                {
                    userDto.AGMTime = DBSafeUtils.DBTimeToTimeN(dr["bodycorp_agm_time"]);
                }
            }

            return userDto;
        }

    }
}

