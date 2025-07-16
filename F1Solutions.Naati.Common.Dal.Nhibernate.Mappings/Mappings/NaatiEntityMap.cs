using System;
using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NHibernate.Properties;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class NaatiEntityMap : IAutoMappingOverride<NaatiEntity>
    {
        public void Override(AutoMapping<NaatiEntity> mapping)
        {
            mapping.Id(x => x.Id).Column("EntityId").GeneratedBy.Identity();
            mapping.HasMany(x => x.PrimaryAddresses).KeyColumn("EntityId")
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();
            mapping.HasMany(x => x.People).KeyColumn("EntityId")
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            //mapping.Map(x => x.NaatiNumberType)
            //    .Insert()
            //    .Not.Update()
            //    .Access.Using(typeof(NoopSetterAccessor))
            //    .Column("NaatiNumber"); // HACK! This isn't a typo. There's no NaatiNumberType column so we'll 
            // get an exception when NH generates a select with a 'NaatiNumberType' column.
            // Just load the NaatiNumber column instead. The value will be ignored because we're using a NoopSetterAccessor.    
            mapping.Map(x => x.EntityTypeId);
            mapping.Map(x => x.Abn);
            mapping.Map(x => x.UseEmail);
            mapping.Map(x => x.GstApplies);
            mapping.Map(x => x.WebsiteInPD);
            mapping.Map(x => x.WebsiteUrl);
            mapping.Map(x => x.Note).CustomType("StringClob").CustomSqlType("varchar(max)");
            mapping.Map(x => x.AccountNumber);
            mapping.Map(x => x.NaatiNumber);

            mapping.Table("tblEntity");
            mapping.SqlInsert("EXEC NH_EntityInsert ?,?,?,?,?,?,?,?,?");

            mapping.HasMany(x => x.Emails).KeyColumn("EntityId").Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.None();

            mapping.HasMany(x => x.Phones).KeyColumn("EntityId").Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.None();

            mapping.HasMany(x => x.Addresses).KeyColumn("EntityId").Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.None();

            mapping.HasMany(p => p.NaatiEntityNotes)
                .KeyColumn("EntityId")
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }

    public class NoopSetterAccessor : IPropertyAccessor
    {
        private IPropertyAccessor mBasicAccessor = new BasicPropertyAccessor();
        private IPropertyAccessor mNoopAccessor = new NoopAccessor();

        public IGetter GetGetter(Type theClass, string propertyName)
        {
            return mBasicAccessor.GetGetter(theClass, propertyName);
        }

        public ISetter GetSetter(Type theClass, string propertyName)
        {
            return mNoopAccessor.GetSetter(theClass, propertyName);
        }

        public bool CanAccessThroughReflectionOptimizer
        {
            get { return false; }
        }
    }
}