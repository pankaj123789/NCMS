using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Bl.AddressParser;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.User;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserRoleDto, UserRolesModel>();

            CreateMap<UserDetailsModel, UserDto>()
                .ForMember(x => x.Office, y => y.Ignore())
                .ForMember(x => x.UserRoles, y => y.Ignore())
                .ForMember(x => x.LastPasswordChangeDate, y => y.Ignore())
                .ForMember(x => x.IsLockedOut, y => y.Ignore())
                .ForMember(x => x.LastLockoutDate, y => y.Ignore())
                .ForMember(x => x.FailedPasswordAttemptCount, y => y.Ignore());

            CreateMap<UserDto, UserDetailsModel>()
                .ForMember(x => x.UpdatePassword, y => y.Ignore());
        }
    }
}
