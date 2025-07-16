//using System;
//using System.Linq;
//using MyNaati.Ui.ViewModels.ApplicationByCourseWizard;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationCourseFeeReplacements : ITokenReplacement
//    {
//        private WizardModel mWizardModel;

//        public ApplicationCourseFeeReplacements(WizardModel wizardModel)
//        {
//            mWizardModel = wizardModel;
//        }

//        public string GetReplacement(string title)
//        {
//            switch (title)
//            {
//                case "TotalFee":
//                    return mWizardModel.DownloadForm.ApplicationFeesList
//                        .Where(e => string.Equals(e.Description, "Total", StringComparison.InvariantCultureIgnoreCase))
//                        .Select(e => e.Total)
//                        .SingleOrDefault()
//                        .ToString("c");
//            }
//            return null;
//        }
//    }
//}