using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Azure_SSO_POC.Services;

public class AuthService
{
    private IPublicClientApplication _app;
    
    // Values from Azure Portal
    private const string ClientId = "60ea9f59-131b-45ed-85ab-7093555c3c12";
    private const string TenantId = "3dccf637-7afa-4bff-9ddc-7fd51c8a3fda";
    private const string Instance = $"https://login.microsoftonline.com/";
    

    private readonly string[] _scopes = new[] { "User.Read" };

    public AuthService()
    {
        _app = PublicClientApplicationBuilder.Create(ClientId)
            .WithAuthority($"{Instance}{TenantId}")
            .WithRedirectUri("http://localhost") // Must match Azure Portal
            .Build();
    }

    public async Task<AuthenticationResult> LoginAsync()
    {
        AuthenticationResult result;
        var accounts = await _app.GetAccountsAsync();

        try
        {
            // 1. Try to sign in silently (using cached token)
            result = await _app.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
                .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            // 2. If silent fails, open the browser for interactive login
            try 
            {
                
                result = await _app.AcquireTokenInteractive(_scopes)
                    .WithUseEmbeddedWebView(false) // System browser
                    .ExecuteAsync();
            }
            catch (MsalException ex)
            {
                return null;
            }
        }
        
        return result;
    }

    public async Task LogoutAsync()
    {
        var accounts = await _app.GetAccountsAsync();
        if (accounts.Any())
        {
            await _app.RemoveAsync(accounts.First());
        }
    }
}