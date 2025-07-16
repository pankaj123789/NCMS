using System;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl.AutoMappingProfiles;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using Ncms.Bl;
using Ncms.Bl.AutoMappingProfiles;
using Ncms.Contracts;
using Ncms.Test.Utils;
using NSubstitute;
using Xunit;


namespace Ncms.Test.BusinessServices
{
    public class CredentialPeriodCalculator
    {
        [Fact]
        public void CheckCurrentDate()
        {
            var applicationService = Setup();

            var calcDate = DateTime.Now.Date.GetLeapYearAdjustedEndDate(3);
            Assert.Equal(DateTime.Now.Date.AddDays(1095).AddDays(-1), calcDate);
        }

        [Fact]
        public void CheckIntRound()
        {
            var totalDays = 1094;
            var requirementYears =  totalDays / 365m;
            var requirement = Math.Round(requirementYears) * 10000;
            Assert.Equal(30000, requirement);
        }

        [Fact]
        public void CheckDayBeforeLeapDay()
        {
            var applicationService = Setup();
            var date = DateTime.Parse("2020-02-27");
            var calcDate = date.GetLeapYearAdjustedEndDate(3);
            Assert.Equal(date.Date.AddDays(1095).AddDays(-1), calcDate);
        }

        [Fact]
        public void CheckDayAfterLeapDay()
        {
            var applicationService = Setup();
            var date = DateTime.Parse("2020-03-01");
            var calcDate = date.GetLeapYearAdjustedEndDate(3);
            Assert.Equal(date.Date.AddDays(1095).AddDays(-1), calcDate);
        }

        private ApplicationService Setup()
        {
            var assemblies = new[]
{
                typeof(AccountingProfile).Assembly,
                typeof(PodHistoryProfile).Assembly,
            };

            var autoMapperHelper = new AutoMapperHelper();
            autoMapperHelper.Configure(assemblies, true);
            var serviceLocator = ServiceLocatorTestHelper.GetInstance();
            serviceLocator.MockService<IAutoMapperHelper>(autoMapperHelper);

            var userService = Substitute.For<IUserService>();
            var tokenReplacementService = Substitute.For<ITokenReplacementService>();
            var systemService = Substitute.For<ISystemService>();
            var emailMessageService = Substitute.For<IEmailMessageService>();
            var applicationQueryService = Substitute.For<IApplicationQueryService>();
            var wizardLogicService = Substitute.For<IApplicationWizardLogicService>();
            var personQueryService = Substitute.For<IPersonQueryService>();
            var institutionService = Substitute.For<IInstitutionService>();
            var financeService = Substitute.For<IFinanceService>();
            var testMaterialQueryService = Substitute.For<ITestMaterialQueryService>();
            var applicationBusinessLogicService = Substitute.For<IApplicationBusinessLogicService>();
            var personService = Substitute.For<IPersonService>();
            var examinerQueryService = Substitute.For<IExaminerQueryService>();
            var credentialPrerequisiteService = Substitute.For<ICredentialPrerequisiteService>();
            var prerequisiteApplicationsDalService = Substitute.For<ICredentialPrerequisiteDalService>();

            return new ApplicationService(userService, tokenReplacementService, systemService, emailMessageService
            , applicationQueryService, wizardLogicService, personQueryService, institutionService, financeService,
            testMaterialQueryService, applicationBusinessLogicService, personService, examinerQueryService, autoMapperHelper,
            credentialPrerequisiteService, prerequisiteApplicationsDalService);
            
        }
    }
}
