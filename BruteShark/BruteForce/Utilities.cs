using System;

namespace BruteForce
{
    [Serializable]
    public class NotSupportedHashcatHash : Exception 
    {
        public NotSupportedHashcatHash(string message) : base(message) { }
    }

    public static class Utilities
    {
        public static string ConvertToHashcatFormat(Hash hash)
        {
            var res = string.Empty;

            if (hash is HttpDigestHash)
            {
                res = ConvertToHashcatFormat(hash as HttpDigestHash);
            }
            else if (hash is CramMd5Hash)
            {
                res = ConvertToHashcatFormat(hash as CramMd5Hash);
            }
            else if (hash is NtlmHash)
            {
                res = ConvertToHashcatFormat(hash as NtlmHash);
            }
            else if (hash is KerberosHash)
            {
                res = ConvertToHashcatFormat(hash as KerberosHash);
            }
            else if (hash is KerberosTgsRepHash)
            {
                res = ConvertToHashcatFormat(hash as KerberosTgsRepHash);
            }
            else if (hash is KerberosAsRepHash)
            {
                res = ConvertToHashcatFormat(hash as KerberosAsRepHash);
            }
            else
            {
                throw new NotSupportedHashcatHash("Hash type not supported");
            }

            return res;
        }
        
        public static string ConvertToHashcatFormat(KerberosTgsRepHash kerberosHash)
        {
            if (kerberosHash.Etype == 23)
            {
                return string.Format("$krb5tgs${0}$*{1}${2}${3}*${4}${5}",
                    kerberosHash.Etype,
                    kerberosHash.Username,
                    kerberosHash.Realm,
                    kerberosHash.ServiceName,
                    kerberosHash.HashedData.Substring(0, 32),
                    kerberosHash.HashedData.Substring(32));
            }
            else if (kerberosHash.Etype == 18 || kerberosHash.Etype == 17)
            {
                var checksumStartPosition = kerberosHash.HashedData.Length - 24;

                return String.Format("$krb5tgs${0}${1}${2}$*{3}*${4}${5}",
                    kerberosHash.Etype, 
                    kerberosHash.Username, 
                    kerberosHash.Realm, 
                    kerberosHash.ServiceName, 
                    kerberosHash.HashedData.Substring(checksumStartPosition),
                    kerberosHash.HashedData.Substring(0, checksumStartPosition));
            }
            else
            {
                throw new NotSupportedHashcatHash($"Kerberos TGS-REP Etype {kerberosHash.Etype} is not supported by Hashcat");
            }
        }

        public static string ConvertToHashcatFormat(KerberosAsRepHash kerberosHash)
        {
            if (kerberosHash.Etype == 23)
            {
                // $krb5asrep$23$user@domain.com:3e156ada591263b8aab0965f5aebd837$007497cb5....
                return string.Format("$krb5asrep$23${0}@{1}:{2}${3}",
                    kerberosHash.Username,
                    kerberosHash.Realm,
                    kerberosHash.HashedData.Substring(0, 32),
                    kerberosHash.HashedData.Substring(32));
            }
            else
            {
                throw new NotSupportedHashcatHash($"Kerberos AS-REP Etype {kerberosHash.Etype} is not supported by Hashcat");
            }
        }

        public static string ConvertToHashcatFormat(KerberosHash kerberosHash)
        {
            return string.Format("$krb5pa$23${0}${1}$dummy${2}",
                kerberosHash.User,
                kerberosHash.Domain,
                kerberosHash.HashedData);
        }

        public static string ConvertToHashcatFormat(HttpDigestHash httpDigestHash)
        {
            // https://hashcat.net/forum/thread-6571.html
            // $sip$***[username] *[realm] * GET *[uri_protocol] *[uri_ip] *[uri_port] *[nonce] *[clientNonce] *[nonceCount] *[qop] * MD5 *[response]
            // TODO: Test and parse protocol and algorithm if needed.
            return string.Format("$sip$***{0}*{1}*{2}**{3}**{4}*{5}*{6}*{7}*{8}*{9}",
                httpDigestHash.Username,
                httpDigestHash.Realm,
                "GET",
                httpDigestHash.Uri,
                httpDigestHash.Nonce,
                httpDigestHash.Cnonce,
                httpDigestHash.Nc,
                httpDigestHash.Qop,
                "MD5",
                httpDigestHash.Response);
        }

        public static string ConvertToHashcatFormat(CramMd5Hash cramMd5Hash)
        {
            return $"$cram_md5${cramMd5Hash.Challenge}${cramMd5Hash.HashedData}";
        }

        public static string ConvertToHashcatFormat(NtlmHash ntlmHash)
        {
            var res = string.Empty;

            // Ntlm v1
            if (ntlmHash.NtHash.Length == 24)
            {
                res = string.Format("{0}::{1}:{2}:{3}:{4}",
                    ntlmHash.User,
                    ntlmHash.Domain,
                    ntlmHash.LmHash,
                    ntlmHash.NtHash,
                    ntlmHash.Challenge);
            }
            // Ntlm v2
            else if (ntlmHash.NtHash.Length > 24)
            {
                res = string.Format("{0}::{1}:{2}:{3}:{4}",
                    ntlmHash.User,
                    ntlmHash.Domain,
                    ntlmHash.Challenge,
                    ntlmHash.NtHash.Substring(0, 32),
                    ntlmHash.NtHash.Substring(32));
            }
            else
            {
                throw new Exception("Ntlm hash has nt part shorter than 24 chars");
            }

            return res;
        }

    }
}
