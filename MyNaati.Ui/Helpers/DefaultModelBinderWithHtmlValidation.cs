using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;

namespace MyNaati.Ui.Helpers
{
    public class DefaultModelBinderWithHtmlValidation : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            try
            {
                var obj = base.BindModel(controllerContext, bindingContext);
                var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName)?.AttemptedValue;
                
                if (value != null)
                {
                    char tab = '\u0009';
                    value = value.Replace(tab.ToString(), " ");
                    if (value.Any(char.IsControl))
                    {
                        var newValue = GetValuesFromAttributes(bindingContext, value);
                        if (newValue.Any(char.IsControl))
                        {
                            LoggingHelper.LogWarning(
                                "Illegal characters were found in the {Field} field. Characters: {IllegalCharacters}. URL: {URL}",
                                bindingContext.ModelName, newValue.Where(char.IsControl).Select(x => (int) x),
                                controllerContext.HttpContext.Request.Url);
                            throw new HttpRequestValidationException(
                                $"Illegal characters were found in the {bindingContext.ModelMetadata.DisplayName ?? bindingContext.ModelName} field.");
                        }
                    }
                }
                return obj;
            }
            catch (HttpRequestValidationException ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName,
                    string.Format("Ilegal characters were found in field {0}", bindingContext.ModelMetadata.DisplayName ?? bindingContext.ModelName));
                throw new IllegalInputCharacterException(ex.Message);
            }
        }

        private static string GetValuesFromAttributes(ModelBindingContext bindingContext, string value)
        {
            var holderType = bindingContext.ModelMetadata.ContainerType;
            if (holderType == null)
            {
                return value;
            }
            var propertyType = holderType.GetProperty(bindingContext.ModelMetadata.PropertyName);

            if (propertyType == null)
            {
                return value;
            }
            var attributes = propertyType.GetCustomAttributes(true);
            var characters = attributes
                .Cast<Attribute>()
                .FirstOrDefault(a => a.GetType().IsEquivalentTo(typeof(LegalCharactersAttribute)));
            var legalCharactersAttribute = (LegalCharactersAttribute)characters;

            if (legalCharactersAttribute == null)
            {
                return value;
            }
            var temp = value.Split(legalCharactersAttribute.Values, StringSplitOptions.RemoveEmptyEntries);
            var newValue = string.Join(string.Empty, temp);
            return newValue;
        }
    }
}