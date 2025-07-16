using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl.AutoMappingProfiles;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.AutoMappingProfiles;
using MyNaati.Bl.AutoMappingProfiles;
using MyNaati.Contracts.Portal;
using MyNaati.Test.Utils;
using MyNaati.Ui.AutoMappingProfiles;
using MyNaati.Ui.Controllers;
using MyNaati.Ui.Models;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace MyNaati.Test.AutoMapper
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
            var serviceLocator = ServiceLocatorTestHelper.GetInstance();

            serviceLocator.MockService<ISecretsCacheQueryService>();
            serviceLocator.MockService<ILookupProvider>();

            ServiceLocator.Configure(serviceLocator);

            var assemblies = new[]
            {
                typeof(ApplicationForAccreditationWizardProfile).Assembly,
                typeof(AccountingProfile).Assembly,
                typeof(CertificationPeriodProfile).Assembly,
                typeof(PodHistoryProfile).Assembly,
            };
            var autoMapperHelper = new AutoMapperHelper();
            autoMapperHelper.Configure(assemblies, true);
         
            serviceLocator.MockService<IAutoMapperHelper>(autoMapperHelper);

        
        }

        [Theory]
        [InlineData(typeof(ApplicationForAccreditationWizardProfile))]
        [InlineData(typeof(AccountingProfile))]
        [InlineData(typeof(CertificationPeriodProfile))]
        [InlineData(typeof(PodHistoryProfile))]
        public void Configure_WhenAssemblyIsPassedMyNaati_ProfilesConfigured(Type profileType)
        {
            var serviceLocator = ServiceLocatorTestHelper.GetInstance();

            serviceLocator.MockService<ISecretsCacheQueryService>();
            serviceLocator.MockService<ILookupProvider>();

            ServiceLocator.Configure(serviceLocator);

            var assemblies = new[]
            {
                profileType.Assembly,
            };

            var autoMapperHelper = new AutoMapperHelper();
            autoMapperHelper.Configure(assemblies, true);
            serviceLocator.MockService<IAutoMapperHelper>(autoMapperHelper);
        }

        [Theory]
        [InlineData(typeof(ApplicationForAccreditationWizardProfile))]
        [InlineData(typeof(AccountingProfile))]
        [InlineData(typeof(CertificationPeriodProfile))]
        [InlineData(typeof(PodHistoryProfile))]
        public void AutoMapperConfigure_WhenProfileIsPassedMyNaati_ProfileIsConfigured(Type profileType)
        {
            var serviceLocator = ServiceLocatorTestHelper.GetInstance();

            serviceLocator.MockService<ISecretsCacheQueryService>();
            serviceLocator.MockService<ILookupProvider>();
            ServiceLocator.Configure(serviceLocator);

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
