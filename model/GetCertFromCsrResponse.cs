using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace certdognet.model
{
    public class GetCertFromCsrResponse
    {
        public String id { get; set; }
        public String pemCert { get; set; }
    }
}
