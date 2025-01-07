using System;
using System.Threading;

namespace Server
{
    class Program
    {
        static ServerObject server; // сервер
        static Thread listenThread; // потока для прослушивания

        /// <summary>
        /// При запуске сервера создается объект serverObject и потоку присваивается метод класса serverObject - Listen
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start(); //старт потока
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
