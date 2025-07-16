namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    public interface ICacheQueryService
    {
        void RefreshAllCache();
    }

    public interface ICacheQueryService<TKeyType> : ICacheQueryService
    {
        bool AddOrRefreshItem(TKeyType key);
    }
}
