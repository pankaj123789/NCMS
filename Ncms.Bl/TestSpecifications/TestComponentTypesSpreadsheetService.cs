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
    public static class TestComponentTypesSpreadsheetService
    {
        public static GenericResponse<TestSpecification> ValidateSheet(XLWorkbook workbook, TestSpecification testSpecification)
        {
            var result = new GenericResponse<TestSpecification>()
            {
                Data = testSpecification
            };

            try
            {
                var worksheet = workbook.Worksheet(Constants.TestComponentType);

                var lastRow = worksheet.LastRowUsed().RowNumber();
                if (lastRow < 2)
                {
                    result.Errors.Add("Failed: Test Component Types, Reason: No Test Component Types found in worksheet, please define the necessary test tasks");
                    result.Success = false;
                    return result;
                }

                var i = 2;
                var keepGoing = true;
                while (keepGoing)
                {
                    var oldTestComponentNameData = worksheet.Cell(i, 27).GetValue<string>();
                    var testComponentTypeNameData = worksheet.Cell(i, 3).GetValue<string>();

                    if (string.IsNullOrEmpty(oldTestComponentNameData) && string.IsNullOrEmpty(testComponentTypeNameData))
                    {
                        keepGoing = false;
                        break;
                    }

                    // Inserts
                    if (string.IsNullOrEmpty(oldTestComponentNameData) && !string.IsNullOrEmpty(testComponentTypeNameData))
                    {
                        var testComponentTypeInsert = new TestComponentType()
                        {
                            TestSpecificationDescription = testSpecification.Description,
                            Name = worksheet.Cell(i, 1).GetValue<string>(),
                            Label = worksheet.Cell(i, 2).GetValue<string>(),
                            Description = worksheet.Cell(i, 3).GetValue<string>(),
                            TestComponentBaseTypeId = worksheet.Cell(i, 4).GetValue<int>(),
                            MinExaminerCommentLength = worksheet.Cell(i, 5).GetValue<int>(),
                            MinNaatiCommentLength = worksheet.Cell(i, 6).GetValue<int>(),
                            RoleplayersRequired = worksheet.Cell(i, 7).GetValue<bool>(),
                            CandidateBriefRequired = worksheet.Cell(i, 8).GetValue<bool>(),
                            CandidateBriefavailabilitydays = worksheet.Cell(i, 9).GetValue<int>(),
                            DefaultMaterialRequestHours = worksheet.Cell(i, 10).GetValue<int>(),
                            DefaultMaterialRequestDueDays = worksheet.Cell(i, 11).GetValue<int>(),
                            ModelState = ModelState.inserted,

                            // Natural Keys
                            NaturalKey = worksheet.Cell(i, 27).GetValue<string>(),
                        };

                        i++;
                        testSpecification.TestComponentTypes.Add(testComponentTypeInsert);
                        continue;
                    }

                    // Edits
                    var testComponentType = new TestComponentType()
                    {
                        TestSpecificationDescription = testSpecification.Description,
                        Name = worksheet.Cell(i, 1).GetValue<string>(),
                        Label = worksheet.Cell(i, 2).GetValue<string>(),
                        Description = worksheet.Cell(i, 3).GetValue<string>(),
                        TestComponentBaseTypeId = worksheet.Cell(i, 4).GetValue<int>(),
                        MinExaminerCommentLength = worksheet.Cell(i, 5).GetValue<int>(),
                        MinNaatiCommentLength = worksheet.Cell(i, 6).GetValue<int>(),
                        RoleplayersRequired = worksheet.Cell(i, 7).GetValue<bool>(),
                        CandidateBriefRequired = worksheet.Cell(i, 8).GetValue<bool>(),
                        CandidateBriefavailabilitydays = worksheet.Cell(i, 9).GetValue<int>(),
                        DefaultMaterialRequestHours = worksheet.Cell(i, 10).GetValue<int>(),
                        DefaultMaterialRequestDueDays = worksheet.Cell(i, 11).GetValue<int>(),

                        // Natural Keys
                        NaturalKey = worksheet.Cell(i, 27).GetValue<string>(),
                    };

                    testSpecification.TestComponentTypes.Add(testComponentType);

                    i++;
                }

                return result;
            }
            catch (FormatException ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add($"Failed: Test Component Types, Reason: Incorrect Values Used");
                result.Success = false;
                return result;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add("Failed: Test Component Type, Reason: Unexpected error occured");
                result.Success = false;
                return result;
            }

        }

        public static bool WriteTestComponentType(XLWorkbook document, TestSpecification testSpecification)
        {
            var testComponentTypesData = testSpecification.TestComponentTypes.ToList();
            var worksheet = document.Worksheets.Add(Constants.TestComponentType);

            var i = 2;
            foreach (var result in testComponentTypesData)
            {
                worksheet.Cell(i, 1).Value = result.Name;
                worksheet.Cell(i, 2).Value = result.Label;
                worksheet.Cell(i, 3).Value = result.Description;
                worksheet.Cell(i, 4).Value = result.TestComponentBaseTypeId;
                worksheet.Cell(i, 5).Value = result.MinExaminerCommentLength;
                worksheet.Cell(i, 6).Value = result.MinNaatiCommentLength;
                worksheet.Cell(i, 7).Value = result.RoleplayersRequired;
                worksheet.Cell(i, 8).Value = result.CandidateBriefRequired;
                worksheet.Cell(i, 9).Value = result.CandidateBriefavailabilitydays;
                worksheet.Cell(i, 10).Value = result.DefaultMaterialRequestHours;
                worksheet.Cell(i, 11).Value = result.DefaultMaterialRequestDueDays;

                // Hidden Fields
                worksheet.Cell(i, 27).Value = string.Empty;
                i++;
            }

            worksheet.Cell(1, 1).Value = "Name";
            worksheet.Cell(1, 1).Style.Font.SetBold();
            worksheet.Column(1).Style.Fill.BackgroundColor = XLColor.FromArgb(231, 230, 230);
            worksheet.Cell(1, 2).Value = "Label";
            worksheet.Cell(1, 2).Style.Font.SetBold();
            worksheet.Cell(1, 3).Value = "Description";
            worksheet.Cell(1, 3).Style.Font.SetBold();
            worksheet.Cell(1, 4).Value = "TestComponentBaseTypeId";
            worksheet.Cell(1, 4).Style.Font.SetBold();
            worksheet.Cell(1, 5).Value = "MinExaminerCommentLength";
            worksheet.Cell(1, 5).Style.Font.SetBold();
            worksheet.Cell(1, 6).Value = "MinNaatiCommentLength";
            worksheet.Cell(1, 6).Style.Font.SetBold();
            worksheet.Cell(1, 7).Value = "RoleplayersRequired";
            worksheet.Cell(1, 7).Style.Font.SetBold();
            worksheet.Cell(1, 8).Value = "CandidateBriefRequired";
            worksheet.Cell(1, 8).Style.Font.SetBold();
            worksheet.Cell(1, 9).Value = "CandidateBriefavailabilitydays";
            worksheet.Cell(1, 9).Style.Font.SetBold();
            worksheet.Cell(1, 10).Value = "DefaultMaterialRequestHours";
            worksheet.Cell(1, 10).Style.Font.SetBold();
            worksheet.Cell(1, 11).Value = "DefaultMaterialRequestDueDays";
            worksheet.Cell(1, 11).Style.Font.SetBold();

            // Hidden Field Headers
            worksheet.Cell(1, 27).Value = $"OldName";
            worksheet.Cell(1, 27).Style.Font.SetBold();
            worksheet.Column(27).Hide();

            worksheet.Columns().AdjustToContents();
            worksheet.Rows().AdjustToContents();
            return true;
        }

        public static GenericResponse<bool> CompareTestComponentTypes(TestSpecification testSpecification, TestSpecification testSpecificationCompare, GenericResponse<bool> response)
        {
            var testComponentTypes = testSpecification.TestComponentTypes.ToList();
            var testComponentTypesCompare = testSpecificationCompare.TestComponentTypes.ToList();

            foreach (var testComponentType in testComponentTypes)
            {
                var testComponentTypeCompare = testComponentTypesCompare.FirstOrDefault(x => (x.TestSpecificationDescription == testComponentType.TestSpecificationDescription)
                                                                                        && (x.Name == testComponentType.Name));
                if (testComponentTypeCompare == null)
                {
                    response.Messages.Add($"Inserted: Test Component Type, Row: {testComponentType.Name}" +
                        $" in spreadsheet.");
                    continue;
                }
                response = EqualsModel(testComponentType, testComponentTypeCompare, response);
            }

            foreach (var testComponentTypeCompare in testComponentTypesCompare)
            {
                var testComponentType = testComponentTypes.FirstOrDefault(x => (x.TestSpecificationDescription == testComponentTypeCompare.TestSpecificationDescription)
                                                                          && (x.Name == testComponentTypeCompare.Name));
                if (testComponentType == null)
                {
                    response.Messages.Add($"Deleted: Test Component Type, Old Value: {testComponentTypeCompare.Name}");
                    continue;
                }
            }

            return response;
        }

        public static GenericResponse<bool> EqualsModel(this TestComponentType testComponentType, TestComponentType testComponentTypeCompare, GenericResponse<bool> response)
        {
            var startMessageCount = response.Messages.Count;
            var sheetName = Constants.TestComponentType;
            var noParents = "";
            response = testComponentType.TestSpecificationDescription.CompareField(testComponentTypeCompare.TestSpecificationDescription, "TestSpecificationDescription", response, sheetName, noParents);
            response = testComponentType.Label.CompareField(testComponentTypeCompare.Label, "Label", response, sheetName, noParents);
            response = testComponentType.Name.CompareField(testComponentTypeCompare.Name, "Name", response, sheetName, noParents);
            response = testComponentType.Description.CompareField(testComponentTypeCompare.Description, "Description", response, sheetName, noParents);
            response = testComponentType.TestComponentBaseTypeId.CompareField(testComponentTypeCompare.TestComponentBaseTypeId, "TestComponentBaseTypeId", response, sheetName, noParents);
            response = testComponentType.MinExaminerCommentLength.CompareField(testComponentTypeCompare.MinExaminerCommentLength, "MinExaminerCommentLength", response, sheetName, noParents);
            response = testComponentType.MinNaatiCommentLength.CompareField(testComponentTypeCompare.MinNaatiCommentLength, "MinNaatiCommentLength", response, sheetName, noParents);
            response = testComponentType.RoleplayersRequired.CompareField(testComponentTypeCompare.RoleplayersRequired, "RoleplayersRequired", response, sheetName, noParents);
            response = testComponentType.CandidateBriefRequired.CompareField(testComponentTypeCompare.CandidateBriefRequired, "CandidateBriefRequired", response, sheetName, noParents);
            response = testComponentType.CandidateBriefavailabilitydays.CompareField(testComponentTypeCompare.CandidateBriefavailabilitydays, "CandidateBriefavailabilitydays", response, sheetName, noParents);
            response = testComponentType.DefaultMaterialRequestHours.CompareField(testComponentTypeCompare.DefaultMaterialRequestHours, "DefaultMaterialRequestHours", response, sheetName, noParents);
            response = testComponentType.DefaultMaterialRequestDueDays.CompareField(testComponentTypeCompare.DefaultMaterialRequestDueDays, "DefaultMaterialRequestDueDays", response, sheetName, noParents);

            if (response.Messages.Count > startMessageCount)
            {
                testComponentType.ModelState = ModelState.altered;
            }

            return response;
        }
    }

}
