using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;
using System.Reflection;
using Cities.Models;
using System.Linq;

namespace Cities.Infrastructure.TagHelpers
{
    [HtmlTargetElement("select", Attributes = "model-for")]
    public class SelectOptionTagHelper : TagHelper {
        private IRepository repository;

        public SelectOptionTagHelper(IRepository repo) {
            this.repository = repo;
        }

        public ModelExpression ModelFor { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, 
                                     TagHelperOutput output) {

            var childrenHtml = (await output.GetChildContentAsync(false)).GetContent();

            output.Content.AppendHtml(childrenHtml);

            string selected = ModelFor.Model as string;

            PropertyInfo property = typeof(City).GetTypeInfo().GetDeclaredProperty(ModelFor.Name);

            foreach (string country in repository.Cities.Select(c => property.GetValue(c)).Distinct()) {
                if (selected != null &&
                        selected.Equals(country, StringComparison.OrdinalIgnoreCase)) { 
                    output.Content.AppendHtml($"<option selected>{country}</option>");
                }
                else {
                    output.Content.AppendHtml($"<option>{country}</option>");
                }
            }
            output.Attributes.SetAttribute("Name", ModelFor.Name);
            output.Attributes.SetAttribute("Id", ModelFor.Name);
        }

    }
}
