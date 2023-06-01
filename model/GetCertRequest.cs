using System;

namespace certdognet.model
{
    public class GetCertRequest
    {
        public String id { get; set; }
        
        public String caName { get; set; }
        
        public String issuerName { get; set; }
        public String issuerId { get; set; }

        public String dn { get; set; }

        public String teamName { get; set; }
        
        public String teamId { get; set; }

        public String csr { get; set; }

        public String[] subjectAltNames { get; set; }

        public String csrGeneratorName { get; set; }
        // They can provide either name or ID
        public String csrGeneratorId { get; set; }

        public String p12Password { get; set; }

        public String extraInfo { get; set; }

        public String[] extraEmails { get; set; }

        public bool reverseCsrDn { get; set; }
    }
}
