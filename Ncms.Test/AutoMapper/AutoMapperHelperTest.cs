using System;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl.AutoMappingProfiles;
using F1Solutions.Naati.Common.Dal.AutoMappingProfiles;
using Ncms.Bl.AutoMappingProfiles;
using Xunit;
using Xunit.Abstractions;

namespace Ncms.Test.AutoMapper
{
    public class AutoMapperHelperTest
    {
        private readonly ITestOutputHelper _output;

        public AutoMapperHelperTest(ITestOutputHelper output)
        {
            this._output = output;
        }

        [Fact]
        public void Configure_WhenAssembliesArePassedMyNaati_ProfilesConfigured()
        {
            var assemblies = new[]
            {
                typeof(AccountingProfile).Assembly,
                typeof(CertificationPeriodProfile).Assembly,
                typeof(PodHistoryProfile).Assembly,
            };
            var autoMapperHelper = new AutoMapperHelper();
            autoMapperHelper.Configure(assemblies, true);
        }

        [Theory]
        [InlineData(typeof(AccountingProfile))]
        [InlineData(typeof(CertificationPeriodProfile))]
        [InlineData(typeof(PodHistoryProfile))]
        public void Configure_WhenAssemblyIsPassedMyNaati_ProfilesConfigured(Type profileType)
        {
            var assemblies = new[]
            {
                profileType.Assembly,
            };

            var autoMapperHelper = new AutoMapperHelper();
            autoMapperHelper.Configure(assemblies, true);
        }

        [Theory]
        [InlineData(typeof(AccountingProfile))]
        [InlineData(typeof(CertificationPeriodProfile))]
        [InlineData(typeof(PodHistoryProfile))]
        public void AutoMapperConfigure_WhenProfileIsPassedMyNaati_ProfileIsConfigured(Type profileType)
        {
            var profiles = profileType.Assembly.GetExportedTypes()
                .Where(t => t.IsSubclassOf(typeof(Profile)));

            foreach (var profile in profiles)
            {
                _output.WriteLine("This is output from {0}", profile.FullName);

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddMaps(profile);
                });

                config.CreateMapper();
                config.AssertConfigurationIsValid();
            }
        }
    }
}
