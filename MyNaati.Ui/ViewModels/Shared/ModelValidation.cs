using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels.Shared
{
    /// <summary>
    /// This is used to validate the format of an email address.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateEmail : ValidationAttribute, IClientValidatable
    {
        private const string EMAIL_REGEX = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

        public ValidateEmail() : base("Enter a valid email address.") { }

        public override bool IsValid(object value)
        {
            var regEx = new System.Text.RegularExpressions.Regex(EMAIL_REGEX);
            var valueString = value != null ? value.ToString() : string.Empty;
            return regEx.IsMatch(valueString);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]{
                new ModelClientValidationRegexRule(FormatErrorMessage(metadata.GetDisplayName()), EMAIL_REGEX)
            };
        }
    }

    /// <summary>
    /// THis is used to validate a Naati to make sure it is a positive integer
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateNaatiNumberAttribute : ValidationAttribute, IClientValidatable
    {
        private const int MIN_VALUE = 1;
        private bool mAllowBlank = false;

        public ValidateNaatiNumberAttribute() : base("Enter a valid Customer Number.") { }

        public ValidateNaatiNumberAttribute(bool allowBlank)
            : this()
        {
            mAllowBlank = allowBlank;
        }

        public override bool IsValid(object value)
        {
            int tryValue;
            var valueString = value != null ? value.ToString() : string.Empty;

            if (string.IsNullOrEmpty(valueString) && mAllowBlank)
                return true;

            if (int.TryParse(valueString, out tryValue))
            {
                if (tryValue >= MIN_VALUE)
                    return true;
            }

            return false;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]{
                new ModelClientValidationRangeRule(FormatErrorMessage(metadata.GetDisplayName()), MIN_VALUE, int.MaxValue)
            };
        }
    }
}