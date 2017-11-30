using SimpleBlockChain.Core.Common;
using SimpleBlockChain.Core.Connectors;
using SimpleBlockChain.Core.Evts;
using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Messages.DataMessages;
using SimpleBlockChain.Core.Storages;
using SimpleBlockChain.Core.Stores;
using SimpleBlockChain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlockChain.Core.Launchers
{
    public class MessageLauncher
    {
        private readonly P2PNetworkConnector _p2pNetworkConnector;
        private PeersRepository _peersStorage;

        public MessageLauncher(P2PNetworkConnector p2pNetworkConnector)
        {
            _p2pNetworkConnector = p2pNetworkConnector;
            _peersStorage = new PeersRepository();
        }

        public Message ReceiverRespond(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var result = CommonRespond(message);
            if (result != null)
            {
                return result;
            }

            if (message.GetCommandName() == Constants.MessageNames.Verack) // RECEIVE VERACK MESSAGE.
            {
                var msg = message as VerackMessage;
                return new VerackMessage(msg.MessageHeader.Network);
            }

            if (message.GetCommandName() == Constants.MessageNames.Version) // RECEIVE VERSION MESSAGE.
            {
                var msg = message as VersionMessage;
                var instance = PeersStore.Instance();
                var transmittingNode = instance.GetMyIpAddress();
                var receivingNode = msg.TransmittingNode;
                return new VersionMessage(transmittingNode, receivingNode, msg.Nonce, msg.UserAgent, msg.StartHeight, msg.Relay, msg.MessageHeader.Network);
            }

            if (message.GetCommandName() == Constants.MessageNames.Ping) // RECEIVE A PING REQUEST.
            {
                var msg = message as PingMessage;
                var pong = new PongMessage(msg.Nonce, msg.MessageHeader.Network);
                return pong;
            }

            if (message.GetCommandName() == Constants.MessageNames.GetAddr) // RECEIVE ADDRS.
            {
                var msg = message as GetAddressMessage;
                var ipAdrLst = _peersStorage.GetAll();
                var response = new AddrMessage(new CompactSize { Size = (ulong)ipAdrLst.Count() }, msg.MessageHeader.Network);
                foreach (var ipAdr in ipAdrLst)
                {
                    response.IpAddresses.Add(ipAdr);
                }

                return response;
            }
            
            if (message.GetCommandName() == Constants.MessageNames.MemPool) // RETURNS ALL THE TXIDS.
            {
                var msg = message as MemPoolMessage;
                var memoryPool = MemoryPool.Instance();
                var txIds = memoryPool.GetTransactions().Select(t => t.GetTxId());
                var inventories = new List<Inventory>();
                foreach (var txId in txIds)
                {
                    inventories.Add(new Inventory(InventoryTypes.MSG_TX, txId));
                }

                return new InventoryMessage(inventories, msg.MessageHeader.Network);
            }

            if (message.GetCommandName() == Constants.MessageNames.Transaction) // ADD A TRANSACTION INTO MEMORY POOL.
            {
                var msg = message as TransactionMessage;
                AddTransaction(msg.Transaction);
                return null;
            }

            if (message.GetCommandName() == Constants.MessageNames.Block) // ADD BLOCK INTO THE BLOCK CHAIN.
            {
                // TODO : IMPLEMENT.
            }

            throw new InterpretMessageException(ErrorCodes.MessageNotSupported);
        }

        public IEnumerable<Message> SenderRespond(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var result = CommonRespond(message);
            if (result != null)
            {
                return new[] { result };
            }

            if (message.GetCommandName() == Constants.MessageNames.Addr) // ADD THE PEERS.
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

            if (message.GetCommandName() == Constants.MessageNames.GetData) // GET THE DATA.
            {
                var msg = message as GetDataMessage;
                var blockChain = BlockChainStore.Instance().GetBlockChain();
                var inventories = new List<Inventory>();
                var messages = new List<Message>();
                if (msg.Inventories.Any())
                {
                    foreach(var inventory in msg.Inventories)
                    {
                        switch(inventory.Type)
                        {
                            case InventoryTypes.MSG_BLOCK:
                                var block = blockChain.GetBlock(inventory.Hash);
                                if (block != null)
                                {
                                    var blockMessage = new BlockMessage(block, msg.MessageHeader.Network);
                                    messages.Add(blockMessage);
                                }

                                break;
                            case InventoryTypes.MSG_TX:
                                var transaction = blockChain.GetTransaction(inventory.Hash);
                                if (transaction != null)
                                {
                                    var transactionMessage = new TransactionMessage(transaction, msg.MessageHeader.Network);
                                    messages.Add(transactionMessage);
                                }
                                break;
                        }
                    }
                }

                return messages;
            }

            if (message.GetCommandName() == Constants.MessageNames.Version) // RECEIVE VERSION MESSAGE.
            {
                var msg = message as VersionMessage;
                return new[] { new VerackMessage(msg.MessageHeader.Network) };
            }

            if (message.GetCommandName() == Constants.MessageNames.Inventory) // RECEIVE INVENTORY MESSAGE.
            {

            }

            throw new InterpretMessageException(ErrorCodes.MessageNotSupported);
        }

        private Message CommonRespond(Message message)
        {
            if (message.GetCommandName() == Constants.MessageNames.Inventory) // RETURNS NOT KNOWN OBJECTS.
            {
                var msg = message as InventoryMessage;
                var blockChain = BlockChainStore.Instance().GetBlockChain();
                var inventories = new List<Inventory>();
                if (msg.Inventories.Any())
                {
                    foreach (var inventory in msg.Inventories)
                    {
                        bool addIntoInventory = false;
                        switch (inventory.Type)
                        {
                            case InventoryTypes.MSG_TX:
                                addIntoInventory = !blockChain.ContainsTransaction(inventory.Hash);
                                break;
                            case InventoryTypes.MSG_BLOCK:
                                addIntoInventory = !blockChain.ContainsBlock(inventory.Hash);
                                break;
                        }

                        if (addIntoInventory)
                        {
                            inventories.Add(new Inventory(inventory.Type, inventory.Hash));
                        }
                    }
                }

                if (inventories.Any())
                {
                    return new GetDataMessage(inventories, msg.MessageHeader.Network);
                }

                return null;
            }

            return null;
        }

        private void AddTransaction(BaseTransaction transaction)
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

            var blockChain = BlockChainStore.Instance().GetBlockChain();
            if (blockChain.ContainsTransaction(transaction.GetTxId()))
            {
                return;
            }

            instance.AddTransaction(transaction);
            _p2pNetworkConnector.Broadcast(transaction);
        }
    }
}
