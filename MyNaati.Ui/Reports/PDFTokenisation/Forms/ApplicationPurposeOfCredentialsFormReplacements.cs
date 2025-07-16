//using System.Collections.Generic;
//using MyNaati.Ui.Common;
//using MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationPurposeOfCredentialsFormReplacements : ITokenReplacement
//    {
//        private PurposeOfCredentialsModel mPurposeOfCredentials;

//        public ApplicationPurposeOfCredentialsFormReplacements(PurposeOfCredentialsModel purposeOfCredentials)
//        {
//            mPurposeOfCredentials = purposeOfCredentials;
//        }

//        public string GetReplacement(string title)
//        {
//            switch (title)
//            {
//                case "PurposeOfCredentials":
//                    var purposes = new List<string>();
//                    if (mPurposeOfCredentials.ProfessionalQualification)
//                    {
//                        purposes.Add(DisplayHelpers.GetDisplayNameAttribute<PurposeOfCredentialsModel>(e=>e.ProfessionalQualification));
//                    }
//                    if (mPurposeOfCredentials.SkillsAssessment)
//                    {
//                        purposes.Add(DisplayHelpers.GetDisplayNameAttribute<PurposeOfCredentialsModel>(e => e.SkillsAssessment));
//                    }
//                    if (mPurposeOfCredentials.Other)
//                    {
//                        purposes.Add(mPurposeOfCredentials.OtherDetails);
//                    }
//                    return string.Join(", ", purposes.ToArray());
//            }
//            return null;
//        }
//    }
//}