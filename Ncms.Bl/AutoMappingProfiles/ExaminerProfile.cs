using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Examiner;
using Ncms.Contracts.Models.RolePlayer;
using Ncms.Contracts.Models.Test;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class ExaminerProfile : Profile
    {
        public ExaminerProfile()
        {
            CreateMap<AddOrUpdateTestExaminersResponse, AddExaminerResponseModel>();

            CreateMap<TestSessionRolePlayerDetailModel, TestSessionRolePlayerDetailDto>()
                .ForMember(x => x.TaskLabel, y => y.Ignore())
                .ForMember(x => x.TaskTypeLabel, y => y.Ignore());

            CreateMap<PersonNoteModel, PersonNoteData>()
                .ForMember(x => x.Description, y => y.Ignore());

            CreateMap<RolePlayerActionModel, UpsertSessionRolePlayerRequest>();

            CreateMap<GetJobExaminersRequestModel, GetJobExaminersRequest>();

            CreateMap<PersonExaminerDto, PersonExaminerModel>();

            CreateMap<ExaminerMarkingDto, ExaminerMarkingModel>();

            CreateMap<ExaminerTestComponentResultDto, ExaminerTestComponentResultModel>();

            CreateMap<JobExaminerDto, JobExaminerModel>()
                .ForMember(x => x.ExaminerStatusId, y => y.Ignore())
                .ForMember(x => x.ExaminerStatusName, y => y.Ignore())
                .ForMember(x => x.ExaminerTypeId, y => y.Ignore())
                .ForMember(x => x.ExaminerTypeName, y => y.Ignore());

            CreateMap<GetExaminersByLanguageRequestModel, GetPersonExaminersByLanguageRequest>();
        }
    }
}
