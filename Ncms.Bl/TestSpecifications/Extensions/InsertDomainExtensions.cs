using Ncms.Contracts.Models.TestSpecification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;

namespace TestSpecImporter.NCMS.Bl.TestSpecifications.Extensions
{
    public static class InsertDomainExtensions
    {
        public static void InsertDomain(this Ncms.Contracts.Models.TestSpecification.TestComponentType testComponentTypeModel, F1Solutions.Naati.Common.Dal.Domain.TestSpecification testSpecification,
                                        IList<F1Solutions.Naati.Common.Dal.Domain.TestComponentType> testComponentTypeDomains)
        {
            var testComponenetBaseTypeDomain = NHibernateSession.Current.Get<TestComponentBaseType>(testComponentTypeModel.TestComponentBaseTypeId);

            if (testComponenetBaseTypeDomain == null)
            {
                throw new Exception("testComponenetBaseType is Null.");
            }

            // TODO get user properly via request.Id
            var user = NHibernateSession.Current.Load<User>(2418);

            var testComponentTypeDomain = new F1Solutions.Naati.Common.Dal.Domain.TestComponentType()
            {
                TestSpecification = testSpecification,
                Label = testComponentTypeModel.Label,
                Name = testComponentTypeModel.Name,
                Description = testComponentTypeModel.Description,
                TestComponentBaseType = testComponenetBaseTypeDomain,
                MinExaminerCommentLength = testComponentTypeModel.MinExaminerCommentLength,
                MinNaatiCommentLength = testComponentTypeModel.MinNaatiCommentLength,
                RoleplayersRequired = testComponentTypeModel.RoleplayersRequired,
                CandidateBriefRequired = testComponentTypeModel.CandidateBriefRequired,
                CandidateBriefAvailabilityDays = testComponentTypeModel.CandidateBriefavailabilitydays,
                DefaultMaterialRequestHours = testComponentTypeModel.DefaultMaterialRequestHours,
                DefaultMaterialRequestDueDays = (int)testComponentTypeModel.DefaultMaterialRequestDueDays,
                ModifiedUser = user,
                ModifiedByNaati = false,
                ModifiedDate = DateTime.Now
            };
            testComponentTypeDomains.Add(testComponentTypeDomain);
            NHibernateSession.Current.Save(testComponentTypeDomain);
        }

        public static void InsertDomain(this Ncms.Contracts.Models.TestSpecification.TestComponent testComponentModel, F1Solutions.Naati.Common.Dal.Domain.TestSpecification testSpecification, 
                                        IList<F1Solutions.Naati.Common.Dal.Domain.TestComponentType> testComponentTypeDomains)
        {
            var testComponentTypeDomain = testComponentTypeDomains.FirstOrDefault(x => x.Name  == testComponentModel.TestComponentTypeName);

            if (testComponentTypeDomain == null)
            {
                throw new NullReferenceException($"TestComponenetTypeDomain is null for {testComponentModel.TestComponentTypeName}.");
            }

            // TODO get user properly via request.Id
            var user = NHibernateSession.Current.Load<User>(2418);

            var testComponentDomain = new F1Solutions.Naati.Common.Dal.Domain.TestComponent()
            {
                TestSpecification = testSpecification,
                Type = testComponentTypeDomain,
                ComponentNumber = testComponentModel.ComponentNumber,
                Label = testComponentModel.Label,
                Name = testComponentModel.Name,
                GroupNumber = testComponentModel.GroupNumber,
                ModifiedUser = user,
                ModifiedByNaati = false,
                ModifiedDate = DateTime.Now
            };
            NHibernateSession.Current.Save(testComponentDomain);
        }

        public static void InsertDomain(this Ncms.Contracts.Models.TestSpecification.TestComponentTypeStandardMarkingScheme testComponentTypeStandardMarkingSchemeModel,
                                        IList<F1Solutions.Naati.Common.Dal.Domain.TestComponentType> testComponentTypeDomains)
        {
            var testComponentTypeDomain = testComponentTypeDomains.FirstOrDefault(x => x.Name == testComponentTypeStandardMarkingSchemeModel.ComponentType);

            if (testComponentTypeDomain == null)
            {
                throw new NullReferenceException($"TestComponenetTypeDomain is null for {testComponentTypeStandardMarkingSchemeModel.ComponentType}");
            }

            // TODO get user properly via request.Id
            var user = NHibernateSession.Current.Load<User>(2418);

            var testComponentTypeStandardMarkingSchemeDomain = new F1Solutions.Naati.Common.Dal.Domain.TestComponentTypeStandardMarkingScheme()
            {
                TestComponentType = testComponentTypeDomain,
                TotalMarks = testComponentTypeStandardMarkingSchemeModel.TotalMarks,
                PassMark = testComponentTypeStandardMarkingSchemeModel.PassMark,
                ModifiedUser = user,
                ModifiedByNaati = true,
                ModifiedDate = DateTime.Now
            };

            NHibernateSession.Current.Save(testComponentTypeStandardMarkingSchemeDomain);
        }

        public static void InsertDomain(this Ncms.Contracts.Models.TestSpecification.TestSpecificationStandardMarkingScheme testStandardMarkingSchemeModel,
                                        F1Solutions.Naati.Common.Dal.Domain.TestSpecification testSpecification)
        {
            // TODO get user properly via request.Id
            var user = NHibernateSession.Current.Load<User>(2418);

            var testStandardMarkingSchemeDomain = new F1Solutions.Naati.Common.Dal.Domain.TestSpecificationStandardMarkingScheme()
            {
                TestSpecification = testSpecification,
                OverallPassMark = testStandardMarkingSchemeModel.OverallPassMark,
                ModifiedUser = user,
                ModifiedByNaati = false,
                ModifiedDate = DateTime.Now
            };

            NHibernateSession.Current.Save(testStandardMarkingSchemeDomain);
        }

