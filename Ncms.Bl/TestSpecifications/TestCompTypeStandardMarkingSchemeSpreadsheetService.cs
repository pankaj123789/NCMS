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
    public static class TestCompTypeStandardMarkingSchemeSpreadsheetService
    {
        public static GenericResponse<TestSpecification> ValidateSheet(XLWorkbook workbook, TestSpecification testSpecification)
        {
            var result = new GenericResponse<TestSpecification>()
            {
                Data = testSpecification
            };

            try
            {
                var testComponentTypes = testSpecification.TestComponentTypes.ToList();
                var worksheet = workbook.Worksheet(Constants.TestCompTypeStandardScheme);

                var i = 2;
                var keepGoing = true;
                while (keepGoing)
                {
                    var oldTestComponentTypeData = worksheet.Cell(i, 27).GetValue<string>();
                    var testComponentTypeData = worksheet.Cell(i, 1).GetValue<string>();

                    if (string.IsNullOrEmpty(oldTestComponentTypeData) && string.IsNullOrEmpty(testComponentTypeData))
                    {
                        keepGoing = false;
                        break;
                    }
                    if (string.IsNullOrEmpty(oldTestComponentTypeData) && !string.IsNullOrEmpty(testComponentTypeData))
                    {
                        var testComponentTypeStandardMarkingSchemeInsert = new TestComponentTypeStandardMarkingScheme()
                        {
                            TotalMarks = worksheet.Cell(i, 2).GetValue<int>(),
                            PassMark = worksheet.Cell(i, 3).GetValue<int>(),
                            ModelState = ModelState.inserted,

                            // Natural Keys
                            ComponentType = testComponentTypeData,
                        };

                        var testComponentTypeForInsert = testComponentTypes.FirstOrDefault(x => x.Name == testComponentTypeStandardMarkingSchemeInsert.ComponentType);
                        if (testComponentTypeForInsert == null)
                        {
                            result.Errors.Add($"Failed: Test Component Type Standard Marking Scheme, " +
                                $"Reason: Given Test Component Type does not exist");
                            result.Success = false;
                            i++;
                            continue;
                        }

                        testComponentTypeForInsert.TestComponentTypeStandardMarkingScheme.Add(testComponentTypeStandardMarkingSchemeInsert);
                        i++;
                        continue;
                    }

                    var testComponentType = testComponentTypes.FirstOrDefault(x => x.NaturalKey == worksheet.Cell(i, 1).GetValue<string>());
                    if (testComponentType == null)
                    {
                        result.Errors.Add($"Failed: Test Component Type Standard Marking Scheme, " +
                                $"Reason: Given Test Component Type does not exist");
                        result.Success = false;
                        i++;
                        continue;
                    }

                    var testComponentTypeStandardMarkingScheme = new TestComponentTypeStandardMarkingScheme()
                    {
                        
                        TotalMarks = worksheet.Cell(i, 2).GetValue<int>(),
                        PassMark = worksheet.Cell(i, 3).GetValue<int>(),

                        // Natural Keys
                        ComponentType = testComponentType.NaturalKey
                    };

                    testComponentType.TestComponentTypeStandardMarkingScheme.Add(testComponentTypeStandardMarkingScheme);

                    i++;
                }

                return result;
            }
            catch (FormatException ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add($"Failed: Test Component Type Standard Marking Scheme, Reason: Incorrect Values Used");
                result.Success = false;
                return result;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add("Failed: Test Component Type Standard Marking Scheme, Reason: Unexpected error occured");
                result.Success = false;
                return result;
            }
        }

        public static bool WriteTestComponentTypeStandardMarkingScheme(XLWorkbook document, TestSpecification testSpecification)
        {
            var testComponentTypeStandardMarkingSchemeData = testSpecification.TestComponentTypes.SelectMany(x => x.TestComponentTypeStandardMarkingScheme).ToList();
            var worksheet = document.Worksheets.Add(Constants.TestCompTypeStandardScheme);

            var i = 2;
            foreach (var result in testComponentTypeStandardMarkingSchemeData)
            {
                worksheet.Cell(i, 1).Value = result.ComponentType;
                worksheet.Cell(i, 2).Value = result.TotalMarks;
                worksheet.Cell(i, 3).Value = result.PassMark;
                i++;
            }

            worksheet.Cell(1, 1).Value = "TestComponentType";
            worksheet.Cell(1, 1).Style.Font.SetBold();
            worksheet.Column(1).Style.Fill.BackgroundColor = XLColor.FromArgb(226, 239, 218);
            worksheet.Cell(1, 2).Value = "TotalMarks";
            worksheet.Cell(1, 2).Style.Font.SetBold();
            worksheet.Cell(1, 3).Value = "PassMark";
            worksheet.Cell(1, 3).Style.Font.SetBold();

            worksheet.Columns().AdjustToContents();
            worksheet.Rows().AdjustToContents();

            return true;
        }

        public static GenericResponse<bool> CompareTestCompTypeStandardMarkingScheme(TestSpecification testSpecification, TestSpecification testSpecificationCompare,
                                                                                     GenericResponse<bool> response)
        {
            var testCompTypeStandardMarkingSchemes = testSpecification.TestComponentTypes.SelectMany(x => x.TestComponentTypeStandardMarkingScheme).ToList();
            var testCompTypeStandardMarkingSchemesCompare = testSpecificationCompare.TestComponentTypes.SelectMany(x => x.TestComponentTypeStandardMarkingScheme).ToList();

            foreach (var testCompTypeStandardMarkingScheme in testCompTypeStandardMarkingSchemes)
            {
                var testCompTypeStandardMarkingSchemeCompare = testCompTypeStandardMarkingSchemesCompare.FirstOrDefault(x => x.ComponentType == testCompTypeStandardMarkingScheme.ComponentType);
                if (testCompTypeStandardMarkingSchemeCompare == null)
                {
                    response.Messages.Add($"Inserted: Test Component Type Standard Marking Scheme, Parent: {testCompTypeStandardMarkingScheme.ComponentType}");
                    continue;
                }
                response = EqualsModel(testCompTypeStandardMarkingScheme, testCompTypeStandardMarkingSchemeCompare, response);
            }

            foreach (var testCompTypeStandardMarkingSchemeCompare in testCompTypeStandardMarkingSchemesCompare)
            {
                var testCompTypeStandardMarkingScheme = testCompTypeStandardMarkingSchemes.FirstOrDefault(x => x.ComponentType == testCompTypeStandardMarkingSchemeCompare.ComponentType);
                if (testCompTypeStandardMarkingSchemeCompare == null)
                {
                    response.Messages.Add($"Deleted: Test Component Type Standard Marking Scheme, Old Parent: {testCompTypeStandardMarkingScheme.ComponentType}");
                }
            }

            return response;
        }

        public static GenericResponse<bool> EqualsModel(this TestComponentTypeStandardMarkingScheme testCompTypeStandardMarkingScheme,
                                                        TestComponentTypeStandardMarkingScheme testCompTypeStandardMarkingSchemeCompare, GenericResponse<bool> response)
        {
            var startMessageCount = response.Messages.Count;
            var sheetName = Constants.TestCompTypeStandardScheme;
            var noParents = "";
            response = testCompTypeStandardMarkingScheme.ComponentType.CompareField(testCompTypeStandardMarkingSchemeCompare.ComponentType, "ComponentType", response, sheetName, noParents);
            response = testCompTypeStandardMarkingScheme.TotalMarks.CompareField(testCompTypeStandardMarkingSchemeCompare.TotalMarks, "TotalMarks", response, sheetName, noParents);
            response = testCompTypeStandardMarkingScheme.PassMark.CompareField(testCompTypeStandardMarkingSchemeCompare.PassMark, "PassMark", response, sheetName, noParents);

            if (response.Messages.Count > startMessageCount)
            {
                testCompTypeStandardMarkingScheme.ModelState = ModelState.altered;
            }

            return response;
        }
    }

}
