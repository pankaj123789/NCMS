using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Examiner;

namespace Ncms.Contracts
{
    public interface IExaminerService
    {
        IList<ExtendedExaminerModel> GetExaminers(string request, out bool extended);
        GenericResponse<IEnumerable<JobExaminerModel>> GetTestExaminers(GetJobExaminersRequestModel request);
        GenericResponse<IEnumerable<PersonExaminerModel>> GetActiveExaminersByLanguageAndCredentialType(GetExaminersByLanguageRequestModel request);

        void RemoveJobExaminer(int jobExaminerId);
        GenericResponse<JobExaminerModel> GetJobExaminer(int jobExaminerId, bool includeExaminerMarks);
        
        GetMarksResponseModel GetMarks(GetExaminerMarksRequestModel request);
        void SaveMarks(SaveExaminerMarksRequestModel request);
        void UpdateCountMarks(UpdateCountMarksRequestModel request);
    }

    public class GetJobExaminersRequestModel
    {
        public IEnumerable<int> TestAttendanceIds { get; set; }

        public bool IncludeExaminerMarkings { get; set; }
    }

    public class GetExaminersByLanguageRequestModel
    {
        public int Language1Id { get; set; }
        public int Language2Id { get; set; }
        public int CredentialTypeId { get; set; }

        public int TestAttendanceId { get; set; }
    }

}
