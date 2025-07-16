//using MyNaati.Ui.ViewModels.ApplicationByTestingWizard;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationTestingLocationFormReplacements : ITokenReplacement
//    {
//        private TestLocationModel mTestLocation;

//        public ApplicationTestingLocationFormReplacements(TestLocationModel testLocation)
//        {
//            mTestLocation = testLocation;
//        }

//        public string GetReplacement(string title)
//        {
//            switch (title)
//            {
//                case "PreferredTestingLocation":
//                    if (mTestLocation.IsOtherLocation)
//                        return mTestLocation.OtherLocation ?? string.Empty;
//                    return mTestLocation.Location ?? string.Empty;
//            }
//            return null;
//        }
//    }
//}