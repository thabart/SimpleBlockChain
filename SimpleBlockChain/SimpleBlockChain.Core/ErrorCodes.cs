﻿namespace SimpleBlockChain.Core
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
        public const string P2PNotReachable = "p2p_not_connected";
        public const string NotCorrectNodeService = "not_correct_node_service";
        public const string NotEnoughDifficult = "not_enough_difficult";
        public const string BadPassword = "bad_password";
        public const string AlreadyExists = "already_exists";
        public const string DoesntExist = "doesnt_exist";
        public const string CannotExtractKey = "cannot_extract_key";
        public const string CannotExtractAddress = "cannot_extract_address";
        public const string ParameterMissing = "{0}_missing";
        public const string NotValidJson = "{0}_not_json";
        public const string NotCorrectNetwork = "not_correct_network";
        public const string NotCorrectType = "not_correct_type";
        public const string NoResult = "no_result";
        public const string AlreadySpentInMemoryPool = "already_spent_in_memory_pool";
        public const string FromInvalidLength = "from_invalid_length";
        public const string ToInvalidLength = "to_invalid_length";
        public const string SmartContractNotValid = "smart_contract_not_valid";
        public const string SmartContractDoesntExist = "smart_contract_not_exist";
        public const string ErrorExecutionContract = "error_execution_contract";
    }
}
