using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyNaati.Ui.Common
{
    public class RequiredIfAttribute : RequiredAttribute
    {
        // this class has been implemented to allow an attribute to be required ONLY if other properties
        // have been provided, if other conditionaly required functionality is needed it will need to be added to this class

        private String propertyName { get; set; }

        // isprovided determines wether we need to check that "propertyName" is null/empty or not null/empty
        private bool isProvided { get; set; }

        private bool currentDate { get; set; }

        private string currentDateErrorMessage { get; set; }

        public RequiredIfAttribute(String propertyName, bool IsProvided)
        {
            this.propertyName = propertyName;
            isProvided = IsProvided;
            currentDate = false;
        }

        public RequiredIfAttribute(String propertyName, bool IsProvided, bool LessThanOrEqualToCurrentDate, string CurrentDateErrorMessage = "error")
        {
            this.propertyName = propertyName;
            isProvided = IsProvided;
            currentDate = LessThanOrEqualToCurrentDate;
            currentDateErrorMessage = CurrentDateErrorMessage;
        }

        public static bool LessThanOrEqualToCurrentDate(object value)
        {
            if (!(value is DateTime))
            {
                return false; // This should include null checks
            }

            var date = (DateTime)value;
            return date <= DateTime.Now;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object instance = validationContext.ObjectInstance;
            Type type = instance.GetType();
            object propertyValue = new object();

            // first get the property value
            propertyValue = type.GetProperty(propertyName).GetValue(instance, null);

            // then determine the result

            // test status of required property, then based on requirement (base required if property given or not)
            // either return validation success (the base is not required because the property does not match our requirement)
            // or perform base validation (the base is required because the property matches our requirement)
            var result = ((propertyValue == null || propertyValue.ToString() == string.Empty) ? isProvided : !isProvided)
                ? base.IsValid(value, validationContext) : ValidationResult.Success;

            if (result != ValidationResult.Success)
            {
                return result;
            }

            if (((propertyValue == null || propertyValue.ToString() == string.Empty) ? isProvided : !isProvided) && currentDate)
            {
                if (!LessThanOrEqualToCurrentDate(value))
                {
                    return new ValidationResult(currentDateErrorMessage, new List<string>() { validationContext.DisplayName });
                }
            }

            return result;
        }
    }
}