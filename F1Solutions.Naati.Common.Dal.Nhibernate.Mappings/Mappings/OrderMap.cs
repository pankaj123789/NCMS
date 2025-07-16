using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class OrderMap : IAutoMappingOverride<Order>
    {
        public void Override(AutoMapping<Order> mapping)
        {
            mapping.HasMany(x => x.AccreditationProducts)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan().Inverse();
        }
    }
}
