using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Cryptography;
using Ncms.Contracts;

namespace Ncms.Bl
{
    public class SecurityService : ISecurityService
    {
        private readonly IUserService _userService;
        private const int SaltSize = 16;
        private const int HashSize = 20;
        private const int HashIterations = 10000;

        public SecurityService(IUserService userService)
        {
            _userService = userService;
        }

        private bool VerifyPassword(string passwordHash, string password)
        {
            if (!String.IsNullOrWhiteSpace(passwordHash) && !String.IsNullOrWhiteSpace(password))
            {
                // decode the actual bytes of the combined salt+hash
                byte[] combinedBytes = Convert.FromBase64String(passwordHash);

                // extract the salt and hashed password
                byte[] salt = new byte[SaltSize];
                byte[] storedHash = new byte[HashSize];
                Array.Copy(combinedBytes, 0, salt, 0, SaltSize);
                Array.Copy(combinedBytes, SaltSize, storedHash, 0, HashSize);

                // compute the hash for the given password, using the same salt
                var hash = PrepareHashWithSalt(password, salt);

                // compare
                return hash.SequenceEqual(storedHash);
            }
            return false;
        }

        public string GetPasswordHash(string password)
        {
            // create the salt value with a cryptographic PRNG
            byte[] salt = new byte[SaltSize];
            new RNGCryptoServiceProvider().GetBytes(salt);

            // get the hash value
            var hash = PrepareHashWithSalt(password, salt);

            // combine the salt and password bytes
            byte[] combined = new byte[HashSize + SaltSize];
            Array.Copy(salt, 0, combined, 0, SaltSize);
            Array.Copy(hash, 0, combined, SaltSize, HashSize);

            // convert to a string for storage
            return Convert.ToBase64String(combined);
        }

        private byte[] PrepareHashWithSalt(string password, byte[] salt)
        {
            // yum...
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, HashIterations);
            return pbkdf2.GetBytes(HashSize);
        }

        public bool AuthenticateNonWindowsUser(ref string username, string password)
        {
            if (!String.IsNullOrWhiteSpace(username) && !String.IsNullOrWhiteSpace(password))
            {
                var user = _userService.GetUser(username);
                if (user != null)
                {
                    if (VerifyPassword(user.Password, password))
                    {
                        username = user.Name;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool AuthenticateWindowsUser(string username)
        {
            if (!String.IsNullOrWhiteSpace(username))
            {
                var user = _userService.GetUser(username);
                if (user != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool AuthenticateWindowsUser(ref string username, string password)
        {
            if (!String.IsNullOrWhiteSpace(username))
            {
                var user = _userService.GetUser(username);
                if (user != null)
                {
                    PrincipalContext context;
                    var parts = username.Split('\\');
                    if (parts.Length == 1)
                    {
                        context = new PrincipalContext(ContextType.Domain);
                    }
                    else
                    {
                        context = new PrincipalContext(ContextType.Domain, parts[0]);
                        username = parts[1];
                    }

                    using (context)
                    {
                        if (context.ValidateCredentials(username, password))
                        {
                            username = user.Name;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
