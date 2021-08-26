using System;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using Assets.Scripts.Util;
using Assets.Scripts.CBR.Model;
using System.Threading;
using System.Text;
using Assets.Scripts.AI;

namespace Assets.Scripts.TCPIP
{

    /**
     * Diese Klasse stellt die Verbindung via TCP/IP zum Java Projekt her.
     */
    public class ConnectionToPathViewer
    {

        /**
             * TCP-Client
             */
        private TcpClient mClient;
        /**
         * Data Stream.
         */
        private Stream mStream;

        /**
         * Diese Methode stellt konkret die Verbindung her.
         */
        private void InitiateConnection()
        {
            mClient = new TcpClient();
            mClient.Connect(Constants.HOST_ADDRESS, 5558);
            mStream = mClient.GetStream();
        }

        ~ConnectionToPathViewer()
        {
            Console.WriteLine("Connection closed");
            CloseConnection();

        }

        /**
         * Diese Methode schließt die Verbindung zwischen C# und Java.
         */
        private void CloseConnection()
        {
            if (mClient != null && mClient.Connected)
            {
                Console.WriteLine("Shutting down TCP/IP");
                mClient.Close();
            }
        }

        /**
         * Diese Methode sendet die gesammelten Daten des Frame an das Java-Projekt.
         */
        public StatisticsForPathViewer Send(StatisticsForPathViewer statistics)
        {

            InitiateConnection();

            string json = JsonParser<StatisticsForPathViewer>.SerializeObject(statistics) + Environment.NewLine;

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] data = asen.GetBytes(json);

            mStream.Write(data, 0, data.Length);

            Thread.Sleep(100);

            CloseConnection();

            return statistics;
        }
    }
}