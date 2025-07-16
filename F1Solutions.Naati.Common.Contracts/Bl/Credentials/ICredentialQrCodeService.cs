using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Bl.VerifyPractitioner;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace F1Solutions.Naati.Common.Contracts.Bl.Credentials
{
    public interface ICredentialQrCodeService
    {
        /// <summary>
        /// Returns the file path to temp storage with the generated credential qr stamp.
        /// Calls the Dal to get the current QR Code or generate a new one if one doesn't exist. 
        /// </summary>
        /// <param name="credentialId"></param>
        /// <returns>File Path to the Credential QR Stamp</returns>
        GenericResponse<string> GetCredentialQrStamp(int credentialId);
        /// <summary>
        /// Creates a QR Code based off of the GUID from the CredentialQRCode table.
        /// Calls the Dal to get the current QR Code or generate a new one if one doesn't exist.
        /// </summary>
        /// <param name="credentialId"></param>
        /// <returns>Credential QR code Image</returns>
        GenericResponse<Bitmap> GetCredentialQrCodeForStamp(int credentialId);
        /// <summary>
        /// Creates a QR Code based off of the GUID from the CredentialQRCode table.
        /// Calls the Dal to get the current QR Code or generate a new one if one doesn't exist.
        /// </summary>
        /// <param name="credentialId"></param>
        /// <returns>Credential QR code Image</returns>
        GenericResponse<Bitmap> GetCredentialQrCodeForIdCard(int credentialId);
        /// <summary>
        /// Gets a list of credential Id cards from a persons naati number
        /// </summary>
        /// <param name="naatiNumber"></param>
        /// <returns>Response containing a list of images of Credential Id Cards</returns>
        GenericResponse<List<string>> GetCredentialIdCards(int naatiNumber);
        /// <summary>
        /// Calls the Dal to get if MFA is active for the current user
        /// Under new management this is now confdigured in hours.
        /// </summary>
        /// <param name="naatiNumber"></param>
        /// <returns>Generic Response of whether the current user has mfa configured or not</returns>
        GenericResponse<MfaDetailsModel> GetMFAConfiguredAndValid(int naatiNumber);

        /// <summary>
        /// This is acting as the verification code for all QRCodes and is currently in myNaati
        /// This will return just the PractitionerId and then the API call is used for the data
        /// </summary>
        /// <param name="QrCode"></param>
        /// <returns></returns>
        GenericResponse<VerifyPractitionerServiceModel> GetVerifyPractitionerModelFromQrCode(Guid QrCode);
        /// <summary>
        /// Gets true or false depending on the deceased status of a person for the given practitioner Id
        /// </summary>
        /// <param name="practitionerId"></param>
        /// <returns>True if deceased, false if not.</returns>
        GenericResponse<bool> GetIsDeceasedFromPractitionerId(string practitionerId);
        /// <summary>
        /// Gets the default person image as a byte array
        /// </summary>
        /// <returns>Default person Image</returns>
        byte[] GetDefaultImage();


        /// <summary>
        /// Block an exploit where a logged on user could get another persons QRCode
        /// </summary>
        /// <param name="credentialId"></param>
        /// <param name="currentUserNaatiNumber"></param>
        /// <returns></returns>
        GenericResponse<bool> DoesCredentialBelongToUser(int credentialId, int currentUserNaatiNumber);

        /// <summary>
        /// rezised an image but keeps aspect ratio
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns>Resized image in bitmap form</returns>
        Bitmap ResizeImage(Bitmap image, int maxWidth, int maxHeight);


    }
}
