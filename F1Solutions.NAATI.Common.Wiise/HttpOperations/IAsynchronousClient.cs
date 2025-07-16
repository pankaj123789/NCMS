using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.HttpOperations
{
    public interface IAsynchronousClient
    {
        Task<ApiResponse<T>> GetAsync<T>(String path, RequestOptions options, IReadableConfiguration configuration = null);

        Task<ApiResponse<T>> PostAsync<T>(String path, RequestOptions options, IReadableConfiguration configuration = null);

        Task<ApiResponse<T>> PutAsync<T>(String path, RequestOptions options, IReadableConfiguration configuration = null);

        Task<ApiResponse<T>> DeleteAsync<T>(String path, RequestOptions options, IReadableConfiguration configuration = null);

        Task<ApiResponse<T>> HeadAsync<T>(String path, RequestOptions options, IReadableConfiguration configuration = null);

        Task<ApiResponse<T>> OptionsAsync<T>(String path, RequestOptions options, IReadableConfiguration configuration = null);

        Task<ApiResponse<T>> PatchAsync<T>(String path, RequestOptions options, IReadableConfiguration configuration = null);
    }
}


