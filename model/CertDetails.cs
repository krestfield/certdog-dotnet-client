using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace certdognet.model
{
    public class CertDetails
    {
        public String id { get; set; }

        public String caId { get; set; }

        public String localCaId { get; set; }

        public String csrId { get; set; }

        public String pemCert { get; set; }

        public String commonName { get; set; }

        public String subjectDn { get; set; }

        public String issuerDn { get; set; }

        public String issuerCertId { get; set; }

        public String serialNumber { get; set; }

        public String signatureAlgorithm { get; set; }

        public String hashAlgorithm { get; set; }

        public List<String> keyUsages { get; set; }

        public List<String> enhancedKeyUsages { get; set; }

        public List<String> subjectAlternativeNames { get; set; }

        public String validFrom { get; set; }

        public String validTo { get; set; }

        public String validFromStr { get; set; }

        public String validToStr { get; set; }

        public String ownerUserId { get; set; }

        public String ownerUsername { get; set; }

        public String teamId { get; set; }

        public bool renewed { get; set; }

        public String renewedByCertId { get; set; }

        public String renewsCertId { get; set; }

        public bool revoked { get; set; }

        public List<CertHistory> history { get; set; }

        public String extraDetails { get; set; }

        public String[] extraEmails { get; set; }

        public bool trackExpiry { get; set; }

        public bool isCa { get; set; }

        public bool hasKeyData { get; set; }

        public bool imported { get; set; }

        public String importTime { get; set; }

        public List<String> aias { get; set; }

        public List<String> cdps { get; set; }

        public String thumbprint { get; set; }

        public List<String> policies { get; set; }

        public String keySize { get; set; }

        public String eccCurve { get; set; }

        public String msTemplateName { get; set; }

        public bool dnFromCsrWasReversed { get; set; }
    }
}
