using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class TestComponentProfile : Profile
    {
        public TestComponentProfile()
        {
            CreateMap<TestComponent, StandardTestComponentContract>()
                .ForMember(x => x.TotalMarks, y => y.Ignore())
                .ForMember(x => x.PassMark, y => y.Ignore())
                .ForMember(x => x.Mark, y => y.Ignore())
                .ForMember(x => x.MarkingResultTypeId, y => y.Ignore())
                .ForMember(x => x.TestComponentResultId, y => y.Ignore());

            CreateMap<TestComponentResult, StandardTestComponentContract>()
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Type.Name))
                .ForMember(x => x.Label, y => y.MapFrom(z => z.Type.Label))
                .ForMember(x => x.TestComponentResultId, y => y.MapFrom(z => z.Id));
        }
    }
}
