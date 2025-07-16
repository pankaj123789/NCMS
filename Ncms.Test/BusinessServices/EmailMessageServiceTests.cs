using System;
using System.Collections.Generic;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Bl.AutoMappingProfiles;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Bl;
using Ncms.Bl.AutoMappingProfiles;
using Ncms.Contracts;
using Ncms.Test.AutoMappingProfiles;
using Ncms.Test.Utils;
using NSubstitute;
using Xunit;

namespace Ncms.Test.BusinessServices
{
    public class EmailMessageServiceTests 
    {
       

        public static IEnumerable<object[]> CreateEmailMessageInvalidData
            => new[]
            {
                new object[]
                {
                    new CredentialApplicationEmailMessageModel() {CreatedUserId = 1, RecipientEntityId = 0, Subject = "New Created 1"},
                    new EmailMessageResponse { Data =  new EmailMessageDto { EmailMessageId = 1, Subject = "New Created 1"}}
                },
                new object[]
                {
                    new CredentialApplicationEmailMessageModel() {CreatedUserId = 0, RecipientEntityId = 1, Subject = "New Created 1"},
                    new EmailMessageResponse { Data =  new EmailMessageDto { EmailMessageId = 1, Subject = "New Created 1"}}
                },
                new object[]
                {
                    new CredentialApplicationEmailMessageModel() {CreatedUserId = -1, RecipientEntityId = 1, Subject = "New Created 1"},
                    new EmailMessageResponse { Data =  new EmailMessageDto { EmailMessageId = 1, Subject = "New Created 1"}}
                },
                new object[]
                {
                    new CredentialApplicationEmailMessageModel() {CreatedUserId = 1, RecipientEntityId = -1, Subject = "New Created 1"},
                    new EmailMessageResponse() { Data =  new EmailMessageDto { EmailMessageId = 1, Subject = "New Created 1"}}
                }
            };


        [Fact]
        public void CreateEmailMessage_ValidData_SuccessfulCreated()
        {

            //arrange

            var assemblies = new[]
            {
                typeof(AccountingProfile).Assembly,
                typeof(PodHistoryProfile).Assembly,
            };

            var autoMapperHelper = new AutoMapperHelper();
            autoMapperHelper.Configure(assemblies, true);

            var serviceLocator = ServiceLocatorTestHelper.GetInstance();
            serviceLocator.MockService<IAutoMapperHelper>(autoMapperHelper);
            var emailMessageQueryService = Substitute.For<IEmailMessageQueryService>();
            var personService = Substitute.For<IPersonService>();
            emailMessageQueryService.CreateGenericEmailMessage(Arg.Any<GenericEmailMessageRequest>())
                .Returns(new EmailMessageResponse() { Data = new EmailMessageDto { EmailMessageId = 1, Subject = "New Created" } });

            //act
            var emailMessageService = new EmailMessageService(emailMessageQueryService, personService, autoMapperHelper);
            var result =
                emailMessageService.CreateGenericEmailMessage(new EmailMessageModel()
                {
                    CreatedUserId = 1,
                    RecipientEntityId = 1,
                    Subject = "New Created"
                });

            //assert
            Assert.Equal("New Created", result.Data.Subject);
            Assert.True(result.Data.EmailMessageId > 0);

        }

        [Theory]
        [MemberData(nameof(CreateEmailMessageInvalidData))]
        public void CreateEmailMessage_InValidData_ThrowException(CredentialApplicationEmailMessageModel request, EmailMessageResponse response)
        {
            //arrange
            var channel = Substitute.For<IEmailMessageQueryService>();
            var personService = Substitute.For<IPersonService>();
            channel.CreateGenericEmailMessage(Arg.Any<GenericEmailMessageRequest>()).Returns(response);


            var assemblies = new[]
            {
                typeof(AccountingProfile).Assembly,
            
                typeof(PodHistoryProfile).Assembly,
            };
            var autoMapperHelper = new AutoMapperHelper();
            autoMapperHelper.Configure(assemblies, true);
         
            //act
            var emailMessageService = new EmailMessageService(channel, personService, autoMapperHelper);

            //assert
            Assert.Throws<ArgumentOutOfRangeException>(() => emailMessageService.CreateEmailMessage(request));
        }


        public static IEnumerable<object[]> SendEmailMessageData
            => new[]
            {
                new object[]
                {
                    new EmailMessageResponse { Data = new EmailMessageDto {RecipientEmail = "test1@f1solutions.com.au"}},
                    new GetPersonDetailsBasicResponse()
                    {
                        PersonDetails = new PersonDetailsBasicDto() {PrimaryEmail = "test1@f1solutions.com.au"}
                    }, false
                },
               new object[]
                {
                    new EmailMessageResponse { Data = new EmailMessageDto {RecipientEmail = "test1@f1solutions.com.au"}},
                    new GetPersonDetailsBasicResponse()
                    {
                        PersonDetails = new PersonDetailsBasicDto() {PrimaryEmail = "test2@f1solutions.com.au"}
                    }, true
                }
            };


        [Theory]
        [MemberData(nameof(SendEmailMessageData))]
        public void SendEmailMessage_SuccessfulSent(EmailMessageResponse response, GetPersonDetailsBasicResponse person, bool expectedResult)
        {
            //arrange

            var assemblies = new[]
            {
                typeof(AccountingProfile).Assembly,
                typeof(PodHistoryProfile).Assembly,
            };

            var autoMapperHelper = new AutoMapperHelper();
            autoMapperHelper.Configure(assemblies, true);

            var serviceLocator = ServiceLocatorTestHelper.GetInstance();
            serviceLocator.MockService<IAutoMapperHelper>(autoMapperHelper);
            var sendEmailWithoutChangingRecipientEmailCount = false;
            var emailMessageQueryService = Substitute.For<IEmailMessageQueryService>();
            var personService = Substitute.For<IPersonService>();

            emailMessageQueryService.SendMailById(Arg.Any<int>(), EmailSendStatusTypeName.Retry).Returns(new ServiceResponse());
            emailMessageQueryService.SendMail(Arg.Any<GenericEmailMessageRequest>()).Returns(new ServiceResponse());

            emailMessageQueryService.When(x => x.SendMail(Arg.Any<GenericEmailMessageRequest>()))
                .Do(x => sendEmailWithoutChangingRecipientEmailCount = true);
            emailMessageQueryService.When(x => x.SendMailById(Arg.Any<int>(), EmailSendStatusTypeName.Retry))
             .Do(x => sendEmailWithoutChangingRecipientEmailCount = false);

            emailMessageQueryService.GetEmailMessage(Arg.Any<int>()).Returns(response);
            personService.GetPersonDetailsByEntityId(Arg.Any<int>()).Returns(person);

          
            //act
            var emailMessageService = new EmailMessageService(emailMessageQueryService, personService, autoMapperHelper);
            emailMessageService.SendEmailMessage(3);

            //assert
            Assert.Equal(expectedResult, sendEmailWithoutChangingRecipientEmailCount);

        }
    }
}
