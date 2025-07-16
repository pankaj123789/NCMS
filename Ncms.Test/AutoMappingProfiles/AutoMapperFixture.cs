using System;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl.AutoMappingProfiles;
using F1Solutions.Naati.Common.Dal.AutoMappingProfiles;
using Ncms.Bl.AutoMappingProfiles;

namespace Ncms.Test.AutoMappingProfiles
{
    public class AutoMapperFixture : IDisposable
    {
        public AutoMapperFixture()
        {
            var assemblies = new[]
            {
                typeof(AccountingProfile).Assembly,
                typeof(PodHistoryProfile).Assembly,
            };

            //AutoMapperHelper.Configure(assemblies);
        }

        public void Dispose()
        {
           // AutoMapperHelper.Reset();
        }
    }

}
