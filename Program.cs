using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

// Configure user secrets
var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>();

IConfiguration configuration = builder.Build();

// Retrieve secrets
string? tenantId = configuration["EntraId:TenantId"];
string? clientId = configuration["EntraId:ClientId"];
string? clientSecret = configuration["EntraId:ClientSecret"];
string? frontDoorEndpoint = configuration["FrontDoorEndpoint"];
string? apiScope = configuration["ApiScope"];

Console.WriteLine("1. Secretes retrieved ...\n");

// MSAL Setup
IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
    .Create(clientId)
    .WithClientSecret(clientSecret)
    .Build();

var result = await app.AcquireTokenForClient(scopes: [apiScope])
    .WithTenantId(tenantId)
    .ExecuteAsync();

Console.WriteLine("2. Token acquired ...\n");

// use token in HTTP client
Console.WriteLine("3. Response: \n");
var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

// make a request to APIM routed via Front Door instance / or just APIM directly
var response = await client.GetAsync($"{frontDoorEndpoint}/petstore/pet/4");
var responseContent = await response.Content.ReadAsStringAsync();

// output response headers and content
Console.WriteLine(response);
Console.WriteLine(responseContent);