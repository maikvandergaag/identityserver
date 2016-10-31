# IdentityServer
Repository for my Identity Server Test application that can be hosted wihtin Azure.

##Information Identity server
[https://identityserver.io](https://identityserver.io)

##Using a certificate wihtin Azure
With the following method you can retrieve a certificate that you can use for signing from a kind of certificate store wihtin Azure.

```csharp
public X509Certificate2 GetCertificate(string thumbprint) {
    X509Certificate2 retVal = null;

    X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
    certStore.Open(OpenFlags.ReadOnly);

    X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

    if (certCollection.Count > 0) {
        retVal = certCollection[0];
    }
    certStore.Close();

    return retVal;
}
```

More information about this repository can be read on this blog site:
[https://msftplayground.com/2016/10/using-certificat…ure-app-services/](https://msftplayground.com/2016/10/using-certificat…ure-app-services/)


Before using the application also make sure you update the Client because the test uses in memory clients.
