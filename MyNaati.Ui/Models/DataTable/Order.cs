namespace MyNaati.Ui.Models.DataTable
{
    public enum OrderDirection
    {
        Ascending,
        Descending
    }

    public class Order
    {
        public int Column { get; set; }
        public string Dir { get; set; }

        public OrderDirection OrderDirection
        {
            get { return string.Equals(Dir, "desc") ? OrderDirection.Descending : OrderDirection.Ascending; }
            set { Dir = value == OrderDirection.Descending ? "desc" : "asc"; }
        }
    }
}
