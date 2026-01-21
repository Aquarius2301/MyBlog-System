using Application.Resources;
using Application.Services.Interfaces;
using Microsoft.Extensions.Localization;

namespace Application.Services.Implementations
{
    public class LanguageService : ILanguageService
    {
        private readonly IStringLocalizer _localizer;

        public LanguageService(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResources);
            _localizer = factory.Create(type);
        }

        public string Get(string key)
        {
            var localizedString = _localizer[key];

            return localizedString.Value;
        }
    }
}
