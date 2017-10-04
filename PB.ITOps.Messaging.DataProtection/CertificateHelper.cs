using System;
using System.Security.Cryptography.X509Certificates;

namespace PB.ITOps.Messaging.DataProtection
{
    public static class CertificateHelper
    {
        public static X509Certificate2 FindCertificateByThumbprint(string findValue)
        {
            using (X509Store x509Store1 = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                x509Store1.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certificate2Collection = x509Store1.Certificates.Find(X509FindType.FindByThumbprint, (object)findValue, false);
                if (certificate2Collection.Count == 0)
                {
                    using (X509Store x509Store2 = new X509Store(StoreName.My, StoreLocation.LocalMachine))
                    {
                        x509Store2.Open(OpenFlags.ReadOnly);
                        certificate2Collection = x509Store2.Certificates.Find(X509FindType.FindByThumbprint, (object)findValue, false);
                    }
                    if (certificate2Collection.Count == 0)
                        throw new ArgumentException(string.Format("Could not find certificate with thumbprint starting '{0}'", (object)findValue.Substring(0, 4)));
                }
                return certificate2Collection[0];
            }
        }
    }
}
