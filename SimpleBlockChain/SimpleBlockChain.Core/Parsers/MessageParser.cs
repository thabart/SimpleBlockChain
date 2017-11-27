using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Messages.ControlMessages;
using SimpleBlockChain.Core.Messages.DataMessages;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleBlockChain.Core.Parsers
{
    public class MessageParser
    {
        public Message Parse(byte[] payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (payload.Length < 24)
            {
                throw new ParseMessageException(ErrorCodes.InvalidCommandLength);
            }

            var header = payload.Take(24); // Extract the header.
            var startStringPayload = header.Take(4);
            var network = Networks.MainNet;
            if (startStringPayload.SequenceEqual(new byte[] { 0xf9, 0xbe, 0xb4, 0xd9 }))
            {
                network = Networks.MainNet;
            }
            else if (startStringPayload.SequenceEqual(new byte[] { 0x0b, 0x11, 0x09, 0x07 }))
            {
                network = Networks.TestNet;
            }
            else if (startStringPayload.SequenceEqual(new byte[] { 0xfa, 0xbf, 0xb5, 0xda }))
            {
                network = Networks.RegTest;
            }
            else
            {
                throw new ParseMessageException(ErrorCodes.InvalidStartString);
            }

            var commandNamePayload = header.Skip(4).Take(12).Where(b => b != 0x00).ToArray();
            var commandName = System.Text.Encoding.ASCII.GetString(commandNamePayload);            
            var payloadSizePayload = header.Skip(16).Take(4).ToArray();
            var payloadSize = BitConverter.ToInt32(payloadSizePayload, 0);
            var checkSum = header.Skip(20).Take(4);
            byte[] contentPayload = null;
            if (payloadSize > 0)
            {
                contentPayload = payload.Skip(24).Take(payloadSize).ToArray();
                SHA256 mySHA256 = SHA256Managed.Create();
                var newCheckSum = mySHA256.ComputeHash(mySHA256.ComputeHash(contentPayload)).Take(4);
                if (!newCheckSum.SequenceEqual(checkSum))
                {
                    throw new ParseMessageException(ErrorCodes.InvalidChecksum);
                }
            }
            else if (!checkSum.SequenceEqual(new byte[] { 0x5d, 0xf6, 0xe0, 0xe2 }))
            {
                throw new ParseMessageException(ErrorCodes.InvalidChecksum);
            }
            
            if (!Constants.MessageNameLst.Contains(commandName))
            {
                throw new ParseMessageException(ErrorCodes.InvalidCommandName);
            }

            Message message = null;
            if (commandName == Constants.MessageNames.Ping)
            {
                var nonce = BitConverter.ToUInt64(contentPayload, 0);
                message = new PingMessage(nonce, network);
            }
            else if (commandName == Constants.MessageNames.Addr)
            {
                message = AddrMessage.Deserialize(contentPayload, network);
            }
            else if (commandName == Constants.MessageNames.Version)
            {
                message = VersionMessage.Deserialize(contentPayload, network);
            }
            else if (commandName == Constants.MessageNames.Verack)
            {
                message = new VerackMessage(network);
            }
            else if (commandName == Constants.MessageNames.GetAddr)
            {
                message = new GetAddressMessage(network);
            }
            else if (commandName == Constants.MessageNames.Inventory)
            {
                message = InventoryMessage.Deserialize(contentPayload, network);
            }
            else if (commandName == Constants.MessageNames.Transaction)
            {
                message = TransactionMessage.Deserialize(contentPayload, network, Transactions.TransactionTypes.NoneCoinbase);
            }
            else if (commandName == Constants.MessageNames.Pong)
            {
                var nonce = BitConverter.ToUInt64(contentPayload, 0);
                message = new PongMessage(nonce, network);
            }

            return message;
        }
    }
}
