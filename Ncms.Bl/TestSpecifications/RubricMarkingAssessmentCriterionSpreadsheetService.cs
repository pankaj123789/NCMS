using ClosedXML.Excel;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Bl.TestSpecifications.Extensions;
using Ncms.Contracts.Models.TestSpecification;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncms.Bl.TestSpecifications
{
    public static class RubricMarkingAssessmentCriterionSpreadsheetService
    {
        public static GenericResponse<TestSpecification> ValidateSheet(XLWorkbook workbook, TestSpecification testSpecification)
        {
            var result = new GenericResponse<TestSpecification>()
            {
                Data = testSpecification
            };

            try
            {
                var rubricMarkingCompetencies = testSpecification.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency);
                var worksheet = workbook.Worksheet(Constants.RMarkingAssessmentCriterion);

                var lastRow = worksheet.LastRowUsed().RowNumber();
                if (lastRow < 2)
                {
                    result.Errors.Add("Failed: Rubric Marking Assessment Criteria, Reason: No Rubric Marking Assessment Criterion found in worksheet, " +
                        "this test specification requires records for Rubric Competency, Assessment Criterion, and Marking Band.");
                    result.Success = false;
                    return result;
                }

                var currTestComponentTypeName = string.Empty;
                var currCompetency = string.Empty;
                var rMarkingAssessmentCriteriaNames = new List<string>();
                var displayOrders = new List<int>();

                var i = 2;
                var keepGoing = true;
                while (keepGoing)
                {
                    var oldTestComponentTypeData = worksheet.Cell(i, 27).GetValue<string>();
                    var rMarkingAssessmentCriterionData = worksheet.Cell(i, 3).GetValue<string>();
                    if (string.IsNullOrEmpty(oldTestComponentTypeData) && string.IsNullOrEmpty(rMarkingAssessmentCriterionData))
                    {
                        keepGoing = false;
                        break;
                    }

                    var testComponentTypeName = worksheet.Cell(i, 1).GetValue<string>();
                    var competencyName = worksheet.Cell(i, 2).GetValue<string>();
                    var displayOrder = worksheet.Cell(i, 5).GetValue<int>();
                    if (currTestComponentTypeName != testComponentTypeName)
                    {
                        currTestComponentTypeName = testComponentTypeName;
                        rMarkingAssessmentCriteriaNames.Clear();
                        displayOrders.Clear();
                    }
                    if (currCompetency != competencyName)
                    {
                        currCompetency = competencyName;
                        rMarkingAssessmentCriteriaNames.Clear();
                        displayOrders.Clear();
                    }

                    rMarkingAssessmentCriteriaNames.Add(rMarkingAssessmentCriterionData);
                    displayOrders.Add(displayOrder);


                    if (rMarkingAssessmentCriteriaNames.Count != rMarkingAssessmentCriteriaNames.Distinct().Count())
                    {
                        result.Errors.Add("Failed: Rubric Marking Assessment Criterion, Reason: Test Component Type and Competency cannot have duplicate Rubric Marking Assessment Criteria Names");
                        result.Success = false;
                        i++;
                        continue;
                    }

                    if (displayOrders.Count != displayOrders.Distinct().Count())
                    {
                        result.Errors.Add("Failed: Rubric Marking Assessment Criterion, Reason: Test Component Type and Competency cannot have duplicate Display Orders");
                        result.Success = false;
                        i++;
                        continue;
                    }

                    if (string.IsNullOrEmpty(oldTestComponentTypeData) && !string.IsNullOrEmpty(rMarkingAssessmentCriterionData))
                    {
                        var rubricMarkingCompetencyForInsert = rubricMarkingCompetencies.FirstOrDefault(x => (x.ComponentType == testComponentTypeName)
                                                                                                    && (x.Name == competencyName));
                        if (rubricMarkingCompetencyForInsert == null)
                        {
                            result.Errors.Add($"Failed: Rubric Marking Assessment Criterion, " +
                                $"Reason: Rubric Marking Competency {competencyName} does not exist, Parent: Test Component Type {testComponentTypeName}.");
                            result.Success = false;
                            i++;
                            continue;
                        }

                        var rubricMarkingAssessmentCriterionInsert = new RubricMarkingAssessmentCriterion()
                        {
                            Name = worksheet.Cell(i, 3).GetValue<string>(),
                            Label = worksheet.Cell(i, 4).GetValue<string>(),
                            DisplayOrder = worksheet.Cell(i, 5).GetValue<int>(),
                            ModelState = ModelState.inserted,

                            // Natural Keys
                            TestComponentTypeName = testComponentTypeName,
                            Competency = competencyName,
                            NaturalKey = worksheet.Cell(i, 29).GetValue<string>(),
                        };

                        rubricMarkingCompetencyForInsert.RubricMarkingAssessmentCriterion.Add(rubricMarkingAssessmentCriterionInsert);
                        i++;
                        continue;
                    }

                    var rubricMarkingCompetency = rubricMarkingCompetencies.FirstOrDefault(x => x.NaturalKey == worksheet.Cell(i, 3).GetValue<string>());
                    if (rubricMarkingCompetency == null)
                    {
                        result.Errors.Add($"Failed: Rubric Marking Assessment Criterion, " +
                                $"Reason: Rubric Marking Competency {worksheet.Cell(i, 3).GetValue<string>()} does not exist, Parent: Test Component Type {testComponentTypeName}.");
                        result.Success = false;
                        i++;
                        continue;
                    }

                    var rubricMarkingAssessmentCriterion = new RubricMarkingAssessmentCriterion()
                    {
                        Name = worksheet.Cell(i, 3).GetValue<string>(),
                        Label = worksheet.Cell(i, 4).GetValue<string>(),
                        DisplayOrder = worksheet.Cell(i, 5).GetValue<int>(),

                        // Natural Keys
                        TestComponentTypeName = testComponentTypeName,
                        Competency = competencyName,
                        NaturalKey = worksheet.Cell(i, 29).GetValue<string>(),
                    };

                    rubricMarkingCompetency.RubricMarkingAssessmentCriterion.Add(rubricMarkingAssessmentCriterion);

                    i++;
                }

                return result;
            }
            catch (FormatException ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add($"Failed: Rubric Marking Assessment Criterion, Reason: Incorrect Values Used");
                result.Success = false;
                return result;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add("Failed: Rubric Marking Assessment Criterion, Reason: Unexpected error occured");
                result.Success = false;
                return result;
            }
        }

        public static bool WriteRubricMarkingAssessmentCriterion(XLWorkbook document, TestSpecification testSpecification)
        {
            var rubricMarkingAssessmentCriterionData = testSpecification.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency)
                                                                                           .SelectMany(x => x.RubricMarkingAssessmentCriterion).ToList();
            var worksheet = document.Worksheets.Add(Constants.RMarkingAssessmentCriterion);

            var i = 2;
            foreach (var result in rubricMarkingAssessmentCriterionData)
            {
                worksheet.Cell(i, 1).Value = result.TestComponentTypeName;
                worksheet.Cell(i, 2).Value = result.Competency;
                worksheet.Cell(i, 3).Value = result.Name;
                worksheet.Cell(i, 4).Value = result.Label;
                worksheet.Cell(i, 5).Value = result.DisplayOrder;
                i++;

                // Hidden Fields
                worksheet.Cell(i, 27).Value = string.Empty;
                worksheet.Cell(i, 28).Value = string.Empty;
                worksheet.Cell(i, 29).Value = string.Empty;
            }

            worksheet.Cell(1, 1).Value = "TestComponentTypeName";
            worksheet.Cell(1, 1).Style.Font.SetBold();
            worksheet.Cell(1, 2).Value = "Competency";
            worksheet.Cell(1, 2).Style.Font.SetBold();
            worksheet.Columns(1, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(226, 239, 218);
            worksheet.Cell(1, 3).Value = "Name";
            worksheet.Cell(1, 3).Style.Font.SetBold();
            worksheet.Column(3).Style.Fill.BackgroundColor = XLColor.FromArgb(231, 230, 230);
            worksheet.Cell(1, 4).Value = "Label";
            worksheet.Cell(1, 4).Style.Font.SetBold();
            worksheet.Cell(1, 5).Value = "DisplayOrder";
            worksheet.Cell(1, 5).Style.Font.SetBold();

            // Hidden Field Headers
            worksheet.Cell(1, 26).Value = "TestComponentTypeName";
            worksheet.Cell(1, 26).Style.Font.SetBold();
            worksheet.Column(26).Hide();
            worksheet.Cell(1, 27).Value = "Competency";
            worksheet.Cell(1, 27).Style.Font.SetBold();
            worksheet.Column(28).Hide();
            worksheet.Cell(1, 29).Value = $"OldName";
            worksheet.Cell(1, 29).Style.Font.SetBold();
            worksheet.Column(29).Hide();

            worksheet.Columns().AdjustToContents();
            worksheet.Rows().AdjustToContents();

            return true;
        }

        public static GenericResponse<bool> CompareRMarkingAssessmentCriterion(TestSpecification testSpecification, TestSpecification testSpecificationCompare,
                                                                                     GenericResponse<bool> response)
        {
            var rubricMarkingAssessmentCriterions = testSpecification.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency)
                                                                                        .SelectMany(x => x.RubricMarkingAssessmentCriterion).ToList();
            var rubricMarkingAssessmentCriterionsCompare = testSpecificationCompare.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency)
                                                                                        .SelectMany(x => x.RubricMarkingAssessmentCriterion).ToList();

            foreach (var rubricMarkingAssessmentCriterion in rubricMarkingAssessmentCriterions)
            {
                var rubricMarkingAssessmentCriterionCompare = rubricMarkingAssessmentCriterionsCompare
                                                       .FirstOrDefault(x => (x.Name == rubricMarkingAssessmentCriterion.Name)
                                                                       && (x.TestComponentTypeName == rubricMarkingAssessmentCriterion.TestComponentTypeName)
                                                                       && (x.Competency == rubricMarkingAssessmentCriterion.Competency));
                if (rubricMarkingAssessmentCriterionCompare == null)
                {
                    response.Messages.Add($"Inserted: Rubric Marking Assessment Criterion, Parents: {rubricMarkingAssessmentCriterion.TestComponentTypeName}, {rubricMarkingAssessmentCriterion.Competency}, " +
                        $" Row:{rubricMarkingAssessmentCriterion.Name}");
                    continue;
                }
                response = EqualsModel(rubricMarkingAssessmentCriterion, rubricMarkingAssessmentCriterionCompare, response);
            }

            foreach (var rubricMarkingAssessmentCriterionCompare in rubricMarkingAssessmentCriterionsCompare)
            {
                var rubricMarkingAssessmentCriterion = rubricMarkingAssessmentCriterions
                                                       .FirstOrDefault(x => (x.Name == rubricMarkingAssessmentCriterionCompare.Name)
                                                                       && (x.TestComponentTypeName == rubricMarkingAssessmentCriterionCompare.TestComponentTypeName)
                                                                       && (x.Competency == rubricMarkingAssessmentCriterionCompare.Competency));
                if (rubricMarkingAssessmentCriterion == null)
                {
                    response.Messages.Add($"Deleted: Rubric Marking Assessment Criterion, Old Parents: {rubricMarkingAssessmentCriterionCompare.TestComponentTypeName}, {rubricMarkingAssessmentCriterionCompare.Competency}, " +
                        $"Old Value: {rubricMarkingAssessmentCriterionCompare.Name}");
                }
            }

            return response;
        }

        public static GenericResponse<bool> EqualsModel(this RubricMarkingAssessmentCriterion rubricMarkingAssessmentCriterion,
                                                        RubricMarkingAssessmentCriterion rubricMarkingAssessmentCriterionCompare, GenericResponse<bool> response)
        {
            var startMessageCount = response.Messages.Count;
            var sheetName = Constants.RMarkingAssessmentCriterion;
            var parents = $"Parents: {rubricMarkingAssessmentCriterion.TestComponentTypeName}, {rubricMarkingAssessmentCriterion.Competency}";
            var noParents = "";
            response = rubricMarkingAssessmentCriterion.TestComponentTypeName.CompareField(rubricMarkingAssessmentCriterionCompare.TestComponentTypeName, "TestComponentTypeName", response, sheetName, noParents);
            response = rubricMarkingAssessmentCriterion.Competency.CompareField(rubricMarkingAssessmentCriterionCompare.Competency, "Competency", response, sheetName, noParents);
            response = rubricMarkingAssessmentCriterion.Name.CompareField(rubricMarkingAssessmentCriterionCompare.Name, "Name", response, sheetName, parents);
            response = rubricMarkingAssessmentCriterion.Label.CompareField(rubricMarkingAssessmentCriterionCompare.Label, "Label", response, sheetName, parents);
            response = rubricMarkingAssessmentCriterion.DisplayOrder.CompareField(rubricMarkingAssessmentCriterionCompare.DisplayOrder, "DisplayOrder", response, sheetName, parents);

            if (response.Messages.Count > startMessageCount)
            {
                rubricMarkingAssessmentCriterion.ModelState = ModelState.altered;
            }

            return response;
        }
    }
}

