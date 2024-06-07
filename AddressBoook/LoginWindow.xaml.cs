using System.Windows;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace AddressBoook
{

    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Password;
            string loginFilePath = "login.xml";

            if (File.Exists(loginFilePath))
            {
                XElement loginXml = XElement.Load(loginFilePath);
                if (loginXml.Elements("user").Any(x => (string)x.Attribute("name") == username && (string)x.Attribute("password") == password))
                {
                    usernameTextBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0x74, 0xC4, 0x86));
                    passwordTextBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0x74, 0xC4, 0x86));

                    await Task.Delay(500);

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();
                    return;
                }
            }
            passwordTextBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0xc4, 0x74, 0x74));
            usernameTextBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0xc4, 0x74, 0x74));
            usernameTextBox.ToolTip = "Невірний логін або пароль";
            passwordTextBox.ToolTip = "Невірний логін або пароль";
            usernameTextBox.GotFocus += (s, e) =>
            {
                passwordTextBox.Background = Brushes.DarkGray;
                usernameTextBox.Background = Brushes.DarkGray;
                passwordTextBox.ToolTip = "Пароль повинен містити від 4 до 15 символів";
                usernameTextBox.ToolTip = "Ім'я має бути не коротше ніж 2 символи";
            };
            passwordTextBox.GotFocus += (s, e) =>
            {
                passwordTextBox.Background = Brushes.DarkGray;
                usernameTextBox.Background = Brushes.DarkGray;
                passwordTextBox.ToolTip = "Пароль повинен містити від 4 до 15 символів";
                usernameTextBox.ToolTip = "Ім'я має бути не коротше ніж 2 символи";
            };
            return;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            RegWindow regWindow = new RegWindow();
            regWindow.Show();
            Close();
        }  
    }
}