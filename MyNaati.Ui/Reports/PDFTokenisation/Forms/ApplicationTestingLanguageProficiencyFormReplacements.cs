//using MyNaati.Ui.ViewModels.ApplicationByTestingWizard;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationTestingLanguageProficiencyFormReplacements : ITokenReplacement
//    {
//        private LanguageProficiencyModel mLanguageProficiency;

//        public ApplicationTestingLanguageProficiencyFormReplacements(LanguageProficiencyModel languageProficiency)
//        {
//            mLanguageProficiency = languageProficiency;
//        }

//        public string GetReplacement(string title)
//        {
//            switch (title)
//            {
//                case "HasTakenEnglishProficiency":
//                    return mLanguageProficiency.TookEnglishProficiencyTest ? "Yes" : "No";
//                case "TestSat":
//                    if (mLanguageProficiency.IsIELTS)
//                    {
//                        return "International English Language Testing System (IELTS) - " + (mLanguageProficiency.IsAcademic ? "Academic" : "General Training");
//                    }
//                    else
//                    {
//                        return mLanguageProficiency.OtherTestDetails ?? string.Empty;
//                    }
//                case "ListeningScore":
//                    return mLanguageProficiency.Listening.ToString();
//                case "SpeakingScore":
//                    return mLanguageProficiency.Speaking.ToString();
//                case "ReadingScore":
//                    return mLanguageProficiency.Reading.ToString();
//                case "WritingScore":
//                    return mLanguageProficiency.Writing.ToString();
//                case "OverallScore":
//                    return mLanguageProficiency.Overall.ToString();
//            }
//            return null;
//        }
//    }
//}