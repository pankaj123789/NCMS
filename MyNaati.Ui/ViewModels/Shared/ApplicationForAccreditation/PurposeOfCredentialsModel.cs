using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation
{
    public class PurposeOfCredentialsModel
    {
        public PurposeOfCredentialsModel()
        {
            ProfessionalQualification = false;
            CommunityLanguage = false;
            SkillsAssessment = false;
            Other = false;
            OtherDetails = "";
        }

        [DisplayName("A professional qualification")]
        [AtLeastOnePurpose]
        public bool ProfessionalQualification { get; set; }

        [DisplayName("Credentialed Community Language points")]
        public bool CommunityLanguage { get; set; }

        [DisplayName("Skills Assessment for migration purposes")]
        public bool SkillsAssessment { get; set; }

        [DisplayName("Do you wish NAATI to assess your educational qualifications?")]
        public bool AssessEducationalQualification { get; set; }

        [DisplayName("Are your overseas qualifications comparable to one of the following:")]
        [RequiredIdAssessEducationalQualification]
        public bool OverseasQualificationComparable { get; set; }

        [DisplayName("Do you wish NAATI to assess your skill employment?")]
        [CheckForLevelThreeProfessional]
        public bool AssessSkillEmployment { get; set; }

        public bool AtLeastLevel3Professional { get; set; }
 
        [DisplayName("Other (please specify)")]
        public bool Other { get; set; }

        [MandatoryIfOther]
        public string OtherDetails { get; set; }

        public class AtLeastOnePurposeAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (PurposeOfCredentialsModel)validationContext.ObjectInstance;
                if (model.ProfessionalQualification || model.SkillsAssessment || model.CommunityLanguage || model.Other)
                    return null;

                return new ValidationResult("You must select a purpose.");
            }
        }

        public class MandatoryIfOtherAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (PurposeOfCredentialsModel)validationContext.ObjectInstance;
                if (model.Other && string.IsNullOrWhiteSpace(model.OtherDetails))
                    return new ValidationResult("The Other reason field is required.");

                return null;
            }
        }

        public class RequiredIdAssessEducationalQualification : RequiredAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (PurposeOfCredentialsModel) validationContext.ObjectInstance;
                if (model.AssessEducationalQualification && model.SkillsAssessment && model.OverseasQualificationComparable == false)
                    return new ValidationResult("You must select yes when assessing educational qualifications.");

                return base.IsValid(value, validationContext);
            }
        }

        public class CheckForLevelThreeProfessional : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (PurposeOfCredentialsModel) validationContext.ObjectInstance;                
                
                if(model.AtLeastLevel3Professional == false && model.AssessSkillEmployment)
                {
                    return new ValidationResult("Credential request is not a level 3 Professional request");                                    
                }

                return null;
            }
        }

        public void BuildLists()
        {
            //do nothing.
        }
    }
}