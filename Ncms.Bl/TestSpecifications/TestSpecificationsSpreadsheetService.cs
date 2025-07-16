using F1Solutions.Global.Common.Logging;
using ClosedXML.Excel;
using Ncms.Bl.TestSpecifications.Extensions;
using Ncms.Contracts.Models.TestSpecification;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using System;

namespace Ncms.Bl.TestSpecifications
{
    public static class TestSpecificationsSpreadsheetService
    {
        public static GenericResponse<TestSpecification> ValidateSheet(XLWorkbook workbook, TestSpecification testSpecification)
        {
            var result = new GenericResponse<TestSpecification>()
            {
                Data = testSpecification
            };

            try
            {
                var worksheet = workbook.Worksheet(Constants.TestSpecifications);

                var testSpecificationEmpty = string.IsNullOrEmpty(worksheet.Cell(2, 2).GetValue<string>()); 
                if (testSpecificationEmpty)
                {
                    result.Errors.Add("Failed: Test Specifications, Reason: No Test Specification found in worksheet");
                    result.Success = false;
                    return result;
                }

                testSpecification.CredentialTypeName = worksheet.Cell(2, 1).GetValue<string>();
                testSpecification.Description = worksheet.Cell(2, 2).GetValue<string>();
                testSpecification.Active = worksheet.Cell(2, 3).GetValue<bool>();
                testSpecification.AutomaticIssuing = worksheet.Cell(2, 4).GetValue<bool>();
                testSpecification.MaxScoreDifference = worksheet.Cell(2, 5).GetValue<int?>();
                testSpecification.ResultAutoCalculation = worksheet.Cell(2, 6).GetValue<bool>();

                // Natural Keys
                // testSpecification.NaturalKey = worksheet.Cell(2, 27).GetValue<string>();

                return result;
            }
            catch (FormatException ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add($"Failed: Test Specifications, Reason: Incorrect Values Used");
                result.Success = false;
                return result;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add("Failed: Test Specifications, Reason: Unexpected error occured");
                result.Success = false;
                return result;
            }
        }

        public static bool WriteTestSpecifications(XLWorkbook document, TestSpecification testSpecification)
        {
            var worksheet = document.Worksheets.Add(Constants.TestSpecifications);

            worksheet.Cell(2, 1).Value = testSpecification.CredentialTypeName;
            worksheet.Cell(2, 2).Value = testSpecification.Description;
            worksheet.Cell(2, 3).Value = testSpecification.Active;
            worksheet.Cell(2, 4).Value = testSpecification.AutomaticIssuing;
            if (testSpecification.MaxScoreDifference.HasValue)
            {
                worksheet.Cell(2, 5).Value = testSpecification.MaxScoreDifference;
            }
            worksheet.Cell(2, 6).Value = testSpecification.ResultAutoCalculation;

            //Hidden Fields
            // worksheet.Cell(2, 27).Value = testSpecification.Description;

            worksheet.Cell(1, 1).Value = "CredentialTypeName";
            worksheet.Cell(1, 1).Style.Font.SetBold();
            worksheet.Column(1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 225, 242);
            worksheet.Cell(1, 2).Value = "TestSpecificationDescription";
            worksheet.Cell(1, 2).Style.Font.SetBold();
            worksheet.Column(2).Style.Fill.BackgroundColor = XLColor.FromArgb(231, 230, 230);
            worksheet.Cell(1, 3).Value = "Active";
            worksheet.Cell(1, 3).Style.Font.SetBold();
            worksheet.Cell(1, 4).Value = "AutomaticIssuing";
            worksheet.Cell(1, 4).Style.Font.SetBold();
            worksheet.Cell(1, 5).Value = "MaxScoreDifference";
            worksheet.Cell(1, 5).Style.Font.SetBold();
            worksheet.Cell(1, 6).Value = "ResultAutoCalculation";
            worksheet.Cell(1, 6).Style.Font.SetBold();

            // Hidden Field Headers
            worksheet.Cell(1, 27).Value = $"OldTestSpecDescription";
            worksheet.Cell(1, 27).Style.Font.SetBold();
            worksheet.Column(27).Hide();

            worksheet.Columns().AdjustToContents();
            worksheet.Rows().AdjustToContents();
            return true;
        }

        public static GenericResponse<bool> CompareTestSpecifications(TestSpecification testSpecification, TestSpecification testSpecificationCompare, GenericResponse<bool> response)
        {
            response = EqualsModel(testSpecification, testSpecificationCompare, response);

            return response;
        }

        public static GenericResponse<bool> EqualsModel(this TestSpecification testSpecification, TestSpecification testSpecificationCompare, GenericResponse<bool> response)
        {
            var startMessageCount = response.Messages.Count;
            var sheetName = Constants.TestSpecifications;
            var noParents = "";
            response = testSpecification.CredentialTypeName.CompareField(testSpecificationCompare.CredentialTypeName, "CredentialTypeName", response, sheetName, noParents);
            response = testSpecification.Description.CompareField(testSpecificationCompare.Description, "Description", response, sheetName, noParents);
            response = testSpecification.Active.CompareField(testSpecificationCompare.Active, "Active", response, sheetName, noParents);
            response = testSpecification.AutomaticIssuing.CompareField(testSpecificationCompare.AutomaticIssuing, "AutomaticIssuing", response, sheetName, noParents);
            response = testSpecification.MaxScoreDifference.CompareField(testSpecificationCompare.MaxScoreDifference, "MaxScoreDifference", response, sheetName, noParents);
            response = testSpecification.ResultAutoCalculation.CompareField(testSpecificationCompare.ResultAutoCalculation, "ResultAutoCalculation", response, sheetName, noParents);

            if (response.Messages.Count > startMessageCount)
            {
                testSpecification.ModelState = ModelState.altered;
            }

            return response;
        }
    }

}
