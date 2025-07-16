using System;
using System.ComponentModel.DataAnnotations;

namespace MyNaati.Ui.ViewModels.ApplicationByTestingWizard
{
    public class LanguageProficiencyModel
    {
        public LanguageProficiencyModel()
        {
        }

        public bool TookEnglishProficiencyTest { get; set; }
        public bool IsIELTS { get; set; }
        public bool IsAcademic { get; set; }

        [OtherTestingLocationValidator]
        public string OtherTestDetails { get; set; }

        [Range(0.0, 100.0)]
        public double? Listening { get; set; }

        [Range(0.0, 100.0)]
        public double? Speaking { get; set; }

        [Range(0.0, 100.0)]
        public double? Reading { get; set; }

        [Range(0.0, 100.0)]
        public double? Writing { get; set; }

        [OverallValidator]
        [Range(0.0, 100.0)]
        public double? Overall { get; set; }
    }

    public class OtherTestingLocationValidator : ValidationAttribute
    {
        private const string mErrorMessage = "You need to specify a location for the Other field.";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult result = null;

            var model = (LanguageProficiencyModel)validationContext.ObjectInstance;

            if (!model.IsIELTS && model.TookEnglishProficiencyTest)
            {
                if (String.IsNullOrEmpty((string)value))
                {
                    result = new ValidationResult(mErrorMessage, new[] { "Test" });
                }
            }

            return result;
        }
    }

    public class OverallValidator : ValidationAttribute
    {
        private const string mErrorMessage = "Please enter a value for the Overall Result.";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult result = null;
            var model = (LanguageProficiencyModel)validationContext.ObjectInstance;
            bool scoreEntered = (model.Listening.HasValue || model.Speaking.HasValue || model.Reading.HasValue || model.Writing.HasValue);

            if (model.TookEnglishProficiencyTest && scoreEntered && !model.Overall.HasValue)
            {
                if (String.IsNullOrEmpty((string)value))
                {
                    result = new ValidationResult(mErrorMessage, new[] { "Test" });
                }
            }

            return result;
        }
    }
}