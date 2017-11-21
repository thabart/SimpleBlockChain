using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Messages.DataMessages;
using SimpleBlockChain.Core.States;
using SimpleBlockChain.Core.Storages;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Linq;

namespace SimpleBlockChain.Core.Launchers
{
    public class MessageLauncher
    {
        private PeersStorage _peersStorage;

        public MessageLauncher()
        {
            _peersStorage = new PeersStorage();
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

            if (message.GetCommandName() == Constants.MessageNames.GetAddr)
            {
                var msg = message as GetAddressMessage;
                var ipAdrLst = _peersStorage.GetAll();
                var response = new AddrMessage(new CompactSize { Size = (ulong)ipAdrLst.Count() }, msg.MessageHeader.Network);
                foreach(var ipAdr in ipAdrLst)
                {
                    response.IpAddresses.Add(ipAdr);
                }

                return response;
            }

            if (message.GetCommandName() == Constants.MessageNames.Addr)
            {
                var msg = message as AddrMessage;
                if (msg.IpAddresses != null)
                {
                    foreach(var ipAddress in msg.IpAddresses)
                    {
                        _peersStorage.AddPeer(ipAddress).Wait();
                    }
                }

                return null;
            }

            if (message.GetCommandName() == Constants.MessageNames.Inventory)
            {

            }

            if (message.GetCommandName() == Constants.MessageNames.Transaction)
            {
                var msg = message as TransactionMessage;
                AddTransaction(msg.Transaction);
            }

            throw new InterpretMessageException(ErrorCodes.MessageNotSupported);
        }

        private static void AddTransaction(Transaction transaction)
        {
            if (transaction == null)
            {
                return;
            }

            var instance = MemoryPool.Instance();
            var transactions = instance.GetTransactions();
            if (transactions.Any(t => t.GetTxId().SequenceEqual(transaction.GetTxId())))
            {
                return;
            }

            // TODO : Check the BLOCK CHAIN CONTAINS THE TRANSACTION.

            if (!transaction.Check())
            {
                return;
            }

            instance.AddTransaction(transaction);
        }

        public Message Launch(VerackMessage verackMessage, byte[] ipAdr)
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
            if (pc == null)
            {
                return null;
            }

            pc.Connect();
            return new VerackMessage(verackMessage.MessageHeader.Network);
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
