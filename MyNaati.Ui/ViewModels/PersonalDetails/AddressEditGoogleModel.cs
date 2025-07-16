using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Mvc;

namespace MyNaati.Ui.ViewModels.PersonalDetails
{
    public class AddressEditGoogleModel : IAddressGoogleModel
    {
        public AddressEditGoogleModel()
        {
            IsFromAustralia = true;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Enter street address to search.")]
        [StringLength(300, ErrorMessage = "Address cannot be more than 300 characters long.")]
        public string StreetDetails { get; set; }

        public string SuburbName { get; set; }

        public string Postcode { get; set; }

        public string State { get; set; }

        public IList<SelectListItem> OdAddressVisibilityTypes { get; set; }

        private string mCountryName;
        public string CountryName
        {
            get
            {
                return mCountryName;
            }
            set
            {
                mCountryName = value;
            }
        }

        public bool IsFromAustralia { get; set; }

        [DisplayName("Preferred address")]
        public bool IsPreferred { get; set; }

        public bool Success { get; set; }

        public bool ExaminerCorrespondence { get; set; }
        public bool ValidateInExternalTool { get; set; }
        

        public List<string> Errors { get; set; }
        
        public int OdAddressVisibilityTypeId { get; set; }
        
        public string OdAddressVisibilityTypeName { get; set; }

        public string FullAddress
        {
            get
            {
                var stringBuilder = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(StreetDetails))
                    stringBuilder.Append(StreetDetails).Append(", ");

                if (IsFromAustralia)
                {
                    if (!string.IsNullOrWhiteSpace(SuburbName))
                        stringBuilder.Append(SuburbName).Append(", ");

                    if (!string.IsNullOrWhiteSpace(Postcode) && (string.IsNullOrWhiteSpace(SuburbName) || !SuburbName.Contains(Postcode)))
                    {
                        stringBuilder.Append(Postcode).Append(", ");
                    }
                    
                }

                if (!string.IsNullOrWhiteSpace(CountryName))
                    stringBuilder.Append(CountryName);

                return stringBuilder.ToString().Trim().TrimEnd(',');
            }
        }
    }
}
