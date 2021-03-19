using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace certdognet.model
{
    public class GetCertFromCsrRequest
    {
        public String caName { get; set; }
        public String csr { get; set; }

        public String teamName { get; set; }
    }
}
