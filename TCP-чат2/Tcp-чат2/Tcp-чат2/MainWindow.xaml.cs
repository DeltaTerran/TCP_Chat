using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Tcp_чат2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool alive = false; // будет ли работать поток для приема
        public string userName;
        private string host;
        //private const string host = "192.168.1.159";
        //private const int port = 8888;
        private int port;
        static TcpClient client;
        static NetworkStream stream;
        public List<Users> usersList_Sv = new List<Users>(); // все подключения
        public List<string> usersListToSend = new List<string>(); // все подключения


        public MainWindow()
        {
            InitializeComponent();

            SendButton.IsEnabled = false;
            Host_Add.Text = "192.168.1.159";
            Port_Add.Text = "8888";
            #region Тестовые пользователи
            //UserList.Items.Add("Сергей");
            //UserList.Items.Add("Малой");
            //UserList.Items.Add("Dick");
            #endregion

        }
        /*
         * Создается TcpClient который подключается к адрессу сервера
         * Отправляет сообщение о входе userName и создается поток с функцией ReceiveMessage
        */
        private void chatEntry_Click(object sender, RoutedEventArgs e)
        {
            userName = userName_Add.Text;
            
            client = new TcpClient();
            try
            {
                host = Host_Add.Text;
                port = int.Parse(Port_Add.Text);
                client.Connect(host, port); //подключение клиента
                stream = client.GetStream(); // получаем поток
                #region Свойства
                userName_Add.IsReadOnly = true;
                chatEntry.IsEnabled = false;
                SendButton.IsEnabled = true;
                #endregion
                
                ChatArea.Text += $"Добро пожаловать, {userName}\n";
                string message = userName_Add.Text;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);


                Task receiveThread = new Task(ReceiveMessage);
                receiveThread.Start(); //старт потока

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }

        private void chatExit_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < UserList.SelectedItems.Count; i++)
            {
                ChatArea.Text += UserList.SelectedItems[i];
            }

        }
        /// <summary>
        /// получение сообщений
        /// </summary>


        /*
         * 
         */
        private void ReceiveMessage()
        {
            alive = true;
            try
            {
                while (alive)
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    //   while (message.Length > 0)
                    //  {
                    if (message.IndexOf("%%@@") != -1)
                    {
                        //1. Получить список ID выделенных пользователей
                        List<string> markeredUsers = new List<string>();
                                                
                        this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            if (UserList.SelectedItems.Count > 0)
                            {
                                for (int i = 0; i < UserList.SelectedItems.Count; i++)
                                {
                                    foreach (var userSearch in usersList_Sv)
                                    {
                                        if (userSearch.userName == UserList.SelectedItems[i])
                                        {
                                            markeredUsers.Add(userSearch.id);
                                        }
                                    }
                                }
                        }
                        }));
                        //2. Очистить списко пользователей
                        this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            UserList.Items.Clear();
                        }));
                        //3. Очистить список пользователей "скрытый"
                        usersList_Sv.Clear();
                        int endOfList = message.IndexOf("%%@@");
                        string arrayUsers = message.Substring(0, endOfList + 3);    //Строка со всеми активными чатланами
                        if (arrayUsers != "%%@")                        
                        {
                            while (arrayUsers.IndexOf("%%@") != -1)
                            {
                                var currUser = new Users();
                                int substrLenght = arrayUsers.IndexOf("%%@");
                                string gross = arrayUsers.Substring(0, substrLenght);   //подстрока содержит индекс и имя
                                arrayUsers = arrayUsers.Substring(substrLenght + 3);    //"хвост" подстроки (с остальніми пользователями)
                                currUser.userName = gross.Substring(36);                //имя пользователя
                                currUser.id = gross.Substring(0, 36);                   //индекс пользователя

                                usersList_Sv.Add(currUser);                                         //добавляем в список Имя пользователя и его АйДи

                                this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>  //добавляем в видимый список Имя пользовтаееля
                                {
                                    UserList.Items.Add(currUser.userName);
                                }));

                                if (markeredUsers.Contains(currUser.userName))                            //если пользователь был ранее выделен, то выделяем его
                                {
                                    ListBoxItem lbi = (ListBoxItem)UserList.ItemContainerGenerator.ContainerFromIndex(UserList.Items.Count - 1);
                                    lbi.IsSelected = true;
                                }
                            }
                           
                            message = message.Substring(endOfList + 4);
                        }

                        if (message == "%%@@")
                        {
                            message = "";
                        }
                    }
                    
                    #region Сейв
                    /*if (message.IndexOf("%%@@") != -1)
                {
                    usersList.Clear();
                    int endOfList = message.IndexOf("%%@@");
                    string arrayUsers = message.Substring(0, endOfList + 3);    //Строка со всеми активными чатланами
                    if (arrayUsers == "%%@")
                    {
                        UserList.Items.Clear();
                    }
                    else 
                    {
                        while (arrayUsers.IndexOf("%%@") != -1)
                        {
                            var currUser = new Users();
                            int substrLenght = arrayUsers.IndexOf("%%@");
                            string gross = arrayUsers.Substring(0, substrLenght);
                            arrayUsers = arrayUsers.Substring(substrLenght + 3);
                            currUser.userName = gross.Substring(36);
                            currUser.id = gross.Substring(0, 36);
                            this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                            {
                                UserList.Items.Add(currUser.userName);
                            }));
                        }
                        this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            UserList.Items.Add(arrayUsers);
                        }));
                        message = message.Substring(endOfList + 4);
                    }

                }*/
                    //}
                    #endregion

                    this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        ChatArea.Text += "\n" + message;
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|| Сломался метод ReciveMessage");
            }
        }
       
        void SendMessage()
        {
            try
            {
                //1. Получить список имён віделенных пользоваателей.
                //Если выделеных нет - посылаем %%@@, а на сервере организуем равссылку всем!!!
                usersListToSend.Clear();
                for (int i = 0; i < UserList.SelectedItems.Count; i++)
                {
                    foreach (var item in usersList_Sv)
                    {
                        if (item.userName == UserList.SelectedItems[i])
                        {
                            usersListToSend.Add(item.id);
                        }
                    }
                }
                string idListToSend = "";
                foreach (var item in usersListToSend)
                {
                    idListToSend += item;
                }
                idListToSend += "%%@@";

                string message = idListToSend + SendText.Text;
                byte[] data = Encoding.Unicode.GetBytes(message);


                stream.Write(data, 0, data.Length);
                ChatArea.Text += String.Format("\n{0}: {1}", userName, SendText.Text);
                SendText.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void SendText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }
        public class Users 
        {
            public string userName;
            public string id;
        }

        private void chatSelect_Click(object sender, RoutedEventArgs e)
        {
            // Select every other item, starting with
            // the first.
            int i = 0;
            while (i < UserList.Items.Count)
            {
                // Get item's ListBoxItem
                ListBoxItem lbi = (ListBoxItem)UserList.ItemContainerGenerator.ContainerFromIndex(i);
                lbi.IsSelected = true;
                i += 2;
            }


        }
    }

}
