using Ncms.Contracts.Models.TestSpecification;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ncms.Bl.TestSpecifications.Extensions
{
    public static class UpdateDomainExtensions
    {
        public static void UpdateDomain(this TestSpecification testSpecificationModel, F1Solutions.Naati.Common.Dal.Domain.TestSpecification testSpecificationDomain)
        {

            if (testSpecificationDomain == null)
            {
                throw new Exception("Test Specification not found for domain.");
            }

            testSpecificationDomain.Description = testSpecificationModel.Description;
            testSpecificationDomain.Active = testSpecificationModel.Active;
            testSpecificationDomain.AutomaticIssuing = testSpecificationModel.AutomaticIssuing;
            testSpecificationDomain.MaxScoreDifference = testSpecificationModel.MaxScoreDifference;
            testSpecificationDomain.ResultAutoCalculation = testSpecificationModel.ResultAutoCalculation;
        }

        public static void UpdateDomain(this TestComponentType testComponentTypeModel, IList<F1Solutions.Naati.Common.Dal.Domain.TestComponentType> testComponentTypeDomains)
        {
            var testComponentTypeDomain = testComponentTypeDomains.FirstOrDefault(x => (x.TestSpecification.Description == testComponentTypeModel.TestSpecificationDescription)
                                                                                  && (x.Name == testComponentTypeModel.NaturalKey));

            if (testComponentTypeDomain == null)
            {
                throw new Exception("Test Component Type not found for domain.");
            }

            testComponentTypeDomain.Label = testComponentTypeModel.Label;
            testComponentTypeDomain.Name = testComponentTypeModel.Name;
            testComponentTypeDomain.Description = testComponentTypeModel.Description;
            var value = testComponentTypeModel.TestComponentBaseTypeId;
            var testComponentTypeDomainTestComponentBaseTypeName = (F1Solutions.Naati.Common.Dal.Domain.TestComponentBaseTypeName)value;
            testComponentTypeDomain.MinExaminerCommentLength = testComponentTypeModel.MinExaminerCommentLength;
            testComponentTypeDomain.MinNaatiCommentLength = testComponentTypeModel.MinNaatiCommentLength;
            testComponentTypeDomain.RoleplayersRequired = testComponentTypeModel.RoleplayersRequired;
            testComponentTypeDomain.CandidateBriefRequired = testComponentTypeModel.CandidateBriefRequired;
            testComponentTypeDomain.CandidateBriefAvailabilityDays = testComponentTypeModel.CandidateBriefavailabilitydays;
            testComponentTypeDomain.DefaultMaterialRequestHours = testComponentTypeModel.DefaultMaterialRequestHours;
            testComponentTypeDomain.DefaultMaterialRequestDueDays = (int)testComponentTypeModel.DefaultMaterialRequestDueDays;
        }

        public static void UpdateDomain(this TestComponent testComponentModel, IList<F1Solutions.Naati.Common.Dal.Domain.TestComponent> testComponentDomains)
        {
            var testComponentDomain = testComponentDomains.FirstOrDefault(x => (x.TestSpecification.Description == testComponentModel.TestSpecificationDescription)
                                                                          && (x.Type.Name == testComponentModel.TestComponentTypeName)
                                                                          && (x.Name == testComponentModel.NaturalKey));

            if (testComponentDomain == null)
            {
                throw new Exception("Test Component not found for domain.");
            }
            testComponentDomain.ComponentNumber = testComponentModel.ComponentNumber;
            testComponentDomain.Label = testComponentModel.Label;
            testComponentDomain.Name = testComponentModel.Name;
            testComponentDomain.GroupNumber = testComponentModel.GroupNumber;
        }

        public static void UpdateDomain(this TestComponentTypeStandardMarkingScheme testComponentTypeStandardMarkingSchemeModel, 
                                        IList<F1Solutions.Naati.Common.Dal.Domain.TestComponentTypeStandardMarkingScheme> testComponentTypeStandardMarkingSchemeDomains)
        {
            var testComponentTypeStandardMarkingSchemeDomain = testComponentTypeStandardMarkingSchemeDomains.FirstOrDefault(x => x.TestComponentType.Name == testComponentTypeStandardMarkingSchemeModel.ComponentType);
            if (testComponentTypeStandardMarkingSchemeDomain == null)
            {
                throw new Exception("Test Component Type Standard Marking Scheme not found for domain.");
            }

            //testComponentTypeStandardMarkingSchemeDomain.TestComponentType.Name = testComponentTypeStandardMarkingSchemeModel.ComponentType;
            testComponentTypeStandardMarkingSchemeDomain.TotalMarks = testComponentTypeStandardMarkingSchemeModel.TotalMarks;
            testComponentTypeStandardMarkingSchemeDomain.PassMark = testComponentTypeStandardMarkingSchemeModel.PassMark;
        }

        public static void UpdateDomain(this TestSpecificationStandardMarkingScheme testStandardMarkingSchemeModel,
                                        IList<F1Solutions.Naati.Common.Dal.Domain.TestSpecificationStandardMarkingScheme> testStandardMarkingSchemeDomains)
        {
            var testStandardMarkingSchemeDomain = testStandardMarkingSchemeDomains.FirstOrDefault(x => x.TestSpecification.Description == testStandardMarkingSchemeModel.TestSpecificationDescription);
            if (testStandardMarkingSchemeDomain == null)
            {
                throw new Exception("Test Specification Standard Marking Scheme not found for domain.");
            }

            //testStandardMarkingSchemeDomain.TestSpecification.Description = testStandardMarkingSchemeModel.TestSpecificationDescription;
            testStandardMarkingSchemeDomain.OverallPassMark = testStandardMarkingSchemeModel.OverallPassMark;
        }

        public static void UpdateDomain(this RubricMarkingCompetency rubricMarkingCompetencyModel,
                                        IList<F1Solutions.Naati.Common.Dal.Domain.RubricMarkingCompetency> rubricMarkingCompetencyDomains)
        {
            var rubricMarkingCompetencyDomain = rubricMarkingCompetencyDomains.FirstOrDefault(x => (x.TestComponentType.Name == rubricMarkingCompetencyModel.ComponentType)
                                                                                              && (x.Name == rubricMarkingCompetencyModel.NaturalKey));
            if (rubricMarkingCompetencyDomain == null)
            {
                throw new Exception("Rubric Marking Competency not found for domain.");
            }

            rubricMarkingCompetencyDomain.Name = rubricMarkingCompetencyModel.Name;
            rubricMarkingCompetencyDomain.Label = rubricMarkingCompetencyModel.Label;
            rubricMarkingCompetencyDomain.DisplayOrder = rubricMarkingCompetencyModel.DisplayOrder;
        }

        public static void UpdateDomain(this RubricMarkingAssessmentCriterion rMarkingAssessmentCriterionModel,
                                        IList<F1Solutions.Naati.Common.Dal.Domain.RubricMarkingAssessmentCriterion> rMarkingAssessmentCriterionDomains)
        {
            var rMarkingAssessmentCriterionDomain = rMarkingAssessmentCriterionDomains.FirstOrDefault(x => (x.RubricMarkingCompetency.TestComponentType.Name == rMarkingAssessmentCriterionModel.TestComponentTypeName)
                                                                                                      && (x.Name == rMarkingAssessmentCriterionModel.NaturalKey)
                                                                                                      && (x.RubricMarkingCompetency.Name == rMarkingAssessmentCriterionModel.Competency));
            if (rMarkingAssessmentCriterionDomain == null)
            {
                throw new Exception("Rubric Marking Assessment Criterion not found for domain.");
            }
            rMarkingAssessmentCriterionDomain.Name = rMarkingAssessmentCriterionModel.Name;
            rMarkingAssessmentCriterionDomain.Label = rMarkingAssessmentCriterionModel.Label;
            rMarkingAssessmentCriterionDomain.DisplayOrder = rMarkingAssessmentCriterionModel.DisplayOrder;
        }

        public static void UpdateDomain(this RubricMarkingBand rubricMarkingBandModel,
                                        IList<F1Solutions.Naati.Common.Dal.Domain.RubricMarkingBand> rubricMarkingBandDomains)
        {
            var rubricMarkingBandDomain = rubricMarkingBandDomains
                                          .FirstOrDefault(x => (x.RubricMarkingAssessmentCriterion.RubricMarkingCompetency.TestComponentType.Name == rubricMarkingBandModel.TestComponentTypeName)
                                                          && (x.RubricMarkingAssessmentCriterion.Name == rubricMarkingBandModel.RMarkingAssessmentCriterionName)
                                                          && (x.Label == rubricMarkingBandModel.NaturalKey));
            if (rubricMarkingBandDomain == null)
            {
                throw new Exception("Rubric Marking Band not found for domain.");
            }

            rubricMarkingBandDomain.Label = rubricMarkingBandModel.Label;
            rubricMarkingBandDomain.Description = rubricMarkingBandModel.Description;
            rubricMarkingBandDomain.Level = rubricMarkingBandModel.Level;
        }
    }
}
