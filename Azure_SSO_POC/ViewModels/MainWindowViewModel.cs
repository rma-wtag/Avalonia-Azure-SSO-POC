using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;
using Azure_SSO_POC.Services;

namespace Azure_SSO_POC.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private string _authenticationStatus = "";
    public string AuthenticationStatus
    {
        get => _authenticationStatus;
        set => this.RaiseAndSetIfChanged(ref _authenticationStatus, value);
    }

    private bool _loggedIn;
    public bool LoggedIn
    {
        get => _loggedIn;
        set => this.RaiseAndSetIfChanged(ref _loggedIn, value);
    }

    public string Greeting => "Welcome to Avalonia!";

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

    public MainWindowViewModel()
    {
        LoginCommand = ReactiveCommand.CreateFromTask(DoLoginAsync);
        LogoutCommand = ReactiveCommand.CreateFromTask(DoLogoutAsync);
    }

    public async Task DoLoginAsync()  // ← Changed to public
    {
        var authService = new AuthService();
        var authResult = await authService.LoginAsync();

        if (authResult != null)
        {
            var username = authResult.Account.Username;
            // var token = authResult.AccessToken;  // Store if needed

            LoggedIn = true;
            AuthenticationStatus = "You have successfully logged in";

            System.Diagnostics.Debug.WriteLine($"Logged in as: {username}.");
        }
    }

    public async Task DoLogoutAsync()  // ← Changed to public
    {
        var authService = new AuthService();
        await authService.LogoutAsync();

        LoggedIn = false;
        AuthenticationStatus = "";

        System.Diagnostics.Debug.WriteLine("Logged out!");
    }
}