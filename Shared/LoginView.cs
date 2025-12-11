using Shared.Login;

namespace Shared
{
    public partial class LoginView : Form
    {
        private LoginController? _loginController;

        public LoginView()
        {
            InitializeComponent();
        }

        public void SetLoginController(LoginController controller)
        {
            _loginController = controller;
        }

        private void create_account_btn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(username_txtbx.Text) && !string.IsNullOrEmpty(password_txtbx.Text))
            {
                _loginController?.HandleCreateAccount(username_txtbx.Text, password_txtbx.Text);
            }
            else
            {
                MessageBox.Show("Please enter both username and password.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void login_btn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(username_txtbx.Text) && !string.IsNullOrEmpty(password_txtbx.Text))
            {
                _loginController?.HandleLogin(username_txtbx.Text, password_txtbx.Text);
            }
            else
            {
                MessageBox.Show("Please enter both username and password.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
