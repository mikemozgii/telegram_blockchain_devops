using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.WebApp.TagHelpers
{
    [HtmlTargetElement("vue-template")]
    public class VueTemplateTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var componentId = ViewContext.HttpContext.Items["ComponentId"];

            var attibutes = context.AllAttributes
                .Where(a => a.Name != "class" && a.Name != "autoclosetag")
                .Select(a => $"{a.Name}=\"{a.Value}\"")
                .ToArray();

            output.TagName = "template";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.PreContent.SetHtmlContent($"<div class=\"container-{componentId}\" {string.Join(' ', attibutes)}>");
            output.PostContent.SetHtmlContent("</div>");

            var childContent = await output.GetChildContentAsync();
            var content = childContent.GetContent();

            var isSupportAutoClosedTag = context.AllAttributes.Any(a => a.Name == "autoclosetag");
            if (isSupportAutoClosedTag)
            {
                //\<[a-zA-Z-\s\=\"\'\d\@\$\:\`\.\+\-\{\}\|\_\?\,\[\]\(\)\{\}]{0,}\s\/\>
                var matches = Regex.Matches(content, "\\<[a-zA-Z-\\s\\=\\\"\"\\'\\d\\@\\$\\:\\`\\.\\+\\-\\{\\}|_\\?\\,\\[\\]\\(\\)\\{\\}]{0,}\\s\\/\\>").Cast<Match>();
                foreach (var match in matches)
                {
                    var value = match.Value;
                    var tagName = value.Substring(0, value.IndexOf(" "));
                    if (!tagName.Contains("-")) continue;

                    var replaced = value.Replace("/>", ">") + tagName.Replace("<", "</") + ">";
                    content = content.Replace(value, replaced);
                }           
            }

            output.Content.SetHtmlContent(content.Replace("-component-id", $"-{componentId}"));
        }

        [ViewContext]
        public ViewContext ViewContext { get; set; }
    }
}
