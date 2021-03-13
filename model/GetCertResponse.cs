using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace certdognet.model
{
    public class GetCertResponse
    {
        public String certId { get; set; }
        public String p12Data  { get; set; }
    }
}
