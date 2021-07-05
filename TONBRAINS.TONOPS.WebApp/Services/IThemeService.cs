using System.Collections.Generic;

namespace TONBRAINS.TONOPS.WebApp.Services
{

    public interface IThemeService
    {
        IDictionary<string, string> GetColorPalette(IDictionary<string, string> customThemes = null);

    }

}
