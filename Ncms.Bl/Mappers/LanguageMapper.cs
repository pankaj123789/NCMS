using System;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using Ncms.Contracts.Models;

namespace Ncms.Bl.Mappers
{
    public class LanguageMapper : BaseMapper<LanguageDto, LanguageModel>
    {
        public override LanguageModel Map(LanguageDto source)
        {
            return new LanguageModel
            {
                LanguageId = source.LanguageId,
                Name = source.Name,
                Code = source.Code,
                Indigenous = source.Indigenous,
                GroupLanguageId = source.GroupLanguageId
            };
        }

        public override LanguageDto MapInverse(LanguageModel source)
        {
            throw new NotImplementedException();
        }
    }
}
