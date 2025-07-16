//using System;
//using System.Collections.Generic;
//using MyNaati.Ui.ViewModels.ApplicationByCourseWizard;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationCourseFormChecklistRenderer : BasicChecklistRenderer
//    {
//        private readonly WizardModel mWizardModel;

//        private const int THREE_YEARS = 3;
//        private const string BULLET = "\t\u2022";

//        public ApplicationCourseFormChecklistRenderer(WizardModel wizardModel)
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

//                if (mWizardModel.CourseDetails.DateCompleted.HasValue && mWizardModel.CourseDetails.DateCompleted.Value.AddYears(THREE_YEARS) < DateTime.Now)
//                {
//                    itemList.Add(CheckList.AttachedWorkReferences);
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

//                itemList.Add(CheckList.CopyOfQualificationCertificateTestamur);
//                itemList.Add(CheckList.CopyOfAcademicTranscript);

//                if (!mWizardModel.PurposeOfCredentials.SkillsAssessment)
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