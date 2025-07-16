using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Audit;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class AuditProfile : Profile
    {
        public AuditProfile()
        {
            CreateMap<AuditListRequestModel, FindAuditLogsRequest>();

            CreateMap<AuditLogDto, AuditLogModel>();
        }
    }
}
