using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AzureIoTPortal.Web.Startup
{
    public static class AuthConfigurer
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            if (bool.Parse(configuration["Authentication:JwtBearer:IsEnabled"]))
            {
                services.AddAuthentication()
                    .AddJwtBearer(options =>
                    {
                        options.Audience = configuration["Authentication:JwtBearer:Audience"];
                        
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // The signing key must match!
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Authentication:JwtBearer:SecurityKey"])),

                            // Validate the JWT Issuer (iss) claim
                            ValidateIssuer = true,
                            ValidIssuer = configuration["Authentication:JwtBearer:Issuer"],

                            // Validate the JWT Audience (aud) claim
                            ValidateAudience = true,
                            ValidAudience = configuration["Authentication:JwtBearer:Audience"],

                            // Validate the token expiry
                            ValidateLifetime = true,

                            // If you want to allow a certain amount of clock drift, set that here
                            ClockSkew = TimeSpan.Zero
                        };
                    });
                
            }
        }
    }




    /*
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class SappUserManager : UserManager<User>
    {
        public SappUserManager(IUserStore<User> store)
            : base(store)
        {
        }

        public static SappUserManager Create(IdentityFactoryOptions<SappUserManager> options, IOwinContext context)
        {
            var manager = new SappUserManager(new UserStore<User>(context.Get<IdentityContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<User>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<User>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<User>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class SappPermissionManager : RoleManager<Permission>
    {
        public SappPermissionManager(IRoleStore<Permission, string> roleStore)
            : base(roleStore)
        {
        }

        public static SappPermissionManager Create(IdentityFactoryOptions<SappPermissionManager> options, IOwinContext context)
        {
            return new SappPermissionManager(new RoleStore<Permission>(context.Get<IdentityContext>()));
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class SappSignInManager : SignInManager<User, string>
    {
        public SappSignInManager(SappUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return user.GenerateUserIdentityAsync((SappUserManager)UserManager);
        }

        public static SappSignInManager Create(IdentityFactoryOptions<SappSignInManager> options, IOwinContext context)
        {
            return new SappSignInManager(context.GetUserManager<SappUserManager>(), context.Authentication);
        }
    }

    */




}
