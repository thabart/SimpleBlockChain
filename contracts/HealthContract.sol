pragma solidity ^0.4.8;

contract MedicalPatientContract {	
    uint8 constant max8 = 2**8 - 1;
    uint16 constant max16 = 2**16 - 1;
    uint32 constant max32 = 2**32 - 1;
    uint64 constant max64 = 2**64 - 1;
    uint128 constant max128 = 2**128 - 1;
    uint256 constant  max256 = 2**256 - 1;
	
	event PrestationNotConfirmed(
		string code,
		string inamiPrescriber
	);
	
	struct Prestation {	
		string inamiPrescriber;
		bool isValue;
		string[] nomenCodes;
	}
	
	address owner;
	string nationalRegistrationNumber;
	mapping(string => Prestation) notConfirmedPrestations;
	mapping(string => Prestation) confirmedPrestations;
	
	
    function MedicalPatientContract(address o) public {
        owner = o;
    }	
	
	function addMedicalPrestation(string code, string inamiPrescriber) public returns(string) {
		string[] storage nomenCodes;
		notConfirmedPrestations[code]  = Prestation(inamiPrescriber, true, nomenCodes);
		return code;
	}
	
	function addNomencode(string code, string nomenCode) public returns(bool) {
		if (notConfirmedPrestations[code].isValue == false) { return false; }
		notConfirmedPrestations[code].nomenCodes.push(nomenCode);
		return true;
	}
	
	function publishNotConfirmedPrestation(string code) public returns(bool) {
		if (notConfirmedPrestations[code].isValue == false) { return false; }
		Prestation storage prestation = notConfirmedPrestations[code];
		PrestationNotConfirmed(code, prestation.inamiPrescriber);
	}
	
	function getNomencodesFromUnconfirmedPrestation(string code) public returns(bytes) {
		if (notConfirmedPrestations[code].isValue == false) { return new bytes(0); }		
		string[] storage nomenCodes = notConfirmedPrestations[code].nomenCodes;		
        uint256 bytesSize = 0;    
        bytes memory elem;
        uint8 len;
        for (uint256 a = 0; a < nomenCodes.length; a++) {
            elem = bytes(nomenCodes[a]);
            len = lengthBytes(elem.length);
            require(len != 255);
            bytesSize += 1 + len + elem.length;
        }
	
        uint256 counter = 0;
        bytes memory result = new bytes(bytesSize);
		for (uint256 x = 0; x < nomenCodes.length; x++) {
			elem = bytes(nomenCodes[x]);
            len = lengthBytes(elem.length);        
            result[counter] = byte(len);
            counter++;
            for (uint y = 0; y < len; y++) {
                result[counter] = byte(uint8(elem.length / (2 ** (8 * (len - 1 - y)))));
                counter++;
            }

            for (uint z = 0; z < elem.length ; z++) {
                result[counter] = elem[z];
                counter++;
            }
		}
		
		return result;
	}
	
	
    function lengthBytes(uint256 length) internal returns (uint8)
    {
        require(length <= max256);
        if (length >=0 && length <= max8) {
            return 1;
        }
        if (length > max8 && length <= max16) {
            return 2;
        }
        if (length >= max16 && length < max32) {
            return 4;
        }
        if (length >= max32 && length < max64) {
            return 8;
        }
        if (length >= max64 && length < max128) {
            return 16;
        }
        return 32;
    }
	
	function confirmPrestation() public returns(bool) {
		return true;
	}
}