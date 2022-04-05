using System;

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
        public String teamName { get; set; }
    }
}
