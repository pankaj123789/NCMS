using System;
using System.Collections.Generic;

namespace Ncms.Contracts.Models.Test
{
    public class AddExaminerRequestModel
    {
        public IEnumerable<TestDataModel> TestDataModels { get; set; }
        public DateTime DueDate { get; set; }

        public int CredentialTypeId { get; set; }
        public int SkillId { get; set; }

        public List<ExaminerRequestModel> Examiners { get; set; }
    }

    public class UpdateExaminersRequestModel
    {
        public List<ExaminerRequestModel> Examiners { get; set; }
    }

    public class TestDataModel
    {
        public int? JobId { get; set; }
        public int? TestAttendanceId { get; set; }
    }

    public class TestJobPairDto
    {
        public virtual int TestAttendanceId { get; set; }
        public virtual int? JobId { get; set; }
    }
}
