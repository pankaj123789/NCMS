using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class NoteMap : IAutoMappingOverride<Note>
    {
        public void Override(AutoMapping<Note> mapping)
        {
            mapping.Id(x => x.Id);
            mapping.Map(x => x.Description).CustomType("StringClob").CustomSqlType("nvarchar(max)");

            mapping.HasMany(x => x.NoteAttachments)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();
        }
    }
}
