using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAATI.Domain;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class SmsStatusMap : IAutoMappingOverride<SmsStatus>
    {
        public void Override(AutoMapping<SmsStatus> mapping)
        {
            mapping.Table("tluSmsStatus");
        }
    }
}
