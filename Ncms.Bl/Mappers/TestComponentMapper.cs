using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using TestComponentModel = Ncms.Contracts.Models.TestComponentModel;

namespace Ncms.Bl.Mappers
{
    public class TestComponentMapper : BaseMapper<StandardTestComponentContract, TestComponentModel>
    {
        public override TestComponentModel Map(StandardTestComponentContract source)
        {
            return new TestComponentModel
            {
                Id = source.Id,
                TotalMarks = source.TotalMarks,
                PassMark = source.PassMark,
                Mark = source.Mark,
                ComponentNumber = source.ComponentNumber,
                Label = source.Label,
                Name = source.Name,
                GroupNumber = source.GroupNumber,
                TypeId = source.TypeId,
                MarkingResultTypeId = source.MarkingResultTypeId,
                TestComponentResultId = source.TestComponentResultId,
                ReadOnly = source.MarkingResultTypeId == (int)MarkingResultTypeName.FromOriginal,

            };
        }

        public override StandardTestComponentContract MapInverse(TestComponentModel source)
        {
            return new StandardTestComponentContract
            {
                Id = source.Id,
                TotalMarks = source.TotalMarks,
                PassMark = source.PassMark,
                Mark = source.Mark,
                ComponentNumber = source.ComponentNumber,
                Label = source.Label,
                Name = source.Name,
                GroupNumber = source.GroupNumber,
                TypeId = source.TypeId,
                TestComponentResultId = source.TestComponentResultId
            };
        }
    }
}
