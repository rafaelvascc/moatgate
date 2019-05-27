using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate.TagHelpers
{
    public class InfoLinkTagHelper : TagHelper
    {
        public string Src { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            if (!string.IsNullOrEmpty(Src))
            {
                output.Attributes.Add("src", Src);
                output.Attributes.Add("target", "_blank");
            }
            output.Content.AppendFormat("<i class='far fa-info-circle'><i/>");
        }
    }
}