        public static void InsertDomain(this Ncms.Contracts.Models.TestSpecification.RubricMarkingCompetency rubricMarkingCompetencyModel,
                                        IList<F1Solutions.Naati.Common.Dal.Domain.TestComponentType> testComponentTypeDomains,
                                        IList<F1Solutions.Naati.Common.Dal.Domain.RubricMarkingCompetency> rubricMarkingCompetencyDomains)
        {
            var testComponentTypeDomain = testComponentTypeDomains.FirstOrDefault(x => x.Name == rubricMarkingCompetencyModel.ComponentType);
            
            if (testComponentTypeDomain == null)
            {
                throw new NullReferenceException($"TestComponenetTypeDomain is null for {rubricMarkingCompetencyModel.ComponentType}");
            }
            // TODO get user properly via request.Id
            var user = NHibernateSession.Current.Load<User>(2418);

            var rubricMarkingCompetencyDomain = new F1Solutions.Naati.Common.Dal.Domain.RubricMarkingCompetency()
            {
                TestComponentType = testComponentTypeDomain,
                Name = rubricMarkingCompetencyModel.Name,
                Label = rubricMarkingCompetencyModel.Label,
                DisplayOrder = rubricMarkingCompetencyModel.DisplayOrder,
                ModifiedUser = user,
                ModifiedByNaati = false,
                ModifiedDate = DateTime.Now
            };
            rubricMarkingCompetencyDomains.Add(rubricMarkingCompetencyDomain);
            NHibernateSession.Current.Save(rubricMarkingCompetencyDomain);
        }

        public static void InsertDomain(this Ncms.Contracts.Models.TestSpecification.RubricMarkingAssessmentCriterion rMarkingAssessmentCriterionModel,
                                        IList<F1Solutions.Naati.Common.Dal.Domain.RubricMarkingCompetency> rubricMarkingCompetencyDomains,
                                        IList<F1Solutions.Naati.Common.Dal.Domain.RubricMarkingAssessmentCriterion> rMarkingAssessmentCriterionDomains)
        {
            var rubricMarkingCompetencyDomain = rubricMarkingCompetencyDomains.FirstOrDefault(x => (x.Name == rMarkingAssessmentCriterionModel.Competency)
                                                                                              && (x.TestComponentType.Name == rMarkingAssessmentCriterionModel.TestComponentTypeName));
            if (rubricMarkingCompetencyDomain == null)
            {
                throw new NullReferenceException($"RubricMarkingCompetencyDomain is null for {rMarkingAssessmentCriterionModel.TestComponentTypeName}, {rMarkingAssessmentCriterionModel.Competency}.");
            }

            // TODO get user properly via request.Id
            var user = NHibernateSession.Current.Load<User>(2418);

            var rMarkingAssessmentCriterionDomain = new F1Solutions.Naati.Common.Dal.Domain.RubricMarkingAssessmentCriterion()
            {
                RubricMarkingCompetency = rubricMarkingCompetencyDomain,
                Name = rMarkingAssessmentCriterionModel.Name,
                Label = rMarkingAssessmentCriterionModel.Label,
                DisplayOrder = rMarkingAssessmentCriterionModel.DisplayOrder,
                ModifiedUser = user,
                ModifiedByNaati = false,
                ModifiedDate = DateTime.Now
            };
            rMarkingAssessmentCriterionDomains.Add(rMarkingAssessmentCriterionDomain);
            NHibernateSession.Current.Save(rMarkingAssessmentCriterionDomain);
        }

        public static void InsertDomain(this Ncms.Contracts.Models.TestSpecification.RubricMarkingBand rubricMarkingBandModel,
                                        IList<F1Solutions.Naati.Common.Dal.Domain.RubricMarkingAssessmentCriterion> rubricMarkingAssessmentCriterionDomains)
        {
            var rubricMarkingAssessmentCriterionDomain = rubricMarkingAssessmentCriterionDomains.FirstOrDefault(x => (x.RubricMarkingCompetency.TestComponentType.Name == rubricMarkingBandModel.TestComponentTypeName)
                                                                                                                && (x.RubricMarkingCompetency.Name == rubricMarkingBandModel.Competency)
                                                                                                                && (x.Name == rubricMarkingBandModel.RMarkingAssessmentCriterionName));
            if (rubricMarkingAssessmentCriterionDomain == null)
            {
                throw new NullReferenceException($"RubricMarkingAssessmentCriterionDomain is null for {rubricMarkingBandModel.TestComponentTypeName}, {rubricMarkingBandModel.Competency}, {rubricMarkingBandModel.RMarkingAssessmentCriterionName}.");
            }

            // TODO get user properly via request.Id
            var user = NHibernateSession.Current.Load<User>(2418);

            var rubricMarkingBandDomain = new F1Solutions.Naati.Common.Dal.Domain.RubricMarkingBand()
            {
                RubricMarkingAssessmentCriterion = rubricMarkingAssessmentCriterionDomain,
                Label = rubricMarkingBandModel.Label,
                Description = rubricMarkingBandModel.Description,
                Level = rubricMarkingBandModel.Level,
                ModifiedUser = user,
                ModifiedByNaati = false,
                ModifiedDate = DateTime.Now
            };
            NHibernateSession.Current.Save(rubricMarkingBandDomain);
        }
    }
}
