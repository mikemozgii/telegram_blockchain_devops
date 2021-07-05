using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.WebApp.TagHelpers
{
    [HtmlTargetElement("script", Attributes = "vuescript")]
    public class VueScriptTagHelper : TagHelper
    {

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync();
            var content = childContent.GetContent();

            /*if (ViewContext.ViewData.ContainsKey("PayloadJson"))
            {
                content = ViewContext.ViewData["PayloadJson"] + "\r\n" + content;
            }*/

            output.Content.SetHtmlContent(content);
        }

    }
}
