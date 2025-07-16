using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Person;
using NSubstitute;
using Xunit;

namespace Ncms.Test.BusinessServices
{
    public class TokenReplacementServiceTests
    {
        [Theory]
        [InlineData("My Naati No is [[Naati No]]. Short date is [[Today Short]]. Long date is [[Today Long]]")]
        public void ReplaceTemplateFieldValues_ValidData_ReplacedToken(string emailContent)
        {
            var personService = Substitute.For<IPersonService>();
            var applicationQueryService = Substitute.For<IApplicationQueryService>();
            var applicationService = Substitute.For<IApplicationService>();

            var personDetails = new GetPersonDetailsResponse { Results = new List<PersonEntityDto> { new PersonEntityDto {Title = "Mr", NaatiNumber = 123}}};
            var contactDetails = new GetContactDetailsResponse { Addresses = new List<AddressDetailsDto> { new AddressDetailsDto { AddressId = 789}}};
            var applicationDetails = new GetApplicationDetailsResponse { ApplicantDetails = new PersonDetailsBasicDto { NaatiNumber = 123}};

            var tokenReplacementService = new TokenReplacementService();

            personService.GetPerson(Arg.Any<int>()).Returns(new GenericResponse<PersonModel>() { Data = new PersonModel() { NaatiNumber = 123 } });

            var applicationDetailsResponse = applicationService.GetApplicationDetailsByApplicationId(Arg.Any<GetApplicationDetailsRequest>()).Returns(new GetApplicationDetailsResponse
            {
                ApplicantDetails = new PersonDetailsBasicDto { NaatiNumber = 123, PersonId = 321, EntityId = 987}
            });

            var personInfoResponse = personService.GetPersonInfoResponse(Arg.Any<int>()).Returns(
                new GetPersonAndContactDetailsResponse
                {
                    PersonDetails = personDetails,
                    ContactDetails = contactDetails
                });

            applicationQueryService.GetApplicationDetails(Arg.Any<GetApplicationDetailsRequest>())
                .Returns(new GetApplicationDetailsResponse
                {
                    ApplicantDetails = new PersonDetailsBasicDto { NaatiNumber = 123 }
                });

            var todayShort = DateTime.Now.ToShortDateString();
            var todayLong = DateTime.Now.ToLongDateString();
            var valueToReplace = emailContent;
            IEnumerable<string> errors;
            Action<string, string> defaultReplacementAction = (token, value) => valueToReplace = valueToReplace.Replace(token, value ?? string.Empty);

            tokenReplacementService.ReplaceTemplateFieldValues(emailContent, applicationDetails, personDetails, contactDetails, defaultReplacementAction, new Dictionary<string, string>(), false, out errors);

            Assert.Equal("My Naati No is 123. Short date is " + todayShort + ". Long date is " + todayLong, valueToReplace);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("My Naati No is [[NaatiNo]]", "My Naati No is [[NaatiNo]]")]
        public void ReplaceTemplateFieldValues_InvalidData_LogException(string emailContent, string expectedResult)
        {
            var personService = Substitute.For<IPersonService>();
            var applicationQueryService = Substitute.For<IApplicationQueryService>();

            var personDetails = new GetPersonDetailsResponse { Results = new List<PersonEntityDto> { new PersonEntityDto { Title = "Mr", NaatiNumber = 123 } } };
            var contactDetails = new GetContactDetailsResponse { Addresses = new List<AddressDetailsDto> { new AddressDetailsDto { AddressId = 789 } } };
            var applicationDetails = new GetApplicationDetailsResponse { ApplicantDetails = new PersonDetailsBasicDto { NaatiNumber = 123 } };

            var tokenReplacementService = new TokenReplacementService();

            personService.GetPerson(Arg.Any<int>())
                .Returns(new GenericResponse<PersonModel>() { Data = new PersonModel() { NaatiNumber = 123 } });
            applicationQueryService.GetApplicationDetails(Arg.Any<GetApplicationDetailsRequest>())
                .Returns(new GetApplicationDetailsResponse
                {
                    ApplicantDetails = new PersonDetailsBasicDto { NaatiNumber = 123 }
                });

            var valueToReplace = emailContent;
            IEnumerable<string> errors;
            Action<string, string> defaultReplacementAction = (token, value) => valueToReplace = valueToReplace.Replace(token, value ?? string.Empty);
            tokenReplacementService.ReplaceTemplateFieldValues(emailContent, applicationDetails, personDetails, contactDetails, defaultReplacementAction, new Dictionary<string, string>(), false, out errors);

            Assert.Equal(expectedResult, valueToReplace);
            Assert.True(errors.Any());
        }

    }
}
