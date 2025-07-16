//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using F1Solutions.NAATI.ePortal.Data.Conventions;
//using F1Solutions.NAATI.ePortal.Domain;
//using FluentNHibernate.Automapping;
//using FluentNHibernate.Conventions.Helpers;
//using FluentNHibernate.Conventions.Instances;

//namespace F1Solutions.NAATI.ePortal.Data
//{
//    public class AutoPersistenceModelGenerator 
//    {
//        public AutoPersistenceModel Generate()
//        {
//            return AutoMap.Assemblies(new AutoMappingConfiguration(), typeof(EntityBase).Assembly)
//                .Conventions.AddFromAssemblyOf<PrimaryKeyConvention>()
//                .UseOverridesFromAssemblyOf<AutoPersistenceModelGenerator>()
//                .Conventions.Add(ConventionBuilder.Property.Always(delegate(IPropertyInstance instance)
//                                                                       {
//                                                                           if (instance.Property.PropertyType == typeof(byte[]))
//                                                                               instance.Length(16777216);
//                                                                       }));
//        }
//    }
//}
