using System;
using System.Net;
using RestSharp;
using certdognet.model;
using CredentialManagement;
using System.Collections.Generic;
using System.Text.Json;

namespace certdognet
{
    public class Certdog
    {
        public static String CERTDOGCREDS = "CERTDOGCREDS";
        
        public static String REVOKE_REASON_UNSPECIFIED = "unspecified";
        public static String REVOKE_REASON_KEY_COMPROMISE = "key compromise";
        public static String REVOKE_REASON_CA_COMPROMISE = "ca compromise";
        public static String REVOKE_REASON_AFFILIATION_CHANGED = "affiliation changed";
        public static String REVOKE_REASON_SUPERSEDED = "superseded";
        public static String REVOKE_REASON_CESSATION_OF_OPERATION = "cessation of operation";
        public static String REVOKE_REASON_CERTIFICATE_HOLD = "hold";

        #region Public Static Calls

        /// <summary>
        /// Given a certificate DN can generate a CSR and return a PKCS#12
        /// This is a one hit static method which performs logon, cert request and logoff
        /// </summary>
        /// <param name="url">The certdog API URL</param>
        /// <param name="certIssuer">The name of the certificate issuer</param>
        /// <param name="dn">The requested Distinguished Name</param>
        /// <param name="generator">The name of the CSR Generator</param>
        /// <param name="p12Password">The password to protect the returned PKCS#12</param>
        /// <param name="sans">Subject Alternative Names. E.g. ["DNS:domain.com","IP:10.11.1.2"]</param>
        /// <param name="teamName">The name of the team to associate this certificate with</param>
        /// <param name="username">The certdog API Username</param>
        /// <param name="password">The certdog API Password</param>
        /// <returns>Base64 encoded PKCS#12</returns>
        public static GetCertResponse GetCert(String url, String certIssuer, String dn, String generator, 
            String p12Password, List<String> sans, String teamName, String username, String password)
        {
            RestClient client = GetClient(url);
            String jwt = Login(client, username, password);

            if (sans == null)
                sans = new List<String>();

            var request = new RestRequest("/certs/request", Method.Post);
            request.AddHeader("Authorization", "Bearer " + jwt);
            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(
                new GetCertRequest { caName = certIssuer, dn = dn, csrGeneratorName = generator,
                    subjectAltNames = sans.ToArray(), p12Password = p12Password, teamName = teamName });

            var response = client.Execute<GetCertResponse>(request);
            CheckError(response, "Get Certificate");

            Logout(client, jwt);

            return response.Data;
        }

        /// <summary>
        /// Given a certificate DN can generate a CSR and return a PKCS#12
        /// Using the credentials stored in the windows credential manager
        /// </summary>
        /// <param name="url">The certdog API URL</param>
        /// <param name="certIssuer">The name of the certificate issuer</param>
        /// <param name="dn">The requested Distinguished Name</param>
        /// <param name="generator">The name of the CSR Generator</param>
        /// <param name="p12Password">The password to protect the returned PKCS#12</param>
        /// <param name="sans">Subject Alternative Names. E.g. ["DNS:domain.com","IP:10.11.1.2"]</param>
        /// <param name="teamName">The name of the team to associate this certificate with</param>
        /// <returns>Base64 encoded PKCS#12</returns>
        public static GetCertResponse GetCert(String url, String certIssuer, String dn, String generator, 
            String p12Password, List<String> sans, String teamName)
        {
            var credManager = new Credential { Target = CERTDOGCREDS };
            credManager.Load();
            if (credManager == null || credManager.Username == null || credManager.Password == null)
                throw new Exception("Unable to obtain credentials from the credential store. Ensure that a Generic credential has been saved called " + CERTDOGCREDS + " set by the same user running this application");

            return GetCert(url, certIssuer, dn, generator, p12Password, sans, teamName, credManager.Username, credManager.Password);
        }

        /// <summary>
        /// Issues a certificate from a CSR
        /// This is a one hit static method which performs logon, cert request and logoff
        /// </summary>
        /// <param name="url">The certdog API URL</param>
        /// <param name="certIssuer">The name of the certificate issuer</param>
        /// <param name="csrData">The csr data in PEM format</param>
        /// <param name="teamName">The name of the team to associate this certificate with</param>
        /// <param name="username">The certdog API Username</param>
        /// <param name="password">The certdog API Password</param>
        /// <returns>The issued certificate in PEM format</returns>
        public static GetCertResponse GetCertFromCsr(String url, String certIssuer, String csrData, String teamName, String username, String password)
        {
            RestClient client = GetClient(url);
            String jwt = Login(client, username, password);

            var request = new RestRequest("/certs/requestp10", Method.Post);
            request.AddHeader("Authorization", "Bearer " + jwt);
            request.AddJsonBody(new GetCertRequest { caName = certIssuer, csr = csrData, teamName = teamName });

            var response = client.Execute<GetCertResponse>(request);
            CheckError(response, "Get Certificate");

            Logout(client, jwt);

            return response.Data;
        }

