using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace certdognet.model
{
    public class GetCertRequest
    {
        public String caName { get; set; }
        public String dn { get; set; }
        public String[] subjectAltNames { get; set; }
        public String csrGeneratorName { get; set; }
        public String p12Password { get; set; }
        public String extraInfo { get; set; }
        public String extraEmails { get; set; }
    }
}
