using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MyNaati.Ui.Common
{
    //TODO: THis could be made more generic, with a way to pass in the direction of validation.
    public class DateComparison : ValidationAttribute
    {

        public string OtherProperty { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo property = validationContext.ObjectType.GetProperty(this.OtherProperty);

            if (property == null)
            {
                return
                    new ValidationResult(string.Format("Unknown property type in validation attribute : {0}",
                                                       OtherProperty));
            }

            var otherValue = (DateTime)property.GetValue(validationContext.ObjectInstance, null);

            if ((DateTime)value > otherValue)
            {
                return new ValidationResult(ErrorMessage);
            }


            return null;

        }
    }
}