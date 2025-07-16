using System;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Dal.Domain;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using GetTestDetailsRequest = F1Solutions.Naati.Common.Contracts.Dal.Request.GetTestDetailsRequest;
using PaymentDto = MyNaati.Contracts.BackOffice.PaymentDto;
using TestAttendanceDocumentContract = MyNaati.Contracts.BackOffice.TestAttendanceDocumentContract;
using TestContract = F1Solutions.Naati.Common.Contracts.Dal.TestContract;

namespace MyNaati.Bl.AutoMappingProfiles
{
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            CreateMap<TestContract, Contracts.BackOffice.TestContract>()
                .ForMember(x => x.Category, y => y.Ignore())
                .ForMember(x => x.Direction, y => y.Ignore())
                .ForMember(x => x.Level, y => y.Ignore());//could not find anything for these and it was causing an error.


            CreateMap<StandardTestComponentContract, TestComponentContract>()
                .ForMember(x => x.ReadOnly, y => y.Ignore());

            CreateMap<TestComponentContract, StandardTestComponentContract>()
                .ForMember(x => x.TypeId, y => y.Ignore())
                .ForMember(x => x.MarkingResultTypeId, y => y.Ignore())
                .ForMember(x => x.TestComponentResultId, y => y.Ignore());

            CreateMap<Contracts.BackOffice.GetTestDetailsRequest, GetTestDetailsRequest>()
                .ForMember(x => x.UseOriginalResultMark, y => y.Ignore());

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.TestAttendanceDocumentContract, TestAttendanceDocumentContract>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.Response.SubmitTestResponse, SubmitTestResponse>();

            CreateMap<F1Solutions.Naati.Common.Contracts.Dal.Response.GetTestAttendanceDocumentResponse, GetTestAttendanceDocumentResponse>()
                .IncludeBase<F1Solutions.Naati.Common.Contracts.Dal.Response.GetDocumentResponse, GetDocumentResponse>();
        }
    }
}
