using ClosedXML.Excel;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Dal;
using Ncms.Bl.TestSpecifications.Extensions;
using Ncms.Contracts;
using Ncms.Contracts.Models.TestSpecification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Ncms.Bl.TestSpecifications
{
    public class TestSpecificationSpreadsheetService : ITestSpecificationSpreadsheetService
    {
        private readonly ISharedAccessSignature _sharedAccessSignature;

        public TestSpecificationSpreadsheetService()
        {
            var serviceLocatorInstance = ServiceLocator.GetInstance();

            _sharedAccessSignature = serviceLocatorInstance.Resolve<ISharedAccessSignature>();
        }
        public GenericResponse<string> GetTestSpecificationDescription(int testSpecificationId)
        {
            var testSpecificationsQueryService = new TestSpecificationsQueryService();
            return testSpecificationsQueryService.GetTestSpecificationDescription(testSpecificationId);
        }

        public GenericResponse<string> GetTestSpecifications(string testSpecificationFilter)
        {
            var testSpecificationsQueryService = new TestSpecificationsQueryService();
            var credentialTypeData = testSpecificationsQueryService.GetData();

            var testSpecification = credentialTypeData.SelectMany(x => x.TestSpecifications).FirstOrDefault(x => x.Description == testSpecificationFilter);
            var workbook = new XLWorkbook();

            return WriteWorkbook(workbook, testSpecification);
        }

        public GenericResponse<TestSpecification> ReadWorkbook(string filePath)
        {
            var result = new GenericResponse<TestSpecification>()
            {
                Data = new TestSpecification()
            };

            using (var workbook = new XLWorkbook(filePath))
            {
                result = TestSpecificationsSpreadsheetService.ValidateSheet(workbook, result.Data);
                result = TestComponentTypesSpreadsheetService.ValidateSheet(workbook, result.Data);
                result = TestComponentsSpreadsheetService.ValidateSheet(workbook, result.Data);
                result = TestCompTypeStandardMarkingSchemeSpreadsheetService.ValidateSheet(workbook, result.Data);
                result = TestSpecificationStandardMarkingSchemeSpreadsheetService.ValidateSheet(workbook, result.Data);
                if (result.Messages.Count > 0 && result.Success)
                {
                    return result;
                }
                result = RubricMarkingCompetencySpreadsheetService.ValidateSheet(workbook, result.Data);
                result = RubricMarkingAssessmentCriterionSpreadsheetService.ValidateSheet(workbook, result.Data);
                result = RubricMarkingBandSpreadsheetService.ValidateSheet(workbook, result.Data);
            }
            if (!result.Success)
            {
                return result;
            }
            return result;
        }

        public GenericResponse<string> UploadTestSpecifications(string filePath, string testSpecificationFilter, bool verifyOnly)
        {
            var testSpecificationsQueryService = new TestSpecificationsQueryService();
            var verificationService = new TestSpecificationVerificationService();

            var response = new GenericResponse<string>();

            var workbookResult = ReadWorkbook(filePath);
            
            var workbookTestSpecification = workbookResult.Data;

            // Error has occured in workbook
            if (!workbookResult.Success)
            {
                var output = verificationService.HandleReadOutput(workbookResult);
                response.Errors = output;
                response.Data = $"Error has occured. Check spreadsheet has been filled correctly.";

                foreach (var error in response.Errors)
                {
                    LoggingHelper.LogError($"{error}");
                }

                return response;
            }


            var databaseData = testSpecificationsQueryService.GetData();
            var databaseTestSpecification = databaseData.SelectMany(x => x.TestSpecifications).FirstOrDefault(x => x.Description == testSpecificationFilter);

            if (verifyOnly)
            {
                LoggingHelper.LogInfo($"User has clicked the verify button for {testSpecificationFilter} upload.");

                // Error has occured with test specification filter
                if (databaseTestSpecification == null)
                {
                    response.Data = $"Error has occured. Database has no record for {testSpecificationFilter}";
                    LoggingHelper.LogError($"Error has occured. Database has no record for {testSpecificationFilter}");
                    return response;
                }

                var compareResponse = verificationService.HandleCompare(workbookTestSpecification, databaseTestSpecification);
                var compareMessages = compareResponse.Messages;
                var compareErrors = compareResponse.Errors;

                if (compareMessages.Count > 0)
                {
                    response.Messages = compareMessages;
                }

                // Error occured during HandleCompare()
                if (compareErrors.Count > 0)
                {
                    response.Errors = compareErrors;
                    response.Data = "Error has occured. Could not complete verification process.";
                    foreach (var error in response.Errors)
                    {
                        LoggingHelper.LogError($"{error}");
                    }
                    return response;
                }

                // Verification has completed but there has been no changes
                if (response.Messages.Count == 0 && response.Errors.Count == 0)
                {
                    response.Data = $"Verification complete. Warning: No changes found.";
                    LoggingHelper.LogWarning($"No changes found during spreadsheet verification.");
                    return response;
                }

                // Verification has completed successfully with no errors
                if (response.Messages.Count > 0 && response.Errors.Count == 0)
                {
                    response.Data = $"Verification complete. No errors occured.";
                    return response;
                }
                return response;
            }

            var seqMessagesToPostOnSuccess = verificationService.HandleCompare(workbookTestSpecification, databaseTestSpecification).Messages;
            return testSpecificationsQueryService.WriteData(workbookTestSpecification, testSpecificationFilter, seqMessagesToPostOnSuccess);
        }

        private GenericResponse<string> WriteWorkbook(XLWorkbook document, TestSpecification testSpecification)
        {
            TestSpecificationsSpreadsheetService.WriteTestSpecifications(document, testSpecification);
            TestComponentTypesSpreadsheetService.WriteTestComponentType(document, testSpecification);
            TestComponentsSpreadsheetService.WriteTestComponent(document, testSpecification);
            TestCompTypeStandardMarkingSchemeSpreadsheetService.WriteTestComponentTypeStandardMarkingScheme(document, testSpecification);
            TestSpecificationStandardMarkingSchemeSpreadsheetService.WriteTestStandardMarkingScheme(document, testSpecification);
            RubricMarkingCompetencySpreadsheetService.WriteRubricMarkingCompetency(document, testSpecification);
            RubricMarkingAssessmentCriterionSpreadsheetService.WriteRubricMarkingAssessmentCriterion(document, testSpecification);
            RubricMarkingBandSpreadsheetService.WriteRubricMarkingBand(document, testSpecification);

            var tempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"];
            var filePath = $"{tempFileStorePath}\\TestSpecifications.xlsx";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            document.SaveAs(filePath);

            var token = _sharedAccessSignature.GetUrlForFile(filePath);

            return token;
        }
    }
}
