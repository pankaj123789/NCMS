//using System.Collections.Generic;
//using System.Linq;
//using MyNaati.Ui.ViewModels.ApplicationByCourseWizard;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationCourseCredentialGridRenderer : BasicGridRenderer
//    {
//        private CredentialDetailsModel mCredentialDetails;

//        public ApplicationCourseCredentialGridRenderer(CredentialDetailsModel credentialDetails)
//        {
//            mCredentialDetails = credentialDetails;
//        }

//        protected override List<string[]> ItemList
//        {
//            get { return mCredentialDetails.RequestedCredentials.Select(e => new[] {e.Skill, e.LevelName, e.Direction}).ToList(); }
//        }
    
//        protected override string[]  FieldTitles
//        {
//	        get { return new[] {"CredentialSkill", "CredentialLevel", "CredentialDirection"}; }
//        }
//    }
//}