using System.Windows.Input;

namespace Chefs.Views;

public sealed partial class LoginPage : Page
{
	private ICommand? _loginCommand;

	public LoginPage()
	{
		this.InitializeComponent();

		DataContextChanged += OnDataContextChanged;
	}

	// XamButton has no Command property, so mirror the Login command's CanExecute onto the button
	private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
	{
		if (_loginCommand is not null)
		{
			_loginCommand.CanExecuteChanged -= OnLoginCanExecuteChanged;
		}

		_loginCommand = (args.NewValue as LoginViewModel)?.Login;

		if (_loginCommand is not null)
		{
			_loginCommand.CanExecuteChanged += OnLoginCanExecuteChanged;
		}

		UpdateLoginButton();
	}

	private void OnLoginCanExecuteChanged(object? sender, EventArgs e) => UpdateLoginButton();

	private void UpdateLoginButton() => LoginButton.IsDisabled = !(_loginCommand?.CanExecute(null) ?? false);

	private void OnLoginClicked(object sender, EventArgs e) => Execute(_loginCommand);

	private void OnLoginWithAppleClicked(object sender, EventArgs e) => Execute((DataContext as LoginViewModel)?.LoginWithApple);

	private void OnLoginWithGoogleClicked(object sender, EventArgs e) => Execute((DataContext as LoginViewModel)?.LoginWithGoogle);

	private static void Execute(ICommand? command)
	{
		if (command?.CanExecute(null) ?? false)
		{
			command.Execute(null);
		}
	}
}
