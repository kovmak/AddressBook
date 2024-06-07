using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Microsoft.Win32;
using System.Data;

namespace AddressBoook
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Controller.LoaodFromXML();
            Controller.AddressCollectionChanged += RefreshCoollection;
            Controller.ClearSearchText += ClearSearchText;
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
        }

        public ObservableCollection<Address> AddressCollection
        {
            get { return Controller.AddressCollection; }
            set { Controller.AddressCollection = value; OnPropertyChanged(nameof(AddressCollection)); }
        }

        private Address _selectedAddress;
        public Address SelectedAddress
        {
            get { return _selectedAddress; }
            set { _selectedAddress = value; OnPropertyChanged(nameof(SelectedAddress)); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                Controller.SearchContacts(value);
            }
        }

        #region AdditionalMethods

        private void RefreshCoollection()
        {
            OnPropertyChanged(nameof(AddressCollection));
        }

        private void ClearSearchText()
        {
            SearchText = "";
        }

        #endregion AdditionalMethods

        #region Commands

        public ICommand AddContact
        {
            get { return new DelegateCommand((obj) => { Controller.OpenAddContactWindow(this); }); }
        }

        public ICommand ChangeContact
        {
            get { return new DelegateCommand((obj) => { Controller.OpenChangeContactWindow(SelectedAddress, this); }); }
        }

        public ICommand DeleteContact
        {
            get { return new DelegateCommand((obj) => { Controller.DeleteContactFromBase(SelectedAddress); }); }
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
        private SupportWindow supportWindow;
        private void ButtonSupport_Click(object sender, RoutedEventArgs e)
        {
            if (supportWindow == null)
            {
                supportWindow = new SupportWindow();
                supportWindow.Closed += SupportWindow_Closed; 
                supportWindow.Show();

                supportButton.IsEnabled = false; 
            }
        }
        private void SupportWindow_Closed(object sender, EventArgs e)
        {
            supportWindow = null;
            supportButton.IsEnabled = true;
        }


        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private AddWindow addWindow;
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (addWindow == null)
            {
                addWindow = new AddWindow();
                addWindow.Closed += AddWindow_Closed;
                addWindow.Show();

                addButton.IsEnabled = false;
            }
        }
        private void AddWindow_Closed(object sender, EventArgs e)
        {
            addWindow = null; 
            addButton.IsEnabled = true; 
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            bool isRowVisible = false;
            foreach (var item in DataGrid.Items)
            {
                DataGridRow row = (DataGridRow)DataGrid.ItemContainerGenerator.ContainerFromItem(item);
                if (row != null && row.ActualHeight > 0 && row.TransformToAncestor(this).TransformBounds(new Rect(0.0, 0.0, row.ActualWidth, row.ActualHeight)).Bottom > ActualHeight)
                {
                    isRowVisible = true;
                    break;
                }
            }

            if (!isRowVisible)
            {
                DataGrid.Width = double.NaN;
                DataGrid.Height = double.NaN;
                return;
            }

            double newWidth = ActualWidth - 10;
            double newHeight = ActualHeight - 10;
            DataGrid.Width = newWidth;
            DataGrid.Height = newHeight;
            while (isRowVisible && newHeight > 0 && newWidth > 0)
            {
                isRowVisible = false;
                foreach (var item in DataGrid.Items)
                {
                    DataGridRow row = (DataGridRow)DataGrid.ItemContainerGenerator.ContainerFromItem(item);
                    if (row != null && row.ActualHeight > 0 && row.TransformToAncestor(this).TransformBounds(new Rect(0.0, 0.0, row.ActualWidth, row.ActualHeight)).Bottom > ActualHeight)
                    {
                        isRowVisible = true;
                        break;
                    }
                }
                if (isRowVisible)
                {
                    newWidth -= 5;
                    DataGrid.Width = newWidth;
                    newHeight -= 5;
                    DataGrid.Height = newHeight;
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            Collection<Address> addresses = new Collection<Address>();
            // Додайте адреси до колекції addresses

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files|*.xlsx";
            saveFileDialog.Title = "Виберіть місце збереження Excel файлу";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                ExcelUtils.WriteExcel(AddressCollection, filePath);
                MessageBox.Show("Excel файл успішно збережено!");
            }
        }
    }
}
