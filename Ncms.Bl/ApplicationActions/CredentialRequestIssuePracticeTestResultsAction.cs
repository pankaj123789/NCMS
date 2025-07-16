using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncms.Bl.ApplicationActions
{
    public class CredentialRequestIssuePracticeTestResultsAction : CredentialRequestStateAction
    {
        protected override CredentialRequestStatusTypeName[] CredentialRequestEntryStates => new[] { CredentialRequestStatusTypeName.TestSat };

        protected override CredentialRequestStatusTypeName CredentialRequestExitState => CredentialRequestStatusTypeName.IssuePracticeTestResults;
        protected override CredentialApplicationStatusTypeName ApplicationExitState => CredentialApplicationStatusTypeName.Completed;
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.TestResult;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Issue;


        protected override TestResultStatusTypeName RequiredTestResultStatus => TestResultStatusTypeName.IssuePracticeTestResults;
        protected override IList<Action> Preconditions => new List<Action>
        {
            ValidateEntryState,
            ValidateUserPermissions,
            ValidateMinimumCredentialRequests,
            ValidateMandatoryFields,
            ValidateMandatoryPersonFields,
            ValidateMandatoryDocuments,
            ValidateAllowIssue,
            ValidateTestResultStatus,
            ValidateStandardMarks
        };

        protected override IList<Action> SystemActions => new List<Action>
        {
            ClearOwner,
            CreateNote,
            SetExitState,
            SetApplicationStatus
        };

        protected override void ValidateStandardMarks()
        {
            if (TestSpecification.MarkingSchemaTypeId != (int)MarkingSchemaTypeName.Standard)
            {
                return;
            }
        }

        protected override void GetEmailTokens(Dictionary<string, string> tokenDictionary)
        {
            GetStandardTestResultEmailTokens(tokenDictionary);

            GetRubricTestResultEmailTokens(tokenDictionary);

            GetSpecificPracticeTestCclEmailTokens(tokenDictionary, IsCclPracticeTest());
        }

        private bool IsCclPracticeTest()
        {
            var request = ApplicationModel.CredentialRequests.First(x=>x.Id == WizardModel.CredentialRequestId);
            return request.CredentialType.InternalName.ToLower().Contains("ccl");
        }

        private void GetSpecificPracticeTestCclEmailTokens(Dictionary<string, string> tokenDictionary,bool isCcl)
        {
            if (isCcl)
            {
                var cclSpecificCommentReferenceTable = GetCclSpecificCommentReferenceTable();
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.CCLSpecificCommentReferenceTable), cclSpecificCommentReferenceTable);
                tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.CCLSpecificExaminerCommentsText), "Examiners provide comments regarding test performance using the code set out below. The comments in relation to your test are as follows:");
                return;
            }
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.CCLSpecificCommentReferenceTable), string.Empty);
            tokenDictionary.Add(TokenReplacementService.GetTokenNameFor(TokenReplacementField.CCLSpecificExaminerCommentsText), string.Empty);
        }

        private string GetCclSpecificCommentReferenceTable()
        {
            return @"
    <p>
        <b>Accuracy</b>
    </p>
    <table class='accuracy'>
        <thead>
        </thead>
        <tbody>
            <tr>
                <th>A</th>
                <td>Omissions</td>
            </tr>
            <tr>
                <th>B</th>
                <td>Distortions</td>
            </tr>
            <tr>
                <th>C</th>
                <td>Unjustified insertions</td>
            </tr>
            <tr>
                <th>D</th>
                <td>Excessive requests for repeats</td>
            </tr>
        </tbody>
        <tfoot>
        </tfoot>
    </table>
    <br />
    <p>
        <b>Quality of Language</b>
    </p>
    <table class='quality-of-language'>
        <thead>
        </thead>
        <tbody>
            <tr>
                <th>E</th>
                <td>Inappropriate choice of register in English</td>
            </tr>
            <tr>
                <th>F</th>
                <td>Unidiomatic usage in English</td>
            </tr>
            <tr>
                <th>G</th>
                <td>Incorrect sentence structures in English</td>
            </tr>
            <tr>
                <th>H</th>
                <td>Grammatical errors in English</td>
            </tr>
            <tr>
                <th>J</th>
                <td>Inappropriate choice of register in LOTE</td>
            </tr>
            <tr>
                <th>K</th>
                <td>Unidiomatic usage in LOTE</td>
            </tr>
            <tr>
                <th>L</th>
                <td>Incorrect sentence structures in LOTE</td>
            </tr>
            <tr>
                <th>M</th>
                <td>Grammatical errors in LOTE</td>
            </tr>
        </tbody>
        <tfoot>
        </tfoot>
    </table>
    <br />
    <p>
        <b>Quality of Delivery</b>
    </p>
    <table class='quality-of-delivery'>
        <thead>
        </thead>
        <tbody>
            <tr>
                <th>O</th>
                <td>Excessive pauses</td>
            </tr>
            <tr>
                <th>P</th>
                <td>Excessive hesitations</td>
            </tr>
            <tr>
                <th>Q</th>
                <td>Excessive self-corrections</td>
            </tr>
            <tr>
                <th>R</th>
                <td>Wrong direction in LOTE</td>
            </tr>
            <tr>
                <th>S</th>
                <td>Wrong direction in English</td>
            </tr>
            <tr>
                <th>T</th>
                <td>Exceeding the time limit</td>
            </tr>
            <tr>
                <th>U</th>
                <td>No attempt to render</td>
            </tr>
        </tbody>
        <tfoot>
        </tfoot>
    </table>
    ";
        }

    }

}