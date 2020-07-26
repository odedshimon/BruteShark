using System;
using System.Collections.Generic;
using System.Text;
using Asn1;

namespace PcapAnalyzer
{
    // All objects definitions are from: http://web.mit.edu/freebsd/head/crypto/heimdal/lib/asn1/krb5.asn1
    public static class KerberosPacketParser
    {
        // 13 - Get service ticket (Response to KRB_TGS_REQ request)
        public enum MessageType : byte
        {
            krb_tgs_rep = 13,
        }

        public static object GetKerberosPacket(byte[] kerberosBuffer)
        {
            object result = null;
            byte[] asn_buffer = AsnIO.FindBER(kerberosBuffer);

            if (asn_buffer != null)
            {
                AsnElt asn_object = AsnElt.Decode(asn_buffer);

                try
                {
                    // Get the application number
                    switch (asn_object.TagValue)
                    {
                        case (int)MessageType.krb_tgs_rep:
                            result = new KerberosTgsRepPacket(kdc_rep: asn_object.Sub[0].Sub);
                            break;
                    }
                }
                catch
                { 
                    // TODO: log
                }
            }

            return result;
        }

    }
}
