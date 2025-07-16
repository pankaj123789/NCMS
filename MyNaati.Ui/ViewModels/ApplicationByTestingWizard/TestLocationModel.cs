using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using F1Solutions.Naati.Common.Contracts.Bl;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.ViewModels.ApplicationByTestingWizard
{
    public class TestLocationModel
    {
        private ILookupProvider mLookupProvider;

        public TestLocationModel()
        {
            mLookupProvider = ServiceLocator.Resolve<ILookupProvider>();
        }

        [Required(ErrorMessage = "You must provide a test location.")]
        public string Location { get; set; }

        public bool IsOtherLocation
        {
            get
            {
                if (string.IsNullOrEmpty(Location))
                    return false;
                return Location.IndexOf("Other", StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
        }

        public bool IsNewZealand
        {
            get
            {
                if (string.IsNullOrEmpty(Location))
                    return false;
                return Location.IndexOf("Zealand", StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
        }

        public bool IsAustralia
        {
            get { return !(IsOtherLocation || IsNewZealand); }
        }

        [DisplayName("Location")]
        [OtherLocationValidator]
        public string OtherLocation { get; set; }

        public IList<SelectListItem> LocationList;

        public void BuildLists()
        {
            LocationList = BuildTestLocationList();
        }

        private IList<SelectListItem> BuildTestLocationList()
        {
            var allItem = new[] { new SelectListItem { Selected = false, Text = "", Value = "0" } };

            var australianStates = mLookupProvider.States
                .Where(s => s.IsAustralian)
                .Select(x => new SelectListItem { Selected = false, Text = x.Abbreviation, Value = x.Abbreviation });

            IEnumerable<SelectListItem> locationList = allItem.Union(australianStates);

            List<SelectListItem> itemList = new List<SelectListItem>();

            itemList.Add(new SelectListItem { Selected = false, Text = "New Zealand", Value = "New Zealand" });
            itemList.Add(new SelectListItem { Selected = false, Text = "Other (please specify)", Value = "Other" });

            locationList = locationList.Union(itemList);

            return locationList.ToList();
        }
    }

    public class OtherLocationValidator : ValidationAttribute
    {
        private const string mErrorMessage = "You need to enter a location.";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (TestLocationModel)validationContext.ObjectInstance;
                      
            if (!(String.IsNullOrEmpty(model.Location)) && model.Location.Contains("Other") && String.IsNullOrEmpty(model.OtherLocation))
            {
                return new ValidationResult(mErrorMessage, new[] { "Location" });
            }

            return null;
        }
    }
}
