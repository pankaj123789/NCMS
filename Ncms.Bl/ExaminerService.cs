using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Bl.Mappers;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Examiner;
using Newtonsoft.Json;
using TestComponentModel = Ncms.Contracts.Models.TestComponentModel;

namespace Ncms.Bl
{
    public class ExaminerService : IExaminerService
    {
        private readonly IExaminerQueryService _examinerQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public ExaminerService(IExaminerQueryService examinerQueryService, IAutoMapperHelper autoMapperHelper)
        {
            _examinerQueryService = examinerQueryService;
            _autoMapperHelper = autoMapperHelper;
        }
        public void UpdateCountMarks(UpdateCountMarksRequestModel request)
        {
            var serviceRequest = new UpdateCountMarksRequest
            {
                TestResultId = request.TestResultId,
                JobExaminersId = request.JobExaminersId,
                IncludePreviousMarks = request.IncludePreviousMarks
            };

            try
            {
                _examinerQueryService.UpdateCountMarks(serviceRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }
        }

        public IList<ExtendedExaminerModel> GetExaminers(string request, out bool extended)
        {
            var examinersRequest = JsonConvert.DeserializeObject<GetExaminersRequest>(request);
            GetExaminersResponse examinersResponse = null;

            try
            {
               examinersResponse = _examinerQueryService.GetExaminers(examinersRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            extended = examinersResponse.Extended;

            var examinerDtoMapper = new ExtendedExaminerDtoMapper();
            return examinersResponse?.Examiners.Select(examinerDtoMapper.Map).ToList() ?? new List<ExtendedExaminerModel>();
        }

        public GenericResponse<IEnumerable<JobExaminerModel>> GetTestExaminers(GetJobExaminersRequestModel request)
        {
            var response = _examinerQueryService.GetJobExaminers(_autoMapperHelper.Mapper.Map<GetJobExaminersRequest>(request));

            var models = response.Results.Select(_autoMapperHelper.Mapper.Map<JobExaminerModel>).ToList();
            models.ForEach(MapJobExaminerStatus);
            return new GenericResponse<IEnumerable<JobExaminerModel>>(models);
        }

        private void MapJobExaminerStatus(JobExaminerModel model)
        {
            var examinerStatus = GetExaminerStatus(model);
            var examinerType = GetExaminerType(model);
            model.ExaminerStatusId = (int)examinerStatus;
            model.ExaminerStatusName = examinerStatus.ToString();
            model.ExaminerTypeId = (int)examinerType;
            model.ExaminerTypeName = examinerType.ToString();
        }
     

        public GenericResponse<IEnumerable<PersonExaminerModel>> GetActiveExaminersByLanguageAndCredentialType(GetExaminersByLanguageRequestModel request)
        {
            var response = _examinerQueryService.GetActiveExaminersByLanguageAndCredentialType(_autoMapperHelper.Mapper.Map<GetPersonExaminersByLanguageRequest>(request));

            var rolePlayersDictionary = _examinerQueryService.GetRolePlayersForTestSitting(request.TestAttendanceId).Data.ToDictionary(x=> x.NaatiNumber, y=> y);
            var filteredExaminers = response.Results.Where(x => !rolePlayersDictionary.ContainsKey(x.NaatiNumber));
            var models = filteredExaminers.Select(_autoMapperHelper.Mapper.Map<PersonExaminerModel>).ToList();
            return new GenericResponse<IEnumerable<PersonExaminerModel>>(models);
        }

        public void RemoveJobExaminer(int jobExaminerId)
        {
            _examinerQueryService.RemoveExaminers(new RemoveExaminersRequest { JobExaminersIds = new [] { jobExaminerId}});
        }

        public GenericResponse<JobExaminerModel> GetJobExaminer(int jobExaminerId, bool includeExaminerMarks)
        {
            var response = _examinerQueryService.GetJobExaminerById(new GetJobExaminerRequest { JobExaminerId = jobExaminerId, IncludeExaminerMarks = includeExaminerMarks });
            var model = _autoMapperHelper.Mapper.Map<JobExaminerModel>(response.Result);
            MapJobExaminerStatus(model);
            return model;
        }

        private ExaminerTypeName GetExaminerType(JobExaminerModel examinerdDto)
        {
            if (examinerdDto.ThirdExaminer)
            {
                return ExaminerTypeName.ThirdExaminer;
            }

            if (examinerdDto.PaidReviewer)
            {
                return ExaminerTypeName.PaidReviewer;
            }

            return ExaminerTypeName.Original;
        }
        private ExaminerStatusTypeName GetExaminerStatus(JobExaminerModel examinerdDto)
        {
            if (examinerdDto.ExaminerReceivedDate.HasValue)
            {
                return ExaminerStatusTypeName.Submitted;
            }

            if (examinerdDto.JobDueDate <  DateTime.Now.Date)
            {
                return ExaminerStatusTypeName.Overdue;
            }

            return ExaminerStatusTypeName.InProgress;
        }

        public GetMarksResponseModel GetMarks(GetExaminerMarksRequestModel request)
        {
            GetMarksResponse marksResponse = null;

            try
            {
                 marksResponse = _examinerQueryService.GetMarks(new GetMarksRequest { TestResultId = request.TestResultId, JobExaminerId = request.JobExaminerId });
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            var response = new GetMarksResponseModel();
            var testComponentMapper = new TestComponentMapper();
            response.Components = marksResponse?.Components.Select(testComponentMapper.Map).ToList() ?? new List<TestComponentModel>();
            response.OverAllPassMark = new TestSpecificationPassMarkModel { OverAllPassMark = (marksResponse?.OverAllPassMark?.OverAllPassMark).GetValueOrDefault() };

            return response;
        }

        public void SaveMarks(SaveExaminerMarksRequestModel request)
        {
            var testComponentMapper = new TestComponentMapper();
            var serviceRequest = new SaveExaminerMarksRequest
            {
                TestResultId = request.TestResultId,
                JobExaminerId = request.JobExaminerId,
                Components = request.Components.Select(testComponentMapper.MapInverse).ToArray(),
                IncludePreviousMarks = request.IncludePreviousMarks
            };

            try
            {
                _examinerQueryService.SaveMarks(serviceRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }
        }
    }
}
