namespace SimpleBlockChain.Core
{
    public static class ErrorCodes
    {
        public const string InvalidChecksum = "invalid_checksum";
        public const string InvalidStartString = "invalid_start_string";
        public const string InvalidCommandLength = "invalid_command_length";
        public const string InvalidCommandName = "invalid_command_name";
        public const string MessageNotSupported = "message_not_supported";
        public const string PayloadSizeInvalid = "payload_size_invalid";
    }
}
