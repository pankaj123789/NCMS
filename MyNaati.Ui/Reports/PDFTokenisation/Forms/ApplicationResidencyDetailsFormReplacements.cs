//using MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationResidencyStatusFormReplacements : ITokenReplacement
//    {
//        private ResidencyStatusModel mResidencyStatus;

//        public ApplicationResidencyStatusFormReplacements(ResidencyStatusModel residencyStatus)
//        {
//            mResidencyStatus = residencyStatus;
//        }

//        public string GetReplacement(string title)
//        {
//            switch (title)
//            {
//                case "IsResidentInAustralia":
//                    switch (mResidencyStatus.IsCurrentlyResidingInAustralia)
//                    {
//                        case true:
//                            return "Yes";
//                        case false:
//                            return "No";
//                        default:
//                            return string.Empty;
//                    }
//                case "IsPermanentResident":
//                    switch (mResidencyStatus.IsAustralianResident)
//                    {
//                        case true:
//                            return "Yes";
//                        case false:
//                            return "No";
//                        default:
//                            return string.Empty;
//                    }
//                case "CountryOfResidence":
//                    return mResidencyStatus.Country ?? string.Empty;
//            }
//            return null;
//        }
//    }
//}