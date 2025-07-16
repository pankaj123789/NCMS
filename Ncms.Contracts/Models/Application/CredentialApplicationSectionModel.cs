using System.Collections.Generic;

namespace Ncms.Contracts.Models.Application
{
    public class CredentialApplicationSectionModel
    {
        public string Name { get; set; }

        public IList<CredentialApplicationFieldModel> Fields { get; set; }
    }
}
