using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Contracts;
using Ncms.Contracts.Models.TestSpecification;
using System.Collections.Generic;
using System.Text;

namespace Ncms.Bl.TestSpecifications
{
    public class TestSpecificationVerificationService : ITestSpecificationVerificationService
    {
        public List<string> HandleReadOutput(GenericResponse<TestSpecification> overallResult)
        {
            var output = new List<string>();
            foreach (var error in overallResult.Errors)
            {
                output.Add(error);
            }
            return output;
        }

        public GenericResponse<bool> HandleCompare(TestSpecification workbookTestSpecification, TestSpecification databaseTestSpecification)
        {
            var response = new GenericResponse<bool>();

            // This is ready only, will never be changed therefore can probably be deleted
            //response = CredentialTypeService.CompareCredentialTypes(credentialTypes, credentialTypesCompare, response);
            response = TestSpecificationsSpreadsheetService.CompareTestSpecifications(workbookTestSpecification, databaseTestSpecification, response);
            response = TestComponentTypesSpreadsheetService.CompareTestComponentTypes(workbookTestSpecification, databaseTestSpecification, response);
            response = TestComponentsSpreadsheetService.CompareTestComponents(workbookTestSpecification, databaseTestSpecification, response);
            response = TestCompTypeStandardMarkingSchemeSpreadsheetService.CompareTestCompTypeStandardMarkingScheme(workbookTestSpecification, databaseTestSpecification, response);
            response = TestSpecificationStandardMarkingSchemeSpreadsheetService.CompareTestStandardMarkingScheme(workbookTestSpecification, databaseTestSpecification, response);
            response = RubricMarkingCompetencySpreadsheetService.CompareRubricMarkingCompetency(workbookTestSpecification, databaseTestSpecification, response);
            response = RubricMarkingAssessmentCriterionSpreadsheetService.CompareRMarkingAssessmentCriterion(workbookTestSpecification, databaseTestSpecification, response);
            response = RubricMarkingBandSpreadsheetService.CompareRMarkingBandService(workbookTestSpecification, databaseTestSpecification, response);

            return response;
        }
    }

}
