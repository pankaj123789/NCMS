using System;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Conventions;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions.Helpers;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings
{
    public class AutoPersistenceModelGenerator
    {
        public AutoPersistenceModel Generate()
        {
            var model = AutoMap.Assemblies(new SAMAutomappingConfiguration(), typeof (NaatiEntity).Assembly)
                .Conventions.AddFromAssemblyOf<TableNameConvention>()
                .Conventions.Add(ForeignKey.EndsWith("Id"))
               
                .UseOverridesFromAssemblyOf<AutoPersistenceModelGenerator>();

            model.MergeMappings = true;
            return model;
        }
    }

    public class SAMAutomappingConfiguration : DefaultAutomappingConfiguration
    {

        public override bool ShouldMap(Type type)
        {
            var map =base.ShouldMap(type) && type.IsSubclassOf(typeof(EntityBase));

            return map;
        }

        public override bool ShouldMap(Member member)
        {
            return base.ShouldMap(member) && member.CanWrite;
        }
    }
}
