using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace certdognet.model
{
    public class CertDogCertIssuer
    {
        public String id { get; set; }
        public String caName { get; set; }
        public int caType { get; set; }
        public String caTypeName { get; set; }
        public String[] caCerts { get; set; }
        public String caSubjectDn { get; set; }
        public String dnRestrictionId { get; set; }
    }
}
