# Certdog Dotnet Client
A .NET client for certdog allowing you to obtain certificates as .cer or PKCS#12/PFX from the certdog application  

An official signed version of the dll can be obtained from [here](https://krestfield.s3.eu-west-2.amazonaws.com/certdog/certdognet.dll)

Usage

1. Build the project
2. From your application, create a reference to certdognet
3. Write a few lines of code to obtain your certificates



More info on certog: https://krestfield.com/certdog

More info on this client: https://krestfield.github.io/docs/certdog/dotnet_client.html

All the documentaion: https://krestfield.github.io/docs/certdog/certdog.html



Remember, certdog can interface to your own Microsoft CAs as well as Primekey EJBCA. Your code will be the same whether its Microsoft, an Internal CA or a PrimeKey EJBCA



## Obtaining a certificate

```c#
// Add the reference to the client
using certdognet;
...
...    

    
// This will return your certificate as a base64 encoded PKCS#12
String p12Base64Data = Certdog.GetCert("https://certdog.net/certdog/api", "Certdog TLS", "CN=mydomain.com", 
		"RSA2048", "somecomplexpassword", "certdogtest", "password");

// Converting to binary and saving means you can import or use wherever you want
byte[] p12BinaryData = Convert.FromBase64String(p12Base64Data);
using (BinaryWriter binWriter = new BinaryWriter(File.Open(pfxFilename, FileMode.Create)))
	binWriter.Write(p12BinaryData);
```

And that's it!  

Some other methods:

* GetCertFromCsr
  * Accepts pre-generated CSR data
* GetCertIssuers
  * Returns a list of the available issuers
* GetCsrGenerators
  * Returns a list of the available CSR generators

The certdog username and password can be omitted for all methods - if you have stored these details in the Windows Credential Store.  See [here](https://krestfield.github.io/docs/certdog/dotnet_client.html) for more details

