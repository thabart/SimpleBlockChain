using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Messages.DataMessages;
using SimpleBlockChain.Core.States;
using SimpleBlockChain.Core.Storages;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Linq;

namespace SimpleBlockChain.Core.Launchers
{
    public class MessageLauncher
    {
        private PeersRepository _peersStorage;

        public MessageLauncher()
        {
            _peersStorage = new PeersRepository();
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
                        PeerEventStore.Instance().NewPeer(ipAddress);
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

        private static void AddTransaction(BaseTransaction transaction)
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

            instance.AddTransaction(transaction);
        }

        public Message ServerRespond(VersionMessage messageVersion)
        {
            if (messageVersion == null)
            {
                throw new ArgumentNullException(nameof(messageVersion));
            }
            
            var instance = PeersStore.Instance();
            var transmittingNode = instance.GetMyIpAddress();
            var receivingNode = messageVersion.TransmittingNode;
            return new VersionMessage(transmittingNode, receivingNode, messageVersion.Nonce, messageVersion.UserAgent, messageVersion.StartHeight, messageVersion.Relay, messageVersion.MessageHeader.Network);
        }

        public Message ServerRespond(VerackMessage verackMessage, Networks network)
        {
            if (verackMessage == null)
            {
                throw new ArgumentNullException(nameof(verackMessage));
            }

            return new VerackMessage(network);
        }
        
        public Message ClientRespond(VersionMessage messageVersion, Networks network)
        {
            if (messageVersion == null)
            {
                throw new ArgumentNullException(nameof(messageVersion));
            }

            return new VerackMessage(network);
        }
    }
}
