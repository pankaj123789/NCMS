using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.Builder
{
    public interface IScriptBuilder
    {
        int Id { get; }
        string GetScript();
    }
}
