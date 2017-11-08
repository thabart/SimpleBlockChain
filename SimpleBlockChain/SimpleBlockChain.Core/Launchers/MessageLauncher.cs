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

            throw new InterpretMessageException(ErrorCodes.MessageNotSupported);
        }
    }
}
