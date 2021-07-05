using TONBRAINS.TONOPS.WebApp.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SharpScss;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.WebApp.TagHelpers
{
    [HtmlTargetElement(Attributes = "scoped")]
    public class ScopedStyleTagHelper : TagHelper
    {
        private readonly IThemeService m_ThemeService;

        public ScopedStyleTagHelper(IThemeService themeService)
        {
            m_ThemeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        }


        /// <summary>
        /// Process scoped styles (with attribute only for style tag).
        /// </summary>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var componentId = ViewContext.HttpContext.Items
                .Where(a => a.Key is string && a.Key.ToString().StartsWith("ComponentId"))
                .Select(a => a.Value)
                .FirstOrDefault();
            //WORKAROUND: omg? but this is works if are few tags 0_0
            ViewContext.HttpContext.Items["ComponentId" + Guid.NewGuid().ToString().Replace("-", "")] = componentId ?? throw new InvalidOperationException("Do not specified component identifier! Use tag vue-component to your cshtml file.");

            var childContent = await output.GetChildContentAsync();
            var content = childContent.GetContent();
            var cleanContent = content.Replace("-component-id", $"-{componentId}");

            //TODO: integrate custom themes

            var palette = m_ThemeService.GetColorPalette().Select(a => $"${a.Key}: {a.Value};").Aggregate("", (left, right) => $"{left}\n{right}") + "\n";

            output.Content.SetHtmlContent(Scss.ConvertToCss(palette + cleanContent).Css);
        }

        [ViewContext]
        public ViewContext ViewContext { get; set; }
    }
}
