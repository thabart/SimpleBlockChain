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
        public const string PreviousHashBlockDoesntMatch = "previous_hash_block_doesnt_match";
        public const string InvalidMerkleRoot = "invalid_merkle_root";
        public const string InvalidPreviousHashHeader = "invalid_previous_hash_header";
        public const string ReferencedTransactionNotValid = "referenced_transaction_not_valid";
        public const string TransactionSignatureNotCorrect = "transaction_signature_not_correct";
        public const string NoTransactionIn = "no_in_transaction";
        public const string TransactionOutputExceedInput = "transaction_output_exceed_input";
        public const string PeerRpcError = "peer_rpc_error";
        public const string SeedPeersNotReachable = "seed_peers_not_reachable";
    }
}
