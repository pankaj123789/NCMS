using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MyNaati.Ui.Attributes
{
    /// <summary>
    /// A simple model validation attribute to ensure tha the date is less than or equal to today's date
    /// </summary>
    public class LessThanOrEqualToCurrentDate : ValidationAttribute
    {
        public LessThanOrEqualToCurrentDate()
        {
        }

        public override bool IsValid(object value)
        {
            if (!(value is DateTime))
            {
                return false; // This should include null checks
            }

            var date = (DateTime)value;
            return date <= DateTime.Now;
        }
    }

    public class IsDifferentToValue : ValidationAttribute
    {
        private readonly int _value;

        public IsDifferentToValue(int value)
        {
            this._value = value;
        }

        public override bool IsValid(object value)
        {
            int val;

            if (int.TryParse(value?.ToString(), out val))
            {
                return val != _value;
            }
            return true;
        }
    }

    public class CaseInsensitiveCompareAttribute : CompareAttribute
    {
        public CaseInsensitiveCompareAttribute(string otherProperty)
            : base(otherProperty)
        { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(this.OtherProperty);
            if (property == null)
            {
                return new ValidationResult(string.Format(CultureInfo.CurrentCulture, "Property {0} Not found", this.OtherProperty));
            }
            var otherValue = property.GetValue(validationContext.ObjectInstance, null) as string;
            if (string.Equals(value as string, otherValue, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
        }
    }

}