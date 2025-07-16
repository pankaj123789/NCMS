using System.Collections.Generic;
using System.Linq;
using SamApplicationFee = MyNaati.Contracts.BackOffice.ApplicationFee;

namespace MyNaati.Ui.ViewModels.ApplicationByCourseWizard
{
    public class DownloadFormModel : IDownloadFormModel
    {
        private const string FIRST_APPLICATION_FEE_DESCRIPTION = "Application assessment (non-refundable)";
        private const string FEE_DESCRIPTION = "Accreditation fee";
        private const string TOTAL_DESCRIPTION = "Total";
        private const int NAATI_NUMBER_CUTOFF = 910000;

        public DownloadFormModel()
        {
            ApplicationFeesList = new List<CalculatedApplicationFee>();
        }

        public bool SkillsAssessmentRequested { get; set; }

        public List<CalculatedApplicationFee> ApplicationFeesList { get; set; }

        public void CalculateApplicationFees(WizardModel model, SamApplicationFee[] applicationFees)
        {
            ApplicationFeesList.Clear();

            int naatiNumber;
            int.TryParse((model.PersonalDetails.NaatiNumber ?? string.Empty), out naatiNumber);

            bool includeFirstApplicationFee = ((model.PersonalDetails.FirstApplication ?? false) || naatiNumber > NAATI_NUMBER_CUTOFF);
            bool useAustralianPrice = (model.ResidencyStatus.IsAustralianResident == true);

            if (includeFirstApplicationFee)
            {
                decimal applicationFee = applicationFees.Where(e => e.IsForFirstApplication)
                    .Sum(e => ((useAustralianPrice ? e.PriceAustralia : e.PriceOtherCountries) ?? 0));

                ApplicationFeesList.Add(new CalculatedApplicationFee() { Description = FIRST_APPLICATION_FEE_DESCRIPTION, Total = applicationFee });
            }

            decimal accreditationFee = applicationFees.Where(e => !e.IsForFirstApplication)
                .Sum(e => (useAustralianPrice ? e.PriceAustralia : e.PriceOtherCountries) ?? 0);

            ApplicationFeesList.Add(new CalculatedApplicationFee() { Description = FEE_DESCRIPTION, Total = accreditationFee });
            ApplicationFeesList.Add(new CalculatedApplicationFee() { Description = TOTAL_DESCRIPTION, Total = ApplicationFeesList.Sum(e => e.Total) });
        }
    }

    public class CalculatedApplicationFee
    {
        public string Description { get; set; }
        public decimal Total { get; set; }
    }
}