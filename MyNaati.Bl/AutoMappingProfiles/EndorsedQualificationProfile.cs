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
using PaymentDto = MyNaati.Contracts.BackOffice.PaymentDto;

namespace MyNaati.Bl.AutoMappingProfiles
{
    public class EndorsedQualificationProfile : Profile
    {
        public EndorsedQualificationProfile()
        {
            CreateMap<EndorsedQualificationDto, EndorsedQualificationContract>();
        }
    }
}
