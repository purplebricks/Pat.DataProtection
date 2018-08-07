using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Pat.DataProtection
{
    /// <summary>
    /// Factory method to create an instance of Microsoft.AspNetCore.DataProtection.DataProtectionProvider
    /// </summary>
    public static class DataProtectionProvider
    {
        /// <summary>
        /// Creates an Microsoft.AspNetCore.DataProtection.DataProtectionProvider given 
        /// appropriate configuration.
        /// </summary>
        /// <param name="config">The configuration applied to the DataProtectionProvider </param>
        /// <returns>An instance of Microsoft.AspNetCore.DataProtection.DataProtectionProvider</returns>
        public static IDataProtectionProvider Create(DataProtectionConfiguration config)
        {
            var account = config.AccountName;
            var keyVal = config.AccountKey;
            var cleanMachineName = Regex.Replace(Environment.MachineName, "[^a-zA-Z0-9]", "");
            var storageAccount = new CloudStorageAccount(new StorageCredentials(account, keyVal), true);

            string relativePath = $"/data-protection-keys/patlite-{config.ApplicationName}.xml";

            var certificate = CertificateHelper.FindCertificateByThumbprint(config.Thumbprint);

            return Microsoft.AspNetCore.DataProtection.DataProtectionProvider.Create(
                new DirectoryInfo(@"c:\keyring"),
                builder =>
                {
                    builder
                        .SetApplicationName(config.ApplicationName)
                        .PersistKeysToAzureBlobStorage(storageAccount, relativePath)
                        .ProtectKeysWithCertificate(certificate);
                });
        }
    }
}