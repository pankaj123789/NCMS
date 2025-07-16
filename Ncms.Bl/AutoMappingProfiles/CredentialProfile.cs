using System;
using System.Linq;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Person;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class CredentialProfile : Profile
    {
        public CredentialProfile()
        {
            CreateMap<WizardIssueCredentialStepModel, WizardIssueCredentialStepModel>();

            CreateMap<WizardIssueCredentialStepModel, IssueCredentialDataModel>()
                .IncludeBase<WizardIssueCredentialStepModel, WizardIssueCredentialStepModel>()
                .ForMember(x => x.CertificationPeriodEnd, y => y.Ignore())
                .ForMember(x => x.CertificationPeriodStart, y => y.Ignore());

            CreateMap<IssueCredentialDataModel, IssueCredentialDataModel>()
                .IncludeBase<WizardIssueCredentialStepModel, WizardIssueCredentialStepModel>();

            CreateMap<CredentialModel, CreateOrUpdateCredentialModel>()
                .ForMember(x => x.CredentialId, y => y.Ignore())
                .ForMember(x => x.CredentialRequestId, y => y.Ignore());

            CreateMap<CredentialDto, CredentialModel>()
                .ForMember(x => x.RecertificationStatus, y => y.Ignore());

            CreateMap<CredentialApplicationTypeCredentialTypeDto, CredentialApplicationTypeCredentialTypeModel>()
                .ForMember(x => x.HasTestFee, y => y.Ignore());

            CreateMap<CredentialTypeDto, CredentialTypeModel>();

            CreateMap<CredentialsDetailsDto, CredentialDetailsModel>()
                .ForMember(x => x.RecertificationStatus, y => y.Ignore());

            CreateMap<CredentialModel, CredentialDto>()
                .ForMember(x => x.CredentialTypeExternalName, y => y.Ignore())
                .ForMember(x => x.CredentialCategoryName, y => y.Ignore());

            CreateMap<CredentialTypeUpgradePathDto, CredentialTypeUpgradePathModel>();

            CreateMap<SetCredentialTerminateDateModel, SetCredentialTerminateDateRequest>()
                .ForMember(x => x.CredentialId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.UserId, y => y.Ignore());

            CreateMap<CredentialTestSearchRequestModel, GetAllCredentialTestsRequest>();

            CreateMap<CredentialRequestTestRequestDto, CredentialRequestTestRequestModel>();

            CreateMap<CredentialApplicationAttachmentDto, CredentialApplicationAttachmentModel>()
                .ForMember(x => x.Title, y => y.Ignore())
                .ForMember(x => x.NoteId, y => y.Ignore());

            CreateMap<CredentialApplicationAttachmentModel, CreateOrReplaceApplicationAttachmentRequest>()
                .ForMember(x => x.TokenToRemoveFromFilename, y => y.Ignore());
        }
    }
}
