using System;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Common;

namespace Ncms.Bl.AutoMappingProfiles
{
    public class AttachmentProfile : Profile
    {
        public AttachmentProfile()
        {
            CreateMap<AttachmentModel, CreateOrReplaceNoteAttachmentRequest>();
        }
    }
}
