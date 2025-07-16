using System;

namespace F1Solutions.Naati.Common.Wiise.HttpOperations
{
    public interface ISynchronousClient
    {
        ApiResponse<T> Get<T>(String path, RequestOptions options, IReadableConfiguration configuration = null);

        ApiResponse<T> Post<T>(String path, RequestOptions options, IReadableConfiguration configuration = null);

        ApiResponse<T> Put<T>(String path, RequestOptions options, IReadableConfiguration configuration = null);

        ApiResponse<T> Delete<T>(String path, RequestOptions options, IReadableConfiguration configuration = null);

        ApiResponse<T> Head<T>(String path, RequestOptions options, IReadableConfiguration configuration = null);

        ApiResponse<T> Options<T>(String path, RequestOptions options, IReadableConfiguration configuration = null);

        ApiResponse<T> Patch<T>(String path, RequestOptions options, IReadableConfiguration configuration = null);
    }
}
