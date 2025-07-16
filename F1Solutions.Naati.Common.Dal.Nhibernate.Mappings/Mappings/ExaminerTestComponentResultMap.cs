using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class ExaminerTestComponentResultMap : IAutoMappingOverride<ExaminerTestComponentResult>
    {
        public void Override(AutoMapping<ExaminerTestComponentResult> mapping)
        {
            mapping.Id(x => x.Id).Column("ExaminerTestComponentResultID");

            mapping.References(x => x.ExaminerMarking).Column("ExaminerMarkingID");
            mapping.References(x => x.Type).Column("TypeID");
        }
    }
}
