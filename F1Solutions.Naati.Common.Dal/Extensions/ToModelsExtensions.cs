using System.Linq;

namespace Ncms.Bl.TestSpecifications.Extensions
{
    public static class ToModelsExtensions
    {
        public static Contracts.Models.TestSpecification.CredentialType ToModel(this F1Solutions.Naati.Common.Dal.Domain.CredentialType credentialType)
        {
            return new Contracts.Models.TestSpecification.CredentialType()
            {
                InternalName = credentialType.InternalName,
                ExternalName = credentialType.ExternalName,
                DisplayOrder = credentialType.DisplayOrder,
                Simultaneous = credentialType.Simultaneous,
                SkillTypeId = credentialType.SkillType.Id,
                Certification = credentialType.Certification,
                DefaultExpiry = credentialType.DefaultExpiry,
                AllowBackdating = credentialType.AllowBackdating,
                TestSessionBookingAvailabilityWeeks = credentialType.TestSessionBookingAvailabilityWeeks,
                TestSessionBookingClosedWeeks = credentialType.TestSessionBookingClosedWeeks,
                TestSessionBookingRejectHours = credentialType.TestSessionBookingRejectHours,
                AllowAvailabilityNotice = credentialType.AllowAvailabilityNotice,
                TestSpecifications = credentialType.TestSpecifications.Select(x => x.ToModel()).ToList(),
            };
        }

        public static Contracts.Models.TestSpecification.TestSpecification ToModel(this F1Solutions.Naati.Common.Dal.Domain.TestSpecification testSpecification)
        {
            return new Contracts.Models.TestSpecification.TestSpecification()
            {
                CredentialTypeName = testSpecification.CredentialType.InternalName,
                NaturalKey = testSpecification.Description,
                Description = testSpecification.Description,
                Active = testSpecification.Active,
                AutomaticIssuing = testSpecification.AutomaticIssuing,
                MaxScoreDifference = testSpecification.MaxScoreDifference,
                ResultAutoCalculation = testSpecification.ResultAutoCalculation,
                TestComponentTypes = testSpecification.TestComponentTypes.Select(x => x.ToModel()).ToList(),
                TestComponents = testSpecification.TestComponents.Select(x => x.ToModel()).ToList(),
                TestSpecificationStandardMarkingSchemes = testSpecification.TestSpecificationStandardMarkingSchemes.Select(x => x.ToModel()).ToList()
            };
        }

        public static Contracts.Models.TestSpecification.TestComponentType ToModel(this F1Solutions.Naati.Common.Dal.Domain.TestComponentType testComponentType)
        {
            return new Contracts.Models.TestSpecification.TestComponentType()
            {
                TestSpecificationDescription = testComponentType.TestSpecification.Description,
                NaturalKey = testComponentType.Name,
                Name = testComponentType.Name,
                Label = testComponentType.Label,
                Description = testComponentType.Description,
                TestComponentBaseTypeId = testComponentType.TestComponentBaseType.Id,
                MinExaminerCommentLength = testComponentType.MinExaminerCommentLength,
                MinNaatiCommentLength = testComponentType.MinNaatiCommentLength,
                RoleplayersRequired = testComponentType.RoleplayersRequired,
                CandidateBriefRequired = testComponentType.CandidateBriefRequired,
                CandidateBriefavailabilitydays = (int)testComponentType.CandidateBriefAvailabilityDays,
                DefaultMaterialRequestHours = testComponentType.DefaultMaterialRequestHours,
                DefaultMaterialRequestDueDays = testComponentType.DefaultMaterialRequestDueDays,
                TestComponentTypeStandardMarkingScheme = testComponentType.TestComponentTypeStandardMarkingSchemes.Select(x => x.ToModel()).ToList(),
                RubricMarkingCompetency = testComponentType.RubricMarkingCompetencies.Select(x => x.ToModel()).ToList()
            };
        }

        public static Contracts.Models.TestSpecification.TestComponent ToModel(this F1Solutions.Naati.Common.Dal.Domain.TestComponent testComponent)
        {
            return new Contracts.Models.TestSpecification.TestComponent()
            {
                TestSpecificationDescription = testComponent.TestSpecification.Description,
                TestComponentTypeName = testComponent.Type.Name,
                ComponentNumber = testComponent.ComponentNumber,
                Label = testComponent.Label,
                NaturalKey = testComponent.Name,
                Name = testComponent.Name,
                GroupNumber = testComponent.GroupNumber
            };
        }

