using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class ExaminerProfile : Profile
    {
        public ExaminerProfile()
        {
            CreateMap<ExaminerMarking, ExaminerMarkingDto>()
                .ForMember(x => x.TestResultId, y => y.MapFrom(z => z.TestResult.Id))
                .ForMember(x => x.JobExaminerId, y => y.MapFrom(z => z.JobExaminer.Id))
                .ForMember(x => x.TestComponentResults, y => y.MapFrom(z => z.ExaminerTestComponentResults));

            CreateMap<ExaminerTestComponentResult, ExaminerTestComponentResultDto>()
                .ForMember(x => x.TestComponentTypeId, y => y.MapFrom(z => z.Type.Id))
                .ForMember(x => x.ExaminerTestComponentResultId, y => y.MapFrom(z => z.Id));

            CreateMap<ExaminerUnavailable, ExaminerUnavailableContract>();

            CreateMap<ExaminerMarking, ExaminerMarking>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.ExaminerTestComponentResults, opt => opt.Ignore());

            CreateMap<ExaminerTestComponentResult, ExaminerTestComponentResult>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            //CreateMap<ExaminerRequest, JobExaminer>()
            //    .ForMember(x => x.ExaminerCost, y => y.MapFrom(z => Convert.ToDecimal(z.ExaminerCost)))
            //    .ForMember(x => x.Job, y => y.MapFrom(z => z.TestResult.Id))
            //    .ForMember(x => x.PanelMembership, y => y.MapFrom(z => z.TestResult.Id))
            //    .ForMember(x => x.ExaminerSentUser, y => y.MapFrom(z => z.TestResult.Id))
            //    .ForMember(x => x.ExaminerReceivedUser, y => y.MapFrom(z => z.TestResult.Id))
            //    .ForMember(x => x.ExaminerToPayrollUser, y => y.MapFrom(z => z.TestResult.Id))
            //    .ForMember(x => x.DateAllocated, y => y.MapFrom(z => z.TestResult.Id))
            //    .ForMember(x => x.Status, y => y.MapFrom(z => z.TestResult.Id))
            //    .ForMember(x => x.ProductSpecification, y => y.MapFrom(z => z.TestResult.Id))
            //    .ForMember(x => x.ProductSpecificationChangedUser, y => y.MapFrom(z => z.TestResult.Id))
            //    .ForMember(x => x.ValidatedUser, y => y.MapFrom(z => z.TestResult.Id))
            //    .ForMember(x => x.PayrollStatusName, y => y.MapFrom(z => z.TestResult.Id))
            //    .ForMember(x => x.Id, y => y.MapFrom(z => z.TestResult.Id));
        }
    }
}
