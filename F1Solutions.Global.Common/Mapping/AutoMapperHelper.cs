using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using F1Solutions.Global.Common.Logging;

namespace F1Solutions.Global.Common.Mapping
{
    public class AutoMapperHelper : IAutoMapperHelper
    {
        public IMapper Mapper { get; private set; }

        public void Configure(IList<Assembly> assemblies, bool test = false)
        {
            if (!assemblies?.Any() ?? false)
            {
                LoggingHelper.LogError("No AutoMapper assemblies has been provided");
                throw new Exception("No AutoMapper assemblies has been provided");
            }

            if (Mapper != null && !test)
            {
                 LoggingHelper.LogError("AutoMapper configuration has been already provided");
                 throw new Exception("AutoMapper configuration has been already provided");
            }

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(assemblies);
            });

            config.AssertConfigurationIsValid();

            Mapper = config.CreateMapper();
        }

        public void Reset()
        {
            Mapper = null;
        }
    }
}
