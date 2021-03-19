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
        private enum SipResponses
        {
            OK = 200,
            BadRequest = 400,
            InternalServerError = 500
        }
        
        private HashSet<VoipCall> _voipCalls = new HashSet<VoipCall>();
        public string Name => "Voip Calls";

        public event EventHandler<ParsedItemDetectedEventArgs> ParsedItemDetected;
        public event EventHandler<UpdatedPropertyInItemeventArgs> UpdatedItemProprertyDetected;

        public void Analyze(TcpPacket tcpPacket) { }

        public void Analyze(TcpSession tcpSession) { }

        public void Analyze(UdpStream udpStream) { }

        public void Analyze(UdpPacket udpPacket) 
        {
            try 
            {   
                // Try to decode as RTP packet.
                RTPPacket packet = new RTPPacket(udpPacket.Data);
                HandleRTPPAcket(packet, udpPacket.DestinationPort, udpPacket.SourcePort, udpPacket.SourceIp, udpPacket.DestinationIp);
            } 
            catch (Exception ex) { }
            
            try
            {   
                SIPRequest sipRequest = TryDecodeSipRequest(udpPacket);
                
                if(sipRequest.Method == SIPMethodsEnum.INVITE)
                {
                    HandleInitiationRequest(sipRequest, udpPacket.SourceIp, udpPacket.DestinationIp);
                }
                // It is a request thats being sent after the invatation message was answered.
                else
                {
                    HandleDuringCallRequest(sipRequest, udpPacket.SourceIp, udpPacket.DestinationIp);
                }
            }
            catch (Exception ex) 
            {    
                // Try to decode as SIP response.
                SIPResponse sipResponse = TryDecodeSipResponse(udpPacket);
                HandleResponse(sipResponse, udpPacket.SourceIp, udpPacket.DestinationIp);
            }
        }

        private SIPRequest TryDecodeSipRequest(UdpPacket udpPacket)
        {
            return SIPRequest.ParseSIPRequest(Encoding.UTF8.GetString(udpPacket.Data));
        }

        private SIPResponse TryDecodeSipResponse(UdpPacket udpPacket)
        {
            return SIPResponse.ParseSIPResponse(Encoding.UTF8.GetString(udpPacket.Data));
        }

        private void HandleInitiationRequest(SIPRequest sipRequest, string sourceIP, string destinationIP)
        {
            var call = CreateVoipCallFromSipMessage(sipRequest, sourceIP, destinationIP);

            // Check if the voip calls set already contains this call.
            if (_voipCalls.Contains(call))
            {
                switch (sipRequest.Method)
                {
                    case SIPMethodsEnum.ACK:
                        GetCall(call).CallState = CallState.InCall;
                        break;
                    // Call is being cancelled and finished.
                    case SIPMethodsEnum.CANCEL:
                        GetCall(call).CallState = CallState.Cancelled;
                        HandleFinishedCall(call);
                        break;
                    // Call has been completed properly.
                    case SIPMethodsEnum.BYE:
                        GetCall(call).CallState = CallState.Completed;
                        HandleFinishedCall(call);
                        break;
                }
            }
            else
            {
                if (sipRequest.Method == SIPMethodsEnum.INVITE)
                {
                    call.CallState = CallState.Invited;
                    call.callGuid = Guid.NewGuid();
                    _voipCalls.Add(call);

                    // Raise new call event.
                    this.ParsedItemDetected(this, new ParsedItemDetectedEventArgs()
                    {
                        ParsedItem = GetCall(call)
                    });
                }
            }
        }

        private void HandleDuringCallRequest(SIPRequest sipRequest, string sourceIP, string destinationIP)
        {
            var call = CreateVoipCallFromSipMessage(sipRequest, sourceIP, destinationIP);
           
            // Check if the voip calls hash set already contains this call.
            if (_voipCalls.Contains(call))
            {
                switch (sipRequest.Method)
                {
                    case SIPMethodsEnum.ACK:
                        GetCall(call).CallState = CallState.InCall;
                        HandleCallStateUpdate(call, CallState.InCall);
                        break;
                    // Call is being cancelled and finished.
                    case SIPMethodsEnum.CANCEL:
                        GetCall(call).CallState = CallState.Cancelled;
                        HandleCallStateUpdate(call, CallState.Cancelled);
                        HandleFinishedCall(call);
                        break;
                    // Call is being cancelled and finished.
                    case SIPMethodsEnum.BYE:
                        GetCall(call).CallState = CallState.Completed;
                        HandleCallStateUpdate(call, CallState.Completed);
                        HandleFinishedCall(call);
                        break;
                }
            }
        }

        private void HandleResponse(SIPResponse response, string sourceIP, string destinationIP)
        {
            VoipCall call = CreateVoipCallFromSipMessage(response, sourceIP, destinationIP);

            // Check if the voip calls hash set already contains this call.
            if (_voipCalls.Contains(call))
            {
                if (response.StatusCode == (int)SipResponses.OK)
                {
                    SDP SDPmessage = SDP.ParseSDPDescription(response.Body);
                    GetCall(call).RTPPort = SDPmessage.Media[0].Port;
                    HandleRTPPortAdded(call);

                    foreach (var m in SDPmessage.Media[0].MediaFormats)
                    {
                        GetCall(call).RTPMediaType += $"{m.Value.Kind}:{m.Value.Rtpmap} ";
                    }

                    HandleRTPMediaTypeAdded(call);
                }
                else if (response.StatusCode >= (int)SipResponses.BadRequest && response.StatusCode < (int)SipResponses.InternalServerError)
                {
                    GetCall(call).CallState = CallState.Rejected;
                    HandleCallStateUpdate(call, CallState.Rejected);
                    HandleFinishedCall(call);
                }
            }
        }

        private void HandleRTPPAcket(RTPPacket packet, int destinationPort, int sourcePort, string sourceAddress, string DestinationAddress)
        {
            foreach (VoipCall call in _voipCalls)
            {
                if (call.RTPPort == destinationPort || call.RTPPort == sourcePort)
                {
                    if ((call.FromIP == sourceAddress || call.FromIP == DestinationAddress) || (call.ToIP == sourceAddress || call.ToIP == DestinationAddress))
                    {
                        GetCall(call).AddRtpPacket(packet);
                        // TODO: check if this usefull, if not delete function.
                        // HandleRTPPacketAdded(call);
                    }
                }
            }
        }

        private VoipCall CreateVoipCallFromSipMessage(SIPMessageBase sipMessage, string sourceIP, string destinationIP)
        {
            return new VoipCall
            {
                From = sipMessage.Header.From.FromURI.User,
                To = sipMessage.Header.To.ToURI.User,
                ToHost = sipMessage.Header.To.ToURI.Host,
                FromHost = sipMessage.Header.From.FromURI.Host,
                FromIP = sourceIP,
                ToIP = destinationIP
            };
        }

        private VoipCall GetCall(VoipCall call)
        {
            return _voipCalls.Where(c => c.Equals(call)).First();
        }

        private void HandleFinishedCall(VoipCall call)
        {
            if (_voipCalls.Contains(call))
            {
                _voipCalls.Remove(call);
            }
        }

        private void HandleCallStateUpdate(VoipCall call, CallState newState)
        {
            this.UpdatedItemProprertyDetected(this, new UpdatedPropertyInItemeventArgs()
            {
                ParsedItem = GetCall(call),
                PropertyChanged = typeof(VoipCall).GetProperty("CallState"),
                NewPropertyValue = newState.ToString()
            });
        }

        private void HandleRTPPacketAdded(VoipCall call)
        {
            this.UpdatedItemProprertyDetected(this, new UpdatedPropertyInItemeventArgs()
            {
                ParsedItem = GetCall(call),
                PropertyChanged = typeof(VoipCall).GetProperty("_rtpPackets"),
                NewPropertyValue = GetCall(call).RTPStream()
            });
        }

        private void HandleRTPPortAdded(VoipCall call)
        {
            this.UpdatedItemProprertyDetected(this, new UpdatedPropertyInItemeventArgs()
            {
                ParsedItem = GetCall(call),
                PropertyChanged = typeof(VoipCall).GetProperty("RTPPort"),
                NewPropertyValue = GetCall(call).RTPPort
            });
        }

        private void HandleRTPMediaTypeAdded(VoipCall call)
        {
            this.UpdatedItemProprertyDetected(this, new UpdatedPropertyInItemeventArgs()
            {
                ParsedItem = GetCall(call),
                PropertyChanged = typeof(VoipCall).GetProperty("RTPMediaType"),
                NewPropertyValue = GetCall(call).RTPMediaType
            });
        }
    }
}