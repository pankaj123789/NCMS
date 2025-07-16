using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Response;

namespace F1Solutions.Naati.Common.Contracts.Dal.Services
{
    public interface ITokenReplacementService
    {
        string ReplaceTemplateFieldValues(string content, GetApplicationDetailsResponse applicationDetails, GetPersonDetailsResponse personDetails, GetContactDetailsResponse personContactDetails, IDictionary<string, string> extraTokens, bool logExceptions, out IEnumerable<string> errors);

        void ReplaceTemplateFieldValues(string content, GetApplicationDetailsResponse applicationDetails, GetPersonDetailsResponse personDetails, GetContactDetailsResponse personContactDetails, Action<string, string> replacementAction, IDictionary<string, string> extraTokens, bool logExceptions, out IEnumerable<string> errors);

        void ReplaceTemplateFieldValues(string content, Action<string, string> replacementAction, IDictionary<string, string> extraTokens, bool logExceptions, out IEnumerable<string> errors);

        string GetTokenNameFor(TokenReplacementField token);

        int TotalTokens(string content);
    }

    public class DocumentTokenReplacement
    {
        public string ApplicationReference { get; set; }
        public string CredentialRequestType { get; set; }
        public string Skill { get; set; }
        public string VenueName { get; set; }
        public string TestTessionId { get; set; }
        public string StrTestTessionDate { get; set; }
        public string StrTestTessionTime { get; set; }
        public string AttendanceId { get; set; }
        public string TestSessionName { get; set; }
        public string TestSpecificationId { get; set; }
    }

    public class MergedDocument
    {
        public int? TestMaterialId { get; set; }
        public int? TestSpecificationId { get; set; }
        public bool IsTestSpecification { get; set; }
        public string TaskTypeLabel { get; set; }
        public int TestComponentNumber { get; set; }
        public string Label { get; set; }

        public StoredFileMarterialDto StoredFile { get; set; }
    }

    public class DownloadAttendeeTestMaterial
    {
        public int? TestMaterialId { get; set; }
        public int? TestSpecificationId { get; set; }
        public bool IsTestSpecification { get; set; }

        public List<StoredFileMarterialDto> StoredFileList { get; set; }
        public string TaskTypeLabel { get; set; }
        public int TestComponentNumber { get; set; }
        public string Label { get; set; }
    }
}
