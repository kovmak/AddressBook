using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace AddressBoook
{
    public static class Controller
    {
        private static string PathToFileXML = Directory.GetCurrentDirectory().ToString() + "\\Contact.xml";

        private static XmlDocument? MainXmlDocument;
        private static XmlNode? RootNode;

        private static ObservableCollection<Address> FullAddressCollection;
        private static ObservableCollection<Address> SearchAddressCollection;

        public delegate void ControllerHeandler();

        public static event ControllerHeandler? AddressCollectionChanged;
        public static event ControllerHeandler? ClearSearchText;

        public static event ControllerHeandler? EmptyFieldFio;
        public static event ControllerHeandler? EmptyFieldTelephoneNumber;
        public static event ControllerHeandler? EmptyFieldHome;
        public static event ControllerHeandler? EmptyFieldGmail;

        private static ObservableCollection<Address> _addressCollection = new ObservableCollection<Address>();
        public static ObservableCollection<Address> AddressCollection
        {
            get { return _addressCollection; }
            set { _addressCollection = value; }
        }

        #region WindowsManipulation

        /// <summary>
        /// Открытие окна добавления 
        /// </summary>
        /// <param name="owner"></param>
        public static void OpenAddContactWindow(Window? owner = null)
        {
            AddWindow addWindow = new AddWindow();
            addWindow.Owner = owner;
            addWindow.Show();
        }

        /// <summary>
        /// Открытие окна изменения
        /// </summary>
        /// <param name="address"></param>
        /// <param name="owner"></param>
        public static void OpenChangeContactWindow(Address address, Window? owner = null)
        {
            if (address != null)
            {
                ChangeWindow changeWindow = new ChangeWindow(address);
                changeWindow.Owner = owner;
                changeWindow.Show();
            }
        }

        /// <summary>
        /// Закрытие окна
        /// </summary>
        /// <param name="window"></param>
        public static void CloseWindow(Window window)
        {
            window.Close();
        }

        #endregion WindowsManipulation

        #region WorkWithBase

        /// <summary>
        /// Добавление в контакта в базу
        /// </summary>
        /// <param name="address"></param>
        /// <param name="window"></param>
        public static void AddContactToBase(Address address, Window window)
        {
            if (address != null)
            {
                AddressCollection.Add(address);
                if (MainXmlDocument != null)
                {
                    AddToXML(address);
                }
                else
                {
                    SaveToXML();
                }

                window.Close();
            }
        }

        /// <summary>
        /// Изменение контакта в базе
        /// </summary>
        /// <param name="address"></param>
        /// <param name="window"></param>
        public static void ChangeContactInBase(Address address, Window window)
        {
            if (address != null)
            {
                ChangeInCollection(address);
                ChangeInXML(address);

                window.Close();
            }
        }

        /// <summary>
        /// Удаление контакта из базы
        /// </summary>
        /// <param name="address"></param>
        public static void DeleteContactFromBase(Address address)
        {
            if (address != null)
            {
                AddressCollection.Remove(address);
                DeleteFromXML(address.Id);
            }
        }

        #endregion WorkWithBase

        #region WorkWithXML

        /// <summary>
        /// Добавление записи в XML файл
        /// </summary>
        /// <param name="currentAddress"></param>
        private static void AddToXML(Address currentAddress)
        {
            try
            {
                XmlNode idNode = MainXmlDocument.CreateElement("Id");
                XmlAttribute idAttr = MainXmlDocument.CreateAttribute("Value");
                idAttr.Value = currentAddress.Id.ToString();
                idNode.Attributes.Append(idAttr);

                XmlNode fioNode = MainXmlDocument.CreateElement("Fio");
                fioNode.InnerText = currentAddress.Fio.ToString();

                XmlNode homeNode = MainXmlDocument.CreateElement("Home");
                homeNode.InnerText = currentAddress.Home.ToString();

                XmlNode GmailNode = MainXmlDocument.CreateElement("Gmail");
                GmailNode.InnerText = currentAddress.Gmail.ToString();

                XmlNode TelephoneNode = MainXmlDocument.CreateElement("TelephoneNumber");
                TelephoneNode.InnerText = currentAddress.TelephoneNumber.ToString();

                RootNode.AppendChild(idNode);

                idNode.AppendChild(fioNode);
                idNode.AppendChild(TelephoneNode);
                idNode.AppendChild(homeNode);
                idNode.AppendChild(GmailNode);

                MainXmlDocument.Save(PathToFileXML);
            }
            catch (Exception ex) { }
        }

        private static void ChangeInXML(Address currentAddress)
        {
            try
            {
                XmlNodeList addressList = MainXmlDocument.SelectNodes("//Contacts/Id");

                foreach (XmlNode address in addressList)
                {
                    int id = int.Parse(address.Attributes["Value"].Value);

                    if (id == currentAddress.Id)
                    {
                        address["Fio"].InnerText = currentAddress.Fio;
                        address["TelephoneNumber"].InnerText = currentAddress.TelephoneNumber;
                        address["Home"].InnerText = currentAddress.Home;
                        address["Gmail"].InnerText = currentAddress.Gmail;
                    }
                }

                MainXmlDocument.Save(PathToFileXML);
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// Удаление записи из XML файла
        /// </summary>
        /// <param name="adressId"></param>
        private static void DeleteFromXML(int adressId)
        {
            try
            {
                XmlNodeList addressList = MainXmlDocument.SelectNodes("//Contacts/Id");

                foreach (XmlNode address in addressList)
                {
                    int id = int.Parse(address.Attributes["Value"].Value);

                    if (id == adressId)
                    {
                        RootNode.RemoveChild(address);
                        break;
                    }
                }

                MainXmlDocument.Save(PathToFileXML);
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// Сохранение XML файла
        /// </summary>
        private static void SaveToXML()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlNode rootNode = xmlDoc.CreateElement("Contacts");
                xmlDoc.AppendChild(rootNode);

                foreach (Address address in AddressCollection)
                {
                    XmlNode idNode = xmlDoc.CreateElement("Id");
                    XmlAttribute idAttr = xmlDoc.CreateAttribute("Value");
                    idAttr.Value = address.Id.ToString();
                    idNode.Attributes.Append(idAttr);

                    XmlNode fioNode = xmlDoc.CreateElement("Fio");
                    fioNode.InnerText = address.Fio.ToString();

                    XmlNode TelephoneNode = xmlDoc.CreateElement("TelephoneNumber");
                    TelephoneNode.InnerText = address.TelephoneNumber.ToString();

                    XmlNode homeNode = MainXmlDocument.CreateElement("Home");
                    homeNode.InnerText = address.Home.ToString();

                    XmlNode GmailNode = MainXmlDocument.CreateElement("Gmail");
                    GmailNode.InnerText = address.Gmail.ToString();

                    rootNode.AppendChild(idNode);

                    idNode.AppendChild(fioNode);
                    idNode.AppendChild(TelephoneNode);
                    idNode.AppendChild(homeNode);
                    idNode.AppendChild(GmailNode);
                }

                xmlDoc.Save(PathToFileXML);

                MainXmlDocument = xmlDoc;
                RootNode = xmlDoc.SelectSingleNode("Contacts");
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// Загурзка XML файла
        /// </summary>
        public static void LoaodFromXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (File.Exists(PathToFileXML))
            {
                xmlDoc.Load(PathToFileXML);
                XmlNodeList addressList = xmlDoc.SelectNodes("//Contacts/Id");

                foreach (XmlNode address in addressList)
                {
                    int id = int.Parse(address.Attributes["Value"].Value);
                    string fio = address["Fio"].InnerText;
                    string telephoneNumber = address["TelephoneNumber"].InnerText;
                    string home = address["Home"].InnerText;
                    string gmail = address["Gmail"].InnerText;

                    AddressCollection.Add(new Address
                    {
                        Id = id,
                        Fio = fio,
                        TelephoneNumber = telephoneNumber,
                        Home = home,
                        Gmail = gmail,
                        FioBrush = Painter(CheckFio(fio)),
                        TelephoneBrush = Painter(CheckTelephoneNumber(telephoneNumber)),
                        HomeBrush = Painter(CheckHome(home)),
                        GmailBrush = Painter(CheckGmail(gmail))
                    });
                }

                MainXmlDocument = xmlDoc;
                RootNode = xmlDoc.SelectSingleNode("Contacts");
            }
        }

        #endregion WorkWithXML

        #region Checking

        /// <summary>
        /// Валлидация ФИО
        /// </summary>
        /// <param name="fio"></param>
        /// <returns></returns>
        public static bool CheckFio(string fio)
        {
            bool checker = false;

            int length = fio.Length;

            if (length >= 2 && length <= 50)
            {
                checker = true;
            }

            return checker;
        }
        public static bool CheckHome(string home)
        {
            bool checker = false;

            int length = home.Length;

            if (length >= 2 && length <= 50)
            {
                checker = true;
            }

            return checker;
        }

        public static bool CheckGmail(string gmail)
        {
            bool checker = false;

            int length = gmail.Length;

            if (length >= 2 && length <= 50)
            {
                checker = true;
            }

            return checker;
        }

        /// <summary>
        /// Валидация номера телефона
        /// </summary>
        /// <param name="telephoneNumber"></param>
        /// <returns></returns>
        public static bool CheckTelephoneNumber(string telephoneNumber)
        {
            bool checker = false;

            Regex regex = new Regex(@"^\+380\d{2}\d{3}\d{4}$");

            MatchCollection matchCollection = regex.Matches(telephoneNumber);

            if (matchCollection.Count > 0)
            {
                checker = true;
            }

            return checker;
        }

        #endregion Checking

        /// <summary>
        /// Получение максимального ID
        /// </summary>
        /// <returns></returns>
        private static int GetMaxId()
        {
            int counter = 0;

            if(AddressCollection != null && AddressCollection.Count > 0)
            {
                counter = AddressCollection.Max(x => x.Id);
            }

            return counter;
        }

        /// <summary>
        /// Приминение изменений адреса в коллекции 
        /// </summary>
        /// <param name="currentAddress"></param>
        private static void ChangeInCollection(Address currentAddress)
        {
            try
            {
                Address address = AddressCollection.Where(x => x.Id == currentAddress.Id).First();

                address.Fio = currentAddress.Fio;
                address.TelephoneNumber = currentAddress.TelephoneNumber;
                address.Home = currentAddress.Home;
                address.Gmail = currentAddress.Gmail;
            }
            catch (Exception ex) { }
        }

        public static Address CreateAddress(string fio, string telephoneNumber, string home, string gmail, bool raisingId = false, Address? baseAddress = null)
        {
            Address returnAddress = null;

            if (fio != null && fio != "" && telephoneNumber != null && telephoneNumber != "" && home != null && home != "" && gmail != null && gmail != "")
            {
                int riser = 0;

                if (raisingId)
                {
                    riser = 1;
                }

                if (baseAddress != null)
                {
                    returnAddress = baseAddress;

                    returnAddress.Fio = fio;
                    returnAddress.TelephoneNumber = telephoneNumber;
                    returnAddress.Home = home;
                    returnAddress.Gmail = gmail;
                    returnAddress.FioBrush = Painter(CheckFio(fio));
                    returnAddress.TelephoneBrush = Painter(CheckTelephoneNumber(telephoneNumber));
                    returnAddress.HomeBrush = Painter(CheckHome(home));
                    returnAddress.GmailBrush = Painter(CheckGmail(gmail));
                }
                else
                {
                    returnAddress = new Address
                    {
                        Id = GetMaxId() + riser,
                        Fio = fio,
                        TelephoneNumber = telephoneNumber,
                        Home = home,
                        Gmail = gmail,
                        FioBrush = Painter(CheckFio(fio)),
                        TelephoneBrush = Painter(CheckTelephoneNumber(telephoneNumber)),
                        HomeBrush = Painter(CheckHome(home)),
                        GmailBrush = Painter(CheckGmail(gmail))
                    };
                }
            }
            else
            {
                if(fio == null || fio == "")
                {
                    EmptyFieldFio.Invoke();
                }

                if (telephoneNumber == null || telephoneNumber == "")
                {
                    EmptyFieldTelephoneNumber.Invoke();
                }

                if (home == null || home == "")
                {
                    EmptyFieldHome.Invoke();
                }

                if (gmail == null || gmail == "")
                {
                    EmptyFieldGmail.Invoke();
                }

            }

            return returnAddress;
        }

        public static Brush Painter(bool checker)
        {
            Brush brush = Brushes.White;

            if(!checker)
            {
                brush = Brushes.LightPink;
            }

            return brush;
        }

        /// <summary>
        /// Поиск контактов в коллекции
        /// </summary>
        /// <param name="searchText"></param>
        public static void SearchContacts(string searchText)
        {


            if (searchText != null && searchText != "")
            {
                if (FullAddressCollection == null || FullAddressCollection.Count == 0)
                {
                    FullAddressCollection = new ObservableCollection<Address>(AddressCollection);
                }


                SearchAddressCollection = new ObservableCollection<Address>();


                Regex regex = new Regex(searchText + @"\w*", RegexOptions.IgnoreCase);
                foreach (Address address in FullAddressCollection)
                {
                    MatchCollection matchFioCollection = regex.Matches(address.Fio);
                    MatchCollection matchTelephoneNumberCollection = regex.Matches(address.TelephoneNumber);
                    MatchCollection matchHomeCollection = regex.Matches(address.Home);
                    MatchCollection matchGmailCollection = regex.Matches(address.Gmail);

                    if (matchFioCollection.Count > 0 || matchTelephoneNumberCollection.Count > 0 || matchHomeCollection.Count > 0 || matchGmailCollection.Count > 0)
                    {
                        SearchAddressCollection.Add(address);
                    }
                }

                AddressCollection = new ObservableCollection<Address>(SearchAddressCollection);
                AddressCollectionChanged.Invoke();
            }
            else
            {
                if (FullAddressCollection != null && FullAddressCollection.Count > 0)
                {
                    AddressCollection = new ObservableCollection<Address>(FullAddressCollection);

                    SearchAddressCollection.Clear();
                    FullAddressCollection.Clear();

                    AddressCollectionChanged.Invoke();
                }
            }
        }

        /// <summary>
        /// Сброс поиска
        /// </summary>
        public static void SearchReset()
        {
            if (FullAddressCollection != null && FullAddressCollection.Count > 0)
            {
                AddressCollection = new ObservableCollection<Address>(FullAddressCollection);

                SearchAddressCollection.Clear();
                FullAddressCollection.Clear();

                AddressCollectionChanged.Invoke();
                ClearSearchText.Invoke();
            }
        }
    }
}
