using F1Solutions.Naati.Common.Contracts.Dal.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static F1Solutions.Naati.Common.Contracts.Dal.CreatePrerequisiteApplicationsDalModel;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class CreateApplicationAndCredentialRequestResult
    {
        public UpsertApplicationResponse UpsertedApplication { get; set; }
        public PrerequisiteApplicationDalModel PrerequisiteApplicationModel { get; set; }
    }
}
