using System;
using System.Linq;
using MyNaati.Contracts.Portal;

namespace MyNaati.Bl.Portal
{
    public class SystemService : ISystemService
    {
        public DiagnosticResponse RunDiagnostics(DiagnosticRequest request)
        {
            var response = new DiagnosticResponse()
            {
                //Errors = InvokeAndFilterNull(CheckDatabase)
                Errors = new MessageNode[0]
            };

            return response;
        }

        private MessageNode[] InvokeAndFilterNull(params Func<MessageNode>[] diagnosticChecks)
        {
            var results = diagnosticChecks.Select(c => c.Invoke());
            return results.Where(r => r != null).ToArray();
        }

        //private MessageNode CheckDatabase()
        //{
        //    try
        //    {
        //        var comparer = new VersionComparer();
        //        var comparisonResult = comparer.CompareToDatabase(typeof(DummyMigration), NHibernateSetup.ConnectionString);
        //        if (comparisonResult.DoMigrationsMatch)
        //            return null;

        //        var resultNode = new MessageNode() { Message = "Database does not match recorded migrations." };
        //        var unknownMigrationNode = new MessageNode() 
        //        { 
        //            Message = string.Format("{0} unknown migrations in DB", comparisonResult.MigrationsNotInAssembly.Count),
        //            Children = comparisonResult.MigrationsNotInAssembly.Select(e => new MessageNode() { Message = e }).ToArray()
        //        };
        //        var missingMigrationNode = new MessageNode()
        //        {
        //            Message = string.Format("{0} missing from DB", comparisonResult.MigrationsNotInDatabase.Count),
        //            Children = comparisonResult.MigrationsNotInDatabase.Select(e => new MessageNode() { Message = e }).ToArray()
        //        };

        //        resultNode.Children = new[] { unknownMigrationNode, missingMigrationNode };

        //        return resultNode;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new MessageNode()
        //        {
        //            Message = string.Format("Exception/s encountered while checking database."),
        //            Children = new[] { new MessageNode() { Message = ex.ToString() } }
        //        };
        //    }

        //}
    }
}
