using System;
using System.Collections.Generic;
using System.Text;
using Asn1;

namespace PcapAnalyzer
{
    public class KerberosPacket
    {
        private AsnElt Ticket;
        private AsnElt Enc_part;

        public long Pvno { get; private set; }
        public long Msg_type { get; private set; }
        public string Crealm { get; private set; }
        public AsnElt Cname { get; private set; }
        public AsnElt Padata { get; private set; }

        // 13 - Get service ticket (Response to KRB_TGS_REQ request)
        public enum MessageType : byte
        {
            krb_tgs_rep = 13,
        }

        public KerberosPacket(byte[] kerberosBuffer)
        {
            AsnElt[] kdc_rep = null;
            byte[] asn_buffer = AsnIO.FindBER(kerberosBuffer);

            if (asn_buffer is null)
            {
                throw new Exception("Not a valid Kerberos V5 data");
            }

            AsnElt asn_object = AsnElt.Decode(asn_buffer);

            // Get the application number
            switch (asn_object.TagValue)
            {
                case (int)MessageType.krb_tgs_rep:
                    kdc_rep = asn_object.Sub[0].Sub;
                    break;
            }

            foreach (AsnElt s in kdc_rep)
            {
                switch (s.TagValue)
                {
                    case 0:
                        this.Pvno = s.Sub[0].GetInteger();
                        break;
                    case 1:
                        this.Msg_type = s.Sub[0].GetInteger();
                        break;
                    case 2:
                        // pa-data part, skip
                        this.Padata = s.Sub[0];
                        break;
                    case 3:
                        this.Crealm = Encoding.ASCII.GetString(s.Sub[0].GetOctetString());
                        break;
                    case 4:
                        this.Cname = s.Sub[0];
                        break;
                    case 5:
                        this.Ticket = s.Sub[0].Sub[0];
                        break;
                    case 6:
                        this.Enc_part = s.Sub[0];
                        break;
                    default:
                        break;
                }
            }



        }

    }
}

// TGS-REP         ::= [APPLICATION 13] KDC-REP
   
// KDC-REP         ::= SEQUENCE {
//         pvno            [0] INTEGER (5),
//         msg-type        [1] INTEGER (13 -- TGS),
//         padata          [2] SEQUENCE OF PA-DATA OPTIONAL
//                                 -- NOTE: not empty --,
//         crealm          [3] Realm,
//         cname           [4] PrincipalName,
//         ticket          [5] Ticket,
//         enc-part        [6] EncryptedData
//                                 -- EncTGSRepPart
// }