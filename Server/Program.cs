using Shared;
using Shared.Interfaces;
using Shared.Login;
using Server.Interfaces;

namespace Server
{
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
                    isServerMode: true,
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
                    MessageBox.Show("Login required to access the server.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Login successful, create dependencies and run ServerView
            IUserRepository userRepository = new Users();
            IContactRepository contactRepository = new ContactsManager();
            ICredentialRepository credentialRepository = new CredentialsM();
            IChatHistoryRepository chatHistoryRepository = new ChatHistoryRepository();
            IRoomTypeClassifier roomTypeClassifier = new RoomTypeClassifier();

            IServerController serverController = new ServerController(
                userRepository,
                contactRepository,
                credentialRepository,
                chatHistoryRepository,
                roomTypeClassifier
            );

            IServerCommsController serverCommsController = new ServerCommsController(
                serverController,
                (msg) => Console.WriteLine($"[SERVER] {msg}") // Log callback
            );

            Application.Run(new ServerView(loggedInUsername, serverController, serverCommsController));
        }
    }
}