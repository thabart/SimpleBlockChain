using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Helpers;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.States;
using SimpleBlockChain.Core.Storages;
using System;
using System.Linq;

namespace SimpleBlockChain.Core.Launchers
{
    public class MessageLauncher
    {
        public MessageLauncher()
        {
        }

        public Message Launch(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.GetCommandName() == Constants.MessageNames.Ping)
            {
                var msg = message as PingMessage;
                var pong = new PongMessage(msg.Nonce, msg.MessageHeader.Network);
                return pong;
            }

            if (message.GetCommandName() == Constants.MessageNames.Addr)
            {
                var msg = message as AddrMessage;
                Console.WriteLine("register the address ");
                return null;
            }

            throw new InterpretMessageException(ErrorCodes.MessageNotSupported);
        }

        public void Launch(VerackMessage verackMessage, byte[] ipAdr)
        {
            if (verackMessage == null)
            {
                throw new ArgumentNullException(nameof(verackMessage));
            }

            if (ipAdr == null)
            {
                throw new ArgumentNullException(nameof(ipAdr));
            }

            var instance = ConfigurationStorage.Instance();
            var peerConnectionLst = instance.GetPeerConnectionLst();
            var pc = peerConnectionLst.FirstOrDefault(ar => ar.IpAddress.SequenceEqual(ipAdr));
            if (pc == null || pc.State != PeerConnectionStates.Accepted)
            {
                return;
            }

            Console.WriteLine("Connected to a peer");
            pc.Connect();
        }

        public Message Launch(VersionMessage messageVersion)
        {
            if (messageVersion == null)
            {
                throw new ArgumentNullException(nameof(messageVersion));
            }
            
            var instance = ConfigurationStorage.Instance();
            var peerConnectionLst = instance.GetPeerConnectionLst();
            var pc = peerConnectionLst.FirstOrDefault(ar => ar.IpAddress.SequenceEqual(messageVersion.TransmittingNode.Ipv6));
            if (pc == null)
            {
                pc = new PeerConnection(messageVersion.TransmittingNode.Ipv6);
                instance.AddPeerConnection(pc);
            }

            if (pc.State == PeerConnectionStates.NotConnected)
            {
                var transmittingNode = instance.GetMyIpAddress();
                var receivingNode = messageVersion.TransmittingNode;
                pc.Accept();
                return new VersionMessage(transmittingNode, receivingNode, messageVersion.Nonce, messageVersion.UserAgent, messageVersion.StartHeight, messageVersion.Relay, messageVersion.MessageHeader.Network);
            }

            if (pc.State == PeerConnectionStates.Accepted)
            {
                return new VerackMessage(messageVersion.MessageHeader.Network);
            }

            return null;            
        }
    }
}
