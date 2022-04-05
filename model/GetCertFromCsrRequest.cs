using System;

namespace certdognet.model
{
    public class GetCertFromCsrRequest
    {
        public String caName { get; set; }
        public String csr { get; set; }

        public String teamName { get; set; }
    }
}
