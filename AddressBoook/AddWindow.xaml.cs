using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Threading.Tasks;

namespace AddressBoook
{
    public partial class AddWindow : Window, INotifyPropertyChanged
    {
        public AddWindow()
        {
            InitializeComponent();
            DataContext = this;

            Controller.EmptyFieldFio += HighlightFioFild;
            Controller.EmptyFieldTelephoneNumber += HighlightTelephoneNumberFild;
            Controller.EmptyFieldHome += HighlightHomeFild;
            Controller.EmptyFieldGmail += HighlightGmailFild;
        }

        private string _fio;
        public string Fio
        {
            get { return _fio; }
            set
            {
                _fio = value;
                OnPropertyChanged(nameof(Fio));

                if (value != null && value != "")
                {
                    DefaultFioFild();
                }
            }
        }

        private string _telephoneNumber;
        public string TelephoneNumber
        {
            get { return _telephoneNumber; }
            set
            {
                _telephoneNumber = value;
                OnPropertyChanged(nameof(TelephoneNumber));

                if (value != null && value != "")
                {
                    DefaultTelephoneNumberFild();
                }
            }
        }

        private string _home;
        public string Home
        {
            get { return _home; }
            set
            {
                _home = value;
                OnPropertyChanged(nameof(Home));

                if (value != null && value != "")
                {
                    DefaultHomeFild();
                }
            }
        }

        private string _gmail;
        public string Gmail
        {
            get { return _gmail; }
            set
            {
                _gmail = value;
                OnPropertyChanged(nameof(Gmail));

                if (value != null && value != "")
                {
                    DefaultGmailFild();
                }
            }
        }

        private Brush _fioBrush;
        public Brush FioBrush
        {
            get { return _fioBrush; }
            set { _fioBrush = value; OnPropertyChanged(nameof(FioBrush)); }
        }

        private Brush _telephoneBrush;
        public Brush TelephoneBrush
        {
            get { return _telephoneBrush; }
            set { _telephoneBrush = value; OnPropertyChanged(nameof(TelephoneBrush)); }
        }

        private Brush _homeBrush;
        public Brush HomeBrush
        {
            get { return _homeBrush; }
            set { _homeBrush = value; OnPropertyChanged(nameof(HomeBrush)); }
        }

        private Brush _gmailBrush;
        public Brush GmailBrush
        {
            get { return _gmailBrush; }
            set { _gmailBrush = value; OnPropertyChanged(nameof(GmailBrush)); }
        }

        #region AdditionalMethods

        private void HighlightFioFild()
        {
            FioBrush = Controller.Painter(false);
        }

        private void HighlightTelephoneNumberFild()
        {
            TelephoneBrush = Controller.Painter(false);
        }

        private void HighlightHomeFild()
        {
            HomeBrush = Controller.Painter(false);
        }

        private void HighlightGmailFild()
        {
            GmailBrush = Controller.Painter(false);
        }

        private void DefaultFioFild()
        {
            FioBrush = Controller.Painter(true);
        }
        private void DefaultTelephoneNumberFild()
        {
            TelephoneBrush = Controller.Painter(true);
        }

        private void DefaultHomeFild()
        {
            HomeBrush = Controller.Painter(true);
        }
        private void DefaultGmailFild()
        {
            GmailBrush = Controller.Painter(true);
        }

        #endregion AdditionalMethods

        #region Commands

        public ICommand AddConfirm
        {
            get
            {
                Controller.SearchReset();
                return new DelegateCommand((obj) => { Controller.AddContactToBase(Controller.CreateAddress(Fio, TelephoneNumber, Home, Gmail, true), this); });
            }
        }

        public ICommand Cancel
        {
            get { return new DelegateCommand((obj) => { Controller.CloseWindow(this); }); }
        }

        #endregion Commands

        #region PropertyChangedEventHandler

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion PropertyChangedEventHandler

        private void Telephone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[0-9+]+$"))
            {
                e.Handled = true;
            }
        }

        private void Telephone_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string phoneNumber = textBox.Text.Trim();

            if (phoneNumber.Length > 0)
            {
                bool isValid = Regex.IsMatch(phoneNumber, @"^\+380\d{9}$");
                if (!isValid)
                {
                    textBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0xc4, 0x74, 0x74));
                    textBox.ToolTip = "Невірна структура номера";
                    textBox.GotFocus += (s, e) =>
                    {
                        textBox.Background = Brushes.DarkGray;
                        textBox.ToolTip = "Номер має виглядати так +380*********";
                    };
                }
                else
                {
                    textBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0x74, 0xC4, 0x86));
                }
            }
        }

        private void Home_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[0-9a-zA-Z]+$"))
            {
                e.Handled = true;
            }
        }

        private void Home_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string homeAddress = textBox.Text.Trim();

            if (homeAddress.Length > 0)
            {
                bool isValid = Regex.IsMatch(homeAddress, @"^\d.*$");
                if (!isValid)
                {
                    textBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0xc4, 0x74, 0x74));
                    textBox.ToolTip = "Неправильний формат домашньої адреси";
                    textBox.GotFocus += (s, e) =>
                    {
                        textBox.Background = Brushes.DarkGray;
                        textBox.ToolTip = "Адреса має виглядати так 'Номер будинку' + 'Назва вулиці' ";
                    };
                }
                else
                {
                    textBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0x74, 0xC4, 0x86));
                }
            }
        }

        private void Gmail_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[0-9a-zA-Z@.]+$"))
            {
                e.Handled = true;
            }
        }

        private void Gmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string email = textBox.Text.Trim();

            if (email.Length > 0)
            {
                bool isValid = Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
                if (!isValid)
                {
                    textBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0xc4, 0x74, 0x74));
                    textBox.ToolTip = "Неправильний формат електронної пошти";
                    textBox.GotFocus += (s, e) =>
                    {
                        textBox.Background = Brushes.DarkGray;
                        textBox.ToolTip = "Електронна пошта має виглядати так ******@****.**";
                    };
                }
                else
                {
                    textBox.Background = new SolidColorBrush(Color.FromArgb(0xff, 0x74, 0xC4, 0x86));
                }
            }
        }
    }
}