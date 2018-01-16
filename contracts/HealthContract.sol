pragma solidity ^0.4.19;

contract MedicalPrestationContract {	
	event NewNotConfirmedPrestation(string code, string inamiPrescriber);	
	event NewConfirmedPrestation(string code, string inamiPrescriber);
	
	struct Prestation {	
		address adr;
		string inamiPrescriber;
		bool isValue;
	}
	
	address owner;
	mapping(string => Prestation) notConfirmedPrestations;
	
    function MedicalPatientContract(address o) public {
        owner = o;
    }
	
	function addMedicalPrestation(string code, address o, string inamiPrescriber) public returns(bool) {
		if (notConfirmedPrestations[code].isValue == true) { return false; }
		notConfirmedPrestations[code]  = Prestation(o, inamiPrescriber, true);
		NewNotConfirmedPrestation(code, inamiPrescriber);
		return true;
	}
}