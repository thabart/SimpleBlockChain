using System.Xml.Serialization;

namespace Kmehr.Core.DTOs
{
    public class KmehrPatient
    {
        public KmehrPatient(string id)
        {
            Id = new KmehrId(Constants.KmehrIdentifiers.IDPATIENT, "1.0", id);
        }

        public KmehrPatient(string id, string firstName) : this(id)
        {
            FirstName = firstName;
        }

        public KmehrPatient(string id, string firstName, string lastName) : this(id, firstName)
        {
            LastName = lastName;
        }

        public KmehrPatient(string id, string firstName, string lastName, KmehrDate birthDate) : this(id, firstName, lastName)
        {
            BirthDate = birthDate;
        }

        public KmehrPatient(string id, string firstName, string lastName, KmehrDate birthDate, KmehrSex sex) : this(id, firstName, lastName, birthDate)
        {
            Sex = sex;
        }

        [XmlElement(Constants.KmehrPatientNames.Id)]
        public KmehrId Id { get; private set; }
        [XmlElement(Constants.KmehrPatientNames.FirstName)]
        public string FirstName { get; set; }
        [XmlElement(Constants.KmehrPatientNames.LastName)]
        public string LastName { get; set; }
        [XmlElement(Constants.KmehrPatientNames.BirthDate)]
        public KmehrDate BirthDate { get; set; }
        [XmlElement(Constants.KmehrPatientNames.Sex)]
        public KmehrSex Sex { get; set; }
    }
}
