using System.ComponentModel.DataAnnotations;

namespace AzureIoTPortal.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}