using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoatGate.TagHelpers
{
    public class HelpTagHelper : TagHelper
    {
        public string Text { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "i";
            output.Attributes.Add("class", "far fa-question-circle");
            output.Attributes.Add("data-toggle", "tooltip");
            output.Attributes.Add("data-placement", "right");
            output.Attributes.Add("title", Text ?? "Some text.");
        }
    }
}
