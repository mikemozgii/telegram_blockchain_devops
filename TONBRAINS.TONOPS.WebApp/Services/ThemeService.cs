using System.Collections.Generic;

namespace TONBRAINS.TONOPS.WebApp.Services.Implementations
{
    public class ThemeService : IThemeService
    {

        public IDictionary<string, string> GetColorPalette(IDictionary<string, string> customThemes = null)
        {
            var colorPalette = new Dictionary<string, string>
            {
                ["global-orange"] = "rgb(240, 173, 78)",
                ["global-red"] = "#ee1c29",
                ["global-blue"] = "#ee1c29",
                ["global-white"] = "#fff",
                ["global-black"] = "#000",
                ["global-bordergray"] = "rgba(0,0,0,.125)",
                ["global-lightgray"] = "lightgrey",
                ["global-green"] = "#00B00D",
                ["global-darkgrey"] = "#424242",
                ["global-gray"] = "gray",
                ["global-yellow"] = "#FFCF82",
                ["global-color"] = "#000",
                ["global-background-color"] = "#fff",
                ["global-border-color"] = "rgba(0,0,0,.125)",
                ["global-border-style"] = "solid",
                ["global-border"] = "1px solid",
                ["global-border-width"] = "1px",
                ["global-border-style"] = "solid",
                ["global-font-size"] = "13px",
                ["global-disable-color"] = "gray",
                ["global-disable-background-color"] = "lightgray",
                ["global-disable-font-size"] = "14px",
                ["global-disable-border-color"] = "lightgray",
                ["global-not-valid-color"] = "#ff6633",

                ["global-hover-background-color"] = "#fff",
                ["global-hover-color"] = "#fff",
                ["global-trash-red"] = "#FF0624",
                ["global-green-button"] = "#6FD967",
                ["global-cadetblue"] = "#17a2b8",

                ["global-header-block-background-color"] = "#5E5D5D",
                ["global-icon-fixedsidebar-color"] = "#5e6e82",
                ["global-page-background-color"] = "#f8f9fa",
                ["global-event-group"] = "#d9f3db",

                ["global-default-avatar-background-color"] = "#FF7222",
                ["global-sign-in-window-border-color"] = "#ee1c29",

                ["global-chart-blue"] = "#ee1c29",
                ["global-chart-green"] = "#00B00D",
                ["global-chart-yellow"] = "#FFCF82",
                ["global-chart-orange"] = "#f0ad4e",
                ["global-chart-lime"] = "#89BA0C",
                ["global-chart-dark-orange"] = "#D96020",
                ["global-chart-light-blue"] = "#589ED4",
                ["global-chart-pink"] = "#D63E8C",
                ["global-chart-aquamarine"] = "#44B19B",
            };

            if (customThemes != null)
            {
                foreach (var theme in customThemes)
                {
                    if (colorPalette.ContainsKey(theme.Key)) colorPalette[theme.Key] = theme.Value;
                }
            }

            return colorPalette;
        }
    }

}
