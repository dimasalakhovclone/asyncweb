using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString ValidationSummaryFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.ValidationSummaryFor(expression, new RouteValueDictionary());
        }

        public static MvcHtmlString ValidationSummaryFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return htmlHelper.ValidationSummaryFor(expression,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString ValidationSummaryFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            var modelMetadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            var modelName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            var formContext = htmlHelper.ViewContext.GetFormContextForClientValidation1();

            if (!htmlHelper.ViewData.ModelState.ContainsKey(modelName) && formContext == null)
            {
                return null;
            }

            var modelState = htmlHelper.ViewData.ModelState[modelName];
            if (modelState == null || modelState.Errors.Any() == false)
                return MvcHtmlString.Empty;

            var messageBuilder = new StringBuilder();
            foreach (var error in modelState.Errors)
            {
                var builder = new TagBuilder("span");
                builder.MergeAttributes(htmlAttributes);
                builder.AddCssClass(HtmlHelper.ValidationMessageCssClassName);
                builder.SetInnerText(error.ErrorMessage);

                if (formContext != null)
                {
                    if (htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
                    {
                        builder.MergeAttribute("data-valmsg-for", modelName);
                        builder.MergeAttribute("data-valmsg-replace", "false");
                    }
                    else
                    {
                        FieldValidationMetadata fieldMetadata = ApplyFieldValidationMetadata(htmlHelper, modelMetadata, modelName);
                        // rules will already have been written to the metadata object
                        fieldMetadata.ReplaceValidationMessageContents = false;
                        // client validation always requires an ID
                        builder.GenerateId(modelName + "_validationMessage");
                        fieldMetadata.ValidationMessageId = builder.Attributes["id"];
                    }
                }
                messageBuilder.Append(builder.ToString(TagRenderMode.Normal));
            }

            return new MvcHtmlString(messageBuilder.ToString());
        }

        private static FieldValidationMetadata ApplyFieldValidationMetadata(HtmlHelper htmlHelper, ModelMetadata modelMetadata, string modelName)
        {
            var formContext = htmlHelper.ViewContext.FormContext;
            var fieldMetadata = formContext.GetValidationMetadataForField(modelName, true /* createIfNotFound */);

            // write rules to context object
            var validators = ModelValidatorProviders.Providers.GetValidators(modelMetadata, htmlHelper.ViewContext);
            foreach (var rule in validators.SelectMany(v => v.GetClientValidationRules()))
            {
                fieldMetadata.ValidationRules.Add(rule);
            }

            return fieldMetadata;
        }
    }

    public static class ViewContextExtensions
    {
        public static FormContext GetFormContextForClientValidation1(this ViewContext context)
        {
            return (context.ClientValidationEnabled) ? context.FormContext : null;
        }
    }
}