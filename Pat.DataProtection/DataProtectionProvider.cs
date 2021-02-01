using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Azure.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;

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
            var relativePath = $"/patlite-{config.ApplicationName}.xml";

            var certificate = string.IsNullOrEmpty(config.Certificate)
                ? CertificateHelper.FindCertificateByThumbprint(config.Thumbprint)
                : new X509Certificate2(Convert.FromBase64String(config.Certificate));

            return Microsoft.AspNetCore.DataProtection.DataProtectionProvider.Create(
                new DirectoryInfo(@"~/keyring"),
                builder =>
                {
                    builder.SetApplicationName(config.ApplicationName);

                    if (!string.IsNullOrEmpty(config.StorageAccountUrl))
                    {
                        builder.PersistKeysToAzureBlobStorage(new Uri(config.StorageAccountUrl + relativePath), new DefaultAzureCredential());
                    }
                    else
                    {
                        builder.PersistKeysToAzureBlobStorage(new CloudStorageAccount(new StorageCredentials(config.AccountName, config.AccountKey), true), relativePath);
                    }

                    builder.ProtectKeysWithCertificate(certificate);
                });
        }
    }
}