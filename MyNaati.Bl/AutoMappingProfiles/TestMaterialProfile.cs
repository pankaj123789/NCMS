using System.Linq;
using AutoMapper;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using MyNaati.Contracts.BackOffice;
namespace MyNaati.Bl.AutoMappingProfiles
{
    public class TestMaterialProfile : Profile
    {
        public TestMaterialProfile()
        {
            CreateMap<TestMaterialCreationPaymentDto, TestMaterialCreationPaymentContract>();
        }
    }
}