        /// <summary>
        /// Issues a certificate from a CSR using the credentials stored in the windows credential manager
        /// </summary>
        /// <param name="url">The certdog API URL</param>
        /// <param name="certIssuer">The name of the certificate issuer</param>
        /// <param name="csrData">The csr data in PEM format</param>
        /// <param name="teamName">The name of the team to associate this certificate with</param>
        /// <returns>The issued certificate in PEM format</returns>
        public static GetCertResponse GetCertFromCsr(String url, String certIssuer, String csrData, String teamName)
        {
            var credManager = new Credential { Target = CERTDOGCREDS };
            credManager.Load();
            if (credManager == null || credManager.Username == null || credManager.Password == null)
                throw new Exception("Unable to obtain credentials from the credential store. Ensure that a Generic credential has been saved called " + CERTDOGCREDS + " set by the same user running this application");

            return GetCertFromCsr(url, certIssuer, csrData, teamName, credManager.Username, credManager.Password);
        }

        /// <summary>
        /// Revoke a certificate
        /// Provide either the certIssuer name or the certIssuer ID (for certIssuer) together with the certificate's serialNumber OR
        /// provide the certificate ID as returned from the GetCert calls
        /// </summary>
        /// <param name="url">The certdog API URL</param>
        /// <param name="certIssuer">Either the certificate issuer name or its ID. If you provide this you must also provide the certificate serialNumber</param>
        /// <param name="serialNumber">The certificate serial number to revoke. If this is provided certIssuer must also be provided</param>
        /// <param name="certId">The ID of the certificate to revoke. certIssuer and serialNumber are not required when this is provided</param>
        /// <param name="reason">The revocation reason, use the constants (e.g. Certdog.REVOKE_REASON_UNSPECIFIED)</param>
        /// <exception cref="Exception"></exception>
        public static void RevokeCert(String url, String certIssuer, String serialNumber, String certId, String reason)
        {
            var credManager = new Credential { Target = CERTDOGCREDS };
            credManager.Load();
            if (credManager == null || credManager.Username == null || credManager.Password == null)
                throw new Exception("Unable to obtain credentials from the credential store. Ensure that a Generic credential has been saved called " + CERTDOGCREDS + " set by the same user running this application");

            RevokeCert(url, certIssuer, serialNumber, certId, reason, credManager.Username, credManager.Password);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">The certdog API URL</param>
        /// <param name="certIssuer">Either the certificate issuer name or its ID. If you provide this you must also provide the certificate serialNumber</param>
        /// <param name="serialNumber">The certificate serial number to revoke. If this is provided certIssuer must also be provided</param>
        /// <param name="certId">The ID of the certificate to revoke. certIssuer and serialNumber are not required when this is provided</param>
        /// <param name="reason">The revocation reason, use the constants (e.g. Certdog.REVOKE_REASON_UNSPECIFIED)</param>
        /// <param name="username">The certdog API Username</param>
        /// <param name="password">The certdog API Password</param>
        public static void RevokeCert(String url, String certIssuer, String serialNumber, String certId, String reason, String username, String password)
        {
            RestClient client = GetClient(url);
            String jwt = Login(client, username, password);

            var request = new RestRequest("/certs/revoke", Method.Post);
            request.AddHeader("Authorization", "Bearer " + jwt);
            request.AddJsonBody(new RevokeCertRequest{ caName = certIssuer, serialNumber = serialNumber, certId = certId, reason = reason });

            var response = client.Execute<GetCertResponse>(request);
            CheckError(response, "Revoke Certificate");

            Logout(client, jwt);
        }

        /// <summary>
        /// Returns details of the stored certificate
        /// </summary>
        /// <param name="url">The certdog API URL</param>
        /// <param name="certId">The ID of the certificate to get the details of</param>
        /// <returns>CertDetails</returns>
        /// <exception cref="Exception"></exception>
        public static CertDetails GetCertDetails(String url, String certId)
        {
            var credManager = new Credential { Target = CERTDOGCREDS };
            credManager.Load();
            if (credManager == null || credManager.Username == null || credManager.Password == null)
                throw new Exception("Unable to obtain credentials from the credential store. Ensure that a Generic credential has been saved called " + CERTDOGCREDS + " set by the same user running this application");

            return GetCertDetails(url, certId, credManager.Username, credManager.Password);
        }

        /// <summary>
        /// Returns details of the stored certificate
        /// </summary>
        /// <param name="url">The certdog API URL</param>
        /// <param name="certId">The ID of the certificate to get the details of</param>
        /// <param name="username">The certdog API Username</param>
        /// <param name="password">The certdog API Password</param>
        /// <returns></returns>
        public static CertDetails GetCertDetails(String url, String certId, String username, String password)
        {
            RestClient client = GetClient(url);
            String jwt = Login(client, username, password);

            var request = new RestRequest("/certs/" + certId, Method.Get);
            request.AddHeader("Authorization", "Bearer " + jwt);

            var response = client.Execute<CertDetails>(request);
            CheckError(response, "Get Certificate Details");

            Logout(client, jwt);

            return response.Data;
        }

        /// <summary>
        /// Returns the available certificate issuers
        /// </summary>
        /// <param name="url">The certdog API URL</param>
        /// <param name="username">The certdog API Username</param>
        /// <param name="password">The certdog API Password</param>
        /// <returns>A list of CertDogCertIssuer</returns>
        public static List<CertDogCertIssuer> GetCertIssuers(String url, String username, String password)
        {
            RestClient client = GetClient(url);
            String jwt = Login(client, username, password);

            var request = new RestRequest("/admin/ca", Method.Get);
            request.AddHeader("Authorization", "Bearer " + jwt);

            var response = client.Execute(request);

            CheckError(response, "Getting Issuers");

            var issuers = JsonSerializer.Deserialize<List<CertDogCertIssuer>>(response.Content);

            Logout(client, jwt);

            return issuers;
        }

        /// <summary>
        /// Returns the available certificate issuers using the credentials stored in the windows credential manager
        /// </summary>
        /// <param name="url">The certdog API URL</param>
        /// <returns>A list of CertDogCertIssuer</returns>
        public static List<CertDogCertIssuer> GetCertIssuers(String url)
        {
            var credManager = new Credential { Target = CERTDOGCREDS };
            credManager.Load();
            if (credManager == null || credManager.Username == null || credManager.Password == null)
                throw new Exception("Unable to obtain credentials from the credential store. Ensure that a Generic credential has been saved called " + CERTDOGCREDS + " set by the same user running this application");

            return GetCertIssuers(url, credManager.Username, credManager.Password);
        }

        /// <summary>
        /// Returns the available CSR Generators
        /// </summary>
        /// <param name="url">The certdog API URL</param>
        /// <param name="username">The certdog API Username</param>
        /// <param name="password">The certdog API Password</param>
        /// <returns>A list of CertDogCsrGenerator</returns>
        public static List<CertDogCsrGenerator> GetCsrGenerators(String url, String username, String password)
        {
            RestClient client = GetClient(url);
            String jwt = Login(client, username, password);

            var request = new RestRequest("/admin/generators", Method.Get);
            request.AddHeader("Authorization", "Bearer " + jwt);

            var response = client.Execute(request);

            var generators = JsonSerializer.Deserialize<List<CertDogCsrGenerator>>(response.Content);

            CheckError(response, "Getting CSR Generators");

            Logout(client, jwt);

            return generators;
        }

        /// <summary>
        /// Returns the available CSR Generators using the credentials stored in the windows credential manager
        /// </summary>
        /// <param name="url">The certdog API URL</param>
        /// <returns>A list of CertDogCsrGenerator</returns>
        public static List<CertDogCsrGenerator> GetCsrGenerators(String url)
        {
            var credManager = new Credential { Target = CERTDOGCREDS };
            credManager.Load();
            if (credManager == null || credManager.Username == null || credManager.Password == null)
                throw new Exception("Unable to obtain credentials from the credential store. Ensure that a Generic credential has been saved called " + CERTDOGCREDS + " set by the same user running this application");

            return GetCsrGenerators(url, credManager.Username, credManager.Password);
        }
        #endregion

        #region Private Static Calls
        /// <summary>
        /// If the error code is anything other than 200 OK, throws an exception
        /// Attempts to obtain the error message
        /// </summary>
        /// <param name="response"></param>
        /// <param name="requestName"></param>
        private static void CheckError(RestResponse response, String requestName)
        {
            if (response == null)
                throw new Exception(requestName + " failed. No response was returned from the server");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                String errorMessage = requestName + " failed with error code ";
                try
                {
                    ErrorResponse err = JsonSerializer.Deserialize<ErrorResponse>(response.Content);
                    errorMessage += err.status + ". Details: " + err.message;
                }
                catch (Exception)
                {
                    errorMessage += response.StatusCode + (response.Content != null ? ". Details: " + response.Content : "");
                }

                throw new Exception(errorMessage);
            }
        }

        /// <summary>
        /// Gets the Rest Sharp client
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static RestClient GetClient(String url)
        {

            var options = new RestClientOptions(url);
            options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            var client = new RestClient(options);

            return client;
        }

        /// <summary>
        /// Logs the user in and returns the auth (JWT) token
        /// </summary>
        /// <param name="client"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static String Login(RestClient client, String username, String password)
        {
            var request = new RestRequest("/login", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new LoginRequest { username = username, password = password });

            var response = client.Execute<LoginResponse>(request);
            CheckError(response, "Logging In");


            String tok = response.Data.token;

            return tok;
        }

        /// <summary>
        /// Logs out the user
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        private static void Logout(RestClient client, String token)
        {
            var request = new RestRequest("/logouthere", Method.Get);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);

            client.Execute(request);
        }
        #endregion        
    }
}
