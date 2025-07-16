namespace MyNaati.Ui.Helpers
{
    public interface IExaminerHelper
    {
        bool IsValidated(string userName);
        void LoadExaminerRoles(string userName, int naatiNumber);
    }
}