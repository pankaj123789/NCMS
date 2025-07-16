using System.Collections.Generic;

namespace Ncms.Contracts
{
    public interface IResourceService
    {
        Dictionary<string, Dictionary<string, string>> List();
    }
}
