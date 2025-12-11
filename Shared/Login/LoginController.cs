namespace Shared.Login
{
    public class LoginController
    {
        private readonly CredentialsM _credentialsManager;
        private readonly LoginView _loginView;
        private readonly bool _isServerMode;
        private readonly Action<string, bool> _onLoginSuccess;

        public LoginController(LoginView loginView, bool isServerMode, Action<string, bool> onLoginSuccess)
        {
            _loginView = loginView;
            _isServerMode = isServerMode;
            _onLoginSuccess = onLoginSuccess;
            _credentialsManager = new CredentialsM();
        }

        public void HandleLogin(string username, string password)
        {
            if (_credentialsManager.ValidateUser(username, password))
            {
                bool isAdmin = _credentialsManager.IsAdmin(username);

                // Verify the user has the correct privileges for this mode
                if (_isServerMode && !isAdmin)
                {
                    MessageBox.Show("Only administrators can access the server.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!_isServerMode && isAdmin)
                {
                    MessageBox.Show("Administrators must use the server application.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Login successful
                _loginView.DialogResult = DialogResult.OK;
                _onLoginSuccess?.Invoke(username, isAdmin);
                _loginView.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void HandleCreateAccount(string username, string password)
        {
            if (_credentialsManager.UserExists(username))
            {
                MessageBox.Show("Username already exists. Please choose a different username.", "Account Creation Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Determine admin status based on mode
            bool isAdmin = _isServerMode;

            if (_credentialsManager.CreateUser(username, password, isAdmin))
            {
                MessageBox.Show($"Account created successfully! You are logged in as {(isAdmin ? "an administrator" : "a regular user")}.", "Account Created", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Auto-login after account creation
                _loginView.DialogResult = DialogResult.OK;
                _onLoginSuccess?.Invoke(username, isAdmin);
                _loginView.Close();
            }
            else
            {
                MessageBox.Show("Failed to create account. Please try again.", "Account Creation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
