using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class MergeDataMap : IAutoMappingOverride<MergeData>
    {
        public void Override(AutoMapping<MergeData> mapping)
        {
            mapping.HasManyToMany(x => x.Applications).Table("tblMergeDataApplication")
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m);

            mapping.HasManyToMany(x => x.AccreditationProducts).Table("tblMergeDataAccreditationProduct")
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m);

            mapping.HasManyToMany(x => x.Jobs).Table("tblMergeDataJob")
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m);

            mapping.HasManyToMany(x => x.Invoices).Table("tblMergeDataInvoice")
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m);

            mapping.HasManyToMany(x => x.TestAttendances).Table("tblMergeDataTestAttendance")
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m);
        }
    }
}