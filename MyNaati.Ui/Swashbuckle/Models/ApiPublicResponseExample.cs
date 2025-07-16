using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using MyNaati.Contracts.BackOffice;
using Swashbuckle.Examples;

namespace MyNaati.Ui.Swashbuckle.Models
{
    public class ApiPublicPractitionerCountResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ApiPublicPractitionerCountResponse
            {
                ByCredentialTypeId = new []{ new ItemCountValueDto { Id = 0, Count = 7 }, new ItemCountValueDto { Id = 2, Count = 7 }},
                ByCountryId = new[] { new ItemCountValueDto { Id = 0, Count = 7 }, new ItemCountValueDto { Id = 13, Count = 7 } },
                ByStateId = new[] { new ItemCountValueDto { Id = 0, Count = 7 }, new ItemCountValueDto { Id = 5, Count = 7 } },
                TotalPractitinerCount = 7,
                Message = null,
                ErrorCode = 0
            };
        }
    }

    public class ApiPublicPractitionerSearchResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ApiPublicPractitionerSearchResponse
            {
                Results = new []
                {
                    new ApiPublicPractitionerSearchDto
                    {
                        PersonId = 200216,
                        Surname = "Fizefozeni",
                        GivenName = "Alicia",
                        OtherNames = "",
                        Title = "Mrs",
                        CredentialTypes = new [] {new ApiPubicPractitionerCredentialTypeDto{ExternalName = "Certified Interpreter", Skill = "Serbian and English"},new ApiPubicPractitionerCredentialTypeDto {ExternalName = "Certified Interpreter", Skill = "Croatian and English"}},
                        Address = new ApiPublicPractitionerAddressDto{Postcode = "5049", State = "South Australia", StreetDetails = "", Country = "Australia", Suburb = "SEACLIFF PARK"},
                        ContactDetails = new [] {new ApiPublicContactDetailsDto {Type = "Phone",Contact = "62457999"}, new ApiPublicContactDetailsDto { Type = "Email", Contact = "person196347@altf4solutions.com.au"} }
                    },
                },
                Message = null,
                ErrorCode = 0
            };
        }
    }

    public class PublicLookupResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new PublicLookupResponse
            {
                Results = new[]
                {
                   new LookupDto{Id = 3,DisplayName = "Certified Advanced Translator"}, new LookupDto{Id = 4,DisplayName = "Certified Advanced Translator"}, new LookupDto{Id = 10,DisplayName = "Certified Conference Interpreter"},
                   new LookupDto{Id = 11,DisplayName = "Certified Advanced Interpreter"},new LookupDto{Id = 24,DisplayName = "Certified Advanced Interpreter"}, new LookupDto{Id = 21,DisplayName = "Certified Interpreter"},
                   new LookupDto{Id = 27,DisplayName = "Certified Interpreter"},new LookupDto{Id = 7,DisplayName = "Certified Interpreter"},
                },
                Message = null,
                ErrorCode = 0
            };
        }
    }

    public class LanguagesResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new LanguagesResponse
            {
                Results = new[]
                {
                   new ApiLanguage{ DisplayName = "Aceh and English", SkillIds = new [] {213}}, new ApiLanguage{ DisplayName = "Acholi and English", SkillIds = new [] {214}}, new ApiLanguage{ DisplayName = "Afrikaans and English", SkillIds = new [] {215}},
                   new ApiLanguage{ DisplayName = "Albanian and English", SkillIds = new [] {216,415,538}}, new ApiLanguage{ DisplayName = "Alyawarr and English", SkillIds = new [] {217,416,1024}}, new ApiLanguage{ DisplayName = "American Sign Language and English", SkillIds = new [] {218}}

                },
                Message = null,
                ErrorCode = 0
            };
        }
    }

    public class PublicLegacyAccreditionsResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new PublicLegacyAccreditionsResponse
            {
                Results = new[]
                {
                   new PublicAccreditationLegacy{Level = "Professional", Category = "Interpreter", Direction = "German to/from English", StartDate = "30-01-1985", EndDate = ""},
                   new PublicAccreditationLegacy{Level = "Professional", Category = "Translator", Direction = "German to English", StartDate = "30-01-1981", EndDate = ""},
                   new PublicAccreditationLegacy{Level = "Professional", Category = "Translator", Direction = "English to German", StartDate = "30-01-1981", EndDate = ""}
                },
                Message = null,
                ErrorCode = 0
            };
        }
    }

    public class ApiTestSessionSearchResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ApiTestSessionSearchResponse
            {
                Results = new[]
                {
                   new ApiTestSessionContract{TestDateTime = "09-10-2019 09:0:00 AM", Venue = "NAATI VIC Office", TestSessionId = 1436, CredentialTypeId = 2, CredentialType = "Certified Translator", Capacity = 6, Completed = false, TestLocationName = "Melbourne", TestLocationStateName = "VIC", SessionName = "VIC - CTE - Chinese, Spanish",DurationInMinute = 210},
                   new ApiTestSessionContract{TestDateTime = "10-10-2019 09:0:00 AM", Venue = "Karstens", TestSessionId = 1437, CredentialTypeId = 2, CredentialType = "Certified Translator", Capacity = 16, Completed = false, TestLocationName = "Melbourne", TestLocationStateName = "VIC", SessionName = "VIC - CTO - Chinese, Spanish", DurationInMinute = 210},
                   new ApiTestSessionContract{TestDateTime = "01-10-2019 09:0:00 AM", Venue = "The Portside Centre, Level 5, Symantec House", TestSessionId = 1436, CredentialTypeId = 2, CredentialType = "Certified Translator", Capacity = 6, Completed = false, TestLocationName = "Melbourne", TestLocationStateName = "VIC", SessionName = "NSW - CTE - Spanish/Chinese", DurationInMinute = 210},
                },
                Message = null,
                ErrorCode = 0
            };
        }
    }

    public class ApiEndorsedQualificationResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ApiEndorsedQualificationResponse
            {
                Results = new[]
                {
                    new ApiEndorsedQualificationContract{InstitutionId = 22185,InstitutionName = "Abbey College", Location = "Sydney, NSW", Qualification = "Advanced Diploma of Translation (PSP60816)", CredentialTypeId = 2,CredentialType = "Certified Translator", EndorsementPeriodFrom = "01-01-2018", EndorsementPeriodTo = "31-12-2020", Active = true},
                    new ApiEndorsedQualificationContract{InstitutionId = 22185,InstitutionName = "Abbey College", Location = "Sydney, NSW", Qualification = "Diploma of Interpreting (PSP50916)", CredentialTypeId = 6,CredentialType = "Certified Provisional Interpreter", EndorsementPeriodFrom = "01-01-2018", EndorsementPeriodTo = "31-12-2020", Active = true},
                },
                Message = null,
                ErrorCode = 0
            };
        }
    }

    public class ApiTestSessionAvailabilityResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ApiTestSessionAvailabilityResponse
            {
                Results = new[]
                {
                    new ApiTestSessionAvailabilityContract{TestSessionId = 1077,Name = "NSW CCL KOR SPA FRE IND 02122019", TestDateTime = "02-12-2019 01:00 PM", DurationInMinute = 180, TestLocationId = 1, TestLocation = "NSW - Sydney", Venue = "The Portside Centre, Level 5, Symantec House", VenueAddress = "207 kent Street, Sydney NSW 2000", AvailableSeats = 3, IsPreferredLocation = true},
                    new ApiTestSessionAvailabilityContract{TestSessionId = 1081,Name = "TAS CCL KOR SPA FRE IND 02122019", TestDateTime = "02-12-2019 01:00 PM", DurationInMinute = 180, TestLocationId = 5, TestLocation = "TAS - Hobart", Venue = "NAATI TAS Office", VenueAddress = "McDougall Building, 9 Ellerslie Road, Battery Point TAS 7004", AvailableSeats = 2, IsPreferredLocation = true},
                    new ApiTestSessionAvailabilityContract{TestSessionId = 1079,Name = "SA CCL KOR SPA FRE IND 02122019", TestDateTime = "02-12-2019 01:30 PM", DurationInMinute = 180, TestLocationId = 6, TestLocation = "SA - Adelaide", Venue = "Adelaide Meeting Room Hire", VenueAddress = "97 Pirie Street, Adelaide SA 5000", AvailableSeats = 1, IsPreferredLocation = true},
                    new ApiTestSessionAvailabilityContract{TestSessionId = 1792,Name = "CCL ACT FRE MAL TUR 2201 AM", TestDateTime = "22-01-2020 09:00 AM", DurationInMinute = 180, TestLocationId = 2, TestLocation = "ACT - Canberra", Venue = "Dialogue", VenueAddress = "4 National Circuit, Barton ACT 2600", AvailableSeats = 29, IsPreferredLocation = true}
                },
                Message = null,
                ErrorCode = 0
            };
        }
    }

    public class ApiPublicCertificationsResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ApiPublicCertificationsResponse
            {
               Practitioner = new ApiPublicPractitioner
               {
                   PractitionerId = "CPN6PD01C",
                   GivenName = "Yalcin",
                   FamilyName = "Thtofitoni",
                   Country = "Turkey"
               },
               CurrentCertifications = new []{new ApiPublicCredentials{CertificationType = "Certified Translator", Skill = "Turkish into English", StartDate = "01-01-2018", EndDate = "01-11-2020"}, new ApiPublicCredentials { CertificationType = "Certified Translator", Skill = "English into Turkish", StartDate = "01-01-2018", EndDate = "01-11-2020" }, new ApiPublicCredentials { CertificationType = "Certified Interpreter", Skill = "Turkish and English", StartDate = "01-01-2018", EndDate = "01-11-2020" } },
               PreviousCertifications = new []{new ApiPublicCredentials{CertificationType = "Certified Provisional Interpreter", Skill = "Turkish into English", StartDate = "01-01-2018", EndDate = "10-01-2018"} },
               Message = null,
               ErrorCode = 0
            };
        }
    }

    public class ApiPersonImageResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ApiPersonImageSwaggerResponse
            {
                PersonImageData = "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",
                ErrorCode = 0,
                Message = null
            };
        }
    }

    public class ApiPersonImageSwaggerResponse :BaseResponse
    {
        public string PersonImageData { get; set; }
    }

    public class ErrorMessageResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ErrorResponse { ErrorCode = 0, Message = "No error occurred." };
        }
    }

    public class ErrorResponse
    {
        public int ErrorCode { get; set; }

        public string Message { get; set; }
    }

}