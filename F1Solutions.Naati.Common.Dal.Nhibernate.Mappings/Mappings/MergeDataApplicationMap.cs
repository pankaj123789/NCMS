using System;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Conventions;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class MergeDataApplicationMap : IAutoMappingOverride<MergeDataApplication>
    {
        public void Override(AutoMapping<MergeDataApplication> mapping)
        {
            
        }

    }
}
