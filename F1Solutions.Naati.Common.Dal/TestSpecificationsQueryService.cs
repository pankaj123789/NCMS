using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Bl.TestSpecifications.Extensions;
using Ncms.Contracts.Models.TestSpecification;
using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NCMS.Bl.TestSpecifications.Extensions;
using F1Solutions.Global.Common.Logging;

namespace F1Solutions.Naati.Common.Dal
{
    public class TestSpecificationsQueryService
    {
        public IList<CredentialType> GetData()
        {
               var dalResult = NHibernateSession.Current.Query<Domain.CredentialType>();
            
            var result = dalResult.Select(x => x.ToModel());
            
            return result.ToList();
        }

        public GenericResponse<string> WriteData(TestSpecification testSpecification, string testSpecificationFilter, List<string> seqMessagesToPostOnSuccess)
        {
            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                transaction.Begin();
                try
                {
                    var credentialTypeDomain = NHibernateSession.Current.Query<Domain.CredentialType>().ToList();
                    var testSpecificationDomain = credentialTypeDomain.SelectMany(x => x.TestSpecifications).FirstOrDefault(x => x.Description == testSpecificationFilter);
                    //also used for inserts
                    var testComponentTypeDomains = testSpecificationDomain.TestComponentTypes.ToList();
                    var rubricMarkingCompetencyDomains = testSpecificationDomain.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetencies).ToList();
                    var rMarkingAssessmentCriterionDomains = testSpecificationDomain.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetencies)
                                                            .SelectMany(x => x.RubricMarkingAssessmentCriteria).ToList();

