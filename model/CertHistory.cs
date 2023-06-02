using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace certdognet.model
{
    public class CertHistory
    {
        public String timestamp { get; set; }
        public String timestampStr { get; set; }
        public String @event { get; set; }
        public String details { get; set; }
    }
}
