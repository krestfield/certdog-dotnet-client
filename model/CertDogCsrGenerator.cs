using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace certdognet.model
{
    public class CertDogCsrGenerator
    {
        public String id { get; set; }
        public String name { get; set; }
        public String signatureAlgorithm { get; set; }
        public String keySize { get; set; }
        public String ellipticCurveName { get; set; }
        public String hashAlgorithm { get; set; }
    }
}
