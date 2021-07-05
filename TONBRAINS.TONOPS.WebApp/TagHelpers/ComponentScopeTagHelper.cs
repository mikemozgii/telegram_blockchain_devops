using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.WebApp.TagHelpers
{
    [HtmlTargetElement(Attributes = "componentscope")]
    public class ComponentScopeTagHelper : TagHelper
    {
        /// <summary>
        /// Process scripts scope context (with attribute only for script tag).
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
            output.Content.SetHtmlContent($"const componentId = '{componentId}';\n" + content.Replace("-component-id", $"-{componentId}"));
        }

        [ViewContext]
        public ViewContext ViewContext { get; set; }
    }
}
