using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SimpleBlockChain.Core.Messages
{
    public abstract class Message
    {
        public MessageHeader MessageHeader { get; set;}

        public Message(Networks network)
        {
            MessageHeader = new MessageHeader
            {
                Network = network
            };
        }

        public byte[] Serialize()
        {
            var result = new List<byte>();
            result.AddRange(MessageHeader.GetStartString());
            var commandName = GetCommandName();
            List<byte> commandNameBuffer = System.Text.Encoding.ASCII.GetBytes(commandName).ToList();
            for (var i = commandNameBuffer.Count(); i < 12; i++)
            {
                commandNameBuffer.Add(0x00);
            }

            result.AddRange(commandNameBuffer);
            var content = GetSerializedContent();
            var size = BitConverter.GetBytes(content.Length);
            result.AddRange(size);
            byte[] checksum = null;
            if (content == null)
            {
                checksum = new byte[] { 0x5d, 0xf6, 0xe0, 0xe2 };
            }
            else
            {
                SHA256 mySHA256 = SHA256.Create();
                var hashed = mySHA256.ComputeHash(mySHA256.ComputeHash(content));
                checksum = hashed.Take(4).ToArray();
            }

            result.AddRange(checksum);
            result.AddRange(content);
            return result.ToArray();
        }

        protected abstract byte[] GetSerializedContent();
        public abstract string GetCommandName();
    }
}
