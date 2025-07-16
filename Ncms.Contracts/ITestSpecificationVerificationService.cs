using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Contracts.Models.TestSpecification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Contracts
{
    public interface ITestSpecificationVerificationService
    {
        List<string> HandleReadOutput(GenericResponse<TestSpecification> output);

        //string HandleCompare(IList<CredentialType> credentialTypes, IList<CredentialType> credentialTypesCompare);

        GenericResponse<bool> HandleCompare(TestSpecification testSpecification, TestSpecification testSpecificationCompare);
    }
}
