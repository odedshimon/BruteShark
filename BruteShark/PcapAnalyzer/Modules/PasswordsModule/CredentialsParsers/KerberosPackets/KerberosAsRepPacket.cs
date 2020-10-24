using Asn1;
using System;
using System.Text;

namespace PcapAnalyzer
{
    // AS-REP          ::= [APPLICATION 11] KDC-REP

    // KDC-REP         ::= SEQUENCE {
    //         pvno            [0] INTEGER (5),
    //         msg-type        [1] INTEGER (11 -- AS),
    //         padata          [2] SEQUENCE OF PA-DATA OPTIONAL
    //                                 -- NOTE: not empty --,
    //         crealm          [3] Realm,
    //         cname           [4] PrincipalName,
    //         ticket          [5] Ticket,
    //         enc-part        [6] EncryptedData
    //                                 -- EncASRepPart
    // }
    public class KerberosAsRepPacket
    {
        public long Pvno { get; private set; }
        public long Msg_type { get; private set; }
        public string Crealm { get; private set; }
        public KerberosPrincipalName Cname { get; private set; }
        public AsnElt Padata { get; private set; }
        public KerberosTicket Ticket { get; private set; }
        public AsnElt EncPart { get; private set; }

        public KerberosAsRepPacket(AsnElt[] kdc_rep)
        {
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
                        this.Padata = s.Sub[0];
                        break;
                    case 3:
                        this.Crealm = Encoding.ASCII.GetString(s.Sub[0].GetOctetString());
                        break;
                    case 4:
                        this.Cname = new KerberosPrincipalName(principalNameData: s.Sub[0]);
                        break;
                    case 5:
                        this.Ticket = new KerberosTicket(ticketData: s.Sub[0].Sub[0]);
                        break;
                    case 6:
                        this.EncPart = s.Sub[0];
                        break;
                    default:
                        break;
                }
            }

            var x = 1;
        }
    }
}
