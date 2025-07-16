//using F1Solutions.Naati.Common.Contracts.Security;
//using MyNaati.Contracts.Portal;
//using System;
//using System.Configuration;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Security.Cryptography.X509Certificates;
//using System.Security.Permissions;
//using System.Text;

//namespace MyNaati.Bl.Portal
//{
//    public class SamCreditCardEncryptionService : ISamCreditCardEncryptionService
//    {
//        private readonly ISecretsProvider _secretsProvider;

//        public SamCreditCardEncryptionService(ISecretsProvider secretsProvider)
//        {
//            _secretsProvider = secretsProvider;
//        }

//        private X509Certificate2 RetrieveCertificateFromStore()
//        {
//            var store = new X509Store(_secretsProvider.Get("SamEncryptionStorageName"), StoreLocation.LocalMachine);
//            var storePermission = new StorePermission(PermissionState.None);
//            storePermission.Assert();

//            store.Open(OpenFlags.OpenExistingOnly);

//            var subjectName = _secretsProvider.Get("SamEncryptionCertificateSubjectName");
//            return store.Certificates.Cast<X509Certificate2>().FirstOrDefault(c => c.SubjectName.Name == subjectName);
//        }

//        public string Encrypt(string plainText)
//        {
//            var encryptedBytes = Encrypt(Encoding.ASCII.GetBytes(plainText));
//            return Convert.ToBase64String(encryptedBytes);
//        }

//        public string Decrypt(string encryptedString)
//        {
//            var encryptedBytes = Convert.FromBase64String(encryptedString);
//            var decryptedBytes = Decrypt(encryptedBytes);
//            return Encoding.ASCII.GetString(decryptedBytes);
//        }

//        private byte[] Decrypt(byte[] encrypted)
//        {
//            var rsaProvider = RetrieveCertificateFromStore();
//            return ((RSACryptoServiceProvider)rsaProvider.PrivateKey).Decrypt(encrypted, true);
//        }

//        private byte[] Encrypt(byte[] dencrypted)
//        {
//            var rsaProvider = RetrieveCertificateFromStore();
//            return ((RSACryptoServiceProvider)rsaProvider.PublicKey.Key).Encrypt(dencrypted, true);
//        }
//    }
//}
