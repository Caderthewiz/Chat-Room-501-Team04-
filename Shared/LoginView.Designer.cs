namespace Shared
{
    partial class LoginView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            username_lbl = new Label();
            password_lbl = new Label();
            username_txtbx = new TextBox();
            password_txtbx = new TextBox();
            login_btn = new Button();
            create_account_btn = new Button();
            SuspendLayout();
            // 
            // username_lbl
            // 
            username_lbl.AutoSize = true;
            username_lbl.Location = new Point(6, 32);
            username_lbl.Name = "username_lbl";
            username_lbl.Size = new Size(63, 15);
            username_lbl.TabIndex = 0;
            username_lbl.Text = "Username:";
            // 
            // password_lbl
            // 
            password_lbl.AutoSize = true;
            password_lbl.Location = new Point(171, 32);
            password_lbl.Name = "password_lbl";
            password_lbl.Size = new Size(60, 15);
            password_lbl.TabIndex = 1;
            password_lbl.Text = "Password:";
            // 
            // username_txtbx
            // 
            username_txtbx.Location = new Point(65, 24);
            username_txtbx.Name = "username_txtbx";
            username_txtbx.Size = new Size(100, 23);
            username_txtbx.TabIndex = 3;
            // 
            // password_txtbx
            // 
            password_txtbx.Location = new Point(227, 24);
            password_txtbx.Name = "password_txtbx";
            password_txtbx.Size = new Size(100, 23);
            password_txtbx.TabIndex = 4;
            // 
            // login_btn
            // 
            login_btn.Location = new Point(12, 81);
            login_btn.Name = "login_btn";
            login_btn.Size = new Size(114, 23);
            login_btn.TabIndex = 6;
            login_btn.Text = "Login";
            login_btn.UseVisualStyleBackColor = true;
            login_btn.Click += login_btn_Click;
            // 
            // create_account_btn
            // 
            create_account_btn.Location = new Point(213, 86);
            create_account_btn.Name = "create_account_btn";
            create_account_btn.Size = new Size(114, 23);
            create_account_btn.TabIndex = 7;
            create_account_btn.Text = "Create Account";
            create_account_btn.UseVisualStyleBackColor = true;
            create_account_btn.Click += create_account_btn_Click;
            // 
            // LoginView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(343, 121);
            Controls.Add(create_account_btn);
            Controls.Add(login_btn);
            Controls.Add(password_txtbx);
            Controls.Add(username_txtbx);
            Controls.Add(password_lbl);
            Controls.Add(username_lbl);
            Name = "LoginView";
            Text = "LoginView";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label username_lbl;
        private Label password_lbl;
        private TextBox username_txtbx;
        private TextBox password_txtbx;
        private Button login_btn;
        private Button create_account_btn;
    }
}