using Shared;
using Shared.Interfaces;
using Client.Interfaces;

namespace Client
{
    public partial class ClientView : Form
    {
        private readonly IClientCommsController commsController;
        private readonly IContactListProxy clientProxy;
        private readonly IRoomTypeClassifier roomTypeClassifier;
        private string username;
        private string currentRoom;
        private string pendingRoomCreation;
        private string pendingRoomPassword;
        private bool isDirectMessage = false;
        private string currentContact = "";

        // Constructor with Dependency Injection
        public ClientView(string username, IClientCommsController commsController, IContactListProxy clientProxy, IRoomTypeClassifier roomTypeClassifier)
        {
            this.username = username;
            this.commsController = commsController ?? throw new ArgumentNullException(nameof(commsController));
            this.clientProxy = clientProxy ?? throw new ArgumentNullException(nameof(clientProxy));
            this.roomTypeClassifier = roomTypeClassifier ?? throw new ArgumentNullException(nameof(roomTypeClassifier));

            InitializeComponent();

            // Subscribe to events
            commsController.OnCreateRoomResponse += HandleCreateRoomResponse;
            commsController.OnChatMessage += HandleChatMessage;
            commsController.OnRoomListResponse += HandleRoomList;
            commsController.OnJoinRoomResponse += HandleJoinRoomResponse;
            commsController.OnAddContactResponse += HandleAddContactResponse;
            commsController.OnContactListResponse += HandleContactList;
            commsController.OnError += (msg) => AppendChat(msg);
            commsController.OnDisconnect += () => AppendChat("Disconnected from server");

            clientProxy.OnContactListChanged += RefreshContactList;

            // Connect to server
            commsController.ConnectToServer();
        }

