using Shared;
using Shared.Interfaces;
using Shared.Login;
using Client.Interfaces;

namespace Client
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            string? loggedInUsername = null;
            bool isAdmin = false;

            // Show login view first
            using (var loginView = new LoginView())
            {
                var loginController = new LoginController(
                    loginView,
                    isServerMode: false,
                    onLoginSuccess: (username, admin) =>
                    {
                        loggedInUsername = username;
                        isAdmin = admin;
                    }
                );

                loginView.SetLoginController(loginController);

                if (loginView.ShowDialog() != DialogResult.OK || loggedInUsername == null)
                {
                    // Login failed or cancelled
                    MessageBox.Show("Login required to access the client.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Login successful, create dependencies and run ClientView
            IServerConnectionProvider connectionProvider = new ServerConnectionProvider();
            IClientCommsController commsController = new ClientCommsController(loggedInUsername, connectionProvider);
            IContactListProxy clientProxy = new ClientProxy();
            IRoomTypeClassifier roomTypeClassifier = new RoomTypeClassifier();

            Application.Run(new ClientView(loggedInUsername, commsController, clientProxy, roomTypeClassifier));
        }
    }
}