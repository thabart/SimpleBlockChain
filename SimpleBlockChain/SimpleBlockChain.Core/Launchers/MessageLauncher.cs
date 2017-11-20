using SimpleBlockChain.Core.Exceptions;
using SimpleBlockChain.Core.Messages;
using SimpleBlockChain.Core.Messages.ControlMessages;
using System;

namespace SimpleBlockChain.Core.Launchers
{
    public class MessageLauncher
    {
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

            if (message.GetCommandName() == Constants.MessageNames.Version)
            {
                var msg = message as VersionMessage;
                return new VerackMessage(msg.MessageHeader.Network);   
            }

            throw new InterpretMessageException(ErrorCodes.MessageNotSupported);
        }
    }
}
