//using System;
//using System.Linq;
//using MyNaati.Ui.ViewModels.ApplicationByTestingWizard;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationTestingFeeReplacements : ITokenReplacement
//    {
//        private WizardModel mWizardModel;

//        public ApplicationTestingFeeReplacements(WizardModel wizardModel)
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