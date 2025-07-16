using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using F1Solutions.Naati.Common.Bl.ProfessionalDevelopment;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using NSubstitute;
using Xunit;

namespace Ncms.Test.F1Solutions.Naati.Common.ProfessionalDevelopment
{
    public class NcmsCredentialPointsCalculatorHelperTest
    {

        private CertificationPeriodDto CreatePeriod(int id, DateTime startDate, DateTime endDate, CertificationPeriodStatus status)
        {
            return new CertificationPeriodDto
            {
                Id = id,
                StartDate = startDate,
                OriginalEndDate = endDate,
                EndDate = endDate,
                CertificationPeriodStatus = status
            };
        }

        private CredentialsDetailsDto CreateCredential(int credentialId, DateTime startDate, DateTime endDate, int? points)
        {
            return new CredentialsDetailsDto
            {
                Id = credentialId,
                CategoryId = 2,
                CredentialApplicationEnteredDate = new DateTime(2019,5,28).AddDays(-50),
                CredentialApplicationTypeCategoryId = 1,
                SkillId = 12,
                CredentialRequestId = 456,
                StartDate = startDate,
                EndDate = endDate,

                WorkPracticePoints = points

            };
        }

        [Theory]
        [InlineData(CertificationPeriodStatus.Current, CertificationPeriodRecertificationStatus.None, true, true, true)]
        [InlineData(CertificationPeriodStatus.Future, CertificationPeriodRecertificationStatus.None, true, true, true)]
        [InlineData(CertificationPeriodStatus.Expired, CertificationPeriodRecertificationStatus.None, true, true, true)]
        public void GetCertificationPeriodDetails_WhenCalledAndCertificationPeriodHasNotBeenSubmitted_ThenCertificationPeriodForCredentialAreLoaded(CertificationPeriodStatus status, CertificationPeriodRecertificationStatus recertificationStatus, bool expectedDefault, bool expectedEditable, bool expectedCurrent)
        {
            var credentialId = 1111;
            var naatiNumber = 1234;
            var recertificationPeriodMonths = 3;
            var validPeriodId = 465;
            var certificationPeriodStartDate = new DateTime(2019,5,28).AddDays(-365);
            var certificationPeriodEndDate = new DateTime(2019,5,28).AddDays(365);

            var allPeriods = new GetCertificationPeriodsResponse
            {
                Results = new List<CertificationPeriodDto>
                {
                    CreatePeriod(validPeriodId,certificationPeriodStartDate, certificationPeriodEndDate, status),
                    CreatePeriod(2, certificationPeriodEndDate.AddDays(1),certificationPeriodEndDate.AddDays(365), CertificationPeriodStatus.Future)
                }
            };

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Is<GetCertificationPeriodsRequest>(x =>
                    x.NaatiNumber == naatiNumber
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Current)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Future)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Expired)))
                .Returns(allPeriods);

            var logBookService = Substitute.For<ILogbookQueryService>();
            logBookService.GetCertificationPeriodCredentials(Arg.Is<int>(x => x == validPeriodId))
                .Returns(new[]
                {
                  CreateCredential(credentialId,certificationPeriodStartDate, certificationPeriodEndDate, 10 )
                });

            logBookService.GetCertificationPeriodCredentials(Arg.Is<int>(x => x != validPeriodId))
                .Returns(new CredentialsDetailsDto[0]);

            var systemQueryService = Substitute.For<ISystemQueryService>();
            systemQueryService
                .GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse { Value = recertificationPeriodMonths.ToString() });

            var activityPointsCalculator = Substitute.For<IActivityPointsCalculatorService>();

            var businessLogicService = Substitute.For<IApplicationBusinessLogicService>();
            businessLogicService.CalculateCertificationPeriodRecertificationStatus(Arg.Any<int>())
                .Returns(recertificationStatus);

            var calculator = new NcmsCredentialPointsCalculatorHelper(logBookService, systemQueryService, activityPointsCalculator, personQueryService, businessLogicService);

            var periods = calculator.GetCertificationPeriodDetails(naatiNumber, credentialId).ToList();

            var foundPeriod = periods.First(x => x.Id == validPeriodId);
            Assert.Equal(expectedDefault, foundPeriod.IsDefault);
            Assert.Equal(expectedDefault, foundPeriod.IsDefault);
            Assert.Equal(expectedEditable, foundPeriod.Editable);
            Assert.Equal(expectedCurrent, foundPeriod.IsCurrent);
            Assert.False(foundPeriod.IsCredentialSubmitted);
        }

        [Theory]
        [InlineData(CertificationPeriodStatus.Current, CertificationPeriodRecertificationStatus.BeingAssessed, true, true, true, false)]
        [InlineData(CertificationPeriodStatus.Current, CertificationPeriodRecertificationStatus.BeingAssessed, false, true, true, true)]
        [InlineData(CertificationPeriodStatus.Current, CertificationPeriodRecertificationStatus.Completed, true, true, false, false)]
        [InlineData(CertificationPeriodStatus.Current, CertificationPeriodRecertificationStatus.Completed, false, true, true, true)]
        [InlineData(CertificationPeriodStatus.Expired, CertificationPeriodRecertificationStatus.BeingAssessed, true, true, true, false)]
        [InlineData(CertificationPeriodStatus.Expired, CertificationPeriodRecertificationStatus.BeingAssessed, false, true, true, true)]
        [InlineData(CertificationPeriodStatus.Expired, CertificationPeriodRecertificationStatus.Completed, true, true, false, false)]
        [InlineData(CertificationPeriodStatus.Expired, CertificationPeriodRecertificationStatus.Completed, false, true, true, true)]
        public void GetCertificationPeriodDetails_WhenCalledAndCertificationPeriodHasBeenSubmitted_ThenCertificationPeriodForCredentialAreLoaded(CertificationPeriodStatus status, CertificationPeriodRecertificationStatus recertificationStatus, bool credentialSubmitted, bool expectedDefault, bool expectedEditable, bool expectedCurrent)
        {
            var credentialId = 1111;
            var naatiNumber = 1234;
            var recertificationPeriodMonths = 3;
            var validPeriodId = 465;
            var certificationPeriodStartDate = new DateTime(2019,5,28).AddDays(-365);
            var certificationPeriodEndDate = new DateTime(2019,5,28).AddDays(365);
            var submittedApplicationId = 45474;

            var period = CreatePeriod(validPeriodId, certificationPeriodStartDate, certificationPeriodEndDate, status);
            var allPeriods = new GetCertificationPeriodsResponse
            {
                Results = new List<CertificationPeriodDto>
                {
                    period,
                    CreatePeriod(2, certificationPeriodEndDate.AddDays(1),certificationPeriodEndDate.AddDays(365), CertificationPeriodStatus.Future)
                }
            };

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Is<GetCertificationPeriodsRequest>(x =>
                    x.NaatiNumber == naatiNumber
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Current)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Future)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Expired)))
                .Returns(allPeriods);

            var logBookService = Substitute.For<ILogbookQueryService>();

            logBookService.GetSubmittedRecertificationApplicationForPeriod(Arg.Is<int>(x => x == validPeriodId))
                .Returns(new RecertificationDto() { ApplicationId = submittedApplicationId });

            logBookService.GetCertificationPeriodCredentials(Arg.Is<int>(x => x == validPeriodId))
                .Returns(new[]
                {
                    CreateCredential(credentialId,certificationPeriodStartDate, certificationPeriodEndDate, 10 )
                });

            logBookService.GetSubmittedRecertificationApplicationCredentials(Arg.Is<int>(x => x == submittedApplicationId && credentialSubmitted))
                .Returns(new[]
                {
                    CreateCredential(credentialId,certificationPeriodStartDate, certificationPeriodEndDate, 10 )
                });

            logBookService.GetCertificationPeriodCredentials(Arg.Is<int>(x => x != validPeriodId))
                .Returns(new CredentialsDetailsDto[0]);

            var systemQueryService = Substitute.For<ISystemQueryService>();
            systemQueryService
                .GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse { Value = recertificationPeriodMonths.ToString() });

            var activityPointsCalculator = Substitute.For<IActivityPointsCalculatorService>();

            var businessLogicService = Substitute.For<IApplicationBusinessLogicService>();
            businessLogicService.CalculateCertificationPeriodRecertificationStatus(Arg.Any<int>())
                .Returns(recertificationStatus);

            var calculator = new NcmsCredentialPointsCalculatorHelper(logBookService, systemQueryService, activityPointsCalculator, personQueryService, businessLogicService);

            var periods = calculator.GetCertificationPeriodDetails(naatiNumber, credentialId).ToList();


            var foundPeriod = periods.First(x => x.Id == validPeriodId);
            Assert.Equal(expectedDefault, foundPeriod.IsDefault);
            Assert.Equal(expectedEditable, foundPeriod.Editable);
            Assert.Equal(expectedCurrent, foundPeriod.IsCurrent);
            Assert.Equal(credentialSubmitted, foundPeriod.IsCredentialSubmitted);
        }


        private WorkPracticeDetails CreateWorkPractice(int id, DateTime date, int points)
        {
            return new WorkPracticeDetails
            {
                Id = id,
                Date = date,
                Points = points
            };
        }

        [Fact]
        public void GetCertificationPeriodCredential_WhenCalledAndCertificationPeriodIsFromATransitionApplication_ThenPDTransitionStartDateIsTakenAsStartDate()
        {
            var credentialId = 1111;
            var naatiNumber = 1234;
            var recertificationPeriodMonths = 3;
            var validPeriodId = 465;
            var certificationPeriodStartDate = new DateTime(2019,5,28).AddDays(-365);
            var certificationPeriodEndDate = new DateTime(2019,5,28).AddDays(365);

            var credential = CreateCredential(credentialId, certificationPeriodStartDate, certificationPeriodEndDate, 10);
            credential.CredentialApplicationTypeCategoryId = (int)CredentialApplicationTypeCategoryName.Transition;

            var period = CreatePeriod(validPeriodId, certificationPeriodStartDate, certificationPeriodEndDate, CertificationPeriodStatus.Current);

            var allPeriods = new GetCertificationPeriodsResponse
            {
                Results = new List<CertificationPeriodDto>
                {
                    period,
                    CreatePeriod(2, certificationPeriodEndDate.AddDays(1),certificationPeriodEndDate.AddDays(365), CertificationPeriodStatus.Future)
                }
            };

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Is<GetCertificationPeriodsRequest>(x =>
                    x.NaatiNumber == naatiNumber
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Current)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Future)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Expired)))
                .Returns(allPeriods);

            var logBookService = Substitute.For<ILogbookQueryService>();
            logBookService.GetCertificationPeriodCredentials(Arg.Is<int>(x => x == validPeriodId)).Returns(new[] { credential });


            logBookService.GetWorkPractices(Arg.Is<int>(x => x == credentialId)).Returns(new List<WorkPracticeDetails>{
                   CreateWorkPractice(1, new DateTime(2019,5,28), 10),
                    CreateWorkPractice(2, new DateTime(2013,12,01), 20),
                    CreateWorkPractice(3, new DateTime(2015,12,01), 20),
                    CreateWorkPractice(4, certificationPeriodEndDate.AddDays(20), 20),
                });

            logBookService.GetCertificationPeriodCredentials(Arg.Is<int>(x => x != validPeriodId))
                .Returns(new CredentialsDetailsDto[0]);

            var systemQueryService = Substitute.For<ISystemQueryService>();
            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse { Value = recertificationPeriodMonths.ToString() });

            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "PDTransitionStartDate"))
                .Returns(new GetSystemValueResponse { Value = new DateTime(2014, 01, 01).ToString(DateTimeFormatInfo.InvariantInfo) });

            var activityPointsCalculator = Substitute.For<IActivityPointsCalculatorService>();

            var businessLogicService = Substitute.For<IApplicationBusinessLogicService>();
            businessLogicService.CalculateCertificationPeriodRecertificationStatus(Arg.Any<int>())
                .Returns(CertificationPeriodRecertificationStatus.None);

            var calculator = new NcmsCredentialPointsCalculatorHelper(logBookService, systemQueryService, activityPointsCalculator, personQueryService, businessLogicService);

            var credentialDetails = calculator.GetCertificationPeriodCredential(naatiNumber, validPeriodId, credentialId);

            Assert.Equal(30, credentialDetails.Points);
            Assert.True(credentialDetails.Calculated);
            Assert.Equal(2, credentialDetails.IncludedWorkPracticeIds.Count());
            Assert.Contains(1, credentialDetails.IncludedWorkPracticeIds);
            Assert.Contains(3, credentialDetails.IncludedWorkPracticeIds);

        }

        [Fact]
        public void GetCertificationPeriodCredential_WhenCalledAndCertificationPeriodIsFromRecertificationApplication_ThenEnteredDateOfRecertificationApplicationIsTakenAsStartDate()
        {
            var credentialId = 1111;
            var naatiNumber = 1234;
            var recertificationPeriodMonths = 3;
            var validPeriodId = 465;
            var certificationPeriodStartDate = new DateTime(2019,5,28).AddDays(-365);
            var certificationPeriodEndDate = new DateTime(2019,5,28).AddDays(365);
            var applicationEnteredDate = certificationPeriodStartDate.AddDays(-20);
            var credential = CreateCredential(credentialId, certificationPeriodStartDate, certificationPeriodEndDate, 10);
            credential.CredentialApplicationTypeCategoryId = (int)CredentialApplicationTypeCategoryName.Recertification;
            credential.CredentialApplicationEnteredDate = applicationEnteredDate;

            var period = CreatePeriod(validPeriodId, certificationPeriodStartDate, certificationPeriodEndDate, CertificationPeriodStatus.Current);

            var allPeriods = new GetCertificationPeriodsResponse
            {
                Results = new List<CertificationPeriodDto>
                {
                    period,
                    CreatePeriod(2, certificationPeriodEndDate.AddDays(1),certificationPeriodEndDate.AddDays(365), CertificationPeriodStatus.Future)
                }
            };

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Is<GetCertificationPeriodsRequest>(x =>
                    x.NaatiNumber == naatiNumber
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Current)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Future)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Expired)))
                .Returns(allPeriods);

            var logBookService = Substitute.For<ILogbookQueryService>();
            logBookService.GetCertificationPeriodCredentials(Arg.Is<int>(x => x == validPeriodId)).Returns(new[] { credential });

            logBookService.GetWorkPractices(Arg.Is<int>(x => x == credentialId)).Returns(new List<WorkPracticeDetails>{
                CreateWorkPractice(1, new DateTime(2019,5,28), 10),
                CreateWorkPractice(2, new DateTime(2013,12,01), 20),
                CreateWorkPractice(3, new DateTime(2015,12,01), 20),
                CreateWorkPractice(4, certificationPeriodEndDate.AddDays(20), 20),
                CreateWorkPractice(5, applicationEnteredDate, 1),
            });

            logBookService.GetCertificationPeriodCredentials(Arg.Is<int>(x => x != validPeriodId))
                .Returns(new CredentialsDetailsDto[0]);

            var systemQueryService = Substitute.For<ISystemQueryService>();
            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse { Value = recertificationPeriodMonths.ToString() });

            var activityPointsCalculator = Substitute.For<IActivityPointsCalculatorService>();

            var businessLogicService = Substitute.For<IApplicationBusinessLogicService>();
            businessLogicService.CalculateCertificationPeriodRecertificationStatus(Arg.Any<int>())
                .Returns(CertificationPeriodRecertificationStatus.None);

            var calculator = new NcmsCredentialPointsCalculatorHelper(logBookService, systemQueryService, activityPointsCalculator, personQueryService, businessLogicService);

            var credentialDetails = calculator.GetCertificationPeriodCredential(naatiNumber, validPeriodId, credentialId);

            Assert.Equal(11, credentialDetails.Points);
            Assert.True(credentialDetails.Calculated);
            Assert.Equal(2, credentialDetails.IncludedWorkPracticeIds.Count());
            Assert.Contains(1, credentialDetails.IncludedWorkPracticeIds);
            Assert.Contains(5, credentialDetails.IncludedWorkPracticeIds);


        }

        [Fact]
        public void GetCertificationPeriodCredential_WhenCalled_ThenStartDateOFTheCredentialIsTakenAsStartDateForPointCalculation()
        {
            var credentialId = 1111;
            var naatiNumber = 1234;
            var recertificationPeriodMonths = 3;
            var validPeriodId = 465;
            var certificationPeriodStartDate = new DateTime(2019,5,28).AddDays(-365);
            var certificationPeriodEndDate = new DateTime(2019,5,28).AddDays(365);
            var credentialStartDate = certificationPeriodStartDate.AddDays(30);
            var credential = CreateCredential(credentialId, credentialStartDate, certificationPeriodEndDate, 10);

            var period = CreatePeriod(validPeriodId, certificationPeriodStartDate, certificationPeriodEndDate, CertificationPeriodStatus.Current);

            var allPeriods = new GetCertificationPeriodsResponse
            {
                Results = new List<CertificationPeriodDto>
                {
                    period,
                    CreatePeriod(2, certificationPeriodEndDate.AddDays(1),certificationPeriodEndDate.AddDays(365), CertificationPeriodStatus.Future)
                }
            };

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Is<GetCertificationPeriodsRequest>(x =>
                    x.NaatiNumber == naatiNumber
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Current)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Future)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Expired)))
                .Returns(allPeriods);

            var logBookService = Substitute.For<ILogbookQueryService>();
            logBookService.GetCertificationPeriodCredentials(Arg.Is<int>(x => x == validPeriodId)).Returns(new[] { credential });

            logBookService.GetWorkPractices(Arg.Is<int>(x => x == credentialId)).Returns(new List<WorkPracticeDetails>{
                CreateWorkPractice(1, new DateTime(2019,5,28), 10),
                CreateWorkPractice(2, certificationPeriodStartDate, 20),
                CreateWorkPractice(3, credentialStartDate, 20),
                CreateWorkPractice(4, certificationPeriodEndDate.AddDays(20), 20),

            });

            logBookService.GetCertificationPeriodCredentials(Arg.Is<int>(x => x != validPeriodId))
                .Returns(new CredentialsDetailsDto[0]);

            var systemQueryService = Substitute.For<ISystemQueryService>();
            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse { Value = recertificationPeriodMonths.ToString() });

            var activityPointsCalculator = Substitute.For<IActivityPointsCalculatorService>();

            var businessLogicService = Substitute.For<IApplicationBusinessLogicService>();
            businessLogicService.CalculateCertificationPeriodRecertificationStatus(Arg.Any<int>())
                .Returns(CertificationPeriodRecertificationStatus.None);

            var calculator = new NcmsCredentialPointsCalculatorHelper(logBookService, systemQueryService, activityPointsCalculator, personQueryService, businessLogicService);

            var credentialDetails = calculator.GetCertificationPeriodCredential(naatiNumber, validPeriodId, credentialId);

            Assert.Equal(30, credentialDetails.Points);
            Assert.True(credentialDetails.Calculated);
            Assert.Equal(2, credentialDetails.IncludedWorkPracticeIds.Count());
            Assert.Contains(1, credentialDetails.IncludedWorkPracticeIds);
            Assert.Contains(3, credentialDetails.IncludedWorkPracticeIds);


        }

        [Fact]
        public void GetCertificationPeriodCredential_WhenCalledAndCertificationPeriodHasASubmittedApplication_ThenAllActivitiesAttachedToTheApplicationAreUsedForPointsCalculation()
        {
            var credentialId = 1111;
            var naatiNumber = 1234;
            var recertificationPeriodMonths = 3;
            var validPeriodId = 465;
            var certificationPeriodStartDate = new DateTime(2019,5,28).AddDays(-365);
            var certificationPeriodEndDate = new DateTime(2019,5,28).AddDays(365);
            var submittedApplicationId = 45474;
            var credential = CreateCredential(credentialId, certificationPeriodStartDate, certificationPeriodEndDate, 10);


            var period = CreatePeriod(validPeriodId, certificationPeriodStartDate, certificationPeriodEndDate, CertificationPeriodStatus.Current);

            var allPeriods = new GetCertificationPeriodsResponse
            {
                Results = new List<CertificationPeriodDto>
                {
                    period,
                    CreatePeriod(2, certificationPeriodEndDate.AddDays(1),certificationPeriodEndDate.AddDays(365), CertificationPeriodStatus.Future)
                }
            };

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Is<GetCertificationPeriodsRequest>(x =>
                    x.NaatiNumber == naatiNumber
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Current)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Future)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Expired)))
                .Returns(allPeriods);

            var logBookService = Substitute.For<ILogbookQueryService>();
            logBookService.GetCertificationPeriodCredentials(Arg.Is<int>(x => x == validPeriodId)).Returns(new[] { credential });

            logBookService.GetSubmittedRecertificationApplicationForPeriod(Arg.Is<int>(x => x == validPeriodId))
                .Returns(new RecertificationDto() { ApplicationId = submittedApplicationId });

            logBookService.GetSubmittedRecertificationApplicationCredentials(Arg.Is<int>(x => x == submittedApplicationId))
                .Returns(new[]
                {
                    credential
                });


            logBookService.GetWorkPracticesForRecertificationApplication(
                    Arg.Is<int>(x => x == credentialId),
                    Arg.Is<int>(x => x == submittedApplicationId))
                .Returns(new List<WorkPracticeDetails>
                {
                    CreateWorkPractice(1, new DateTime(2019,5,28), 10),
                    CreateWorkPractice(2, new DateTime(2013,12,01), 20),
                    CreateWorkPractice(3, new DateTime(2015,12,01), 20),
                    CreateWorkPractice(4, certificationPeriodStartDate.AddDays(20), 20),
                });

            logBookService.GetCertificationPeriodCredentials(Arg.Is<int>(x => x != validPeriodId))
                .Returns(new CredentialsDetailsDto[0]);

            var systemQueryService = Substitute.For<ISystemQueryService>();
            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse { Value = recertificationPeriodMonths.ToString() });

            var activityPointsCalculator = Substitute.For<IActivityPointsCalculatorService>();

            var businessLogicService = Substitute.For<IApplicationBusinessLogicService>();
            businessLogicService.CalculateCertificationPeriodRecertificationStatus(Arg.Any<int>())
                .Returns(CertificationPeriodRecertificationStatus.None);

            var calculator = new NcmsCredentialPointsCalculatorHelper(logBookService, systemQueryService, activityPointsCalculator, personQueryService, businessLogicService);

            var credentialDetails = calculator.GetCertificationPeriodCredential(naatiNumber, validPeriodId, credentialId);

            Assert.Equal(70, credentialDetails.Points);
            Assert.True(credentialDetails.Calculated);
            Assert.Equal(4, credentialDetails.IncludedWorkPracticeIds.Count());
            Assert.Contains(1, credentialDetails.IncludedWorkPracticeIds);
            Assert.Contains(2, credentialDetails.IncludedWorkPracticeIds);
            Assert.Contains(3, credentialDetails.IncludedWorkPracticeIds);
            Assert.Contains(4, credentialDetails.IncludedWorkPracticeIds);
        }

        [Theory]
        [MemberData(nameof(ProperRequirementCalculationData))]
        public void GetCertificationPeriodCredential_WhenCalled_ProperRequirementIsCalculated(DateTime periodStartDate, DateTime periodEndDate, DateTime credentialStartDate, int requiredPointsPerYear, int expectedRequirement)
        {
            var credentialId = 1111;
            var naatiNumber = 1234;
            var recertificationPeriodMonths = 3;
            var validPeriodId = 465;
            var certificationPeriodStartDate = periodStartDate;
            var certificationPeriodEndDate = periodEndDate;
            var credential = CreateCredential(credentialId, credentialStartDate, certificationPeriodEndDate, requiredPointsPerYear);

            var period = CreatePeriod(validPeriodId, certificationPeriodStartDate, certificationPeriodEndDate, CertificationPeriodStatus.Current);

            var allPeriods = new GetCertificationPeriodsResponse
            {
                Results = new List<CertificationPeriodDto>
                {
                    period,
                    CreatePeriod(2, certificationPeriodEndDate.AddDays(1),certificationPeriodEndDate.AddDays(365), CertificationPeriodStatus.Future)
                }
            };

            var personQueryService = Substitute.For<IPersonQueryService>();
            personQueryService.GetCertificationPeriods(Arg.Is<GetCertificationPeriodsRequest>(x =>
                    x.NaatiNumber == naatiNumber
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Current)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Future)
                    && x.CertificationPeriodStatus.Contains(CertificationPeriodStatus.Expired)))
                .Returns(allPeriods);

            var logBookService = Substitute.For<ILogbookQueryService>();
            logBookService.GetCertificationPeriodCredentials(Arg.Is<int>(x => x == validPeriodId)).Returns(new[] { credential });

            logBookService.GetWorkPractices(Arg.Is<int>(x => x == credentialId)).Returns(new List<WorkPracticeDetails>{
                CreateWorkPractice(2, certificationPeriodStartDate, 3),
                CreateWorkPractice(3, credentialStartDate, 4),
                CreateWorkPractice(1, certificationPeriodEndDate, 5),
                CreateWorkPractice(4, certificationPeriodEndDate.AddDays(20), 6),
            });

            logBookService.GetCertificationPeriodCredentials(Arg.Is<int>(x => x != validPeriodId))
                .Returns(new CredentialsDetailsDto[0]);

            var systemQueryService = Substitute.For<ISystemQueryService>();
            systemQueryService.GetSystemValue(Arg.Is<GetSystemValueRequest>(x => x.ValueKey == "CertificationPeriodRecertifyMonths"))
                .Returns(new GetSystemValueResponse { Value = recertificationPeriodMonths.ToString() });

            var activityPointsCalculator = Substitute.For<IActivityPointsCalculatorService>();

            var businessLogicService = Substitute.For<IApplicationBusinessLogicService>();
            businessLogicService.CalculateCertificationPeriodRecertificationStatus(Arg.Any<int>())
                .Returns(CertificationPeriodRecertificationStatus.None);

            var calculator = new NcmsCredentialPointsCalculatorHelper(logBookService, systemQueryService, activityPointsCalculator, personQueryService, businessLogicService);

            var credentialDetails = calculator.GetCertificationPeriodCredential(naatiNumber, validPeriodId, credentialId);

            Assert.Equal(expectedRequirement, credentialDetails.Requirement);
            Assert.True(credentialDetails.Calculated);
        }

        public static readonly object[][] ProperRequirementCalculationData =
        {
            new object[] {
                new DateTime(2018, 05, 05),
                new DateTime(2019, 05, 05),
                new DateTime(2018, 05, 05),
                1,
                1,
            },
            new object[]
            {
                new DateTime(2017, 05, 05),
                new DateTime(2019, 05, 05),
                new DateTime(2017, 05, 05),
                2,
                4,
            },
            new object[]
            {
                new DateTime(2017, 05, 05),
                new DateTime(2019, 05, 05),
                new DateTime(2018, 05, 05),
                3,
                3,
            },
            new object[]
            {
                new DateTime(2017, 05, 05),
                new DateTime(2019, 05, 05),
                new DateTime(2018, 05, 06),
                4,
                0,
            },
            new object[]
            {
                new DateTime(2017, 05, 05),
                new DateTime(2020, 05, 05),
                new DateTime(2018, 05, 05),
                5,
                5,
            },
            new object[]
            {
                new DateTime(2019, 05, 05),
                new DateTime(2022, 05, 05),
                new DateTime(2019, 05, 05),
                6,
                12,
            },
            new object[]
            {
                new DateTime(2019, 05, 05),
                new DateTime(2022, 05, 05),
                new DateTime(2019, 06, 05),
                7,
                7,
            },
            new object[]
            {
                new DateTime(2019, 05, 05),
                new DateTime(2022, 05, 05),
                new DateTime(2020, 04, 05),
                8,
                8,
            },
            new object[]
            {
                new DateTime(2020, 05, 05),
                new DateTime(2023, 05, 05),
                new DateTime(2020, 05, 05),
                9,
                18,
            },
            new object[]
            {
                new DateTime(2020, 05, 05),
                new DateTime(2023, 05, 05),
                new DateTime(2020, 06, 05),
                10,
                10,
            },
            new object[]
            {
                new DateTime(2017, 05, 05),
                new DateTime(2020, 05, 05),
                new DateTime(2018, 06, 05),
                11,
                0,
            },
            new object[]
            {
                new DateTime(2019, 05, 05),
                new DateTime(2022, 05, 05),
                new DateTime(2019, 05, 05),
                12,
                24,
            },
            new object[]
            {
                new DateTime(2019, 05, 05),
                new DateTime(2022, 05, 05),
                new DateTime(2019, 06, 06),
                13,
                13,
            },
            new object[]
            {
                new DateTime(2020, 05, 05),
                new DateTime(2023, 05, 05),
                new DateTime(2021, 01, 01),
                14,
                14,
            }
        };
    }
}
