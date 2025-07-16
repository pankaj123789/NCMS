using System;
using System.Collections.Generic;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.CredentialRequest;
using Ncms.Contracts.Models.Test;
using TestTaskModel = Ncms.Contracts.Models.Test.TestTaskModel;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class TestSessionProfile : Profile
    {
        public TestSessionProfile()
        {
            CreateMap<TestSessionSearchResultDto, TestSessionSearchResultModel>()
                .ForMember(x => x.Skill, y => y.MapFrom(z => z.SkillDisplayName));

            CreateMap<TestTaskDto, TestTaskModel>();

            CreateMap<TestSessionApplicantsDto, ApplicantModel>();

            CreateMap<TestSessionDto, TestSessionModel>()
                .ForMember(x => x.TestTime, y => y.Ignore())//set manually
                .ForMember(x => x.RehearsalDate, y => y.Ignore())//set manually
                .ForMember(x => x.RehearsalTime, y => y.Ignore())//set manually
                .ForMember(x => x.Applicants, y => y.Ignore());//set manually

            CreateMap<TestSessionSkillDto, TestSessionSkillModel>()
                .ForMember(x => x.MaximumCapacity, y => y.MapFrom(z => z.Capacity))
                .ForMember(x => x.Selected, y => y.Ignore());

            CreateMap<TestSessionApplicantDto, TestSessionApplicantModel>()
                .ForMember(x => x.Status,y => y.MapFrom(z => GetStatusDisplayNameForTestSesssionApplicant((CredentialRequestStatusTypeName)z.StatusId, z.Status, z.StatusModifiedDate)));

            CreateMap<TestSessionRolePlayerDetailDto, TestSessionRolePlayerDetailModel>();

            CreateMap<TestSessionRolePlayerModel, TestSessionRolePlayerDto>()
                .ForMember(x => x.VenueName, y => y.Ignore())
                .ForMember(x => x.CredentialTypeExternalName, y => y.Ignore())
                .ForMember(x => x.RehearsalNotes, y => y.Ignore())
                .ForMember(x => x.PublicNote, y => y.Ignore())
                .ForMember(x => x.VenueAddress, y => y.Ignore())
                .ForMember(x => x.VenueCoordinates, y => y.Ignore())
                .ForMember(x => x.NaatiNumber, y => y.Ignore());

            CreateMap<TestSessionRolePlayerDto, TestSessionRolePlayerModel>();

            CreateMap<RolePlayerAvailabilityDto, TestSessionRolePlayerAvailabilityModel>()
                .ForMember(x => x.CustomerNo, y => y.MapFrom(z => z.NaatiNumber))
                .ForMember(x => x.Name, y => y.MapFrom(z => $"{z.GivenName} {z.Surname}"))
                .ForMember(x => x.TestSessionRolePlayerId, y => y.Ignore())
                .ForMember(x => x.Attended, y => y.Ignore())
                .ForMember(x => x.Rejected, y => y.Ignore())
                .ForMember(x => x.Rehearsed, y => y.Ignore())
                .ForMember(x => x.RolePlayerStatusId, y => y.Ignore())
                .ForMember(x => x.Details, y => y.MapFrom(z => new List<TestSessionRolePlayerTaskModel>()))
                .ForMember(x => x.LanguageIds, y => y.MapFrom(z => new HashSet<int>()))
                .ForMember(x => x.ReadOnly, y => y.Ignore())
                .ForMember(x => x.LastSystemActionTypeId, y => y.Ignore())
                .ForMember(x => x.DisplayOrder, y => y.Ignore());

            CreateMap<RolePlayerAvailabilityDto, RolePlayerAvailabilityModel>()
                .ForMember(x => x.CustomerNo, y => y.Ignore())
                .ForMember(x => x.Name, y => y.Ignore())
                .ForMember(x => x.LanguageIds, y => y.Ignore())
                .ForMember(x => x.ReadOnly, y => y.Ignore())
                .ForMember(x => x.DisplayOrder, y => y.Ignore());

              CreateMap<TestSessionRolePlayerAvailabilityDto, TestSessionRolePlayerAvailabilityModel>()
                .IncludeBase<RolePlayerAvailabilityDto, RolePlayerAvailabilityModel>()
                .ForMember(x => x.CustomerNo, y => y.MapFrom(z => z.NaatiNumber))
                .ForMember(x => x.Name, y => y.MapFrom(z => $"{z.GivenName} {z.Surname}"))
                .ForMember(x => x.LanguageIds, y => y.MapFrom(z => new HashSet<int>()))
                .ForMember(x => x.LastSystemActionTypeId,
                    y => y.MapFrom(rp => GetViewRolePlayerActionMapping()[rp.RolePlayerStatusId]))
                .ForMember(x => x.ReadOnly, y => y.Ignore())
                .ForMember(x => x.DisplayOrder, y => y.Ignore());

            CreateMap<RolePlayerTaskDetailDto, TestSessionRolePlayerTaskModel>()
                .ForMember(x => x.LanguageId, y => y.Ignore());

            CreateMap<TestSessionSpecificationDetailsDto, TestSessionSpecificationDetailsModel>();

            CreateMap<TestSessionSkillDetailsDto, TestSessionLanguageDetailsModel>();
        }


        private string GetStatusDisplayNameForTestSesssionApplicant(CredentialRequestStatusTypeName status,
            string statusDisplayName, DateTime modifiedStatusDate)
        {
            switch (status)
            {
                case CredentialRequestStatusTypeName.TestSessionAccepted:
                    return string.Format(Naati.Resources.Shared.Confirmed, modifiedStatusDate.ToShortDateString());
                default:
                    return statusDisplayName;
            }
        }

        private Dictionary<int, int> GetViewRolePlayerActionMapping()
        {
            var lastActioStatusnMapping = new Dictionary<int, int>()
            {
                {(int) RolePlayerStatusTypeName.Pending, (int) SystemActionTypeName.RolePlayerMarkAsPending},
                {(int) RolePlayerStatusTypeName.Accepted, (int) SystemActionTypeName.RolePlayerMarkAsAccepted},
                {(int) RolePlayerStatusTypeName.Attended, (int) SystemActionTypeName.RolePlayerMarkAsAttendedTest},
                {
                    (int) RolePlayerStatusTypeName.Rehearsed,
                    (int) SystemActionTypeName.RolePlayerMarkAsAttendedRehearsal
                },
                {(int) RolePlayerStatusTypeName.NoShow, (int) SystemActionTypeName.RolePlayerMarkAsNoShow},
                {(int) RolePlayerStatusTypeName.Rejected, (int) SystemActionTypeName.RolePlayerMarkAsRejected},
                {(int) RolePlayerStatusTypeName.None, (int) SystemActionTypeName.RolePlayerMarkAsRemoved},
            };

            return lastActioStatusnMapping;
        }
    }
}
