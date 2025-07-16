using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAATI.Domain;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class GadgetTypeMap : IAutoMappingOverride<GadgetType>
    {
        public void Override(AutoMapping<GadgetType> mapping)
        {
            mapping.Table("tluGadgetType");
        }
    }
}
