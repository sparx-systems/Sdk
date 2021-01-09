# Sparx SDK for C#
 
![Sdk](https://github.com/sparx-systems/Sdk/workflows/Sdk/badge.svg)

Class which help you work with API Sparx (v1)

## Requirements

- .NET 5.0 and higher

## How to use?
```
 private const string Domain = "https://test.sparx.com.ua";
 private const string Client = "test";
 private const string Secret = "secret";

 public async Task<string> GetBalance()
 {
     var api = new Api(Domain, Client, Secret);
     var result = await api.MakeRequestAsync("/v1/balance", null, HttpMethod.Get);
 }
```
