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
            krb_as_rep = 11,
            krb_tgs_rep = 13
        }

        public static object GetKerberosPacket(byte[] kerberosBuffer, string protocol)
        {
            object result = null;

            if (protocol == "TCP")
            {
                var recordMarkLengthBuffer = kerberosBuffer.SubArray(0, 4);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(recordMarkLengthBuffer);

                var recordMarkLength = BitConverter.ToInt32(recordMarkLengthBuffer, 0);

                if (recordMarkLength + 4 <= kerberosBuffer.Length)
                    kerberosBuffer = kerberosBuffer.SubArray(4, recordMarkLength);
            }

            byte[] asn_buffer = AsnIO.FindBER(kerberosBuffer);

            if (asn_buffer != null)
            {
                AsnElt asn_object = AsnElt.Decode(asn_buffer);

                // Get the application number
                switch (asn_object.TagValue)
                {
                    case (int)MessageType.krb_tgs_rep:
                        result = new KerberosTgsRepPacket(kdc_rep: asn_object.Sub[0].Sub);
                        break;
                    case (int)MessageType.krb_as_rep:
                        result = new KerberosAsRepPacket(kdc_rep: asn_object.Sub[0].Sub);
                        break;
                }
            }

            return result;
        }

    }
}
