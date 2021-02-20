using SIPSorcery.SIP;
using SIPSorcery.Net;
using System;
using System.Net;
using System.Text;
using System.Linq;
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
                HandleRTPPAcket(packet, udpPacket.DestinationPort, udpPacket.SourceIp, udpPacket.DestinationIp);
            } 
            catch (Exception ex) { }
            
            try
            {   
                SIPRequest sipRequest = tryDecodeSipRequest(udpPacket);
                
                if(sipRequest.Method == SIPMethodsEnum.INVITE)
                {
                    HandleInitiationRequest(sipRequest ,udpPacket.Data, udpPacket.SourceIp, udpPacket.DestinationIp);
                }
                // it's a request that's being sent after the invatation message was answered
                else
                {
                    HandleDuringCallRequest(sipRequest ,udpPacket.Data, udpPacket.SourceIp, udpPacket.DestinationIp);
                }
            }
            catch (Exception ex) 
            {    
                // try Decode as SIP response
                SIPResponse sipResponse = tryDecodeSipResponse(udpPacket);
                HandleResponse(sipResponse, udpPacket.SourceIp, udpPacket.DestinationIp);
            }
        }
        public void Analyze(TcpPacket tcpPacket) {}
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
        private void HandleInitiationRequest(SIPRequest sipRequest, byte[] rawData, string sourceIP, string destinationIP)
        {
            var call = createVoipCallFromSipMessage(sipRequest, sourceIP, destinationIP);

            // check if the voip calls set already contains this call
            if (VoipCalls.Contains(call))
            {
                switch (sipRequest.Method)
                {
                    case SIPMethodsEnum.ACK:
                        getCall(call).CallState = CallState.InCall;
                        break;
                    case SIPMethodsEnum.CANCEL:
                        // the call is being cancelled and finished
                        getCall(call).CallState = CallState.Cancelled;
                        handleFinishedCall(call);
                        break;
                    // the call is being completed propertly
                    case SIPMethodsEnum.BYE:
                        getCall(call).CallState = CallState.Completed;
                        handleFinishedCall(call);
                        break;
                }
            }
            else
            {
                if (sipRequest.Method == SIPMethodsEnum.INVITE)
                {
                    call.CallState = CallState.Invited;
                    VoipCalls.Add(call);
                }
            }
        }
        private void HandleDuringCallRequest(SIPRequest sipRequest, byte[] rawData, string sourceIP, string destinationIP)
        {
            var call = createVoipCallFromSipMessage(sipRequest, sourceIP, destinationIP);
           
            // check if the voip calls set already contains this call
            if (VoipCalls.Contains(call))
            {
                switch (sipRequest.Method)
                {
                    case SIPMethodsEnum.ACK:
                        getCall(call).CallState = CallState.InCall;
                        break;
                    case SIPMethodsEnum.CANCEL:
                        getCall(call).CallState = CallState.Cancelled;
                        // call is being cancelled and finished
                        handleFinishedCall(call);
                        break;
                    case SIPMethodsEnum.BYE:
                        // call is being cancelled and finished
                        getCall(call).CallState = CallState.Completed;
                        handleFinishedCall(call);
                        break;
                }
            }
        }
            
        private void HandleResponse(SIPResponse response, string sourceIP, string destinationIP)
        {
            VoipCall call = createVoipCallFromSipMessage(response, sourceIP, destinationIP);

            // check if the voip calls set already contains this call
            if (VoipCalls.Contains(call))
            {
                // parse status
                if (response.StatusCode == 200)
                {
                    SDP SDPmessage = SDP.ParseSDPDescription(response.Body);
                    getCall(call).RTPPort = SDPmessage.Media[0].Port;
                    foreach(var mediaFormat in SDPmessage.Media[0].MediaFormats)
                    {
                        getCall(call).RTPMediaTypes.Add(mediaFormat.Value.Rtpmap);
                    }
                }
                else if(response.StatusCode >= 400 && response.StatusCode < 500)
                {
                    getCall(call).CallState = CallState.Rejected;
                    handleFinishedCall(call);
                }
                    
            }
        }
        private void HandleRTPPAcket(RTPPacket packet, int packetPort, string sourceAddress, string DestinationAddress)
        {
            foreach(VoipCall call in VoipCalls)
            {
                if (call.RTPPort == packetPort)
                {
                    if ((call.FromIP == sourceAddress || call.FromIP == DestinationAddress) || (call.ToIP == sourceAddress || call.ToIP == DestinationAddress))
                    {
                        call.addrtpPacket(packet);
                    }
                }
            }
        }
        private VoipCall createVoipCallFromSipMessage(SIPMessageBase sipMessage, string sourceIP, string destinationIP)
        {
            var call = new VoipCall();
            call.From = sipMessage.Header.From.FromURI.User;
            call.To = sipMessage.Header.To.ToURI.User;
            call.ToHost = sipMessage.Header.To.ToURI.Host;
            call.FromHost = sipMessage.Header.From.FromURI.Host;
            call.FromIP = sourceIP;
            call.ToIP = destinationIP;
            return call;
        }

        private VoipCall getCall(VoipCall call)
        {
            return VoipCalls.Where(c => c.Equals(call)).First();
        }
        
        private void handleFinishedCall(VoipCall call)
        {
            if (VoipCalls.Contains(call))
            {
                this.ParsedItemDetected(this, new ParsedItemDetectedEventArgs()
                {
                    ParsedItem = getCall(call)
                });
                VoipCalls.Remove(call);
            }
        }
    }
}