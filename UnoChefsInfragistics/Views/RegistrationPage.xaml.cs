using System.Windows.Input;

namespace Chefs.Views;

public sealed partial class RegistrationPage : Page
{
	private ICommand? _registerCommand;

	public RegistrationPage()
	{
		this.InitializeComponent();

		DataContextChanged += OnDataContextChanged;
	}

	// XamButton has no Command property. Subscribing to CanExecuteChanged (as a Button would) also lets the
	// MVUX command start reading UserCredentials, which it needs before CanExecute can return true.
	private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
	{
		if (_registerCommand is not null)
		{
			_registerCommand.CanExecuteChanged -= OnRegisterCanExecuteChanged;
		}

		_registerCommand = (args.NewValue as RegistrationViewModel)?.Register;

		if (_registerCommand is not null)
		{
			_registerCommand.CanExecuteChanged += OnRegisterCanExecuteChanged;
		}

		UpdateSignUpButton();
	}

	private void OnRegisterCanExecuteChanged(object? sender, EventArgs e) => UpdateSignUpButton();

	private void UpdateSignUpButton() => SignUpButton.IsDisabled = !(_registerCommand?.CanExecute(null) ?? false);

	private void OnSignUpClicked(object sender, EventArgs e)
	{
		if (_registerCommand?.CanExecute(null) ?? false)
		{
			_registerCommand.Execute(null);
		}
	}
}
