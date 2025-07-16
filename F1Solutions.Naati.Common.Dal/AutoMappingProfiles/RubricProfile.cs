using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.AutoMappingProfiles
{
    public class RubricProfile : Profile
    {
        public RubricProfile()
        {
            CreateMap<RubricQuestionPassRule, RubricQuestionPassRuleDto>();

            CreateMap<RubricTestBandRule, RubricTestBandRuleDto>();

            CreateMap<RubricTestQuestionRule, RubricTestQuestionRuleDto>();

            CreateMap<RubricMarkingBand, RubricMarkingBandDto>()
                .ForMember(x => x.BandId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.Label, y => y.MapFrom(z => z.Label))
                .ForMember(x => x.Level, y => y.MapFrom(z => z.Level))
                .ForMember(x => x.Description, y => y.MapFrom(z => z.Description))
                .ForMember(x => x.CriterionName, y => y.MapFrom(z => z.RubricMarkingAssessmentCriterion.Name))
                .ForMember(x => x.CriterionLabel, y => y.MapFrom(z => z.RubricMarkingAssessmentCriterion.Label));
        }
    }
}