        private void HandleCreateRoomResponse(CreateRoomResponse response)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleCreateRoomResponse(response)));
                return;
            }

            if (response.Success)
            {
                // Auto-join the room if this client created it
                if (response.RoomName == pendingRoomCreation)
                {
                    commsController.SendJoinRoom(response.RoomName, pendingRoomPassword ?? "");
                    pendingRoomCreation = null;
                    pendingRoomPassword = null;
                }
                // Note: Room list will be updated via ROOM_LIST broadcast from server
            }
            else
            {
                AppendChat($"Failed to create room: {response.Message}");
                pendingRoomCreation = null;
                pendingRoomPassword = null;
            }
        }

        private void HandleChatMessage(ChatMessage chatMsg)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleChatMessage(chatMsg)));
                return;
            }

            if (chatMsg.RoomName == currentRoom)
            {
                string timestamp = chatMsg.Timestamp.ToString("HH:mm:ss");
                AppendChat($"[{timestamp}] {chatMsg.Username}: {chatMsg.Text}");
            }
        }

        private void HandleRoomList(RoomListResponse response)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleRoomList(response)));
                return;
            }

            roomsListBox.Items.Clear();
            foreach (var roomName in response.RoomNames)
            {
                roomsListBox.Items.Add(roomName);
            }
        }

        private void HandleJoinRoomResponse(JoinRoomResponse response)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleJoinRoomResponse(response)));
                return;
            }

            if (response.Success)
            {
                currentRoom = response.RoomName;
                room_lbl.Visible = true;
                chatTextBox.Clear();

                if (roomTypeClassifier.IsDirectMessage(currentRoom))
                {
                    isDirectMessage = true;
                    if (!string.IsNullOrEmpty(currentContact))
                    {
                        AppendChat($"Messaging: {currentContact}");
                    }
                }
                else
                {
                    isDirectMessage = false;
                    currentContact = "";
                    room_lbl.Text = $"Room: {currentRoom}";
                    AppendChat($"In room: {currentRoom}");
                }
            }
            else if (response.RequiresPassword)
            {
                if (response.Message == "Incorrect password")
                {
                    MessageBox.Show("Incorrect password. Please try again.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                PromptForRoomPassword(response.RoomName);
            }
            else
            {
                AppendChat($"Failed to join room: {response.Message}");
            }
        }

        private void PromptForRoomPassword(string roomName)
        {
            using (var form = new Form())
            {
                form.Text = "Room Password Required";
                form.Width = 300;
                form.Height = 150;
                form.StartPosition = FormStartPosition.CenterParent;

                var passLabel = new Label { Left = 20, Top = 20, Text = $"Enter password for '{roomName}':" };
                var passBox = new TextBox { Left = 20, Top = 45, Width = 240, UseSystemPasswordChar = true };

                var joinBtn = new Button { Text = "Join", Left = 180, Top = 80, Width = 80 };
                joinBtn.Click += (s, ev) =>
                {
                    commsController.SendJoinRoom(roomName, passBox.Text);
                    form.Close();
                };

                form.Controls.Add(passLabel);
                form.Controls.Add(passBox);
                form.Controls.Add(joinBtn);

                form.ShowDialog();
            }
        }

        private void HandleAddContactResponse(AddContactResponse response)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleAddContactResponse(response)));
                return;
            }

            if (response.Success)
            {
                // Contact list will be updated automatically via server broadcast
                // No need to manually add here
            }
            else
            {
                MessageBox.Show(response.Message, "Add Contact Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void HandleContactList(ContactListResponse response)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleContactList(response)));
                return;
            }

            bool currentContactRemoved = clientProxy.WasContactRemoved(currentContact, response.Contacts) && isDirectMessage;

            clientProxy.UpdateContactsWithStatus(response.ContactsWithStatus);

            if (currentContactRemoved)
            {
                chatTextBox.Clear();
                currentRoom = "";
                currentContact = "";
                isDirectMessage = false;
                room_lbl.Visible = false;
            }
        }

        private void RefreshContactList()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(RefreshContactList));
                return;
            }

            contactsListBox.Items.Clear();
            foreach (var contactInfo in clientProxy.GetContactsWithStatus())
            {
                string displayText = contactInfo.IsOnline
                    ? $"{contactInfo.Username} [Online]"
                    : $"{contactInfo.Username} [Offline]";
                contactsListBox.Items.Add(displayText);
            }
        }

        private void AppendChat(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendChat(text)));
                return;
            }

            chatTextBox.AppendText($"{text}\n");
        }

        void addContactButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(addContactTextBox.Text))
            {
                MessageBox.Show("Please enter a contact username.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            commsController.SendAddContact(addContactTextBox.Text.Trim());
            addContactTextBox.Clear();
        }

        void removeContactButton_Click(object sender, EventArgs e)
        {
            if (contactsListBox.SelectedItem != null)
            {
                string selectedText = contactsListBox.SelectedItem.ToString()!;
                string contactToRemove = ExtractUsername(selectedText);
                commsController.SendRemoveContact(contactToRemove);
                contactsListBox.Items.Remove(selectedText);
            }
        }

        void contactsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (contactsListBox.SelectedItem != null)
            {
                string selectedText = contactsListBox.SelectedItem.ToString()!;
                string selectedContact = ExtractUsername(selectedText);

                string dmRoomName = GetDMRoomName(username, selectedContact);
                commsController.SendJoinRoom(dmRoomName, "");

                currentContact = selectedContact;
                isDirectMessage = true;
                room_lbl.Text = $"Chat with: {selectedContact}";
                room_lbl.Visible = true;
                chatTextBox.Clear();
            }
        }

        private string ExtractUsername(string displayText)
        {
            // Remove " [Online]" or " [Offline]" suffix
            int bracketIndex = displayText.IndexOf(" [");
            return bracketIndex > 0 ? displayText.Substring(0, bracketIndex) : displayText;
        }

        private string GetDMRoomName(string user1, string user2)
        {
            return roomTypeClassifier.CreateDMRoomName(user1, user2);
        }

        void roomsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (roomsListBox.SelectedItem != null)
            {
                string selectedRoom = roomsListBox.SelectedItem.ToString()!;
                commsController.SendJoinRoom(selectedRoom, "");
            }
        }

        void createRoomButton_Click(object sender, EventArgs e)
        {
            using (var form = new Form())
            {
                form.Text = "Create Room";
                form.Width = 300;
                form.Height = 215;
                form.StartPosition = FormStartPosition.CenterParent;

                var nameLabel = new Label { Left = 20, Top = 20, Text = "Room Name:" };
                var nameBox = new TextBox { Left = 20, Top = 45, Width = 240 };

                var passLabel = new Label { Left = 20, Top = 75, Text = "Password (optional):" };
                var passBox = new TextBox { Left = 20, Top = 100, Width = 240, UseSystemPasswordChar = true };

                var createBtn = new Button { Text = "Create", Left = 100, Top = 140, Width = 80 };
                createBtn.Click += (s, ev) =>
                {
                    pendingRoomCreation = nameBox.Text;
                    pendingRoomPassword = passBox.Text;
                    commsController.SendCreateRoom(nameBox.Text, passBox.Text);
                    form.Close();
                };

                form.Controls.Add(nameLabel);
                form.Controls.Add(nameBox);
                form.Controls.Add(passLabel);
                form.Controls.Add(passBox);
                form.Controls.Add(createBtn);

                form.ShowDialog();
            }
        }

        void messageInputTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SendMessage();
                e.Handled = true;
            }
        }

        void sendMessageButton_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(messageInputTextBox.Text) || string.IsNullOrWhiteSpace(currentRoom))
                return;

            commsController.SendChatMessage(currentRoom, messageInputTextBox.Text);
            messageInputTextBox.Clear();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            commsController.Disconnect();
            base.OnFormClosing(e);
        }
    }
}
