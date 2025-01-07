using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class ServerObject
    {
        static TcpListener tcpListener; // сервер для прослушивания
        public List<ClientObject> clients = new List<ClientObject>(); // все подключения



        #region Методы
        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null)
            {
                clients.Remove(client);
            }
        }
        // прослушивание входящих подключений
        /// <summary>
        /// В методе Listen создается tcpListner, Который берет IP устройства на котором он запущен и переходит в режим ожидания
        /// В этом режиме он находится до момента, пока к нему не подключится первый пользователь
        /// Этого пользователя "Регестрируют" по следующим критериям: 
        /// ID(Создается для каждого пользователя),самого пользователя "как его видит программа" сервер к которому он подключается
        /// Так же при создании нового объекта будет происходить его добавление в коллекцию подключений класса ServerObject
        /// Далее создается новый поток которому присваивается метод метод класса СlientObject - Process
        /// 
        /// </summary>
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");
                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }
        /// <summary>
        ///  трансляция сообщения подключенным клиентам
        /// </summary>
        /// <param name="message"></param>
        /// <param name="id"></param>
        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id)// если id клиента не равно id отправляющего
                {
                    clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
            }
        }
        protected internal void BroadcastMessage_2(string message, string id, List<ClientObject> myClients)
        {

            //формирую строку, содержащую все АйДи и Имена пользователей
            string resiversStringFull = "";

            foreach (var item in clients)
            {
                resiversStringFull = resiversStringFull + item.Id + item.userName + "%%@";
            }

            if (resiversStringFull == "")
            {
                resiversStringFull = "%%@@";
            }
            else
            {
                resiversStringFull += "@";
            }

            //для текущего получателя исключаю из строки resiversStringFull его АйДи и Имя
            List<string> exceptionIdList = new List<string>();
            string messageToSend = "";
            foreach (var item in myClients)
            {
                if (item.Id == id)
                {
                    continue;
                }
                string currResiverString = item.Id + item.userName + "%%@";
                string stringToSend = resiversStringFull.Replace(currResiverString, "");
                exceptionIdList.Add(item.Id);

                messageToSend = stringToSend + message;

                byte[] data = Encoding.Unicode.GetBytes(messageToSend);   //преобразование в двоичнніе данные
                item.Stream.Write(data, 0, data.Length);            //передача данных

            }

            foreach (var item in clients) 
            {
                if (exceptionIdList.Contains(item.Id))
                {
                    //всё хорошо, сообщение уже отправлено
                }
                else
                {
                    string currResiverString = item.Id + item.userName + "%%@";
                    string stringToSend = resiversStringFull.Replace(currResiverString, "");

                    if (stringToSend == "@")
                    {
                        continue;
                    }

                    message = stringToSend;

                    byte[] data = Encoding.Unicode.GetBytes(message);   //преобразование в двоичнніе данные
                    item.Stream.Write(data, 0, data.Length);            //передача данных

                }
            }      
        }
        protected internal void Disconnect()
        {
            tcpListener.Stop();
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }
        #endregion
    }
}
