using System.Collections.Generic;

namespace MyNaati.Contracts.BackOffice
{
    
    public interface ICourseApprovalService
    {
        
        CourseApprovalResponse GetCourseApprovalData(CourseApprovalRequest request);
    }

    
    public class CourseApprovalRequest
    {
        
        public int CourseId { get; set; }
    }

    
    public class CourseApproval
    {
        
        public int CourseApprovalId { get; set; }

        
        public int CourseId { get; set; }

        
        public int LanuageId { get; set; }

        
        public int? AccreditationCategoryId { get; set; }

        
        public int? AccreditationLevelId { get; set; }

        
        public bool IsBothDirection { get; set; }

        
        public string LevelCategoryDirectionText { get; set; }

        
        public bool ToEnglish { get; set; }

        
        public bool FromEnglish { get; set; }

        
        public bool IsInterpreter { get; set; }

        
        public bool IsTranslator { get; set; }
    }

    
    public class CourseApprovalResponse
    {
        
        public List<CourseApproval> CourseApprovalList { get; set; }
    }

}
