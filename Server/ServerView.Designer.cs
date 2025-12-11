namespace Server
{
    partial class ServerView
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
            users_lstbx = new ListBox();
            connected_lbl = new Label();
            offline_users_lstbx = new ListBox();
            offline_lbl = new Label();
            rooms_lbl = new Label();
            rooms_listbx = new ListBox();
            SuspendLayout();
            // 
            // users_lstbx
            // 
            users_lstbx.FormattingEnabled = true;
            users_lstbx.ItemHeight = 15;
            users_lstbx.Location = new Point(12, 40);
            users_lstbx.Name = "users_lstbx";
            users_lstbx.Size = new Size(216, 409);
            users_lstbx.TabIndex = 0;
            // 
            // connected_lbl
            // 
            connected_lbl.AutoSize = true;
            connected_lbl.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            connected_lbl.Location = new Point(12, 15);
            connected_lbl.Name = "connected_lbl";
            connected_lbl.Size = new Size(118, 19);
            connected_lbl.TabIndex = 1;
            connected_lbl.Tag = "";
            connected_lbl.Text = "Online Users (0):";
            // 
            // offline_users_lstbx
            // 
            offline_users_lstbx.FormattingEnabled = true;
            offline_users_lstbx.ItemHeight = 15;
            offline_users_lstbx.Location = new Point(247, 39);
            offline_users_lstbx.Name = "offline_users_lstbx";
            offline_users_lstbx.Size = new Size(216, 409);
            offline_users_lstbx.TabIndex = 2;
            // 
            // offline_lbl
            // 
            offline_lbl.AutoSize = true;
            offline_lbl.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            offline_lbl.Location = new Point(247, 14);
            offline_lbl.Name = "offline_lbl";
            offline_lbl.Size = new Size(120, 19);
            offline_lbl.TabIndex = 3;
            offline_lbl.Text = "Offline Users (0):";
            // 
            // rooms_lbl
            // 
            rooms_lbl.AutoSize = true;
            rooms_lbl.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            rooms_lbl.Location = new Point(481, 15);
            rooms_lbl.Name = "rooms_lbl";
            rooms_lbl.Size = new Size(81, 19);
            rooms_lbl.TabIndex = 5;
            rooms_lbl.Text = "Rooms (0):";
            // 
            // rooms_listbx
            // 
            rooms_listbx.FormattingEnabled = true;
            rooms_listbx.ItemHeight = 15;
            rooms_listbx.Location = new Point(481, 40);
            rooms_listbx.Name = "rooms_listbx";
            rooms_listbx.Size = new Size(216, 409);
            rooms_listbx.TabIndex = 4;
            // 
            // ServerView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(709, 460);
            Controls.Add(rooms_lbl);
            Controls.Add(rooms_listbx);
            Controls.Add(offline_lbl);
            Controls.Add(offline_users_lstbx);
            Controls.Add(connected_lbl);
            Controls.Add(users_lstbx);
            Name = "ServerView";
            Text = "Server";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox users_lstbx;
        private Label connected_lbl;
        private ListBox offline_users_lstbx;
        private Label offline_lbl;
        private Label rooms_lbl;
        private ListBox rooms_listbx;
    }
}
