using System.ComponentModel.DataAnnotations;

namespace MyNaati.Ui.Common
{
    public class NotZeroAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || (int)value != 0)
                return null;

            return new ValidationResult(this.ErrorMessage);
        }
    }
}