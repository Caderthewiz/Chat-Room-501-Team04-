namespace Client
{
    partial class ClientView
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            contactsLabel = new Label();
            contactsListBox = new ListBox();
            addContactTextBox = new TextBox();
            addContactButton = new Button();
            removeContactButton = new Button();
            roomsLabel = new Label();
            roomsListBox = new ListBox();
            createRoomButton = new Button();
            chatTextBox = new RichTextBox();
            messageInputTextBox = new TextBox();
            sendMessageButton = new Button();
            room_lbl = new Label();
            SuspendLayout();
            // 
            // contactsLabel
            // 
            contactsLabel.AutoSize = true;
            contactsLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            contactsLabel.Location = new Point(12, 9);
            contactsLabel.Name = "contactsLabel";
            contactsLabel.Size = new Size(76, 21);
            contactsLabel.TabIndex = 0;
            contactsLabel.Text = "Contacts";
            // 
            // contactsListBox
            // 
            contactsListBox.FormattingEnabled = true;
            contactsListBox.ItemHeight = 15;
            contactsListBox.Location = new Point(12, 33);
            contactsListBox.Name = "contactsListBox";
            contactsListBox.Size = new Size(180, 199);
            contactsListBox.TabIndex = 1;
            contactsListBox.SelectedIndexChanged += contactsListBox_SelectedIndexChanged;
            // 
            // addContactTextBox
            // 
            addContactTextBox.Location = new Point(12, 238);
            addContactTextBox.Name = "addContactTextBox";
            addContactTextBox.PlaceholderText = "Contact name...";
            addContactTextBox.Size = new Size(120, 23);
            addContactTextBox.TabIndex = 2;
            // 
            // addContactButton
            // 
            addContactButton.Location = new Point(138, 238);
            addContactButton.Name = "addContactButton";
            addContactButton.Size = new Size(54, 23);
            addContactButton.TabIndex = 3;
            addContactButton.Text = "Add";
            addContactButton.UseVisualStyleBackColor = true;
            addContactButton.Click += addContactButton_Click;
            // 
            // removeContactButton
            // 
            removeContactButton.Location = new Point(12, 267);
            removeContactButton.Name = "removeContactButton";
            removeContactButton.Size = new Size(180, 23);
            removeContactButton.TabIndex = 4;
            removeContactButton.Text = "Remove Selected";
            removeContactButton.UseVisualStyleBackColor = true;
            removeContactButton.Click += removeContactButton_Click;
            // 
            // roomsLabel
            // 
            roomsLabel.AutoSize = true;
            roomsLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            roomsLabel.Location = new Point(12, 303);
            roomsLabel.Name = "roomsLabel";
            roomsLabel.Size = new Size(62, 21);
            roomsLabel.TabIndex = 5;
            roomsLabel.Text = "Rooms";
            // 
            // roomsListBox
            // 
            roomsListBox.FormattingEnabled = true;
            roomsListBox.ItemHeight = 15;
            roomsListBox.Location = new Point(12, 327);
            roomsListBox.Name = "roomsListBox";
            roomsListBox.Size = new Size(180, 139);
            roomsListBox.TabIndex = 6;
            roomsListBox.SelectedIndexChanged += roomsListBox_SelectedIndexChanged;
            // 
            // createRoomButton
            // 
            createRoomButton.Location = new Point(12, 472);
            createRoomButton.Name = "createRoomButton";
            createRoomButton.Size = new Size(180, 23);
            createRoomButton.TabIndex = 7;
            createRoomButton.Text = "Create new room";
            createRoomButton.UseVisualStyleBackColor = true;
            createRoomButton.Click += createRoomButton_Click;
            // 
            // chatTextBox
            // 
            chatTextBox.Location = new Point(198, 33);
            chatTextBox.Name = "chatTextBox";
            chatTextBox.ReadOnly = true;
            chatTextBox.Size = new Size(590, 433);
            chatTextBox.TabIndex = 8;
            chatTextBox.Text = "";
            // 
            // messageInputTextBox
            // 
            messageInputTextBox.Location = new Point(198, 472);
            messageInputTextBox.Name = "messageInputTextBox";
            messageInputTextBox.PlaceholderText = "Type your message...";
            messageInputTextBox.Size = new Size(510, 23);
            messageInputTextBox.TabIndex = 9;
            messageInputTextBox.KeyPress += messageInputTextBox_KeyPress;
            // 
            // sendMessageButton
            // 
            sendMessageButton.Location = new Point(714, 471);
            sendMessageButton.Name = "sendMessageButton";
            sendMessageButton.Size = new Size(74, 23);
            sendMessageButton.TabIndex = 10;
            sendMessageButton.Text = "Send";
            sendMessageButton.UseVisualStyleBackColor = true;
            sendMessageButton.Click += sendMessageButton_Click;
            // 
            // room_lbl
            //
            room_lbl.AutoSize = true;
            room_lbl.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            room_lbl.Location = new Point(198, 9);
            room_lbl.Name = "room_lbl";
            room_lbl.Size = new Size(54, 15);
            room_lbl.TabIndex = 11;
            room_lbl.Text = "Room Name";
            room_lbl.Visible = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 500);
            Controls.Add(room_lbl);
            Controls.Add(sendMessageButton);
            Controls.Add(messageInputTextBox);
            Controls.Add(chatTextBox);
            Controls.Add(createRoomButton);
            Controls.Add(roomsListBox);
            Controls.Add(roomsLabel);
            Controls.Add(removeContactButton);
            Controls.Add(addContactButton);
            Controls.Add(addContactTextBox);
            Controls.Add(contactsListBox);
            Controls.Add(contactsLabel);
            Name = "Form1";
            Text = "Chat Client";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label contactsLabel;
        private ListBox contactsListBox;
        private TextBox addContactTextBox;
        private Button addContactButton;
        private Button removeContactButton;
        private Label roomsLabel;
        private ListBox roomsListBox;
        private Button createRoomButton;
        private RichTextBox chatTextBox;
        private TextBox messageInputTextBox;
        private Button sendMessageButton;
        private Label room_lbl;
    }
}
