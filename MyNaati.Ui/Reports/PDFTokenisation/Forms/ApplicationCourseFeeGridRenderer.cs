//using System.Collections.Generic;
//using System.Linq;
//using MyNaati.Ui.ViewModels.ApplicationByCourseWizard;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationCourseFeeGridRenderer : BasicGridRenderer
//    {
//        private WizardModel mWizardModel;

//        public ApplicationCourseFeeGridRenderer(WizardModel wizardModel)
//        {
//            mWizardModel = wizardModel;
//        }

//        protected override List<string[]> ItemList
//        {
//            get
//            {
//                return mWizardModel.DownloadForm.ApplicationFeesList
//                        .Where(e=>!string.Equals(e.Description, "Total"))
//                        .Select(e=>new string[]{e.Description, e.Total.ToString("c")})
//                        .ToList();
//            }
//        }
    
//        protected override string[]  FieldTitles
//        {
//	        get { return new[] {"FeeDescription", "FeeAmount"}; }
//        }
//    }
//}