//using System;
//using System.Collections.Generic;
//using System.Linq;
//using MyNaati.Ui.ViewModels.ApplicationByTestingWizard;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationTestingFormChecklistRenderer : BasicChecklistRenderer
//    {
//        private readonly WizardModel mWizardModel;

//        private const string ADVANCED_TRANSLATOR_ELIGIBILITY = "AdvancedTranslatorEligibility";
//        private const string PARAPROFESSIONAL_ELIGIBILITY = "ParaprofessionalEligibility";
//        private const string PROFESSIONAL_ELEGIBILITY = "ProfessionalEligibility";
//        private const string BULLET = "\t\u2022";

//        public ApplicationTestingFormChecklistRenderer(WizardModel wizardModel)
//        {
//            mWizardModel = wizardModel;
//        }

//        protected override List<string> ItemList
//        {
//            get
//            {
//                bool isResidingInAustralia = (mWizardModel.ResidencyStatus.IsCurrentlyResidingInAustralia == true);
//                bool isFirstApplication = (mWizardModel.PersonalDetails.FirstApplication == true);
//                bool isAustralianResident = (mWizardModel.ResidencyStatus.IsAustralianResident == true);

//                ParaprofessionalEligibilityModel paraprofessionalEligibility = mWizardModel.ParaprofessionalEligibility;
//                ProfessionalEligibilityModel professionalEligibility = mWizardModel.ProfessionalEligibility;
//                AdvancedTranslatorEligibilityModel advancedTranslatorEligibility = mWizardModel.AdvancedTranslatorEligibility;

//                var itemList = new List<string>
//                {
//                    CheckList.ProvidedPassportPhotos
//                };

//                if (isResidingInAustralia && (!isFirstApplication || !isAustralianResident))
//                {
//                    itemList.Add(CheckList.CopyOfPassportOrAustralianDriversLicence);
//                }
//                else if (!isFirstApplication || !isAustralianResident)
//                {
//                    itemList.Add(CheckList.CopyOfPassport);
//                }

//                itemList.Add(CheckList.WitnessDeclarationCompleted);

//                if (isFirstApplication && isAustralianResident)
//                {
//                    itemList.Add(string.Format(CheckList.CopyOfIdentificationDocument, Environment.NewLine, BULLET));
//                    itemList.Add(string.Format(CheckList.CopyOfPhotoIdentification, Environment.NewLine, BULLET));
//                }

//                if (!mWizardModel.Address.IsAustralia)
//                {
//                    itemList.Add(CheckList.SeparateAddressLabel);
//                }

//                //todo get rid of magic numbers here.
//                if (!mWizardModel.IsStepSkipped(PARAPROFESSIONAL_ELIGIBILITY) && paraprofessionalEligibility.SelectedCriteriaId == 4)
//                {
//                    itemList.Add(CheckList.CopyOfAustralianSecondarySchoolRecords);
//                }

//                if (!mWizardModel.IsStepSkipped(PARAPROFESSIONAL_ELIGIBILITY) && paraprofessionalEligibility.SelectedCriteriaId == 5)
//                {
//                    itemList.Add(CheckList.CopyOfPostSecondaryStudiesRecords);
//                }

//                if ((!mWizardModel.IsStepSkipped(PARAPROFESSIONAL_ELIGIBILITY) && paraprofessionalEligibility.SelectedCriteriaId == 6) ||
//                    (!mWizardModel.IsStepSkipped(PROFESSIONAL_ELEGIBILITY) && professionalEligibility.RequiredCriteria.Any(e => e.SelectedValue == 7)) ||
//                    (!mWizardModel.IsStepSkipped(ADVANCED_TRANSLATOR_ELIGIBILITY) && advancedTranslatorEligibility.HasReference))
//                {
//                    itemList.Add(CheckList.CopyOfWorkExperience);
//                }

//                if (!mWizardModel.IsStepSkipped(PROFESSIONAL_ELEGIBILITY) && professionalEligibility.RequiredCriteria.Any(e => e.SelectedValue == 1))
//                {
//                    itemList.Add(CheckList.CopyOfRecognisedVetInstitutionQualifications);
//                }

//                if ((!mWizardModel.IsStepSkipped(PROFESSIONAL_ELEGIBILITY) && professionalEligibility.RequiredCriteria.Any(e => e.SelectedValue == 2)) ||
//                    (!mWizardModel.IsStepSkipped(ADVANCED_TRANSLATOR_ELIGIBILITY) && advancedTranslatorEligibility.HasDegree))
//                {
//                    itemList.Add(CheckList.CopyOfRecognisedHigherEducationQualifications);
//                }

//                if (!mWizardModel.IsStepSkipped(PROFESSIONAL_ELEGIBILITY) && professionalEligibility.RequiredCriteria.Any(e => e.SelectedValue == 3))
//                {
//                    itemList.Add(CheckList.CopyOfPostSecondaryRelevantSubjects);
//                }

//                if (mWizardModel.LanguageProficiency.TookEnglishProficiencyTest)
//                {
//                    itemList.Add(CheckList.CopyOfProficiencyTestResults);
//                }

//                if (mWizardModel.PurposeOfCredentials.SkillsAssessment)
//                {
//                    itemList.Add(CheckList.ProvidedSupportingDocuments);
//                    itemList.Add(CheckList.ReadTermsAndConditions);
//                    itemList.Add(CheckList.AttachedPurchaseOrder);
//                    itemList.Add(CheckList.AttachedCorrectPayment);

//                    if (mWizardModel.PurposeOfCredentials.AssessEducationalQualification || mWizardModel.PurposeOfCredentials.AssessSkillEmployment)
//                    {
//                        itemList.Add(CheckList.AttachedFormM);
//                    }
//                }
//                else
//                {
//                    itemList.Add(CheckList.ReadTermsAndConditions);
//                    itemList.Add(CheckList.FilledInPaymentDetails);
//                    itemList.Add(CheckList.AttachedCorrectPaymentOrPurchaseOrder);
//                }

//                itemList.Add(CheckList.SignedDeclaration);

//                return itemList;
//            }
//        }

//        protected override string FieldTitle
//        {
//            get { return CheckList.FieldTitle; }
//        }
//    }
//}