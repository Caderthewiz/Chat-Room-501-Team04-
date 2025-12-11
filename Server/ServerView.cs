using Shared;
using Server.Interfaces;

namespace Server
{
    public partial class ServerView : Form
    {
        private readonly IServerController serverController;
        private readonly IServerCommsController serverCommsController;
        private string adminUsername;

        // Constructor with Dependency Injection
        public ServerView(string username, IServerController serverController, IServerCommsController serverCommsController)
        {
            this.adminUsername = username;
            this.serverController = serverController ?? throw new ArgumentNullException(nameof(serverController));
            this.serverCommsController = serverCommsController ?? throw new ArgumentNullException(nameof(serverCommsController));

            InitializeComponent();

            // Subscribe to controller events
            serverController.OnLogUpdate += AppendLog;
            serverController.OnRoomListChanged += RefreshRoomDisplay;
            serverController.OnUserListChanged += RefreshUserCounts;

            // Start server
            serverCommsController.StartServer();
            RefreshUserCounts();
        }

        public void AppendLog(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendLog(text)));
                return;
            }

            Console.WriteLine($"[SERVER] {text}");
        }

        private void RefreshUserCounts()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(RefreshUserCounts));
                return;
            }

            var onlineUsers = serverController.GetOnlineUsers();
            var offlineUsers = serverController.GetOfflineUsers();

            users_lstbx.Items.Clear();
            foreach (var user in onlineUsers)
            {
                users_lstbx.Items.Add(user);
            }
            connected_lbl.Text = $"Online Users ({onlineUsers.Count}):";

            offline_users_lstbx.Items.Clear();
            foreach (var user in offlineUsers)
            {
                offline_users_lstbx.Items.Add(user);
            }
            offline_lbl.Text = $"Offline Users ({offlineUsers.Count}):";
        }

        private void RefreshRoomDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(RefreshRoomDisplay));
                return;
            }

            var rooms = serverController.GetAllRooms();
            rooms_listbx.Items.Clear();

            foreach (var room in rooms)
            {
                rooms_listbx.Items.Add(room.Name);

                if (!string.IsNullOrEmpty(room.Password))
                {
                    rooms_listbx.Items.Add($"  Password: {room.Password}");
                }

                foreach (var member in room.Clients)
                {
                    rooms_listbx.Items.Add($"  {member}");
                }

                if (room != rooms.Last())
                {
                    rooms_listbx.Items.Add("");
                }
            }

            rooms_lbl.Text = $"Rooms ({rooms.Count}):";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            serverCommsController.StopServer();
            base.OnFormClosing(e);
        }
    }
}
