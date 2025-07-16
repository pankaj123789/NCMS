using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.CredentialRequest;
using Ncms.Contracts.Models.Test;
using Ncms.Contracts.Models.TestResult;
using TestComponentModel = Ncms.Contracts.Models.TestComponentModel;
using TestTaskModel = Ncms.Contracts.TestTaskModel;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            CreateMap<TestAttendanceAssetSearchRequestModel, SearchTestAttendanceAssetsRequest>()
                .ForMember(x => x.IncludeDeleted, y => y.Ignore());

            CreateMap<TestComponentModel, StandardTestComponentContract>();

            CreateMap<Ncms.Contracts.Models.Test.TestComponentModel, RubricTestComponentContract>();

            CreateMap<TestTaskDetailDto, TestTaskModel>();

            CreateMap<PersonTestTaskDto, PersonTestTask>();

            CreateMap<SupplementarytTestApplicantDto, SupplementarytTestApplicantModel>();

            CreateMap<TestSearchResultDto, TestSearchResultModel>()
                .ForMember(x => x.AccreditationDescription, y => y.Ignore());

            CreateMap<VenueDto, VenueModel>();

            CreateMap<TestSummaryDto, TestSummaryModel>()
                .ForMember(x => x.TestResultStatus, y => y.MapFrom(z => z.LastTestResultStatus))
                .ForMember(x => x.TestResultStatusId, y => y.MapFrom(z => z.LastTestResultStatusTypeId))
                .ForMember(x => x.Actions, y => y.Ignore());

            CreateMap<TestComponentTypeDto, TestComponentTypeModel>();

            CreateMap<TestResultDto, TestResultModel>()
                .ForMember(x => x.FailureReasonIds, y => y.Ignore());

            CreateMap<TestResultModel, TestResultDto>();

            CreateMap<StandardTestComponentContract, TestComponentModel>()
                .ForMember(x => x.ReadOnly, y => y.Ignore());

            CreateMap<TestSittingDocumentDto, TestSittingDocumentModel>()
                .ForMember(x=>x.SoftDeleteDate, y => y.Ignore());

            CreateMap<CreateOrReplaceTestSittingDocumentModel, CreateOrReplaceTestSittingDocumentDto>()
                .ForMember(x => x.StoragePath, y => y.Ignore());
        }
    }
}