                    // Updates
                    var rubricMarkingBandsToUpdate = testSpecification.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency)
                                                                      .SelectMany(x => x.RubricMarkingAssessmentCriterion).SelectMany(x => x.RubricMarkingBand).Where(x => x.ModelState == ModelState.altered).ToList();
                    if (rubricMarkingBandsToUpdate.Count > 0)
                    {
                        var rubricMarkingBandDomains = testSpecificationDomain.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetencies)
                                                                          .SelectMany(x => x.RubricMarkingAssessmentCriteria).SelectMany(x => x.RubricMarkingBands).ToList();
                        rubricMarkingBandsToUpdate.ForEach(x => x.UpdateDomain(rubricMarkingBandDomains));
                    }

                    var rMarkingAssessmentCriterionToUpdate = testSpecification.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency)
                                                                               .SelectMany(x => x.RubricMarkingAssessmentCriterion).Where(x => x.ModelState == ModelState.altered).ToList();
                    if (rMarkingAssessmentCriterionToUpdate.Count > 0)
                    {

                        rMarkingAssessmentCriterionToUpdate.ForEach(x => x.UpdateDomain(rMarkingAssessmentCriterionDomains));
                    }

                    var rubricMarkingCompetenciesToUpdate = testSpecification.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency)
                                                                             .Where(x => x.ModelState == ModelState.altered).ToList();
                    if (rubricMarkingCompetenciesToUpdate.Count > 0)
                    {
                        rubricMarkingCompetenciesToUpdate.ForEach(x => x.UpdateDomain(rubricMarkingCompetencyDomains));
                    }

                    var testStandardMarkingSchemesToUpdate = testSpecification.TestSpecificationStandardMarkingSchemes.Where(x => x.ModelState == ModelState.altered).ToList();
                    if (testStandardMarkingSchemesToUpdate.Count > 0)
                    {
                        var testStandardMarkingSchemeDomains = testSpecificationDomain.TestSpecificationStandardMarkingSchemes.ToList();

                        testStandardMarkingSchemesToUpdate.ForEach(x => x.UpdateDomain(testStandardMarkingSchemeDomains));
                    }

                    var testCompTypeStandardMarkingShcemesToUpdate = testSpecification.TestComponentTypes.SelectMany(x => x.TestComponentTypeStandardMarkingScheme)
                                                                                      .Where(x => x.ModelState == ModelState.altered).ToList();
                    if (testCompTypeStandardMarkingShcemesToUpdate.Count > 0)
                    {
                        var testCompTypeStandardMarkingSchemeDomains = testSpecificationDomain.TestComponentTypes.SelectMany(x => x.TestComponentTypeStandardMarkingSchemes).ToList();

                        testCompTypeStandardMarkingShcemesToUpdate.ForEach(x => x.UpdateDomain(testCompTypeStandardMarkingSchemeDomains));
                    }

                    var testComponentsToUpdate = testSpecification.TestComponents.Where(x => x.ModelState == ModelState.altered).ToList();
                    if (testComponentsToUpdate.Count > 0)
                    {
                        var testComponentDomains = testSpecificationDomain.TestComponents.ToList();

                        testComponentsToUpdate.ForEach(x => x.UpdateDomain(testComponentDomains));
                    }

                    var testComponentTypesToUpdate = testSpecification.TestComponentTypes.Where(x => x.ModelState == ModelState.altered).ToList();
                    if (testComponentTypesToUpdate.Count > 0)
                    {
                        testComponentTypesToUpdate.ForEach(x => x.UpdateDomain(testComponentTypeDomains));
                    }

                    if (testSpecification.ModelState == ModelState.altered)
                    {
                        testSpecification.UpdateDomain(testSpecificationDomain);
                    }

                    // Inserts
                    var testComponentTypesToInsert = testSpecification.TestComponentTypes.Where(x => x.ModelState == ModelState.inserted).ToList();
                    if (testComponentTypesToInsert.Count > 0)
                    {
                        testComponentTypesToInsert.ForEach(x => x.InsertDomain(testSpecificationDomain, testComponentTypeDomains));
                    }

                    var testComponentsToInsert = testSpecification.TestComponents.Where(x => x.ModelState == ModelState.inserted).ToList();
                    if (testComponentsToInsert.Count > 0)
                    {
                        testComponentsToInsert.ForEach(x => x.InsertDomain(testSpecificationDomain, testComponentTypeDomains));
                    }

                    var testCompTypeStandardMarkingSchemesToInsert = testSpecification.TestComponentTypes.SelectMany(x => x.TestComponentTypeStandardMarkingScheme)
                                                                  .Where(x => x.ModelState == ModelState.inserted).ToList();
                    if (testCompTypeStandardMarkingSchemesToInsert.Count > 0)
                    {
                        testCompTypeStandardMarkingSchemesToInsert.ForEach(x => x.InsertDomain(testComponentTypeDomains));
                    }

                    var testStandardMarkingSchemesToInsert = testSpecification.TestSpecificationStandardMarkingSchemes.Where(x => x.ModelState == ModelState.inserted).ToList();
                    if (testStandardMarkingSchemesToInsert.Count > 0)
                    {
                        testStandardMarkingSchemesToInsert.ForEach(x => x.InsertDomain(testSpecificationDomain));
                    }

                    var rubricMarkingCompetenciesToInsert = testSpecification.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency)
                                                                             .Where(x => x.ModelState == ModelState.inserted).ToList();
                    if (rubricMarkingCompetenciesToInsert.Count > 0)
                    {
                        rubricMarkingCompetenciesToInsert.ForEach(x => x.InsertDomain(testComponentTypeDomains, rubricMarkingCompetencyDomains));
                    }

                    var rMarkingAssessmentCriterionToInsert = testSpecification.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency)
                                                                              .SelectMany(x => x.RubricMarkingAssessmentCriterion).Where(x => x.ModelState == ModelState.inserted).ToList();
                    if (rMarkingAssessmentCriterionToInsert.Count > 0)
                    {
                        rMarkingAssessmentCriterionToInsert.ForEach(x => x.InsertDomain(rubricMarkingCompetencyDomains, rMarkingAssessmentCriterionDomains));
                    }


                    var rubricMarkingBandsToInsert = testSpecification.TestComponentTypes.SelectMany(x => x.RubricMarkingCompetency)
                                                                      .SelectMany(x => x.RubricMarkingAssessmentCriterion).SelectMany(x => x.RubricMarkingBand).Where(x => x.ModelState == ModelState.inserted).ToList();
                    if (rubricMarkingBandsToInsert.Count > 0)
                    {
                        rubricMarkingBandsToInsert.ForEach(x => x.InsertDomain(rMarkingAssessmentCriterionDomains));
                    }
                    NHibernateSession.Current.Flush();
                    transaction.Commit(); 

                    LoggingHelper.LogInfo($"User has successfully uploaded a test specification spreadsheet.");
                    foreach (var messages in seqMessagesToPostOnSuccess)
                    {
                        LoggingHelper.LogInfo($"{messages}");
                    }

                    return "Upload completed successfully.";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LoggingHelper.LogException(ex, $"Test Specification spreadsheet failed to upload.");
                    NHibernateSession.Current.Clear();
                    return new GenericResponse<string>
                    {
                        Data = "Unexpected error occured.",
                        Errors = new List<string>()
                        {
                            ex.Message
                        }
                    };
                }
            }

        }

        public GenericResponse<string> GetTestSpecificationDescription(int testSpecificationId)
        {
            if(testSpecificationId == 0)
            {
                return new GenericResponse<string>()
                {
                    Messages = new List<string>()
                     {
                         "Test Specification Id cannot be null."
                     }
                };
            }

            var testSpecification = NHibernateSession.Current.Query<Domain.TestSpecification>().FirstOrDefault(x=>x.Id == testSpecificationId);
            return testSpecification.Description;
        }
    }
}

