//using MyNaati.Ui.ViewModels.ApplicationByCourseWizard;

//namespace MyNaati.Ui.Reports.PDFTokenisation.Forms
//{
//    public class ApplicationCourseDetailsReplacements : ITokenReplacement
//    {
//        private CourseDetailsModel mCourseDetails;

//        public ApplicationCourseDetailsReplacements(CourseDetailsModel courseDetails)
//        {
//            mCourseDetails = courseDetails;
//        }

//        public string GetReplacement(string title)
//        {
//            switch (title)
//            {
//                case "EducationInstitution":
//                    return mCourseDetails.Institution ?? string.Empty;
//                case "Campus":
//                    return mCourseDetails.Campus ?? string.Empty;
//                case "State":
//                    return mCourseDetails.State ?? string.Empty;
//                case "ApprovedCourse":
//                    return mCourseDetails.Course ?? string.Empty;
//                case "YearStarted":
//                    return mCourseDetails.YearStarted.HasValue ? mCourseDetails.YearStarted.ToString() : string.Empty;
//                case "DateCompleted":
//                    return mCourseDetails.DateCompleted.HasValue ? mCourseDetails.DateCompleted.ToString() : string.Empty;
//            }
//            return null;
//        }
//    }
//}