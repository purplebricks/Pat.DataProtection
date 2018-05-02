using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Pat.DataProtection
{
    public static class DataProtectionProvider
    {
        public static IDataProtectionProvider Create(DataProtectionConfiguration config)
        {
            var account = config.AccountName;
            var keyVal = config.AccountKey;
            var cleanMachineName = Regex.Replace(Environment.MachineName, "[^a-zA-Z0-9]", "");
            var storageAccount = new CloudStorageAccount(new StorageCredentials(account, keyVal), true);

            string relativePath = $"/data-protection-keys/patlite-{config.ApplicationName}.xml";
#if DEBUG
            relativePath = $"/data-protection-keys/patlite-{config.ApplicationName}-{cleanMachineName}.xml";
#endif

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