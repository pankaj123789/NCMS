//using MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationAddressDetailsFormReplacements : ITokenReplacement
//    {
//        private AddressModel mAddress;

//        public ApplicationAddressDetailsFormReplacements(AddressModel address)
//        {
//            mAddress = address;
//        }

//        public string GetReplacement(string title)
//        {
//            switch (title)
//            {
//                case "AddressContactType":
//                    return "Address - -contactype-"; //+ mAddress.ContactType;
//                case "PrimaryAddress":
//                    var streetDetails = (mAddress.StreetDetails ?? string.Empty);
//                    streetDetails = streetDetails.Replace("\r\n", "\n");
//                    if (mAddress.IsAustralia)
//                    {
//                        return streetDetails + "\n" + mAddress.SuburbName;
//                    }
//                    else
//                    {
//                        return streetDetails + "\n" + mAddress.CountryName;
//                    }
//            }
//            return null;
//        }
//    }
//}