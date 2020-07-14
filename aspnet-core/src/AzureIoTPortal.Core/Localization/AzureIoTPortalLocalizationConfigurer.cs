using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace AzureIoTPortal.Localization
{
    public static class AzureIoTPortalLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(AzureIoTPortalConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(AzureIoTPortalLocalizationConfigurer).GetAssembly(),
                        "AzureIoTPortal.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
