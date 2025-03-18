using Auth0.ManagementApi;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;

interface IAuth0Management
{
  Task<ManagementApiClient> getClient();
}

public class Auth0Management: IAuth0Management
{
  private string _domain = "";
  private string _clientId = "";
  private string _clientSecret = "";
  private ManagementApiClient managementClient = null;

  public Auth0Management(string domain, string clientId, string clientSecret)
  {
    _domain = domain;
    _clientId = clientId;
    _clientSecret = clientSecret;
  }

  public async Task<ManagementApiClient> getClient()
  {
    if (managementClient is null)
    {
      var authClient = new AuthenticationApiClient(_domain);
      var token = await authClient.GetTokenAsync(new ClientCredentialsTokenRequest
      {
        Audience = $"https://{_domain}/api/v2/",
        ClientId = _clientId,
        ClientSecret = _clientSecret
      });
      managementClient = new ManagementApiClient(token.AccessToken, _domain);   
    }
    return managementClient;
  }
}