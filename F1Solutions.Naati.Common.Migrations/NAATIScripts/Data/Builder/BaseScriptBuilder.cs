using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.Builder
{
    public abstract class BaseScriptBuilder<T> : IScriptBuilder where T : IScriptBuilder
    {
        protected abstract string TableName { get; }

        private readonly StringBuilder _queryBuilder;
        public int Id { get; }

        private bool _queryAdded;
        protected BaseScriptBuilder(int entityId)
        {
            Id = entityId;

            _queryBuilder = new StringBuilder();
        }

        protected abstract IEnumerable<string> Values { get; }

        protected abstract IEnumerable<string> Columns { get; }

        protected abstract IEnumerable<T> GetParentBuilders();
        protected abstract IEnumerable<T> GetChildBuilders();

        protected virtual IEnumerable<string> GetColumnsToScript()
        {
            return Columns;
        }

        protected virtual IEnumerable<string> GetValuesToInsert()
        {
            return Values;
        }

        public string GetScript()
        {
            if (_queryAdded)
            {
                return string.Empty;
            }
            _queryAdded = true;

            foreach (var builder in GetParentBuilders())
            {
                var script = builder.GetScript();
                if (!string.IsNullOrWhiteSpace(script))
                {
                    _queryBuilder.AppendLine(script);
                }
            }

            _queryBuilder.AppendLine($"SET IDENTITY_INSERT [{TableName}] ON; INSERT INTO [{TableName}] ({string.Join(",", GetColumnsToScript().Select(c => $"[{c}]"))}) VALUES ({Id},{string.Join(",", GetValuesToInsert().Select(x => x == null ? "null" : $"'{x}'"))}); SET IDENTITY_INSERT [{TableName}] OFF;");

            foreach (var builder in GetChildBuilders())
            {
                var script = builder.GetScript();
                if (!string.IsNullOrWhiteSpace(script))
                {
                    _queryBuilder.AppendLine(script);
                }
            }

            return _queryBuilder.ToString();
        }

    }
}
