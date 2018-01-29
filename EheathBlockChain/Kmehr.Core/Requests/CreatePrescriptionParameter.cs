using System.Collections.Generic;

namespace Kmehr.Core.Requests
{
    public class CreatePrescriptionParameter
    {
        /// <summary>
        /// Type of prescription.
        /// Available prescription types : P0, P1 , P2
        /// </summary>
        public string PrescriptionType { get; set; }
        /// <summary>
        /// Should be set to true.
        /// If the prescriber don't want a feedback from the executor, this flag should be set to false.
        /// </summary>
        public bool FeedbackRequested { get; set; }
        public string KeyId { get; set; }
        public IEnumerable<byte> SymmKey { get; set; }
        public string PrescriberLabel { get; set; }
    }
}
