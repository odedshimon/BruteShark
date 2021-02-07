using SIPSorcery.SIP;
using SIPSorcery.Net;
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;


namespace PcapAnalyzer

{
    class VoipCallsModule : IModule
    {
        public string Name => "Voip Calls";
        public event EventHandler<ParsedItemDetectedEventArgs> ParsedItemDetected;
        public HashSet<VoipCall> VoipCalls = new HashSet<VoipCall>();

        public void Analyze(UdpPacket udpPacket) 
        {
            try 
            {   
                // try Decode as RTP
                RTPPacket packet = new RTPPacket(udpPacket.Data);
            } 
            catch (Exception ex) { }
            
            try
            {
                // try Decode as SIP request
                SIPRequest sipRequest = tryDecodeSipRequest(udpPacket);
                AddCall(sipRequest);
            }
            catch (Exception ex)
            { 
                // try Decode as SDP response
                SDP sdpMessage = SDP.ParseSDPDescription(Encoding.UTF8.GetString(udpPacket.Data));
            }

            try
            {
                // try Decode as SIP response
                SIPResponse sipResponse = tryDecodeSipResponse(udpPacket);
                AddCall(sipResponse);
            }
            catch (Exception ex) {}
        }

        public void Analyze(TcpPacket tcpPacket) { }

        public void Analyze(TcpSession tcpSession) {}
        public void Analyze(UdpStream udpStream) {}

        private SIPRequest tryDecodeSipRequest(UdpPacket udpPacket)
        {
            return SIPRequest.ParseSIPRequest(Encoding.UTF8.GetString(udpPacket.Data));
        }
        private SIPResponse tryDecodeSipResponse(UdpPacket udpPacket)
        {
            return SIPResponse.ParseSIPResponse(Encoding.UTF8.GetString(udpPacket.Data));
        }
        private bool AddCall(SIPMessageBase sipMessage)
        {
            var call = new VoipCall(sipMessage.LocalSIPEndPoint, sipMessage.RemoteSIPEndPoint);
            if(sipMessage.GetType() is SIPResponse)
            {
            }
            return VoipCalls.Add(call);
        }
    }
}