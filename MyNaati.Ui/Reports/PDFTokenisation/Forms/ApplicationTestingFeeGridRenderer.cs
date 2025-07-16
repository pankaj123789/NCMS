//using System;
//using System.Collections.Generic;
//using System.Linq;
//using MyNaati.Ui.ViewModels.ApplicationByTestingWizard;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationTestingFeeGridRenderer : BasicGridRenderer
//    {
//        private WizardModel mWizardModel;

//        public ApplicationTestingFeeGridRenderer(WizardModel wizardModel)
//        {
//            mWizardModel = wizardModel;
//        }

//        protected override List<string[]> ItemList
//        {
//            get
//            {
//                return mWizardModel.DownloadForm.ApplicationFeesList
//                        .Where(e=>!string.Equals(e.Description, "Total", StringComparison.InvariantCultureIgnoreCase))
//                        .Select(e=>new string[]{e.Description, e.Skill, e.Level, e.Direction, e.Total.ToString("c")})
//                        .ToList();
//            }
//        }
    
//        protected override string[]  FieldTitles
//        {
//	        get { return new[] {"FeeDescription", "CredentialSkill", "CredentialLevel", "CredentialDirection", "FeeAmount"}; }
//        }
//    }
//}