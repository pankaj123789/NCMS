using F1Solutions.Global.Common.Logging;
using ClosedXML.Excel;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Bl.TestSpecifications.Extensions;
using Ncms.Contracts.Models.TestSpecification;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Ncms.Bl.TestSpecifications
{
    public static class TestSpecificationStandardMarkingSchemeSpreadsheetService
    {
        public static GenericResponse<TestSpecification> ValidateSheet(XLWorkbook workbook, TestSpecification testSpecification)
        {

            var result = new GenericResponse<TestSpecification>()
            {
                Data = testSpecification
            };

            try
            {
                var worksheet = workbook.Worksheet(Constants.TestStandardMarkingScheme);

                var lastRow = worksheet.LastRowUsed().RowNumber();

                if (lastRow > 2)
                {
                    result.Errors.Add("Failed: Test Specification Standard Marking Scheme, Reason: More than one Test Specification Standard Marking Scheme found, " +
                        "only one overall pass mark is required to be defined");
                    result.Success = false;
                    return result;
                }

                if (lastRow == 2)
                {
                    result.Messages.Add("Test Specification Standard Marking Scheme Found. Rubric Marking Not Required.");
                }

                //this might change if first column doesnt have to have data
                var testSpecificationData = "";
                var passMarkData = "";
                try
                {
                    testSpecificationData = worksheet.Cell(2, 1).GetValue<string>();
                    passMarkData = worksheet.Cell(2, 2).GetValue<int>().ToString();
                }
                catch (FormatException ex)
                {
                    LoggingHelper.LogError(ex.ToString());
                    return result;
                }
                catch (Exception ex)
                {
                    LoggingHelper.LogError(ex.ToString());
                    result.Errors.Add("Failed: Test Specification Standard Marking Scheme, Reason: Unexpected error occured");
                    result.Success = false;
                    return result;
                }
                if (string.IsNullOrEmpty(testSpecificationData) && string.IsNullOrEmpty(passMarkData))
                {
                    return result;
                }

                if (string.IsNullOrEmpty(testSpecificationData) && !string.IsNullOrEmpty(passMarkData))
                {
                    var testSpecificationStandardMarkingSchemeInsert = new TestSpecificationStandardMarkingScheme()
                    {
                        TestSpecificationDescription = testSpecification.NaturalKey,
                        OverallPassMark = worksheet.Cell(2, 2).GetValue<int>(),
                        ModelState = ModelState.inserted
                    };
                    testSpecification.TestSpecificationStandardMarkingSchemes.Add(testSpecificationStandardMarkingSchemeInsert);
                    return result;
                }
                var testStandardMarkingScheme = new TestSpecificationStandardMarkingScheme()
                {
                    TestSpecificationDescription = testSpecification.NaturalKey,
                    OverallPassMark = worksheet.Cell(2, 2).GetValue<int>(),
                    ModelState = ModelState.inserted
                };

                testSpecification.TestSpecificationStandardMarkingSchemes.Add(testStandardMarkingScheme);

                return result;
            }
            catch (FormatException ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add($"Failed: Test Specification Standard Marking Scheme, Reason: Incorrect Values Used");
                result.Success = false;
                return result;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add("Failed: Test Specification Standard Marking Scheme, Reason: Unexpected error occured");
                result.Success = false;
                return result;
            }
        }

        public static bool WriteTestStandardMarkingScheme(XLWorkbook document, TestSpecification testSpecification)
        {
            var testStandardMarkingSchemeData = testSpecification.TestSpecificationStandardMarkingSchemes.ToList();
            var worksheet = document.Worksheets.Add(Constants.TestStandardMarkingScheme);

            var i = 2;
            foreach (var result in testStandardMarkingSchemeData)
            {
                worksheet.Cell(i, 1).Value = result.TestSpecificationDescription;
                worksheet.Cell(i, 2).Value = result.OverallPassMark;
                i++;
            }

            worksheet.Cell(1, 1).Value = "TestSpecificationDescription";
            worksheet.Cell(1, 1).Style.Font.SetBold();
            worksheet.Column(1).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 225, 242);
            worksheet.Cell(1, 2).Value = "OverallPassMark";
            worksheet.Cell(1, 2).Style.Font.SetBold();

            worksheet.Columns().AdjustToContents();
            worksheet.Rows().AdjustToContents();

            return true;
        }

        public static GenericResponse<bool> CompareTestStandardMarkingScheme(TestSpecification testSpecification, TestSpecification testSpecificationCompare,
                                                                                     GenericResponse<bool> response)
        {
            var testStandardMarkingSchemes = testSpecification.TestSpecificationStandardMarkingSchemes.ToList();
            var testStandardMarkingSchemesCompare = testSpecificationCompare.TestSpecificationStandardMarkingSchemes.ToList();

            foreach (var testStandardMarkingScheme in testStandardMarkingSchemes)
            {
                var testStandardMarkingSchemeCompare = testStandardMarkingSchemesCompare
                                                       .FirstOrDefault(x => x.TestSpecificationDescription == testStandardMarkingScheme.TestSpecificationDescription);
                if (testStandardMarkingSchemeCompare == null)
                {
                    response.Messages.Add($"Inserted: Test Specification Standard Marking Scheme, Parent: {testSpecification.Description}");
                    continue;
                }
                response = EqualsModel(testStandardMarkingScheme, testStandardMarkingSchemeCompare, response);
            }

            foreach (var testStandardMarkingSchemeCompare in testStandardMarkingSchemesCompare)
            {
                var testStandardMarkingScheme = testStandardMarkingSchemes
                                                       .FirstOrDefault(x => x.TestSpecificationDescription == testStandardMarkingSchemeCompare.TestSpecificationDescription);
                if (testStandardMarkingScheme == null)
                {
                    response.Messages.Add($"Deleted: Test Specification Standard Marking Scheme, Old Parent: {testStandardMarkingSchemeCompare.TestSpecificationDescription}");
                }
            }

            return response;
        }

        public static GenericResponse<bool> EqualsModel(this TestSpecificationStandardMarkingScheme testStandardMarkingScheme,
                                                        TestSpecificationStandardMarkingScheme testStandardMarkingSchemeCompare, GenericResponse<bool> response)
        {
            var startMessageCount = response.Messages.Count;
            var sheetName = Constants.TestStandardMarkingScheme;
            var noParents = "";
            response = testStandardMarkingScheme.TestSpecificationDescription.CompareField(testStandardMarkingSchemeCompare.TestSpecificationDescription, "TestSpecificationDescription", response, sheetName, noParents);
            response = testStandardMarkingScheme.OverallPassMark.CompareField(testStandardMarkingSchemeCompare.OverallPassMark, "OverallPassMark", response, sheetName, noParents);

            if (response.Messages.Count > startMessageCount)
            {
                testStandardMarkingScheme.ModelState = ModelState.altered;
            }

            return response;
        }
    }

}
