using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AddressBoook
{
    public class Address: INotifyPropertyChanged
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        private string _name;
        public string Fio
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(nameof(Fio)); }
        }

        private string _telephoneNumber;
        public string TelephoneNumber
        {
            get { return _telephoneNumber; }
            set { _telephoneNumber = value; OnPropertyChanged(nameof(TelephoneNumber)); }
        }

        private string _home;
        public string Home
        {
            get { return _home; }
            set { _home = value; OnPropertyChanged(nameof(Home)); }
        }

        private string _gmail;
        public string Gmail
        {
            get { return _gmail; }
            set { _gmail = value; OnPropertyChanged(nameof(Gmail)); }
        }

        private Brush _telephoneBrush;
        public Brush TelephoneBrush
        {
            get { return _telephoneBrush; }
            set { _telephoneBrush = value; OnPropertyChanged(nameof(TelephoneBrush)); }
        }

        private Brush _fioBrush;
        public Brush FioBrush
        {
            get { return _fioBrush; }
            set { _fioBrush = value; OnPropertyChanged(nameof(FioBrush)); }
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
