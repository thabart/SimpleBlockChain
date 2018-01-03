pragma solidity ^0.4.7;

contract a {

        mapping(address => string) private mailbox;

        event Mailed(address from, string message);
        event Read(address from, string message);

}