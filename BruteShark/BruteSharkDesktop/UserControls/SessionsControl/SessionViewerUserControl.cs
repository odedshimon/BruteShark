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

        public void SetSessionView(TransportLayerSession session)
        {
            SetSessionDetails(session);
            AddColoredSessionData(session);
        }

        private void SetSessionDetails(TransportLayerSession session)
        {
            this.sourceIpLabel.Text = "Source Ip: " + session.SourceIp;
            this.destinationIpLabel.Text = "Destination IP: " + session.DestinationIp;
            this.sourcePortLabel.Text = "Source Port: " + session.SourcePort.ToString();
            this.destinationPortLabel.Text = "Destination Port: " + session.DestinationPort.ToString();
            this.dataLengthLabel.Text = "Data Length (Bytes): " + session.Data.Length.ToString();
        }

        private void AddColoredSessionData(TransportLayerSession session)
        {
            this.sessionDataRichTextBox.Clear();

            foreach (var packet in session.Packets)
            {
                // TODO: add encoding type
                SetSessionData(
                    this.sessionDataRichTextBox, 
                    Encoding.ASCII.GetString(packet.Data),
                    packet.SourceIp == session.SourceIp ? Color.Blue : Color.Red);
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
