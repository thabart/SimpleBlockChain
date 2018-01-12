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
	
	
    function MedicalPatientContract(address o) {
        owner = o;
    }	
	
	function addMedicalPrestation(string code, string inamiPrescriber) public returns(string) {
		string[] storage nomenCodes;
		notConfirmedPrestations[code]  = Prestation(inamiPrescriber, true, nomenCodes);
		return code;
	}
}