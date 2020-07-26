using Asn1;
using System;

namespace PcapAnalyzer
{
    // EncryptedData::= SEQUENCE {
    //      etype[0]    ENCTYPE, -- EncryptionType
    //      kvno[1]     krb5uint32 OPTIONAL,
    //      cipher[2]   OCTET STRING -- ciphertext
    // }
    //
    // ENCTYPE ::= INTEGER {
    // 	    KRB5_ENCTYPE_NULL(0),
    // 	    KRB5_ENCTYPE_DES_CBC_CRC(1),
    // 	    KRB5_ENCTYPE_DES_CBC_MD4(2),
    // 	    KRB5_ENCTYPE_DES_CBC_MD5(3),
    // 	    KRB5_ENCTYPE_DES3_CBC_MD5(5),
    // 	    KRB5_ENCTYPE_OLD_DES3_CBC_SHA1(7),
    // 	    KRB5_ENCTYPE_SIGN_DSA_GENERATE(8),
    // 	    KRB5_ENCTYPE_ENCRYPT_RSA_PRIV(9),
    // 	    KRB5_ENCTYPE_ENCRYPT_RSA_PUB(10),
    // 	    KRB5_ENCTYPE_DES3_CBC_SHA1(16),	-- with key derivation
    // 	    KRB5_ENCTYPE_AES128_CTS_HMAC_SHA1_96(17),
    // 	    KRB5_ENCTYPE_AES256_CTS_HMAC_SHA1_96(18),
    // 	    KRB5_ENCTYPE_ARCFOUR_HMAC_MD5(23),
    // 	    KRB5_ENCTYPE_ARCFOUR_HMAC_MD5_56(24),
    // 	    KRB5_ENCTYPE_ENCTYPE_PK_CROSS(48),
    // -    - some "old" windows types
    // 	    KRB5_ENCTYPE_ARCFOUR_MD4(-128),
    // 	    KRB5_ENCTYPE_ARCFOUR_HMAC_OLD(-133),
    // 	    KRB5_ENCTYPE_ARCFOUR_HMAC_OLD_EXP(-135),
    // -    - these are for Heimdal internal use
    // 	    KRB5_ENCTYPE_DES_CBC_NONE(-0x1000),
    // 	    KRB5_ENCTYPE_DES3_CBC_NONE(-0x1001),
    // 	    KRB5_ENCTYPE_DES_CFB64_NONE(-0x1002),
    // 	    KRB5_ENCTYPE_DES_PCBC_NONE(-0x1003),
    // 	    KRB5_ENCTYPE_DIGEST_MD5_NONE(-0x1004),	-- private use, lukeh@padl.com
    // 	    KRB5_ENCTYPE_CRAM_MD5_NONE(-0x1005)		-- private use, lukeh@padl.com
    // }
    public class KerberosEncrypedData
    {
        public uint Kvno { get; private set; }
        public int Etype { get; private set; }
        public byte[] Cipher { get; private set; }

        public KerberosEncrypedData(AsnElt encrypedData)
        {
            foreach (AsnElt s in encrypedData.Sub)
            {
                switch (s.TagValue)
                {
                    case 0:
                        this.Etype = Convert.ToInt32(s.Sub[0].GetInteger());
                        break;
                    case 1:
                        this.Kvno = Convert.ToUInt32(s.Sub[0].GetInteger());
                        break;
                    case 2:
                        this.Cipher = s.Sub[0].GetOctetString();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
