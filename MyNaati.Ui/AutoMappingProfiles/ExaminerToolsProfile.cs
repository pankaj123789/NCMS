using AutoMapper;
using MyNaati.Contracts.BackOffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyNaati.Ui.AutoMappingProfiles
{
    public class ExaminerToolsProfile : Profile
    {
        public ExaminerToolsProfile()
        {
            CreateMap<GetRolePlayerSettingsResponse, RolePlayerSettingsDto>()
                 .ForMember(x => x.RolePlayLocations, m => m.Ignore())
                 .ForMember(x => x.MaximumRolePlaySessions, m => m.Ignore())
                 .ForMember(x => x.NaatiNumber, m => m.Ignore());
        }
    }
}