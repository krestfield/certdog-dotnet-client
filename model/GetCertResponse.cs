using System;

namespace certdognet.model
{
    public class GetCertResponse
    {
        public String id { get; set; }
        public String pemCert  { get; set; }
        public String p12Data  { get; set; }
    }
}
