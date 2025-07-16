using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Logbook;
using Ncms.Contracts.Models.Person;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class CertificationProfile : Profile
    {
        public CertificationProfile()
        {
            CreateMap<WorkPracticeDataModel, WorkPracticeData>();

            CreateMap<RecertificationModel, RecertificationDto>()
                .ForMember(x => x.ApplicationId, y => y.MapFrom(z => z.CredentialApplicationId))
                .ForMember(x => x.CredentialApplicationStatus, y => y.Ignore())
                .ForMember(x => x.SubmittedDate, y => y.Ignore());

            CreateMap<PdActivityFieldModel, PdActivityData>();

            CreateMap<CertificationPeriodDto, CertificationPeriodModel>()
                .ForMember(x => x.RecertificationStatus, y => y.Ignore())
                .ForMember(x => x.Credentials, y => y.Ignore());

            CreateMap<GetCertificationPeriodsRequestModel, GetCertificationPeriodsRequest>()
                .ForMember(x => x.PractitionerNumber, y => y.Ignore());

            CreateMap<CertificationPeriodModel, CertificationPeriodDto>()
                .ForMember(x => x.UserId, y => y.Ignore());

            CreateMap<SetCertificationEndDateRequestModel, SetCertificationEndDateRequest>()
                .ForMember(x => x.CertificationPeriodId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.UserId, y => y.Ignore());

            CreateMap<CreateCertificationPeriodModel, CreateCertificationPeriodRequest>();

            CreateMap<ProfessionalDevelopmentRequirementResponse, RequirementModel>();

            CreateMap<ProfessionalDevelopmentCategoryGroupResponse, CategoryGroupModel>();

            CreateMap<ProfessionalDevelopmentCategoryResponse, CategoryModel>();

            CreateMap<ProfessionalDevelopmentActivityDto, ActivityModel>();
        }
    }
}
