using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{

    public interface IScriptGenerator
    {
        void RunScripts();
        void RunDescendantOrderScripts();
    }

    public abstract class BaseScriptGenerator: IScriptGenerator
    {
        public abstract string TableName { get; }
        public abstract IList<string> Columns { get; }
        protected NaatiScriptRunner ScriptRunner { get; }
        protected virtual string KeyColumn => Columns[0];

        protected BaseScriptGenerator(NaatiScriptRunner runner)
        {
            ScriptRunner = runner;
            AddChangeTrackingTablesIfMissing();
        }

        protected virtual void ChangeColumValue(string columnName)
        {
            var columnIndex = Columns.IndexOf(columnName);
            if (columnIndex < 0)
            {
                throw new Exception($"Column {columnName} is not specified on the script columns");
            }

            var script = $"UPDATE {TableName} SET {columnName} = {columnName} * -1";
            ScriptRunner.RunScript(script);
        }
        protected virtual void CreateOrUpdateTableRow(IList<string> values)
        {
            if (values.Count != Columns.Count)
            {
                throw new Exception($"{TableName} was defined with {Columns.Count} columns but {values.Count} values were specified");
            }

            var keyValue = values[0];

            var script = ScriptRunner.TableRowExists(TableName, KeyColumn, keyValue)
                ? GetUpdateScript(keyValue, values)
                : GetInsertScript(values);

            ScriptRunner.RunScript(script);
        }

        protected virtual void CreateTableRow(IList<string> values)
        {
            if (values.Count != Columns.Count)
            {
                throw new Exception($"{TableName} was defined with {Columns.Count} columns but {values.Count} values were specified");
            }

            var keyValue = values[0];

            if (!ScriptRunner.TableRowExists(TableName, KeyColumn, keyValue))
            {
                var script = GetInsertScript(values);
                ScriptRunner.RunScript(script);
            }
        }

        protected virtual void DeleteTableRow(string keyColumnId)
        {
            ScriptRunner.RunScript($"DELETE FROM {TableName} WHERE {KeyColumn} = '{keyColumnId}'");
        }

        protected virtual void DeleteAllRows()
        {
            ScriptRunner.RunScript($"DELETE FROM {TableName}");
        }

        protected void AddChangeTrackingTablesIfMissing()
        {
            var script = $@" 
                IF COL_LENGTH('{TableName}', 'ModifiedByNaati') IS NULL
                BEGIN
                    ALTER TABLE {TableName} ADD ModifiedByNaati bit NOT NULL CONSTRAINT [DF_{TableName}_Temp] DEFAULT 0
                    ALTER TABLE {TableName} DROP CONSTRAINT  [DF_{TableName}_Temp]
                END
    
                IF COL_LENGTH('{TableName}', 'ModifiedDate') IS NULL
                BEGIN 
                    ALTER TABLE {TableName} ADD ModifiedDate datetime NOT NULL CONSTRAINT [DF_{TableName}_Temp] DEFAULT SYSDATETIME()
                    ALTER TABLE {TableName} DROP CONSTRAINT  [DF_{TableName}_Temp]
                END

                IF COL_LENGTH('{TableName}', 'ModifiedUser') IS NULL
                BEGIN 
                    ALTER TABLE {TableName} ADD ModifiedUser int NOT NULL CONSTRAINT [DF_{TableName}_Temp] DEFAULT 40
                    ALTER TABLE {TableName} DROP CONSTRAINT  [DF_{TableName}_Temp]
      
                    ALTER TABLE [dbo].[{TableName}] 
                    ADD CONSTRAINT [FK_{TableName.Substring(3, TableName.Length - 3)}_User]
                        FOREIGN KEY ([ModifiedUser])
                        REFERENCES [dbo].[tblUser] ( [UserId] )      
                END";

            ScriptRunner.RunScript(script);
        }

        private string GetWhereNotEqualClause(string columnName, string value)
        {
            return value == null
                ? $"[{columnName}] IS NOT NULL"
                : $"([{columnName}] IS NULL OR [{columnName}] <> '{ScriptRunner.Escape(value)}')";
        }

        private string GetUpdateScript(string keyValue, IList<string> values)
        {
            var setClause = String.Join(", ", Columns.Skip(1).Select((x, i) => values[i + 1] == null ? $"[{x}] = NULL" : $"[{x}] = '{ScriptRunner.Escape(values[i + 1])}'"))
                            + $", ModifiedDate = '{DateTime.Now:yyyy/MM/dd HH:mm:ss}', ModifiedUser = 40";

            var whereClause = String.Join(" OR ", Columns.Skip(1).Select((x, i) => GetWhereNotEqualClause(x, values[i + 1])));
            whereClause = $"{KeyColumn} = '{keyValue}' AND ModifiedByNaati = 0 AND ({whereClause})";

            return $"UPDATE {TableName} SET {setClause} WHERE {whereClause}";
        }

        private string GetInsertScript(IList<string> values)
        {
            var columnsClause = String.Join(", ", Columns.Select(x => $"[{x}]")) + ", [ModifiedByNaati], [ModifiedDate], [ModifiedUser]";
            var valuesClause = String.Join(", ", values.Select(x => x == null ? "NULL" : $"'{ScriptRunner.Escape(x)}'")) + ", 0, SYSDATETIME(), 40";
            return ScriptRunner.IsIdentityColumn(TableName, KeyColumn)
                ? $"SET IDENTITY_INSERT {TableName} ON; INSERT INTO {TableName} ({columnsClause}) VALUES ({valuesClause}); SET IDENTITY_INSERT {TableName} OFF"
                : $"INSERT INTO {TableName} ({columnsClause}) VALUES ({valuesClause});";
        }

        protected virtual void CreateOrUpdateTableRow<T>(T typeName, IList<string> valuesWithoutKey)
        {
            var values = new[] { Convert.ToInt32(typeName).ToString(), Convert.ToString(typeName) }.Concat(valuesWithoutKey).ToList();

            CreateOrUpdateTableRow(values);
        }

        public abstract void RunScripts();

        public virtual void RunDescendantOrderScripts() { }
    }
}
