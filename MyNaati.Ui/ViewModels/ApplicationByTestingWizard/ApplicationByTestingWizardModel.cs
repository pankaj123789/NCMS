using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common.Mapping;
using MyNaati.Contracts.BackOffice;
using MyNaati.Ui.ViewModels.Shared;
using MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation;

namespace MyNaati.Ui.ViewModels.ApplicationByTestingWizard
{
    public class WizardModel : ApplicationWizardModel
    {
        private const string ParaprofessionalEligibilityStepName = "ParaprofessionalEligibility";
        private const string ProfessionalEligibilityStepName = "ProfessionalEligibility";
        private const string AdvancedTranslatorEligibilityStepName = "AdvancedTranslatorEligibility";
        private const string LEVEL_3 = "Professional";

        public WizardModel(IAutoMapperHelper autoMapperHelper) : base(autoMapperHelper)
        {
            Steps = new List<WizardStep>(new[]
            {
                new WizardStep(){Name = "Test Location", Action = "TestLocation", AllowDirectNavigation = true},
                new WizardStep(){Name = "Test Details", Action = "TestDetails"},
                new WizardStep(){Name = "Paraprofessional Eligibility", Action = ParaprofessionalEligibilityStepName },
                new WizardStep(){Name = "Professional Eligibility", Action = ProfessionalEligibilityStepName },
                new WizardStep(){Name = "Advanced Translator Eligibility", Action = AdvancedTranslatorEligibilityStepName },
                new WizardStep(){Name = "Language Proficiency", Action = "LanguageProficiency"},
                new WizardStep(){Name = "Personal Details", Action = "PersonalDetails"},
                new WizardStep(){Name = "Address", Action = "Address"},
                new WizardStep(){Name = "Phone", Action = "Phone"},
                new WizardStep(){Name = "Email", Action = "Email"},
                new WizardStep(){Name = "Residency Status", Action = "ResidencyStatus"},
                new WizardStep(){Name = "Purpose of Credentials", Action = "PurposeOfCredentials"},
                new WizardStep(){Name = "Download Form", Action = "DownloadForm"}
            });
        }

        public TestLocationModel TestLocation { get; set; }
        public TestDetailsModel TestDetails { get; set; }
        public ParaprofessionalEligibilityModel ParaprofessionalEligibility { get; set; }
        public ProfessionalEligibilityModel ProfessionalEligibility { get; set; }
        public AdvancedTranslatorEligibilityModel AdvancedTranslatorEligibility { get; set; }
        public LanguageProficiencyModel LanguageProficiency { get; set; }
        public DownloadFormModel DownloadForm { get; set; }

        public void UpdateStepsForEligibility(bool eligibilityFound, string stepName)
        {
            var declarationStep = Steps.Single(s => s.Action == stepName);
            declarationStep.Skipped = eligibilityFound;
        }

        public void UpdateTestLocation(TestLocationModel testLocation)
        {
            if (testLocation != null)
            {
                // Clear the list of tests if we change the testing location to/from Other.
                if (String.IsNullOrEmpty(testLocation.OtherLocation) == TestDetails.TestLocationIsOther)
                    ClearTestDetails();

                TestLocation = testLocation;

                UpdateTestDetailsIsOtherLocation();
            }
        }        
        
        public void BuildTestLocationLists()
        {
            if (TestLocation == null)
                throw new NullReferenceException("TestLocation is null");
            TestLocation.BuildLists();
        }

        private void UpdateTestDetailsIsOtherLocation()
        {
            if (TestDetails == null)
                throw new NullReferenceException("TestDetails is null");
            TestDetails.TestLocationIsOther = TestLocation.IsOtherLocation;
        }

        private void ClearTestDetails()
        {
            if (TestDetails == null)
                throw new NullReferenceException("TestDetails is null");
            TestDetails.TestRequestsList = new List<TestDetailsRow>();

            ClearEligibilityCriteria();
        }

        public void BuildTestDetailsLists()
        {
            if (TestDetails == null)
                throw new NullReferenceException("TestDetails is null");
            TestDetails.BuildLists();
        }

        public string AddTest(TestDetailsRow test, string combinationName, bool testExists)
        {
            if (TestDetails == null)
                throw new NullReferenceException("TestDetails is null");
            if (TestDetails.TestRequestsList.Any(e => TestDetailsRow.Equals(test, e)))
                return string.Format("There is already a test for {0} and {1}.", test.LanguageName, combinationName);
            if (testExists == false)
                return string.Format("There is no test offered for the selected combination of language, level and skill.");

            ClearEligibilityCriteria();

            // Add the newly entered test to the list of tests that the user has requested so far.
            int newId = TestDetails.TestRequestsList.Select(e=>e.Id).Union(Enumerable.Repeat(0,1)).Max()+1;
            test.Id = newId;
            TestDetails.TestRequestsList.Add(test);

            if (test.Level == 3)
            {
                // Only add the language/isInterpreter pair if it doesn't already exist
                if (!ProfessionalEligibility.RequiredCriteria.Any(e => e.IsInterpreter == test.IsInterpreter && e.Language == test.LanguageName))
                    ProfessionalEligibility.RequiredCriteria.Add(new ProfessionalCriteriaItem() { Language = test.LanguageName, IsInterpreter = test.IsInterpreter, SelectedValue = null });
            }
            else if (test.Level == 4)
                AdvancedTranslatorEligibility.ProfessionalTranslatorCredentialList.Add(new ProfessionalTranslatorCredentialItem() { Language = test.LanguageName, Selected = false, TestId = test.Id, ToEnglish = test.ToEnglish });

            return string.Empty;
        }

