//using MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationPersonalDetailsFormReplacements : ITokenReplacement
//    {
//        private PersonalDetailsModel mPersonalDetails;

//        public ApplicationPersonalDetailsFormReplacements(PersonalDetailsModel personalDetails)
//        {
//            mPersonalDetails = personalDetails;
//        }

//        public string GetReplacement(string title)
//        {
//            switch (title)
//            {
//                case "NAATINumber":
//                    return mPersonalDetails.NaatiNumber ?? string.Empty;
//                case "Title":
//                    return mPersonalDetails.Title ?? string.Empty;
//                case "GivenName":
//                    return mPersonalDetails.GivenName ?? string.Empty;
//                case "OtherNames":
//                    return mPersonalDetails.OtherNames ?? string.Empty;
//                case "FamilyName":
//                    return mPersonalDetails.FamilyName ?? string.Empty;
//                case "AlternativeGivenName":
//                    return mPersonalDetails.AlternativeGivenName ?? string.Empty;
//                case "AlternativeFamilyName":
//                    return mPersonalDetails.AlternativeFamilyName ?? string.Empty;
//                case "DateOfBirth":
//                    if (!mPersonalDetails.DateOfBirth.HasValue)
//                        return string.Empty;
//                    return mPersonalDetails.DateOfBirth.Value.ToString("dd MMM yyyy");
//                case "Gender":
//                    switch (mPersonalDetails.IsGenderMale)
//                    {
//                        case true:
//                            return "Male";
//                        case false:
//                            return "Female";
//                        case null:
//                            return "Unspecified";
//                        default:
//                            return string.Empty;
//                    }
//                case "CountryOfBirth":
//                    return mPersonalDetails.Country;
//            }
//            return null;
//        }
//    }
//}