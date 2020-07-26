using Asn1;
using System;
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
}
