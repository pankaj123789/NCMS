namespace MyNaati.Ui.Models.DataTable
{
    public abstract class Row
    {
        public virtual string Id
        {
            get { return null; }
        }

        public virtual string Class
        {
            get { return null; }
        }

        public virtual object Data
        {
            get { return null; }
        }

        public virtual object Attributes
        {
            get { return null; }
        }

        public virtual object ToObject()
        {
            return new
            {
                DT_RowId = Id,
                DT_RowClass = Class,
                DT_RowData = Data,
                DT_RowAttr = Attributes
            };
        }
    }
}
