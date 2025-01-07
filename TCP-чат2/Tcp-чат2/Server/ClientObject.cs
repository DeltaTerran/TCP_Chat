using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ClientObject
    {
        /// <summary>
        /// У объекта ClientObject будет устанавливаться свойство Id, которое будет уникально его идентифицировать, и свойство Stream, хранящее поток для взаимодействия с клиентом.
        /// </summary>
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        public string userName;
        TcpClient client;
        ServerObject server; // объект сервера
        //При создании нового объекта в конструкторе будет происходить его добавление в коллекцию подключений класса ServerObject, который мы далее создадим:
        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }
        /// <summary>
        /// Основные действия происходят в методе Process(), в котором реализован простейший протокол для обмена сообщениями с клиентом.
        /// Так, в начале получаем имя подключенного пользователя, а затем в цикле получаем все остальные сообщения.
        /// Для трансляции этих сообщений всем остальным клиентам будет использоваться метод BroadcastMessage_2() класса ServerObject.
        /// метод Получает имя пользователя с помощью метода GetMessage затем рассылает сообщение что пользователь вошел в чат
        /// </summary>
        public void Process()
        {
            try
            {

                Stream = client.GetStream();
                //Получаем имя пользователя
                string message = GetMessage();
                userName = message;
                message = userName + " вошёл в чат";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                server.BroadcastMessage_2(message, this.Id, server.clients);
                Console.WriteLine(message);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        
                        Console.WriteLine(message);

                        //1. Получаем список рассыылки
                        List<ClientObject> myClients = new List<ClientObject>();
                        int endOfList = message.IndexOf("%%@@");
                        if (endOfList == 0)
                        {
                            myClients = server.clients;
                        }
                        else
                        {
                            string gross = message.Substring(0, endOfList);   //подстрока содержит АйДи пользователей
                            while (gross.Length > 0)
                            {
                                string stringId = gross.Substring(0, 36);
                                gross = gross.Substring(36);
                                foreach (var item in server.clients)
                                {
                                    if (item.Id == stringId)
                                    {
                                        myClients.Add(item);
                                        break;
                                    }
                                }
                            }
                        }

                        message = String.Format("{0}: {1}", userName, message.Substring(endOfList + 4));
                        
                        //2. Рассылаем

                        server.BroadcastMessage_2(message, this.Id, myClients);

                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message + "     "+ ex.Source + "     "+ ex.HelpLink);
                        message = String.Format("{0}: покинул чат", userName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                //в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }


        /// <summary>
        /// чтение входящего сообщения и преобразование в строку
        /// </summary>
        /// <returns>входящее сообщение</returns>
        private string GetMessage()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (Stream.DataAvailable);
            string str = builder.ToString();
            return builder.ToString();
        }
        protected internal void Close()
        {
            if (Stream != null)
            {
                Stream.Close();
            }
            if (client != null)
            {
                client.Close();
            }
        }
    }
}
