using F1Solutions.Global.Common.Logging;
using ClosedXML.Excel;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using HtmlAgilityPack;
using Ncms.Bl.TestSpecifications.Extensions;
using Ncms.Contracts.Models.TestSpecification;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncms.Bl.TestSpecifications
{
    public static class RubricMarkingBandSpreadsheetService
    {
        private static bool ValidateHtml(string data)
        {
            var htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(data);

            var parseErrors = htmlDocument.ParseErrors;
            var htmlValid = !parseErrors.Any();

            return htmlValid;
        }

        public static GenericResponse<TestSpecification> ValidateSheet(XLWorkbook workbook, TestSpecification testSpecification)
        {
            var result = new GenericResponse<TestSpecification>()
            {
                Data = testSpecification
            };

            try
            {
                var rubricMarkingAssessmentCriterions = testSpecification.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency)
                                                                                                   .SelectMany(x => x.RubricMarkingAssessmentCriterion).ToList();
                var worksheet = workbook.Worksheet(Constants.RubricMarkingBand);

                var lastRow = worksheet.LastRowUsed().RowNumber();
                if (lastRow < 2)
                {
                    result.Errors.Add("Failed: Rubric Marking Band, Reason: No Rubric Marking Bands found in worksheet, " +
                        "this test specification requires records for Rubric Competency, Assessment Criterion, and Marking Band.");
                    result.Success = false;
                    return result;
                }

                var currTestComponentTypeName = string.Empty;
                var currCompetency = string.Empty;
                var currRMarkingAssessmentCriteria = string.Empty;

                var levels = new List<int>();

                var i = 2;
                var keepGoing = true;
                while (keepGoing)
                {
                    var oldTestComponentTypeData = worksheet.Cell(i, 27).GetValue<string>();
                    var rubricMarkingBandData = worksheet.Cell(i, 4).GetValue<string>();
                    var descriptionData = worksheet.Cell(i, 5).GetValue<string>();

                    if (string.IsNullOrEmpty(oldTestComponentTypeData) && string.IsNullOrEmpty(rubricMarkingBandData))
                    {
                        keepGoing = false;
                        break;
                    }


                    var testComponentTypeData = worksheet.Cell(i, 1).GetValue<string>();
                    var competencyData = worksheet.Cell(i, 2).GetValue<string>();
                    var rMarkingAssessmentCriteriaData = worksheet.Cell(i, 3).GetValue<string>();
                    var level = worksheet.Cell(i, 6).GetValue<int>();

                    if (currTestComponentTypeName != testComponentTypeData)
                    {
                        currTestComponentTypeName = testComponentTypeData;
                        levels.Clear();
                    }
                    if (currCompetency != competencyData)
                    {
                        currCompetency = competencyData;
                        levels.Clear();
                    }
                    if (currRMarkingAssessmentCriteria != rMarkingAssessmentCriteriaData)
                    {
                        currRMarkingAssessmentCriteria = rMarkingAssessmentCriteriaData;
                        levels.Clear();
                    }

                    levels.Add(level);

                    if (levels.Count != levels.Distinct().Count())
                    {
                        result.Errors.Add("Failed: Rubric Marking Band, Reason: Test Component Type, Competency, and Rubric Marking Assessment Criteria " +
                            "cannot have duplicate levels");
                        result.Success = false;
                        i++;
                        continue;
                    }

                    var htmlValid = ValidateHtml(descriptionData);

                    if (!htmlValid)
                    {
                        result.Errors.Add($"Failed: Rubric Marking Band, Reason: Description contains invalid html, " +
                            $"Parents: Test Component Type {testComponentTypeData}, Rubric Marking Competency {competencyData}, " +
                            $"Rubric Marking Assessment Criterion Name {rMarkingAssessmentCriteriaData}, Label {worksheet.Cell(i, 4).GetValue<string>()}");
                        result.Success = false;
                        i++;
                        continue;
                    };

                    if (string.IsNullOrEmpty(oldTestComponentTypeData) && !string.IsNullOrEmpty(rubricMarkingBandData))
                    {
                        var rubricMarkingAssessmentCriterionForInsert = rubricMarkingAssessmentCriterions.FirstOrDefault(x => (x.Name == rMarkingAssessmentCriteriaData)
                                                                                                                     && (x.TestComponentTypeName == testComponentTypeData)
                                                                                                                     && (x.Competency == competencyData));
                        if (rubricMarkingAssessmentCriterionForInsert == null)
                        {
                            result.Errors.Add($"Failed: Rubric Marking Band, Reason: Rubric Marking Assessment Criterion {rMarkingAssessmentCriteriaData} does not exist, " +
                                              $"Parents: Test Component Type {testComponentTypeData}, Rubric Marking Competency {competencyData}");
                            result.Success = false;
                            i++;
                            continue;
                        }

                        var rubricMarkingBandInsert = new RubricMarkingBand()
                        {
                            Label = worksheet.Cell(i, 4).GetValue<string>(),
                            Description = worksheet.Cell(i, 5).GetValue<string>(),
                            Level = worksheet.Cell(i, 6).GetValue<int>(),
                            ModelState = ModelState.inserted,

                            // Natural Keys
                            TestComponentTypeName = testComponentTypeData,
                            RMarkingAssessmentCriterionName = rMarkingAssessmentCriteriaData,
                            Competency = competencyData,
                            NaturalKey = worksheet.Cell(i, 4).GetValue<string>()
                        };

                        rubricMarkingAssessmentCriterionForInsert.RubricMarkingBand.Add(rubricMarkingBandInsert);
                        i++;
                        continue;
                    }

                    var rubricMarkingAssessmentCriterion = rubricMarkingAssessmentCriterions.FirstOrDefault(x => x.NaturalKey == rMarkingAssessmentCriteriaData);
                    if (rubricMarkingAssessmentCriterion == null)
                    {
                        result.Errors.Add($"Failed: Rubric Marking Band, Reason: Rubric Marking Assessment Criterion {rMarkingAssessmentCriteriaData} does not exist, " +
                                                               $"Parents: Test Component Type {testComponentTypeData}, Rubric Marking Competency {competencyData}");
                        result.Success = false;
                        i++;
                        continue;
                    }
                    var rubricMarkingBand = new RubricMarkingBand()
                    {
                        Label = worksheet.Cell(i, 4).GetValue<string>(),
                        Description = worksheet.Cell(i, 5).GetValue<string>(),
                        Level = worksheet.Cell(i, 6).GetValue<int>(),

                        // Natural Keys
                        TestComponentTypeName = testComponentTypeData,
                        RMarkingAssessmentCriterionName = rMarkingAssessmentCriteriaData,
                        Competency = competencyData,
                        NaturalKey = worksheet.Cell(i, 4).GetValue<string>(),
                    };

                    rubricMarkingAssessmentCriterion.RubricMarkingBand.Add(rubricMarkingBand);

                    i++;
                }

                return result;
            }
            catch (FormatException ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add($"Failed: Rubric Marking Band, Reason: Incorrect Values Used");
                result.Success = false;
                return result;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(ex.ToString());
                result.Errors.Add("Failed: Rubric Marking Band, Reason: Unexpected error occured");
                result.Success = false;
                return result;
            }
        }

        public static bool WriteRubricMarkingBand(XLWorkbook document, TestSpecification testSpecification)
        {
            var rubricMarkingBandData = testSpecification.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency)
                                                                                           .SelectMany(x => x.RubricMarkingAssessmentCriterion)
                                                                                           .SelectMany(x => x.RubricMarkingBand).ToList();
            var worksheet = document.Worksheets.Add(Constants.RubricMarkingBand);

            var i = 2;
            foreach (var result in rubricMarkingBandData)
            {
                worksheet.Cell(i, 1).Value = result.TestComponentTypeName;
                worksheet.Cell(i, 2).Value = result.Competency;
                worksheet.Cell(i, 3).Value = result.RMarkingAssessmentCriterionName;
                worksheet.Cell(i, 4).Value = result.Label;
                worksheet.Cell(i, 5).Value = result.Description;
                worksheet.Cell(i, 6).Value = result.Level;

                // Hidden Fields
                worksheet.Cell(i, 27).Value = string.Empty;
                worksheet.Cell(i, 28).Value = string.Empty;
                worksheet.Cell(i, 29).Value = string.Empty;
                worksheet.Cell(i, 30).Value = string.Empty;

                i++;
            }

            worksheet.Cell(1, 1).Value = "TestComponentTypeName";
            worksheet.Cell(1, 1).Style.Font.SetBold();
            worksheet.Cell(1, 2).Value = "Competency";
            worksheet.Cell(1, 2).Style.Font.SetBold();
            worksheet.Cell(1, 3).Value = "RMarkingAssessmentCriterionName";
            worksheet.Cell(1, 3).Style.Font.SetBold();
            worksheet.Columns(1, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(226, 239, 218);
            worksheet.Cell(1, 4).Value = "Label";
            worksheet.Cell(1, 4).Style.Font.SetBold();
            worksheet.Column(4).Style.Fill.BackgroundColor = XLColor.FromArgb(231, 230, 230);
            worksheet.Cell(1, 5).Value = "Description";
            worksheet.Cell(1, 5).Style.Font.SetBold();
            worksheet.Cell(1, 6).Value = "Level";
            worksheet.Cell(1, 6).Style.Font.SetBold();

            // Hidden Field Headers
            worksheet.Cell(1, 27).Value = "TestComponentTypeName";
            worksheet.Cell(1, 27).Style.Font.SetBold();
            worksheet.Column(27).Hide();
            worksheet.Cell(1, 28).Value = "Competency";
            worksheet.Cell(1, 28).Style.Font.SetBold();
            worksheet.Column(28).Hide();
            worksheet.Cell(1, 29).Value = "RMarkingAssessmentCriterionName";
            worksheet.Cell(1, 29).Style.Font.SetBold();
            worksheet.Column(29).Hide();
            worksheet.Cell(1, 30).Value = "OldLabel";
            worksheet.Cell(1, 30).Style.Font.SetBold();
            worksheet.Column(30).Hide();

            worksheet.Columns().AdjustToContents();
            worksheet.Rows().AdjustToContents();

            return true;
        }
        public static GenericResponse<bool> CompareRMarkingBandService(TestSpecification testSpecification, TestSpecification testSpecificationCompare,
                                                                                     GenericResponse<bool> response)
        {
            var rubricMarkingBands = testSpecification.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency)
                                                                         .SelectMany(x => x.RubricMarkingAssessmentCriterion)
                                                                         .SelectMany(x => x.RubricMarkingBand).ToList();
            var rubricMarkingBandsCompare = testSpecificationCompare.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency)
                                                                         .SelectMany(x => x.RubricMarkingAssessmentCriterion)
                                                                         .SelectMany(x => x.RubricMarkingBand).ToList();

            foreach (var rubricMarkingBand in rubricMarkingBands)
            {
                var rubricMarkingBandCompare = rubricMarkingBandsCompare.FirstOrDefault(x => (x.Label == rubricMarkingBand.Label)
                                                                       && (x.RMarkingAssessmentCriterionName == rubricMarkingBand.RMarkingAssessmentCriterionName)
                                                                       && (x.TestComponentTypeName == rubricMarkingBand.TestComponentTypeName));
                if (rubricMarkingBandCompare == null)
                {
                    response.Messages.Add($"Inserted: Rubric Marking Band, Parents: {rubricMarkingBand.TestComponentTypeName}, {rubricMarkingBand.RMarkingAssessmentCriterionName}, {rubricMarkingBand.Competency}, " +
                        $"Row: {rubricMarkingBand.Label}");
                    continue;
                }
                response = EqualsModel(rubricMarkingBand, rubricMarkingBandCompare, response);
            }

            foreach (var rubricMarkingBandCompare in rubricMarkingBandsCompare)
            {
                var rubricMarkingBand = rubricMarkingBands.FirstOrDefault(x => (x.Label == rubricMarkingBandCompare.Label)
                                                                       && (x.RMarkingAssessmentCriterionName == rubricMarkingBandCompare.RMarkingAssessmentCriterionName)
                                                                       && (x.TestComponentTypeName == rubricMarkingBandCompare.TestComponentTypeName));
                if (rubricMarkingBand == null)
                {
                    response.Messages.Add($"Deleted: Rubric Marking Band, Old Parents {rubricMarkingBandCompare.TestComponentTypeName}, {rubricMarkingBandCompare.RMarkingAssessmentCriterionName}," +
                        $" {rubricMarkingBandCompare.Competency}, Old Value: {rubricMarkingBandCompare.Label}");
                }
            }

            return response;
        }

        public static GenericResponse<bool> EqualsModel(this RubricMarkingBand rubricMarkingBand,
                                                        RubricMarkingBand rubricMarkingBandCompare, GenericResponse<bool> response)
        {
            var startMessageCount = response.Messages.Count;
            var sheetName = Constants.RubricMarkingBand;
            var parents = $"Parents: {rubricMarkingBand.TestComponentTypeName}, {rubricMarkingBand.RMarkingAssessmentCriterionName}, {rubricMarkingBand.Competency}";
            var noParents = "";

            response = rubricMarkingBand.TestComponentTypeName.CompareField(rubricMarkingBandCompare.TestComponentTypeName, "TestComponentTypeName", response, sheetName, noParents);
            response = rubricMarkingBand.RMarkingAssessmentCriterionName.CompareField(rubricMarkingBandCompare.RMarkingAssessmentCriterionName, "RMarkingAssessmentCriterionName", response, sheetName, noParents);
            response = rubricMarkingBand.Competency.CompareField(rubricMarkingBandCompare.Competency, "Competency", response, sheetName, noParents);
            response = rubricMarkingBand.Label.CompareField(rubricMarkingBandCompare.Label, "Label", response, sheetName, parents);
            response = rubricMarkingBand.Description.CompareField(rubricMarkingBandCompare.Description, "Description", response, sheetName, parents);
            response = rubricMarkingBand.Level.CompareField(rubricMarkingBandCompare.Level, "Level", response, sheetName, parents);

            if (response.Messages.Count > startMessageCount)
            {
                rubricMarkingBand.ModelState = ModelState.altered;
            }

            return response;
        }
    }
}

