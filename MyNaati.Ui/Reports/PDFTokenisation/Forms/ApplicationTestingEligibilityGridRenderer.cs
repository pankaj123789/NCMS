//using System;
//using System.Collections.Generic;
//using MyNaati.Ui.ViewModels.ApplicationByTestingWizard;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationTestingEligibilityGridRenderer : BasicGridRenderer
//    {
//        private WizardModel mWizardModel;

//        public ApplicationTestingEligibilityGridRenderer(WizardModel wizardModel)
//        {
//            mWizardModel = wizardModel;
//        }

//        //this whole thing is really lame.
//        // I think that the options should be in an enum, and the enum should have display text attributes.
//        // unfortunately due to various constraints that change probably won't happen any time soon.
//        protected override List<string[]> ItemList
//        {
//            get
//            {
//                var itemList = new List<string[]>();
//                if (!mWizardModel.IsStepSkipped("ParaprofessionalEligibility") && mWizardModel.ParaprofessionalEligibility.SelectedCriteria != null)
//                {
//                    if (mWizardModel.ParaprofessionalEligibility.SelectedCriteriaId == 3)
//                    {
//                        itemList.Add(new string[] { "Paraprofessional", mWizardModel.ParaprofessionalEligibility.SelectedCriteria.Replace("<br />",Environment.NewLine) + mWizardModel.CreateParaprofessionalLanguageString() });
//                    }
//                    else
//                    {
//                        itemList.Add(new string[] { "Paraprofessional", mWizardModel.ParaprofessionalEligibility.SelectedCriteria.Replace("<br />",Environment.NewLine) });
//                    }
//                }

//                if (!mWizardModel.IsStepSkipped("ProfessionalEligibility"))
//                {
//                    foreach (var professionalEligibility in mWizardModel.ProfessionalEligibility.RequiredCriteria)
//                    {
//                        if (professionalEligibility.SelectedCriteria != null)
//                        {
//                            itemList.Add(new string[]{
//                                "Professional " + professionalEligibility.Skill + " - " + professionalEligibility.Language, professionalEligibility.SelectedCriteria.Replace("<br />"," ")});
//                        }
                            
//                    }
//                }

//                if (!mWizardModel.IsStepSkipped("AdvancedTranslatorEligibility"))
//                {
//                    var eligibility = new List<string>();

//                    if (mWizardModel.AdvancedTranslatorEligibility.HasDegree)
//                    {
//                        eligibility.Add("I hold a degree (or higher) - in any field - from a recognised higher education institution (supporting documentation required)");
//                    }
//                    foreach (var credential in mWizardModel.AdvancedTranslatorEligibility.ProfessionalTranslatorCredentialList)
//                    {
//                        if (credential.Selected)
//                        {
//                            eligibility.Add("I hold a NAATI Professional Translator credential "+credential.Language);
//                        }
//                    }
//                    if (mWizardModel.AdvancedTranslatorEligibility.HasReference)
//                    {
//                        eligibility.Add("I have provided employer reference(s) as evidence of work as a translator for a minimum of two years. Self employed practitioners need to provide a statutory declaration");
//                    }

//                    itemList.Add(new string[] { "Advanced Translator", string.Join(Environment.NewLine, eligibility) });
//                }

//                return itemList;
//            }
//        }
    
//        protected override string[]  FieldTitles
//        {
//	        get { return new[] {"EligibilityType", "Eligibility"}; }
//        }
//    }
//}