using F1Solutions.Global.Common.Logging;
using ClosedXML.Excel;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Bl.TestSpecifications.Extensions;
using Ncms.Contracts.Models.TestSpecification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Bl.TestSpecifications
{
    public static class RubricMarkingCompetencySpreadsheetService
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
                var worksheet = workbook.Worksheet(Constants.RubricMarkingCompentency);

                var lastRow = worksheet.LastRowUsed().RowNumber();
                if (lastRow < 2)
                {
                    result.Errors.Add("Failed: Rubric Marking Competencies, Reason: No Rubric Marking Competencies found in worksheet, " +
                        "this test specification requires records for Rubric Competency, Assessment Criterion, and Marking Band.");
                    result.Success = false;
                    return result;
                }

                var currTestComponentTypeName = string.Empty;
                var competencies = new List<string>();
                var displayOrders = new List<int>();

                var i = 2;
                var keepGoing = true;
                while (keepGoing)
                { 

                    var oldTestComponentTypeData = worksheet.Cell(i, 27).GetValue<string>();
                    var rubricMarkingCompetencyData = worksheet.Cell(i, 2).GetValue<string>();

                    if (string.IsNullOrEmpty(oldTestComponentTypeData) && string.IsNullOrEmpty(rubricMarkingCompetencyData))
                    {
                        keepGoing = false;
                        break;
                    }


                    var testComponentTypeName = worksheet.Cell(i, 1).GetValue<string>();
                    
                    if (currTestComponentTypeName != testComponentTypeName)
                    {
                        currTestComponentTypeName = testComponentTypeName;
                        competencies.Clear();
                        displayOrders.Clear();
                    }

                    competencies.Add(rubricMarkingCompetencyData);
                    displayOrders.Add(worksheet.Cell(i, 4).GetValue<int>());

                    if (competencies.Count != competencies.Distinct().Count())
                    {
                        result.Errors.Add("Failed: Rubric Marking Competency, Reason: Test Component Types cannot have duplicate Rubric Marking Competency Names");
                        result.Success = false;
                        i++;
                        continue;
                    }

                    if (displayOrders.Count != displayOrders.Distinct().Count())
                    {
                        result.Errors.Add("Failed: Rubric Marking Competency, Reason: Test Component Types cannot have duplicate Rubric Marking Competency Display Orders");
                        result.Success = false;
                        i++;
                        continue;
                    }



                    if (string.IsNullOrEmpty(oldTestComponentTypeData) && !string.IsNullOrEmpty(rubricMarkingCompetencyData))
                    {
                        var testComponentTypeForInsert = testComponentTypes.FirstOrDefault(x => x.Name == testComponentTypeName);
                        if (testComponentTypeForInsert == null)
                        {
                            result.Errors.Add($"Failed: Rubric Marking Competency, " +
                                $"Reason: Test Component Type {testComponentTypeName} does not exist");
                            result.Success = false;
                            i++;
                            continue;
                        }
                        var rubricMarkingCompetencyInsert = new RubricMarkingCompetency()
                        {
                            Name = worksheet.Cell(i, 2).GetValue<string>(),
                            Label = worksheet.Cell(i, 3).GetValue<string>(),
                            DisplayOrder = worksheet.Cell(i, 4).GetValue<int>(),
                            ModelState = ModelState.inserted,

                            // Natural Keys
                            ComponentType = testComponentTypeName,
                            NaturalKey = worksheet.Cell(i, 28).GetValue<string>()
                        };
                        testComponentTypeForInsert.RubricMarkingCompetency.Add(rubricMarkingCompetencyInsert);
                        i++;
                        continue;
                    }

                    var testComponentType = testComponentTypes.FirstOrDefault(x => x.NaturalKey == testComponentTypeName);
                    if (testComponentType == null)
                    {
                        result.Errors.Add($"Failed: Rubric Marking Competency, " +
                                $"Reason: Test Component Type {testComponentTypeName} does not exist");
                        result.Success = false;
                        i++;
                        continue;
                    }
                    var rubricMarkingCompetency = new RubricMarkingCompetency()
                    {
                        Name = worksheet.Cell(i, 2).GetValue<string>(),
                        Label = worksheet.Cell(i, 3).GetValue<string>(),
                        DisplayOrder = worksheet.Cell(i, 4).GetValue<int>(),
                        ModelState = ModelState.inserted,

                        // Natural Keys
                        ComponentType = testComponentTypeName,
                        NaturalKey = worksheet.Cell(i, 28).GetValue<string>()
                    };

                    testComponentType.RubricMarkingCompetency.Add(rubricMarkingCompetency);

                    i++;
                }

                return result;
            }
            catch (FormatException ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add($"Failed: Rubric Marking Competency, Reason: Incorrect Values Used");
                result.Success = false;
                return result;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add("Failed: Rubric Marking Competency, Reason: Unexpected error occured");
                result.Success = false;
                return result;
            }
        }

        public static bool WriteRubricMarkingCompetency(XLWorkbook document, TestSpecification testSpecification)
        {
            var rubricMarkingCompetencyData = testSpecification.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency).ToList();
            var worksheet = document.Worksheets.Add(Constants.RubricMarkingCompentency);

            var i = 2;
            foreach (var result in rubricMarkingCompetencyData)
            {
                worksheet.Cell(i, 1).Value = result.ComponentType;
                worksheet.Cell(i, 2).Value = result.Name;
                worksheet.Cell(i, 3).Value = result.Label;
                worksheet.Cell(i, 4).Value = result.DisplayOrder;
                i++;

                // Hidden Fields
                worksheet.Cell(i, 27).Value = string.Empty;
                worksheet.Cell(i, 28).Value = string.Empty;
            }

            
            worksheet.Cell(1, 1).Value = "ComponentType";
            worksheet.Cell(1, 1).Style.Font.SetBold();
            worksheet.Column(1).Style.Fill.BackgroundColor = XLColor.FromArgb(226, 239, 218);
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 2).Style.Font.SetBold();
            worksheet.Column(2).Style.Fill.BackgroundColor = XLColor.FromArgb(231, 230, 230);
            worksheet.Cell(1, 3).Value = "Label";
            worksheet.Cell(1, 3).Style.Font.SetBold();
            worksheet.Cell(1, 4).Value = "DisplayOrder";
            worksheet.Cell(1, 4).Style.Font.SetBold();

            // Hidden Field Headers
            worksheet.Cell(1, 27).Value = "OldComponentType";
            worksheet.Cell(1, 27).Style.Font.SetBold();
            worksheet.Column(27).Hide();
            worksheet.Cell(1, 28).Value = $"OldName";
            worksheet.Cell(1, 28).Style.Font.SetBold();
            worksheet.Column(28).Hide();

            worksheet.Columns().AdjustToContents();
            worksheet.Rows().AdjustToContents();

            return true;
        }

        public static GenericResponse<bool> CompareRubricMarkingCompetency(TestSpecification testSpecification, TestSpecification testSpecificationCompare,
                                                                                     GenericResponse<bool> response)
        {
            var rubricMarkingCompetencies = testSpecification.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency).ToList();
            var rubricMarkingCompetenciesCompare = testSpecificationCompare.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency).ToList();

            foreach (var rubricMarkingCompetency in rubricMarkingCompetencies)
            {
                var rubricMarkingCompetencyCompare = rubricMarkingCompetenciesCompare
                                                       .FirstOrDefault(x => (x.ComponentType == rubricMarkingCompetency.ComponentType)
                                                                       && (x.Name == rubricMarkingCompetency.Name));
                if (rubricMarkingCompetencyCompare == null)
                {
                    response.Messages.Add($"Inserted: Rubric Marking Competency, Parent: {rubricMarkingCompetency.ComponentType}, Row: {rubricMarkingCompetency.Name}");
                    continue;
                }
                response = EqualsModel(rubricMarkingCompetency, rubricMarkingCompetencyCompare, response);
            }

            foreach (var rubricMarkingCompetencyCompare in rubricMarkingCompetenciesCompare)
            {
                var rubricMarkingCompetency = rubricMarkingCompetencies
                                                       .FirstOrDefault(x => (x.ComponentType == rubricMarkingCompetencyCompare.ComponentType)
                                                                       && (x.Name == rubricMarkingCompetencyCompare.Name));
                if (rubricMarkingCompetency == null)
                {
                    response.Messages.Add($"Deleted: Rubric Marking Competency, Old Parent: {rubricMarkingCompetencyCompare.ComponentType}, Old Value: {rubricMarkingCompetencyCompare.Name}");
                }
            }

            return response;
        }

        public static GenericResponse<bool> EqualsModel(this RubricMarkingCompetency rubricMarkingCompetency,
                                                        RubricMarkingCompetency rubricMarkingCompetencyCompare, GenericResponse<bool> response)
        {
            var startMessageCount = response.Messages.Count;
            var sheetName = Constants.RubricMarkingCompentency;
            var parents = $"Parent: {rubricMarkingCompetency.ComponentType}";
            var noParents = "";
            response = rubricMarkingCompetency.ComponentType.CompareField(rubricMarkingCompetencyCompare.ComponentType, "TaskTypeDescription", response, sheetName, noParents);
            response = rubricMarkingCompetency.Name.CompareField(rubricMarkingCompetencyCompare.Name, "Name", response, sheetName, parents);
            response = rubricMarkingCompetency.Label.CompareField(rubricMarkingCompetencyCompare.Label, "Label", response, sheetName, parents);
            response = rubricMarkingCompetency.DisplayOrder.CompareField(rubricMarkingCompetencyCompare.DisplayOrder, "DisplayOrder", response, sheetName, parents);

            if (response.Messages.Count > startMessageCount)
            {
                rubricMarkingCompetency.ModelState = ModelState.altered;
            }

            return response;
        }
    }

}
