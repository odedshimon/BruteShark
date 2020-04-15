using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BruteSharkCli
{
    public static class Casting
    {
        public static BruteForce.Hash CastAnalyzerHashToBruteForceHash(PcapAnalyzer.NetworkHash hash)
        {
            BruteForce.Hash res = null;

            if (hash is PcapAnalyzer.HttpDigestHash)
            {
                res = CastAnalyzerHashToBruteForceHash(hash as PcapAnalyzer.HttpDigestHash);
            }
            else if (hash is PcapAnalyzer.CramMd5Hash)
            {
                res = CastAnalyzerrHashToBruteForceHash(hash as PcapAnalyzer.CramMd5Hash);
            }
            else if (hash is PcapAnalyzer.NtlmHash)
            {
                res = CastAnalyzerrHashToBruteForceHash(hash as PcapAnalyzer.NtlmHash);
            }
            else
            {
                throw new Exception("Hash type not supported");
            }

            return res;
        }

        public static BruteForce.Hash CastAnalyzerHashToBruteForceHash(PcapAnalyzer.HttpDigestHash httpDigestHash)
        {
            return new BruteForce.HttpDigestHash()
            {
                ServerIp = httpDigestHash.Source,
                Qop = httpDigestHash.Qop,
                Realm = httpDigestHash.Realm,
                Nonce = httpDigestHash.Nonce,
                Uri = httpDigestHash.Uri,
                Cnonce = httpDigestHash.Cnonce,
                Nc = httpDigestHash.Nc,
                Username = httpDigestHash.Username,
                Method = httpDigestHash.Method,
                Response = httpDigestHash.Response
            };
        }

        public static BruteForce.Hash CastAnalyzerrHashToBruteForceHash(PcapAnalyzer.NtlmHash ntlmHash)
        {
            return new BruteForce.NtlmHash()
            {
                Challenge = ntlmHash.Challenge,
                User = ntlmHash.User,
                Domain = ntlmHash.Domain,
                LmHash = ntlmHash.LmHash,
                NtHash = ntlmHash.NtHash,
                Workstation = ntlmHash.Workstation
            };
        }

        public static BruteForce.Hash CastAnalyzerrHashToBruteForceHash(PcapAnalyzer.CramMd5Hash cramMd5Hash)
        {
            return new BruteForce.CramMd5Hash()
            {
                HashedData = cramMd5Hash.Hash,
                Challenge = cramMd5Hash.Challenge
            };
        }
    }
}
