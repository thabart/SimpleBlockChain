namespace Kmehr.Core.Common
{
    public class IdentifierType
    {
        public static IdentifierType SSIN = new IdentifierType("SSIN", "INSS", "SSIN", 11);
        public static IdentifierType NIHII11 = new IdentifierType("NIHI11", 11);

        private string _typeEtk;
        private string _typeEhbox;
        private string _typeRecipe;
        private int _length;

        private IdentifierType(string type, int length)
        {
            _typeEtk = type;
            _typeEhbox = type;
            _typeRecipe = type;
            _length = length;
        }

        private IdentifierType(string typeEtk, string typeEhboxv2, string typeRecipe, int length)
        {
            _typeEtk = typeEtk;
            _typeEhbox = typeEhboxv2;
            _typeRecipe = typeRecipe;
            _length = length;
        }
    }
}