        public static Contracts.Models.TestSpecification.TestComponentTypeStandardMarkingScheme ToModel(this F1Solutions.Naati.Common.Dal.Domain.TestComponentTypeStandardMarkingScheme testComponentTypeStandardMarkingScheme)
        {
            return new Contracts.Models.TestSpecification.TestComponentTypeStandardMarkingScheme()
            {
                ComponentType = testComponentTypeStandardMarkingScheme.TestComponentType.Name,
                TotalMarks = testComponentTypeStandardMarkingScheme.TotalMarks,
                PassMark = testComponentTypeStandardMarkingScheme.PassMark
            };
        }
        public static Contracts.Models.TestSpecification.TestSpecificationStandardMarkingScheme ToModel(this F1Solutions.Naati.Common.Dal.Domain.TestSpecificationStandardMarkingScheme testSpecificationStandardMarkingScheme)
        {
            return new Contracts.Models.TestSpecification.TestSpecificationStandardMarkingScheme()
            {
                TestSpecificationDescription = testSpecificationStandardMarkingScheme.TestSpecification.Description,
                OverallPassMark = testSpecificationStandardMarkingScheme.OverallPassMark
            };
        }

        public static Contracts.Models.TestSpecification.RubricMarkingCompetency ToModel(this F1Solutions.Naati.Common.Dal.Domain.RubricMarkingCompetency rubricMarkingCompetency)
        {
            return new Contracts.Models.TestSpecification.RubricMarkingCompetency()
            {
                TestSpecificationDescription = rubricMarkingCompetency.TestComponentType.TestSpecification.Description,
                ComponentType = rubricMarkingCompetency.TestComponentType.Name,
                NaturalKey = rubricMarkingCompetency.Name,
                Name = rubricMarkingCompetency.Name,
                Label = rubricMarkingCompetency.Label,
                DisplayOrder = rubricMarkingCompetency.DisplayOrder,
                RubricMarkingAssessmentCriterion = rubricMarkingCompetency.RubricMarkingAssessmentCriteria.Select(x => x.ToModel()).ToList()
            };
        }

        public static Contracts.Models.TestSpecification.RubricMarkingAssessmentCriterion ToModel(this F1Solutions.Naati.Common.Dal.Domain.RubricMarkingAssessmentCriterion rubricMarkingAssessmentCriterion)
        {
            return new Contracts.Models.TestSpecification.RubricMarkingAssessmentCriterion()
            {
                TestSpecificationDescription = rubricMarkingAssessmentCriterion.RubricMarkingCompetency.TestComponentType.TestSpecification.Description,
                TestComponentTypeName = rubricMarkingAssessmentCriterion.RubricMarkingCompetency.TestComponentType.Name,
                Competency = rubricMarkingAssessmentCriterion.RubricMarkingCompetency.Name,
                NaturalKey = rubricMarkingAssessmentCriterion.Name,
                Name = rubricMarkingAssessmentCriterion.Name,
                Label = rubricMarkingAssessmentCriterion.Label,
                DisplayOrder = rubricMarkingAssessmentCriterion.DisplayOrder,
                RubricMarkingBand = rubricMarkingAssessmentCriterion.RubricMarkingBands.Select(x => x.ToModel()).ToList()
            };
        }

        public static Contracts.Models.TestSpecification.RubricMarkingBand ToModel(this F1Solutions.Naati.Common.Dal.Domain.RubricMarkingBand rubricMarkingBand)
        {
            return new Contracts.Models.TestSpecification.RubricMarkingBand()
            {
                TestSpecificationDescription = rubricMarkingBand.RubricMarkingAssessmentCriterion.RubricMarkingCompetency.TestComponentType.TestSpecification.Description,
                TestComponentTypeName = rubricMarkingBand.RubricMarkingAssessmentCriterion.RubricMarkingCompetency.TestComponentType.Name,
                RMarkingAssessmentCriterionName = rubricMarkingBand.RubricMarkingAssessmentCriterion.Name,
                Competency = rubricMarkingBand.RubricMarkingAssessmentCriterion.RubricMarkingCompetency.Name,
                NaturalKey = rubricMarkingBand.Label,
                Label = rubricMarkingBand.Label,
                Description = rubricMarkingBand.Description,
                Level =rubricMarkingBand.Level
            };
        }
    }
}