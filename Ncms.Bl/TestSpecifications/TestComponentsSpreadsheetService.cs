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
    public static class TestComponentsSpreadsheetService
    {
        public static GenericResponse<TestSpecification> ValidateSheet(XLWorkbook workbook, TestSpecification testSpecification)
        {
            var result = new GenericResponse<TestSpecification>()
            {
                Data = testSpecification
            };

            try
            {
                var testComponentTypes = testSpecification.TestComponentTypes;
                var worksheet = workbook.Worksheet(Constants.TestComponents);

                var lastRow = worksheet.LastRowUsed().RowNumber();
                if (lastRow < 2)
                {
                    result.Errors.Add("Failed: Test Components, Reason: No Test Components found in worksheet, please define the necessary test tasks");
                    result.Success = false;
                    return result;
                }

                var i = 2;
                var keepGoing = true;
                while (keepGoing)
                {
                    //this might change if first column doesnt have to have data
                    var oldTestComponentNameData = worksheet.Cell(i, 27).GetValue<string>();
                    var testComponentNameData = worksheet.Cell(i, 1).GetValue<string>();

                    if (string.IsNullOrEmpty(oldTestComponentNameData) && string.IsNullOrEmpty(testComponentNameData))
                    {
                        keepGoing = false;
                        break;
                    }

                    // Inserts
                    if (string.IsNullOrEmpty(oldTestComponentNameData) && !string.IsNullOrEmpty(testComponentNameData))
                    {
                        var testComponentTypeForInsert = testComponentTypes.FirstOrDefault(x => x.Name == worksheet.Cell(i, 1).GetValue<string>());
                        if (testComponentTypeForInsert == null)
                        {
                            result.Errors.Add($"Failed: Test Components, Reason: No Test Component Type {worksheet.Cell(i, 1).GetValue<string>()} exists.");
                            result.Success = false;
                            i++;
                            continue;
                        }
                        var testComponentInsert = new TestComponent()
                        {
                            Name = worksheet.Cell(i, 2).GetValue<string>(),
                            ComponentNumber = worksheet.Cell(i, 3).GetValue<int>(),
                            Label = worksheet.Cell(i, 4).GetValue<string>(),
                            GroupNumber = worksheet.Cell(i, 5).GetValue<int>(),
                            ModelState = ModelState.inserted,

                            // Natural Keys
                            TestSpecificationDescription = testSpecification.Description,
                            TestComponentTypeName = testComponentTypeForInsert.Name,
                            NaturalKey = worksheet.Cell(i, 27).GetValue<string>(),
                        };

                        testSpecification.TestComponents.Add(testComponentInsert);

                        i++;
                        continue;
                    }

                    var testComponentType = testComponentTypes.FirstOrDefault(x => x.Name == worksheet.Cell(i, 1).GetValue<string>());
                    if (testComponentType == null)
                    {
                        result.Errors.Add($"No Test Component Type Exists for {worksheet.Cell(i, 1).GetValue<string>()}.");
                        result.Success = false;
                        i++;
                        continue;
                    }
                    
                    // Edits
                    var testComponent = new TestComponent()
                    {
                        Name = worksheet.Cell(i, 2).GetValue<string>(),
                        ComponentNumber = worksheet.Cell(i, 3).GetValue<int>(),
                        Label = worksheet.Cell(i, 4).GetValue<string>(),
                        GroupNumber = worksheet.Cell(i, 5).GetValue<int>(),

                        // Natural Keys
                        TestComponentTypeName = testComponentType.NaturalKey,
                        TestSpecificationDescription = testSpecification.NaturalKey,
                        NaturalKey = worksheet.Cell(i, 27).GetValue<string>(),
                    };

                    testSpecification.TestComponents.Add(testComponent);

                    i++;
                }

                return result;
            }
            catch (FormatException ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add($"Failed: Test Components, Reason: Incorrect Values Used");
                result.Success = false;
                return result;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add("Failed: Test Components, Reason: Unexpected error occured");
                result.Success = false;
                return result;
            }
        }

        public static bool WriteTestComponent(XLWorkbook document, TestSpecification testSpecification)
        {
            var testComponentsData = testSpecification.TestComponents.ToList();
            var worksheet = document.Worksheets.Add(Constants.TestComponents);

            var i = 2;
            foreach (var result in testComponentsData)
            {
                worksheet.Cell(i, 1).Value = result.TestComponentTypeName;
                worksheet.Cell(i, 2).Value = result.Name;
                worksheet.Cell(i, 3).Value = result.ComponentNumber;
                worksheet.Cell(i, 4).Value = result.Label;
                worksheet.Cell(i, 5).Value = result.GroupNumber;

                // Hidden Fields
                worksheet.Cell(i, 27).Value = string.Empty;
                i++;
            }
            worksheet.Cell(1, 1).Value = "TestComponentTypeName";
            worksheet.Cell(1, 1).Style.Font.SetBold();
            worksheet.Column(1).Style.Fill.BackgroundColor = XLColor.FromArgb(226, 239, 218);
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 2).Style.Font.SetBold();
            worksheet.Column(2).Style.Fill.BackgroundColor = XLColor.FromArgb(231, 230, 230);
            worksheet.Cell(1, 3).Value = "ComponentNumber";
            worksheet.Cell(1, 3).Style.Font.SetBold();
            worksheet.Cell(1, 4).Value = "Label";
            worksheet.Cell(1, 4).Style.Font.SetBold();
            worksheet.Cell(1, 5).Value = "GroupNumber";
            worksheet.Cell(1, 5).Style.Font.SetBold();

            // Hidden Field Headers
            worksheet.Cell(1, 27).Value = $"OldName";
            worksheet.Cell(1, 27).Style.Font.SetBold();
            worksheet.Column(27).Hide();

            worksheet.Columns().AdjustToContents();
            worksheet.Rows().AdjustToContents();

            return true;
        }

        public static GenericResponse<bool> CompareTestComponents(TestSpecification testSpecification, TestSpecification testSpecificationCompare, GenericResponse<bool> response)
        {
            var testComponents = testSpecification.TestComponents.ToList();
            var testComponentsCompare = testSpecificationCompare.TestComponents.ToList();

            foreach (var testComponent in testComponents)
            {
                var testComponentCompare = testComponentsCompare.FirstOrDefault(x => (x.TestSpecificationDescription == testComponent.TestSpecificationDescription)
                                                                                && (x.TestComponentTypeName == testComponent.TestComponentTypeName)
                                                                                && (x.Name == testComponent.Name));
                if (testComponentCompare == null)
                {
                    response.Messages.Add($"Inserted: Test Component, Parent: {testComponent.TestComponentTypeName}, Row: {testComponent.Name}");
                    continue;
                }
                response = EqualsModel(testComponent, testComponentCompare, response);
            }

            foreach (var testComponentCompare in testComponentsCompare)
            {
                var testComponent = testComponents.FirstOrDefault(x => (x.TestComponentTypeName == testComponentCompare.TestComponentTypeName)
                                                                  && (x.TestSpecificationDescription == testComponentCompare.TestSpecificationDescription)
                                                                  && (x.Name == testComponentCompare.Name));
                if (testComponent == null)
                {
                    response.Messages.Add($"Deleted: Test Component, Old Parent: {testComponentCompare.TestComponentTypeName}, Old Value: {testComponentCompare.Name}");
                }
            }

            return response;
        }

        public static GenericResponse<bool> EqualsModel(this TestComponent testComponent, TestComponent testComponentCompare, GenericResponse<bool> response)
        {
            var startMessageCount = response.Messages.Count;
            var sheetName = Constants.TestComponents;
            var parents = $"Parent: {testComponent.TestComponentTypeName}";
            var noParents = "";
            response = testComponent.TestSpecificationDescription.CompareField(testComponentCompare.TestSpecificationDescription, "TestSpecificationDescription", response, sheetName, noParents);
            response = testComponent.TestComponentTypeName.CompareField(testComponentCompare.TestComponentTypeName, "TestComponentTypeName", response, sheetName, noParents);
            response = testComponent.ComponentNumber.CompareField(testComponentCompare.ComponentNumber, "ComponentNumber", response, sheetName, parents);
            response = testComponent.Label.CompareField(testComponentCompare.Label, "Label", response, sheetName, parents);
            response = testComponent.Name.CompareField(testComponentCompare.Name, "Name", response, sheetName, parents);
            response = testComponent.GroupNumber.CompareField(testComponentCompare.GroupNumber, "GroupNumber", response, sheetName, parents);
            if (response.Messages.Count > startMessageCount)
            {
                testComponent.ModelState = ModelState.altered;
            }

            return response;
        }
    }

}
