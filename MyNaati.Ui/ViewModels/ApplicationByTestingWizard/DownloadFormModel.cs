using System;
using System.Collections.Generic;
using System.Linq;
using SamApplicationFee = MyNaati.Contracts.BackOffice.ApplicationFee;
using SamTestingFee = MyNaati.Contracts.BackOffice.TestingFee;

namespace MyNaati.Ui.ViewModels.ApplicationByTestingWizard
{
    public class DownloadFormModel : IDownloadFormModel
    {
        private const string FIRST_APPLICATION_FEE_DESCRIPTION = "Application assessment (non-refundable)";
        private const string FEE_DESCRIPTION = "Testing fee";
        private const string TOTAL_DESCRIPTION = "Total";
        private const int NAATI_NUMBER_CUTOFF = 910000;

        public DownloadFormModel()
        {
            ApplicationFeesList = new List<ApplicationFees>();
        }

        public bool SkillsAssessmentRequested { get; set; }

        public List<ApplicationFees> ApplicationFeesList { get; set; }

        public void CalculateApplicationFees(WizardModel model, SamApplicationFee[] applicationFees, SamTestingFee[] testingFees)
        {
            ApplicationFeesList.Clear();

            int naatiNumber;
            int.TryParse((model.PersonalDetails.NaatiNumber ?? string.Empty), out naatiNumber);

            bool includeFirstApplicationFee = ((model.PersonalDetails.FirstApplication ?? false) || naatiNumber > NAATI_NUMBER_CUTOFF);
            bool useAustralianPrice = (model.TestLocation.IsAustralia && model.ResidencyStatus.IsAustralianResident == true);

            if (includeFirstApplicationFee)
            {
                decimal applicationFee = applicationFees.Where(e => e.IsForFirstApplication)
                    .Sum(e => ((useAustralianPrice ? e.PriceAustralia : e.PriceOtherCountries) ?? 0));

                ApplicationFeesList.Add(new ApplicationFees() { Description = FIRST_APPLICATION_FEE_DESCRIPTION, Total = applicationFee });
            }

            TestFeeType type = GetTestFeeType(model);

            foreach (TestDetailsRow test in model.TestDetails.TestRequestsList)
            {
                decimal testingFee = GetTestingFee(test, type, testingFees);
                ApplicationFeesList.Add(new ApplicationFees()
                {
                    Description = FEE_DESCRIPTION,
                    Total = testingFee,
                    Direction = test.Direction,
                    Level = test.LevelName,
                    Skill = test.Skill
                });
            }

            ApplicationFeesList.Add(new ApplicationFees() { Description = TOTAL_DESCRIPTION, Total = ApplicationFeesList.Sum(e => e.Total) });
        }

        private TestFeeType GetTestFeeType(WizardModel model)
        {
            TestFeeType type;
            if (model.TestLocation.IsNewZealand)
            {
                type = TestFeeType.NewZealand;
            }
            else if (model.TestLocation.IsAustralia)
            {
                type = (model.ResidencyStatus.IsAustralianResident == true ? TestFeeType.AustralianCitizenResidents : TestFeeType.AustralianOther);
            }
            else
            {
                type = TestFeeType.Other;
            }
            return type;
        }

        private decimal GetTestingFee(TestDetailsRow test, TestFeeType type, IEnumerable<SamTestingFee> testingFees)
        {
            bool? testToEnglishInSam = ((test.ToEnglish && test.FromEnglish) ? (bool?)null : test.ToEnglish);

            Func<TestFeeType, SamTestingFee, decimal> getFee = delegate (TestFeeType feeType, SamTestingFee fee)
            {
                decimal returnFee;
                switch (feeType)
                {
                    case TestFeeType.AustralianCitizenResidents:
                        returnFee = (fee.PriceAustralianCitizens ?? 0);
                        break;
                    case TestFeeType.AustralianOther:
                        returnFee = (fee.PriceAustralianOther ?? 0);
                        break;
                    case TestFeeType.NewZealand:
                        returnFee = (fee.PriceNewZealand ?? 0);
                        break;
                    default:
                        returnFee = (fee.PriceOtherCountries ?? 0);
                        break;
                }
                return returnFee;
            };

            Func<SamTestingFee, bool> isMatchingFee = (fee => (string.Equals(fee.SkillString, test.LevelName, StringComparison.InvariantCultureIgnoreCase) &&
                                                               fee.ToEnglish == testToEnglishInSam));

            decimal testingFee = testingFees.Where(isMatchingFee).Sum(e => getFee(type, e));
            return testingFee;
        }
    }

    public enum TestFeeType
    {
        AustralianCitizenResidents,
        AustralianOther,
        NewZealand,
        Other
    }

    public class ApplicationFees
    {
        public string Description { get; set; }
        public string Skill { get; set; }
        public string Level { get; set; }
        public string Direction { get; set; }
        public decimal Total { get; set; }
    }
}