using System.Collections.Generic;
using System.Reflection;
using AutoMapper;

namespace F1Solutions.Global.Common.Mapping
{
    public interface IAutoMapperHelper
    {
        IMapper Mapper { get; }

        void Configure(IList<Assembly> assemblies, bool test = false);

        void Reset();
    }
}
