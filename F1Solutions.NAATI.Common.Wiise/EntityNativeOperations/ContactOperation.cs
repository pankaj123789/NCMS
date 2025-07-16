using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.Mappers;
using F1Solutions.Naati.Common.Wiise.NativeModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.EntityNativeOperations
{
    internal class ContactOperation : BaseEntityOperation
    {
        internal ContactOperation(IAsynchronousClient asynchronousClient) : base(asynchronousClient)
        {

        }

        internal async Task<ApiResponse<Contacts>> CreateContactsAsync(string accessToken, string tenantId, Contacts contacts)
        {
            var responseCode = System.Net.HttpStatusCode.OK;
            foreach (Contact contact in contacts._Contacts)
            {
                if (!contact.HasValidationErrors)
                {
                    ApiResponse<Contact> response = null;
                    ApiException errorResponse = null;

                    try
                    {
                        response = await PostAsyncWithHttpInfo(accessToken, tenantId, contact, "/customers");
                    }
                    catch (ApiException ex)
                    {
                        errorResponse = ex;
                    }
                    if (errorResponse != null ||
                       (!string.IsNullOrEmpty(response.ErrorText) || (response.StatusCode.IsStatusCodeAnError())))
                    {
                        //this will be overwritten on multiple errors but should be ok
                        responseCode = errorResponse != null ? (HttpStatusCode)errorResponse.ErrorCode : response.StatusCode;
                        contact.HasValidationErrors = true;
                        if (contact.ValidationErrors == null)
                        {
                            contact.ValidationErrors = new List<ValidationError>();
                        }
                        contact.ValidationErrors.Add(new ValidationError()
                        {
                            ErrorCode = errorResponse != null ? errorResponse.ErrorCode : (int)response.StatusCode,
                            Message = errorResponse != null ? errorResponse.Message : (response.ErrorText == null ? response.StatusCode.ToString() : response.ErrorText)
                        });
                    }
                    else
                    {
                        contact.Id = response.Data.Id;
                    }
                }
            }
            return new ApiResponse<Contacts>(responseCode, contacts);
        }

        internal async Task<ApiResponse<Contacts>> UpdateContactsAsync(string accessToken, string tenantId, Contacts contacts, string eTag)
        {
            var responseCode = System.Net.HttpStatusCode.OK;
            foreach (Contact contact in contacts._Contacts)
            {
                if (!contact.HasValidationErrors)
                {
                    string path = string.Format("/customers({0})", contact.Id);

                    ApiResponse<Contact> response = null;
                    ApiException errorResponse = null;

                    try
                    {
                        response = await PatchAsyncWithHttpInfo(accessToken, tenantId, contact, eTag, path);
                    }
                    catch (ApiException ex)
                    {
                        errorResponse = ex;
                    }

                    if (errorResponse != null ||
                      (!string.IsNullOrEmpty(response.ErrorText) || (response.StatusCode.IsStatusCodeAnError())))
                    {
                        //this will be overwritten on multiple errors but should be ok
                        responseCode = errorResponse != null ? (HttpStatusCode)errorResponse.ErrorCode : response.StatusCode;
                        contact.HasValidationErrors = true;
                        if (contact.ValidationErrors == null)
                        {
                            contact.ValidationErrors = new List<ValidationError>();
                        }
                        contact.ValidationErrors.Add(new ValidationError()
                        {
                            ErrorCode = errorResponse != null ? errorResponse.ErrorCode : (int)response.StatusCode,
                            Message = errorResponse != null ? errorResponse.Message : (response.ErrorText == null ? response.StatusCode.ToString() : response.ErrorText)
                        });
                    }
                }
            }
            return new ApiResponse<Contacts>(responseCode, contacts);
        }

        internal async Task<ApiResponse<Contacts>> GetContactsAsync(string accessToken, string tenantId, string filter)
        {
            return await GetAsyncWithHttpInfo<Contacts>(accessToken, tenantId, $"/customers{filter}");
        }

    }
}