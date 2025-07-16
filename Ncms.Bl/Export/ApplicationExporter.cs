using System;
using System.Collections.Generic;
using System.Linq;
using Ncms.Contracts.Models.Application;

namespace Ncms.Bl.Export
{
    public class ApplicationSummaryModel
    {
        public string SummaryItem { get; set; }
        public string ApplicationCount { get; set; }
        public string CredentialRequestCount { get; set; }
    }

    public enum CredentialType
    {
        RecognisedPractisingTranslator = 1,
        CertifiedTranslator = 2,
        CertifiedAdvancedTranslator = 3,
        CertifiedAdvancedTranslatorLOTEtoLOTE = 4,
        RecognisedPractisingInterpreter = 5,
        CertifiedProvisionalInterpreter = 6,
        CertifiedInterpreter = 7,
        CertifiedSpecialistInterpreter = 8,
        CertifiedConferenceInterpreter = 9,
        CertifiedConferenceInterpreterLOTEtoLOTE = 10,
        RecognisedPractisingDeafInterpreter = 11,
        CertifiedProvisionalDeafInterpreter = 12,
        CCL = 13,
        CLAS = 14,
        Ethics = 15,
        Intercultural = 16
    }

    public enum CredentialRequestStatusType
    {
        Draft = 1,
        Rejected = 2,
        RequestEntered = 3,
        ReadyForAssessment = 4,
        BeingAssessed = 5,
        Pending = 6,
        AssessmentFailed = 7,
        AssessmentPaidReview = 8,
        AssessmentComplete = 9,
        EligibleForTesting = 10,
        TestFailed = 11,
        CertificationIssued = 12,
        Canceled = 13,
        TestAccepted = 14
    }

    public class ApplicationExporter : SearchResultsExporter
    {
        public ApplicationExporter(ApplicationRequest searchRequest,
            IEnumerable<ApplicationModel> applications,
            IEnumerable<Tuple<ApplicationModel,CredentialRequestModel>> credentialRequests,
            IEnumerable<ApplicationSummaryModel> summary)
        {
            if (searchRequest == null)
            {
                throw new ArgumentNullException(nameof(searchRequest));
            }

            if (applications == null)
            {
                throw new ArgumentNullException(nameof(applications));
            }

            if (credentialRequests == null)
            {
                throw new ArgumentNullException(nameof(credentialRequests));
            }

            _searchRequest = searchRequest;
            _applications = applications;
            _credentialRequests = credentialRequests;
            _summary = summary;
        }

        private readonly IEnumerable<ApplicationModel> _applications;
        private readonly IEnumerable<Tuple<ApplicationModel, CredentialRequestModel>> _credentialRequests;
        private readonly IEnumerable<ApplicationSummaryModel> _summary;
        private readonly ApplicationRequest _searchRequest;
        private object[][][] _data;
        private string[] _criteria;
        private const string AbsentCriterionValue = "All";

        private static string FormatStringList(string[] values)
        {
            var list = AbsentCriterionValue;
            if (values != null && values.Any())
            {
                list = string.Join(", ", values);
            }

            return list;
        }

        protected override string TemplateFileName => "ApplicationExport.xltx";

        protected override string[] Criteria => _criteria ?? (_criteria = new List<string>
        {
            _searchRequest.NaatiNumberIntList != null ? $"NAATI Number(s): {FormatStringList(_searchRequest.NaatiNumberIntList.Select(x => x.ToString()).ToArray())}" : "NAATI Number(s):",
            $"Application Reference: {_searchRequest.ApplicationReferenceString}",
            $"Application Owner: {FormatStringList(_searchRequest.ApplicationOwnerIntList)}",
            $"Name: {_searchRequest.PersonNameString}",
            $"Contact No: {_searchRequest.PhoneNumberString}",
            $"Active Application: {_searchRequest.ActiveApplicationBoolean}",
            $"Application Type: {FormatStringList(_searchRequest.ApplicationTypeIntList)}",
            $"Credential Request Type: {FormatStringList(_searchRequest.CredentialRequestTypeIntList)}",
            $"Application Status: {FormatStringList(_searchRequest.ApplicationStatusIntList)}",
            $"Credential Request Status: {FormatStringList(_searchRequest.CredentialRequestStatusIntList)}"
        }.ToArray());

        protected override object[][][] Data => _data ?? (_data = new[]
        {
            _applications.Select(x => new object[]
            {
                x.Reference,
                x.Type,
                x.Status,
                x.StatusDate,
                x.NaatiNumber,
                x.Name,
                x.ContactNumber,
                x.Owner
            }).ToArray(),

            _credentialRequests.Select(x => new object[]
            {
                x.Item2.Category,
                x.Item2.CredentialName,
                x.Item2.Direction,
                x.Item2.Status,
                x.Item2.StatusChangeDate,
                x.Item1.Reference,
                x.Item1.Type,
                x.Item1.Status,
                x.Item1.StatusDate,
                x.Item1.NaatiNumber,
                x.Item1.Name,
                x.Item1.ContactNumber,
                x.Item1.Owner
            }).ToArray(),

            _summary.Select(x => new object[]
            {
                x.SummaryItem,
                x.ApplicationCount,
                x.CredentialRequestCount
            }).ToArray()
        });
    }
}
