using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace certdognet.model
{
    public class RevokeCertRequest
    {
        public String certId { get; set; }
        public String serialNumber { get; set; }
        public String reason { get; set; }
        public String caName { get; set; }
        public String caId { get; set; }
    }
}
