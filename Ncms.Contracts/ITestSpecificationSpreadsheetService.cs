using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Contracts.Models.TestSpecification;
using System.Collections.Generic;

namespace Ncms.Contracts
{
    public interface ITestSpecificationSpreadsheetService
    {
        //GenericResponse<string> RenderSheet(IEnumerable<CredentialType> credentialTypes);

        GenericResponse<TestSpecification> ReadWorkbook(string filePath);

        /// <summary>
        /// Retrieves the current Test Sepcifications from the DB
        /// and returns them as an excel file
        /// </summary>
        /// <returns>Excel Workbook Token</returns>
        GenericResponse<string> GetTestSpecifications(string testSpecificationFilter);

        /// <summary>
        /// Uploads a Workbook, compares against current data
        /// reports back on differences
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        GenericResponse<string> UploadTestSpecifications(string filePath, string testSpecificationFilter, bool verifyOnly);

        /// <summary>
        /// Gets TestSpecificationName when given Id
        /// </summary>
        /// <param name="testSpecificationId"></param>
        /// <returns>testspecification name</returns>
        GenericResponse<string> GetTestSpecificationDescription(int testSpecificationId);
    }
}
