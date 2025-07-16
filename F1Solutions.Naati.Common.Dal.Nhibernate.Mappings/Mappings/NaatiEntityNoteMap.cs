using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class NaatiEntityNoteMap : IAutoMappingOverride<NaatiEntityNote>
    {
        public void Override(AutoMapping<NaatiEntityNote> mapping)
        {
            mapping.Id(x => x.Id).Column("EntityNoteId");
            mapping.Table("tblEntityNote");
        }
    }
}