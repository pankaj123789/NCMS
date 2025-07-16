using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.Mappers;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.EntityPublicOperations
{
    internal class CreditNoteOperation : BaseEntityOperation
    {
        internal CreditNoteOperation(IAsynchronousClient asynchronousClient) : base(asynchronousClient) { }

        public async Task<ApiResponse<CreditNotes>> CreateCreditNotesAsync(string accessToken, string tenantId, CreditNotes creditNotes)
        {
            var result = new List<CreditNote>();
            var combinedStatusCode = System.Net.HttpStatusCode.OK;

            foreach (var creditNote in creditNotes._CreditNotes)
            {
                if (!creditNote.HasValidationErrors)
                {
                    try
                    {
                        var createdCreditNote = await CreateCrediNoteAsync(accessToken, tenantId, creditNote, creditNote.PostCreditNoteOnCreate);
                        combinedStatusCode = GetStausCode(combinedStatusCode, createdCreditNote);
                        result.Add(createdCreditNote);
                    }
                    catch (ApiException ex)
                    {
                        combinedStatusCode = (HttpStatusCode)ex.ErrorCode;
                        creditNote.HasValidationErrors = true;
                        creditNote.ValidationErrors.Add(new ValidationError()
                        {
                            ErrorCode = ex.ErrorCode,
                            Message = ex.Message
                        });
                        result.Add(creditNote);
                        LoggingHelper.LogError($"Credit note operation {creditNote.OperationId} resulted in API exception { ex.ErrorCode } : { ex.Message }");
                    }
                    catch (Exception ex)
                    {
                        combinedStatusCode = 0;
                        creditNote.HasValidationErrors = true;
                        creditNote.ValidationErrors.Add(new ValidationError()
                        {
                            ErrorCode = 0,
                            Message = ex.Message
                        });
                        result.Add(creditNote);
                        LoggingHelper.LogError($"Credit note operation {creditNote.OperationId} resulted in Exception { ex.Message } : { ex.StackTrace }");
                    }
                }
            }
            return new ApiResponse<CreditNotes>(combinedStatusCode, new CreditNotes { _CreditNotes = result });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="tenantId"></param>
        /// <param name="creditNote"></param>
        /// <param name="postCreditNote">If it's Direct Deposit then the credit note is posted immediately. If it's credit card or paypal it will be posted when the payment comes in</param>
        /// <returns></returns>
        private async Task<CreditNote> CreateCrediNoteAsync(string accessToken, string tenantId, CreditNote creditNote, bool postCreditNote)
        {
            var nativeOperation = new EntityNativeOperations.CreditNoteOperation(this.AsynchronousClient);
            var nativeCreditNote = creditNote.ToNativeCreditNote();
            //create credit note
            var createdCreditNote = await nativeOperation.CreateCreditNoteAsync(accessToken, tenantId, nativeCreditNote);
            if (!createdCreditNote.StatusCode.IsStatusCodeAnError() && !createdCreditNote.Data.HasValidationErrors)
            {
                if (postCreditNote)
                {
                    LoggingHelper.LogInfo($"credit note operation {creditNote.OperationId} has draft output value of {createdCreditNote.Data.Number }");
                    //post credit note
                    nativeCreditNote.ExternalDocumentNumber = creditNote.Reference;
                    var postedCreditNote = await nativeOperation.PostCreditNoteAsync(accessToken, tenantId, nativeCreditNote);
                    if (!postedCreditNote.StatusCode.IsStatusCodeAnError() && !postedCreditNote.Data.HasValidationErrors)
                    {
                        //retirieve new credit note number
                        var filter = string.Format("(id eq {0})", createdCreditNote.Data.Id.Value);
                        var newCreditNote = (await nativeOperation.GetCreditNotesAsync(accessToken, tenantId, filter))
                            .Data._CreditNotes.First().ToPublicCreditNote();

                        LoggingHelper.LogInfo($"invoice operation { creditNote.OperationId } has final output value of { newCreditNote.CreditNoteNumber }");
                        return newCreditNote;
                    }
                    return postedCreditNote.Data.ToPublicCreditNote();
                }
            }
            return createdCreditNote.Data.ToPublicCreditNote();
        }

        public async Task<ApiResponse<CreditNotes>> GetCreditNotesAsync(string accessToken, string tenantId, string where = null, string expand = null)
        {
            var filter = where.ToCreditNotesFilter();

            var nativeResponse = await new EntityNativeOperations.CreditNoteOperation(this.AsynchronousClient).GetCreditNotesAsync(accessToken, tenantId, filter, expand);

            return new ApiResponse<CreditNotes>(nativeResponse.StatusCode, nativeResponse.Data.ToPublicCreditNotes());
        }

        public async Task<ApiResponse<Stream>> GetCreditNoteAsPdfAsync(string accessToken, string tenantId, Guid invoiceId)
        {
            var nativeResponse = await new EntityNativeOperations.CreditNoteOperation(this.AsynchronousClient).GetCreditNoteAsPdfAsync(accessToken, tenantId, invoiceId);

            return new ApiResponse<Stream>(nativeResponse.StatusCode, nativeResponse.Data);
        }
    }
}
