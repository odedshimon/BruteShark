# Brute Shark

BruteShark is a Network Forensic Analysis Tool (NFAT) that performs deep processing and inspiction of a network traffic (mainly PCAP files). It includes: password extracting, building a network map, reconstract TCP sessions, extract hashes of encrypted passwords and even convert them to a Hashcat format in order to perform an offline Brute Force attack.

The main goal of the project is to provide solution to security researchers and network administrators with the task of network traffic analysis while they try to identify weaknesses that can be used by a potential attacker to gain access to critical points on the network.

Two BruteShark versions are availble, A GUI based application (Windows) and a Command Line Interface tool (Windows and Linux).  
The various projects in the solution can also be used independently as infrastructure for analyzing network traffic on Linux or Windows machines. For further details see the Architecture section.

The project was developed in my spare time to address two main passions of mine: software arichtecture and analyzing network data.

## What it can do
* Extracting and encoding user credentials 
* Extract authentication hashes and crack them using Hashcat
* Build visual network diagram
* Reconstract all TCP Sessions

## Installation
Windows - Clone and run defauelt Visual Studio Compiler
Linux Users - run BruteSharkCli using MONO.

# Examples
##### How do i crack (by mistake!) Windows 10 user NTLM password
![](NTLM_With_Comments.mp4)
##### Password Extracting (HTTP, TELNET, IMAP, FTP, SMTP)
![](Passwords.PNG)
##### Hashes Extracting (HTTP-Digest, NTLM, CRAM-MD5)
![](Hashes.PNG)
##### Building a Network Diagram
![](Network_Map.mp4)

# Architecture
The solution is designed with three layer architecture, including a one or projects at each layer - DAL, BLL and PL.
The separation between layers is created by the fact that each project refers only its objects.
##### PcapProcessor (DAL)
As the Data Accesss Layer, this project is responsible for reading raw PCAP files using appropriate drivers (WinPcap, libpcap) and their wrapper library SharpPcap.
Can analyze a list of files at once, and provides additional features like reconstraction of all TCP Sessions (using the awesome project TcpRecon).
##### PcapAnalyzer (BLL)
The Bussiness Logic Layer, the 