        public void DeleteTest(TestDetailsRow test)
        {
            if (TestDetails == null)
                throw new NullReferenceException("TestDetails is null");

            ClearEligibilityCriteria();

            if ((test.Level == 3) && (ProfessionalEligibility.RequiredCriteria.Count > 0))
            {
                // Check for other tests with the same isInterpreter and Language.  If we have none, remove all matching professional criteria.
                if (!TestDetails.TestRequestsList.Where(e => e.Id != test.Id).Any(e => e.IsInterpreter == test.IsInterpreter && e.LanguageId == test.LanguageId))
                {
                    ProfessionalEligibility.RequiredCriteria.RemoveAll(t => t.IsInterpreter == test.IsInterpreter && t.Language == test.LanguageName);   
                }
            }
            else if ((test.Level == 4) && (AdvancedTranslatorEligibility.ProfessionalTranslatorCredentialList.Count > 0))
                AdvancedTranslatorEligibility.ProfessionalTranslatorCredentialList.RemoveAll(t => t.TestId == test.Id);

            TestDetails.TestRequestsList.RemoveAll(e => TestDetailsRow.Equals(test, e));
        }

        public void DeleteTestAt(int testIndex)
        {
            if (TestDetails == null)
                throw new NullReferenceException("TestDetails is null");
            DeleteTest(TestDetails.TestRequestsList[testIndex]);
        }

        private void ClearEligibilityCriteria()
        {
            ParaprofessionalEligibility.SelectedCriteriaId = 0;

            foreach (var professionalCriteria in ProfessionalEligibility.RequiredCriteria)
                professionalCriteria.SelectedValue = null;

            foreach (var criteria in AdvancedTranslatorEligibility.ProfessionalTranslatorCredentialList)
                criteria.Selected = false;
            AdvancedTranslatorEligibility.HasDegree = false;
            AdvancedTranslatorEligibility.HasReference = false;
        }

        public string CreateParaprofessionalLanguageString()
        {
            var languageList = TestDetails.TestRequestsList.Where(e => e.Level == 2).Select(e => e.LanguageName).Distinct();

            return string.Join(", ", languageList.ToArray());
        }

        public void UpdateParaprofessionalEligibility(ParaprofessionalEligibilityModel paraprofessionalEligibility)
        {
            if (ParaprofessionalEligibility == null)
                throw new NullReferenceException("ParaprofessionalEligibility is null");

            ParaprofessionalEligibility = paraprofessionalEligibility;
        }

        public void UpdateAdvancedTranslatorEligibility(AdvancedTranslatorEligibilityModel advancedTranslatorEligibility)
        {
            if (AdvancedTranslatorEligibility == null)
                throw new NullReferenceException("AdvancedTranslatorEligibility is null");

            // Copy over checkbox selections only
            AdvancedTranslatorEligibility.HasDegree = advancedTranslatorEligibility.HasDegree;
            AdvancedTranslatorEligibility.HasReference = advancedTranslatorEligibility.HasReference;
            for (int i = 0; i < AdvancedTranslatorEligibility.ProfessionalTranslatorCredentialList.Count(); i++)
            {
                AdvancedTranslatorEligibility.ProfessionalTranslatorCredentialList[i].Selected =
                    advancedTranslatorEligibility.ProfessionalTranslatorCredentialList[i].Selected;
            }
        }

        public void UpdateProfessionalEligibility(ProfessionalEligibilityModel professionalEligibilityModel)
        {
            if (ProfessionalEligibility == null)
                throw new NullReferenceException("ProfessionalEligibility is null");

            // Copy over radio selections only
            for (int i=0; i<ProfessionalEligibility.RequiredCriteria.Count(); i++)
            {
                ProfessionalEligibility.RequiredCriteria[i].SelectedValue =
                    professionalEligibilityModel.RequiredCriteria[i].SelectedValue;
                ProfessionalEligibility.RequiredCriteria[i].SelectedCriteria = ProfessionalEligibility.CriteriaList
                    .Where(e => string.Equals(e.Value, ProfessionalEligibility.RequiredCriteria[i].SelectedValue.ToString()))
                    .Select(e => e.Text)
                    .SingleOrDefault();
            }
        }

        internal void CalculateApplicationFees(ApplicationFee[] applicationFees, TestingFee[] testingFees)
        {
            if (DownloadForm == null)
                throw new NullReferenceException("DownloadForm is null");
            DownloadForm.CalculateApplicationFees(this, applicationFees, testingFees);
        }

        internal void UpdateForSkillsAssessment()
        {
            if (DownloadForm == null)
                throw new NullReferenceException("DownloadForm is null");
            if (PurposeOfCredentials == null)
                throw new NullReferenceException("PurposeOfCredentials is null");

            DownloadForm.SkillsAssessmentRequested = PurposeOfCredentials.SkillsAssessment;
        }

        public override void UpdateForCredentialRequests()
        {
            if (TestDetails == null)
                throw new NullReferenceException("TestDetails is null");
            if (PurposeOfCredentials == null)
                throw new NullReferenceException("PurposeOfCredentials is null");
            PurposeOfCredentials.AtLeastLevel3Professional = TestDetails.TestRequestsList.Any(x => x.LevelName.Contains(LEVEL_3));
        }
    }
}