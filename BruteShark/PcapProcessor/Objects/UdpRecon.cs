using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

// Translated from the file follow.c from WireShark source code
// the code can be found at: http://www.wireshark.org/download.html

namespace PcapProcessor
{
    // Here we are going to try and reconstruct the data portion of a TCP
    // session. We will try and handle duplicates, TCP fragments, and out
    // of order packets in a smart way.

    /// <summary>
    /// A class that represent a node in a linked list that holds partial Tcp session
    /// fragments
    /// </summary>
    internal class Udpfrag
    {
        public ulong seq = 0;
        public ulong len = 0;
        public ulong data_len = 0;
        public byte[] data = null;
        public Udpfrag next = null;
    };

    public class UdpRecon
    {
        // frags - Holds two linked list of the session data, one for each direction    
        // seq - Holds the last sequence number for each direction
        Udpfrag[] frags = new Udpfrag[2];
        ulong[] seq = new ulong[2];
        long[] src_addr = new long[2];
        uint[] src_port = new uint[2];
        uint[] udp_port = new uint[2];
        uint[] bytes_written = new uint[2];
        bool incomplete_tcp_stream = false;
        bool closed = false;
        byte[] data = new byte[] { };
        public byte[] Data
        {
            get { return this.data; }
            private set { }
        }
        public bool IncompleteStream
        {
            get { return incomplete_tcp_stream; }
        }
        public bool EmptyStream { get; private set; } = true;

        internal List<PacketDotNet.UdpPacket> packets;

        public UdpRecon()
        {
            reset_tcp_reassembly();
            this.packets = new List<PacketDotNet.UdpPacket>();
        }

        /// <summary>
        /// Cleans up the class and frees resources
        /// </summary>
        public void Close()
        {
            if (!closed)
            {
                reset_tcp_reassembly();
                closed = true;
            }
        }

        ~UdpRecon()
        {
            Close();
        }

        /// <summary>
        /// The main function of the class receives a tcp packet and reconstructs the stream
        /// </summary>
        /// <param name="tcpPacket"></param>
        public void ReassemblePacket(PacketDotNet.UdpPacket udpPacket)
        {
            // if the paylod length is zero bail out
            ulong length = (ulong)(udpPacket.Bytes.Length - udpPacket.HeaderData.Length);
            if (length == 0) return;

            reassemble_tcp(
                (ulong)udpPacket.SequenceNumber,
                length,
                udpPacket.PayloadData,
                (ulong)udpPacket.PayloadData.Length,
                udpPacket.Synchronize,
                IpAddressToLong(udpPacket.ParentPacket.Extract<PacketDotNet.IPPacket>().SourceAddress.ToString()),
                IpAddressToLong(udpPacket.ParentPacket.Extract<PacketDotNet.IPPacket>().DestinationAddress.ToString()),
                (uint)udpPacket.SourcePort,
                (uint)udpPacket.DestinationPort,
                udpPacket);

        }

