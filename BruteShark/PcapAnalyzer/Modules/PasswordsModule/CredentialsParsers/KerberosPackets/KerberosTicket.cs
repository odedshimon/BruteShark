using Asn1;
using System;
using System.Collections.Generic;
using System.Text;

namespace PcapAnalyzer
{
    // Ticket ::= [APPLICATION 1] SEQUENCE {
	//      tkt-vno[0]      krb5int32,
    //      realm[1]        Realm,
	//      sname[2]        PrincipalName,
    //      enc-part[3]     EncryptedData
    public class KerberosTicket
    {
        public int TicketVersion { get; private set; }
        public string Realm { get; private set; }
        public KerberosPrincipalName Sname { get; private set; }
        public KerberosEncrypedData EncrytedPart { get; private set; }

        public KerberosTicket(AsnElt ticketData)
        {
            foreach (AsnElt s in ticketData.Sub)
            {
                switch (s.TagValue)
                {
                    case 0:
                        this.TicketVersion = Convert.ToInt32(s.Sub[0].GetInteger());
                        break;
                    case 1:
                        this.Realm = Encoding.ASCII.GetString(s.Sub[0].GetOctetString());
                        break;
                    case 2:
                        this.Sname = new KerberosPrincipalName(principalNameData: s.Sub[0]);
                        break;
                    case 3:
                        this.EncrytedPart = new KerberosEncrypedData(encrypedData: s.Sub[0]);
                        break;
                    default:
                        break;
                }
            }
        }
    }


    // PrincipalName::= SEQUENCE {
    //      name-type[0]    NAME-TYPE,
    //      name-string[1]  SEQUENCE OF GeneralString
    // }
    //
    // NAME-TYPE::= INTEGER {
	//      KRB5_NT_UNKNOWN(0),	-- Name type not known
    //      
    //      KRB5_NT_PRINCIPAL(1),	-- Just the name of the principal as in
	//      KRB5_NT_SRV_INST(2),	-- Service and other unique instance(krbtgt)
    //      KRB5_NT_SRV_HST(3),	-- Service with host name as instance
    //      KRB5_NT_SRV_XHST(4),	-- Service with host as remaining components
    //      KRB5_NT_UID(5),		-- Unique ID
    //      KRB5_NT_X500_PRINCIPAL(6), -- PKINIT
    //      KRB5_NT_SMTP_NAME(7),	-- Name in form of SMTP email name
    //      KRB5_NT_ENTERPRISE_PRINCIPAL(10), -- Windows 2000 UPN
    //      KRB5_NT_WELLKNOWN(11),	-- Wellknown
    //      KRB5_NT_ENT_PRINCIPAL_AND_ID(-130), -- Windows 2000 UPN and SID
    //      KRB5_NT_MS_PRINCIPAL(-128), -- NT 4 style name
    //      KRB5_NT_MS_PRINCIPAL_AND_ID(-129), -- NT style name and SID
    //      KRB5_NT_NTLM(-1200) -- NTLM name, realm is domain
    // }

    public class KerberosPrincipalName
    {
        const int KRB5_NT_SRV_INST = 2;

        public int NameType { get; private set; }
        public List<string> NameString { get; private set; }
        public string Username
        {
            get 
            {
                if (this.NameType == KRB5_NT_SRV_INST && this.NameString.Count >= 2)
                {
                    return this.NameString[0];
                }
                return null;
            }
        }
        public string ServiceName
        {
            get
            {
                if (this.NameType == KRB5_NT_SRV_INST && this.NameString.Count >= 2)
                {
                    return this.NameString[1];
                }
                return null;
            }
        }

        public KerberosPrincipalName(AsnElt principalNameData)
        {
            this.NameString = new List<string>();

            foreach (AsnElt s in principalNameData.Sub)
            {
                switch (s.TagValue)
                {
                    case 0:
                        this.NameType = Convert.ToInt32(s.Sub[0].GetInteger());
                        break;
                    case 1:
                        foreach (AsnElt i in s.Sub[0].Sub)
                        {
                            this.NameString.Add(Encoding.ASCII.GetString(i.GetOctetString()));
                        }
                        break;
                }
            }
        }
    }


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
    // 	    KRB5_ENCTYPE_DIGEST_MD5_NONE(-0x1004),		-- private use, lukeh@padl.com
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
