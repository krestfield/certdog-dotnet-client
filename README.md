# Certdog Dotnet Client
A .NET client for certdog allowing you to obtain certificates as .cer or PKCS#12/PFX from the certdog application  

An official signed version of the dll can be obtained from [here](https://krestfield.s3.eu-west-2.amazonaws.com/certdog/certdognet.dll)

Usage

1. Build the project (or just download the dll)
2. From your application, create a reference to certdognet.dll
3. Write a few lines of code to obtain your certificates



More info on certdog: https://krestfield.com/certdog

More info on this client: https://krestfield.github.io/docs/certdog/dotnet_client.html

All the documentation: https://krestfield.github.io/docs/certdog/certdog.html



Remember, certdog can interface to your own Microsoft CAs as well as Primekey EJBCA. Your code will be the same whether its Microsoft, an Internal CA or a PrimeKey EJBCA



## Obtaining a certificate

```c#
// Add the reference to the client
using certdognet;
...
...    
// If you want to add Subject Alternative Names, do it like this
// Otherwise you can pass null (or an empty List)    
List<String> sans = new List<String>();
sans.Add("DNS:mydomain.com");
sans.Add("IP:10.0.0.1");
sans.Add("EMAIL:user@mydomain.com");

// This will return the issued certificate data
GetCertResponse resp = Certdog.GetCert("https://certdog.net/certdog/api", "Certdog TLS", "CN=mydomain.com",
    "RSA2048", "somecomplexpassword", sans, "Test Team", "certdogtest", "password");

// Converting the P12 data binary and saving means you can import or use wherever you want
byte[] p12BinaryData = Convert.FromBase64String(resp.p12Data);
using (BinaryWriter binWriter = new BinaryWriter(File.Open(@"C:\temp\certdog.pfx", FileMode.Create)))
	binWriter.Write(p12BinaryData);

// Also save the PEM certificate
File.WriteAllText(@"C:\temp\certdog.cer", resp.pemCert);
```

And that's it!  

Some other methods:

* GetCertFromCsr
  * Accepts a pre-generated CSR data and returns a certificate
* GetCertIssuers
  * Returns a list of the available issuers - including their certificates
* GetCsrGenerators
  * Returns a list of the available CSR generators
* RevokeCert
  * Revokes a previously issued certificate
* GetCertDetails
  * Returns all the details associated with a stored certificate

The certdog username and password can be omitted for all methods - if you have stored these details in the Windows Credential Store.  See [here](https://krestfield.github.io/docs/certdog/dotnet_client.html) for more details