        /// <summary>
        /// Writes the payload data to the file
        /// </summary>
        /// <param name="index"></param>
        /// <param name="data"></param>
        private void write_packet_data(int index, byte[] i_data, PacketDotNet.UdpPacket udpPacket)
        {
            // Add packet to packets list.
            this.packets.Add(udpPacket);

            // ignore empty packets.
            if (i_data.Length == 0) return;

            data = Combine(this.data, i_data);
            bytes_written[index] += (uint)i_data.Length;
            EmptyStream = false;
        }

        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        /// <summary>
        /// Reconstructs the tcp session
        /// </summary>
        /// <param name="sequence">Sequence number of the tcp packet</param>
        /// <param name="length">The size of the original packet data</param>
        /// <param name="data">The captured data</param>
        /// <param name="data_length">The length of the captured data</param>
        /// <param name="synflag"></param>
        /// <param name="net_src">The source ip address</param>
        /// <param name="net_dst">The destination ip address</param>
        /// <param name="srcport">The source port</param>
        /// <param name="dstport">The destination port</param>
        private void reassemble_udp(ulong sequence, ulong length, byte[] data,
                       ulong data_length, bool synflag, long net_src,
                       long net_dst, uint srcport, uint dstport, PacketDotNet.UdpPacket udpPacket)
        {
            long srcx, dstx;
            int src_index, j;
            bool first = false;
            ulong newseq;
            Udpfrag tmp_frag;

            src_index = -1;

            /* Now check if the packet is for this connection. */
            srcx = net_src;
            dstx = net_dst;

            // Check to see if we have seen this source IP and port before.
            // Note: We have to check both source IP and port, the connection
            // might be between two different ports on the same machine.
            for (j = 0; j < 2; j++)
            {
                if (src_addr[j] == srcx && src_port[j] == srcport)
                {
                    src_index = j;
                }
            }

            // We didn't find it if src_index == -1.
            if (src_index < 0)
            {
                // Assign it to a src_index and get going.
                for (j = 0; j < 2; j++)
                {
                    if (src_port[j] == 0)
                    {
                        src_addr[j] = srcx;
                        src_port[j] = srcport;
                        src_index = j;
                        first = true;
                        break;
                    }
                }
            }

            if (src_index < 0)
            {
                throw new Exception("ERROR in reassemble_udp: Too many addresses!");
            }

            if (data_length < length)
            {
                incomplete_tcp_stream = true;
            }

            /* now that we have filed away the srcs, lets get the sequence number stuff
            figured out */
            if (first)
            {
                /* this is the first time we have seen this src's sequence number */
                seq[src_index] = sequence + length;
                if (synflag)
                {
                    seq[src_index]++;
                }
                /* write out the packet data */
                write_packet_data(src_index, data, udpPacket);
                return;
            }
            /* if we are here, we have already seen this src, let's
            try and figure out if this packet is in the right place */
            if (sequence < seq[src_index])
            {
                /* this sequence number seems dated, but
                check the end to make sure it has no more
                info than we have already seen */
                newseq = sequence + length;
                if (newseq > seq[src_index])
                {
                    ulong new_len;

                    /* this one has more than we have seen. let's get the
                    payload that we have not seen. */

                    new_len = seq[src_index] - sequence;

                    if (data_length <= new_len)
                    {
                        data = null;
                        data_length = 0;
                        incomplete_tcp_stream = true;
                    }
                    else
                    {
                        data_length -= new_len;
                        byte[] tmpData = new byte[data_length];
                        for (ulong i = 0; i < data_length; i++)
                            tmpData[i] = data[i + new_len];

                        data = tmpData;
                    }
                    sequence = seq[src_index];
                    length = newseq - seq[src_index];

                    /* this will now appear to be right on time :) */
                }
            }
            if (sequence == seq[src_index])
            {
                /* right on time */
                seq[src_index] += length;
                if (synflag) seq[src_index]++;
                if (data != null)
                {
                    write_packet_data(src_index, data, udpPacket);
                }
                /* done with the packet, see if it caused a fragment to fit */
                while (check_fragments(src_index, udpPacket))
                    ;
            }
            else
            {
                /* out of order packet */
                if (data_length > 0 && sequence > seq[src_index])
                {
                    tmp_frag = new Udpfrag();
                    tmp_frag.data = data;
                    tmp_frag.seq = sequence;
                    tmp_frag.len = length;
                    tmp_frag.data_len = data_length;

                    if (frags[src_index] != null)
                    {
                        tmp_frag.next = frags[src_index];
                    }
                    else
                    {
                        tmp_frag.next = null;
                    }
                    frags[src_index] = tmp_frag;
                }
            }
        }

        /* here we search through all the frag we have collected to see if
        one fits */
        bool check_fragments(int index, PacketDotNet.UdpPacket udpPacket)
        {
            Udpfrag prev = null;
            Udpfrag current;
            current = frags[index];
            while (current != null)
            {
                if (current.seq == seq[index])
                {
                    /* this fragment fits the stream */
                    if (current.data != null)
                    {
                        write_packet_data(index, current.data, udpPacket);
                    }
                    seq[index] += current.len;
                    if (prev != null)
                    {
                        prev.next = current.next;
                    }
                    else
                    {
                        frags[index] = current.next;
                    }
                    current.data = null;
                    current = null;
                    return true;
                }
                prev = current;
                current = current.next;
            }
            return false;
        }

        // cleans the linked list
        void reset_tcp_reassembly()
        {
            Udpfrag current, next;
            int i;

            EmptyStream = true;
            incomplete_tcp_stream = false;
            for (i = 0; i < 2; i++)
            {
                seq[i] = 0;
                src_addr[i] = 0;
                src_port[i] = 0;
                udp_port[i] = 0;
                bytes_written[i] = 0;
                current = frags[i];
                while (current != null)
                {
                    next = current.next;
                    current.data = null;
                    current = null;
                    current = next;
                }
                frags[i] = null;
            }
        }

        private static long IpAddressToLong(string addr)
        {
            // Careful of sign extension: convert to uint first;
            // unsigned NetworkToHostOrder ought to be provided.
            return (long)(uint)IPAddress.NetworkToHostOrder(
                 (int)IPAddress.Parse(addr).Address);
        }

    }

}
