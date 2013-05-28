using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace System.Web.Mvc
{
    public static partial class HtmlExtensions
    {
        public static MvcHtmlString LabelWithTooltipFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metaData.DisplayName ?? metaData.PropertyName ?? htmlFieldName.Split('.').Last();

            if (String.IsNullOrEmpty(labelText))
                return MvcHtmlString.Empty;

            var label = new TagBuilder("label");
            label.Attributes.Add("for", helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

            if (!string.IsNullOrEmpty(metaData.Description))
                label.Attributes.Add("title", metaData.Description);

            label.AddCssClass("tooltip");

            label.SetInnerText(labelText);
            return MvcHtmlString.Create(label.ToString());
        }
    }
}
