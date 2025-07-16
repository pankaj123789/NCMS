using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class TestResultProfile : Profile
    {
        public TestResultProfile()
        {
            CreateMap<TestResult, TestResultDto>()
                .ForMember(x => x.TestResultId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.ResultTypeId, y => y.MapFrom(z => z.ResultType.Id))
                .ForMember(x => x.CurrentJobId, y => y.MapFrom(z => z.CurrentJob.Id))
                .ForMember(x => x.Comments1, y => y.Ignore())
                .ForMember(x => x.Comments2, y => y.Ignore())
                .ForMember(x => x.CommentsEthics, y => y.Ignore())
                .ForMember(x => x.DueDate, y => y.Ignore());
            
            CreateMap<TestResultDto, TestResult>()
                .ForMember(x => x.Id, y => y.MapFrom(z => z.TestResultId))
                .ForMember(x => x.CurrentJobId, y => y.MapFrom(z => z.CurrentJobId))
                .ForMember(x => x.ResultType, y => y.Ignore())
                .ForMember(x => x.ReviewInvoiceLineId, y => y.Ignore())
                .ForMember(x => x.TestSitting, y => y.Ignore())
                .ForMember(x => x.CurrentJob, y => y.Ignore());
        }
    }
}
