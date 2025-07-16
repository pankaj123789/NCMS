using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation
{
    public class PhoneListModel
    {
      
        //These are here so we can refer to them in calls to HtmlHelper in the View
        //This whole object will actually be posted to the Json address update method,
        //since it seems hard to wrangle EditorFor into loading from one viewmodel 
        //and saving to another. Better solutions invited!
        public PhoneEditModel PhoneForCreate { get; set; }
        public PhoneEditModel PhoneForEdit { get; set; }

        public IList<PhoneEditModel> CurrentPhones { get; set; }

        public PhoneListModel()
        {
            PhoneForCreate = new PhoneEditModel();
            PhoneForEdit = new PhoneEditModel();
            CurrentPhones = new List<PhoneEditModel>();
        }

        public string ValidatePhoneUniqueness(PhoneEditModel editModel)
        {
            //if (CurrentPhones.Any(p => p.Id != editModel.Id && p.ContactTypeId == editModel.ContactTypeId))
            if (CurrentPhones.Any(p => p.Id != editModel.Id))
            {
                //Get names from lists instead of lookupProvider so they're sure to match up.\

                return string.Format("You may only have one active phone number per phone type and contact type. Edit the {0} phone number, or select a different contact type or phone type.",
                    //contactType);
                    string.Empty);
            }

            return null;
        }
    }

    public class PhoneEditModel
    {
        public PhoneEditModel()
        {
           
        }

        public int Id { get; set; }

        [DisplayName("Country")]
        [RegularExpression(@"^[\d\+]*$", ErrorMessage = "{0} can only contain numbers (0 to 9) or a \"+\".")]
        [StringLength(4, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string CountryCode { get; set; }

        [DisplayName("Area")]
        //[AreaCodeValidator]
        [RegularExpression(@"^[\d]*$", ErrorMessage = "{0} can only contain numbers (0 to 9).")]
        [StringLength(4, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string AreaCode { get; set; }

        [DisplayName("Number")]
        [Required]
        [PhoneNumberValidator]
        [StringLength(50, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string Number { get; set; }

        [DisplayName("Preferred Phone")]
        public bool IsPreferred { get; set; }

        [DisplayName("Show in the Online Directory")]
        public bool IsCurrentlyListed { get; set; }

        public string FormattedNumber
        {
            get
            {
                string result = (string.Empty).Trim();
                if (Number.Length == 8)
                    result += string.Format(" {0} {1}", Number.Substring(0, 4), Number.Substring(4, 4));
                else
                    result += " " + Number;

                return result.Trim();
            }
        }
    
        public class PhoneNumberValidator : ValidationAttribute
        {
            private const int mPhoneNumberLength = 25;

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (PhoneEditModel)validationContext.ObjectInstance;

                if (model.Number.Length > mPhoneNumberLength)
                {
                    string errorMessage = string.Format("Phone number must be at most {0} characters long.", mPhoneNumberLength);
                    return new ValidationResult(errorMessage, new[] { "Number" });
                }

                var isNumeric = Regex.IsMatch(model.Number ?? string.Empty, @"^[\d]*$");
                var isPhoneValid = Regex.IsMatch(model.Number ?? string.Empty, @"^([0-9\(\)\+ \-]*)$");
                if (isPhoneValid == false)
                {
                    return new ValidationResult("Phone number can only contain (,),+,- and digits.", new[] { "Number" });
                }
                return null;
            }

            private bool WrongNumberOfDigits(string input, int digitCount)
            {
                string pattern = string.Format(@"^[\d]{{{0}}}$", digitCount);
                return Regex.IsMatch(input ?? string.Empty, pattern) == false;
            }
        }
    }
}
