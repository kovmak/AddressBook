using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace AddressBoook
{

    public partial class RegWindow : Window
    {
        public RegWindow()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Password;
            string loginFilePath = "login.xml";

            string confirmPassword = confirmPasswordTextBox.Password;
            if (password != confirmPassword)
            {
                passwordTextBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0xc4, 0x74, 0x74));
                confirmPasswordTextBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0xc4, 0x74, 0x74));
                passwordTextBox.ToolTip = "Паролі не співпадають";
                confirmPasswordTextBox.ToolTip = "Паролі не співпадають";
                passwordTextBox.GotFocus += (s, e) =>
                {
                    passwordTextBox.Background = Brushes.DarkGray;
                    confirmPasswordTextBox.Background = Brushes.DarkGray;
                    passwordTextBox.ToolTip = "Пароль повинен містити від 3 до 15 символів";
                    confirmPasswordTextBox.ToolTip = "Введіть пароль ще раз";
                };
                return;
            }

            if (File.Exists(loginFilePath) && XElement.Load(loginFilePath).Elements("user").Any(x => (string)x.Attribute("name") == username))
            {
                usernameTextBox.ToolTip = "Користувач з таким логіном вже існує";
                usernameTextBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0xc4, 0x74, 0x74));
                usernameTextBox.GotFocus += (s, e) =>
                {
                    usernameTextBox.Background = Brushes.DarkGray;
                    usernameTextBox.ToolTip = "Ім'я має бути не коротше ніж 2 символи";
                };
                return;
            }

            XElement userXml = new XElement("user", new XAttribute("name", username), new XAttribute("password", password));
            if (File.Exists(loginFilePath))
            {
                XElement loginXml = XElement.Load(loginFilePath);
                loginXml.Add(userXml);
                loginXml.Save(loginFilePath);
            }
            else
            {
                XDocument loginXml = new XDocument(new XElement("users", userXml));
                loginXml.Save(loginFilePath);
            }
            usernameTextBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0x74, 0xC4, 0x86));
            passwordTextBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0x74, 0xC4, 0x86));
            confirmPasswordTextBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0x74, 0xC4, 0x86));

            await Task.Delay(500);

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RegisterButton_Click(sender, e);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
        private void passwordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {  
            int passwordLength = passwordTextBox.Password.Length;

            if (passwordLength < 3 || passwordLength > 15)
            {
                passwordTextBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0xc4, 0x74, 0x74));
                passwordTextBox.ToolTip = "Пароль повинен містити від 3 до 15 символів";
                passwordTextBox.GotFocus += (s, e) =>
                {
                    passwordTextBox.Background = Brushes.DarkGray;
                    confirmPasswordTextBox.Background = Brushes.DarkGray;
                    passwordTextBox.ToolTip = "Пароль повинен містити від 3 до 15 символів";
                    confirmPasswordTextBox.ToolTip = "Введіть пароль ще раз";
                };
                return;
            }
            else
            {
                passwordTextBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0x74, 0xC4, 0x86));
                passwordTextBox.ToolTip = "Пароль повинен містити від 3 до 15 символів";
            }
        }
        private void usernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int usernameLength = usernameTextBox.Text.Length;

            if (usernameLength < 2 || usernameLength > 15)
            {
                usernameTextBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0xc4, 0x74, 0x74));
                usernameTextBox.ToolTip = "Ім'я має містити від 2 до 15 символів";
                usernameTextBox.GotFocus += (s, e) =>
                {
                    usernameTextBox.Background = Brushes.DarkGray;
                    usernameTextBox.ToolTip = "Ім'я має містити від 2 до 15 символів";
                };
                return;
            }
            else
            {
                usernameTextBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0x74, 0xC4, 0x86));
                usernameTextBox.ToolTip = "Ім'я має містити від 2 до 15 символів";
            }
        }
    }
}
