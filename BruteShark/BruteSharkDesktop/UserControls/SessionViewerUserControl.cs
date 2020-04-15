using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PcapProcessor;

namespace BruteSharkDesktop
{
    public partial class SessionViewerUserControl : UserControl
    {
        public SessionViewerUserControl()
        {
            InitializeComponent();
        }

        public void SetSessionView(TcpSession tcpSession)
        {
            SetSessionDetails(tcpSession);
            AddColoredSessionData(tcpSession);
        }

        private void SetSessionDetails(TcpSession tcpSession)
        {
            this.sourceIpLabel.Text = "Source Ip: " + tcpSession.SourceIp;
            this.destinationIpLabel.Text = "Destination IP: " + tcpSession.DestinationIp;
            this.sourcePortLabel.Text = "Source Port: " + tcpSession.SourcePort.ToString();
            this.destinationPortLabel.Text = "Destination Port: " + tcpSession.DestinationPort.ToString();
            this.dataLengthLabel.Text = "Data Length (Bytes): " + tcpSession.Data.Length.ToString();
        }

        private void AddColoredSessionData(TcpSession tcpSession)
        {
            this.sessionDataRichTextBox.Clear();

            foreach (var packet in tcpSession.Packets)
            {
                // TODO: add encoding type
                SetSessionData(
                    this.sessionDataRichTextBox, 
                    Encoding.ASCII.GetString(packet.Data),
                    packet.SourceIp == tcpSession.SourceIp ? Color.Blue : Color.Red);
            }
        }

        public void SetSessionData(RichTextBox richTextBox, string text, Color color)
        {
            richTextBox.SelectionStart = richTextBox.TextLength;
            richTextBox.SelectionLength = 0;
            richTextBox.SelectionColor = color;
            richTextBox.AppendText(text);
            richTextBox.SelectionColor = richTextBox.ForeColor;
        }
    }
}
