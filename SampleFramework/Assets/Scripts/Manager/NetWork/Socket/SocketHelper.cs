using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace BaseFramework
{
    public class SocketHelper : Singleton<SocketHelper>
    {
        private string serverIP = "127.0.0.1";
        private int serverPort = 996;

        private const int RECEIVE_MAX_NUM = 4096;
        private byte[] receiveArray;

        private Socket socket;

        public delegate void ConnectCallback();

        public event ConnectCallback ConnectSucceededCallback;
        public event ConnectCallback ConnectFailedCallback;
        public event ConnectCallback ConnectCloseCallback;

        public override void Init()
        {
            base.Init();
        }

        public override void Release()
        {
            CloseConnect();

            base.Release();
        }

        public void InitConnect()
        {
            Connect();
        }

        public void CloseConnect()
        {
            DisConnect();
        }

        private void Connect()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address = IPAddress.Parse(serverIP);
            IPEndPoint endPoint = new IPEndPoint(address, serverPort);
            IAsyncResult result = socket.BeginConnect(endPoint, new AsyncCallback(ConnectedCallback), socket);
        }

        private void DisConnect()
        {
            if (socket != null && socket.Connected)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    socket = null;
                }
                catch (Exception e)
                {
                    MDebug.LogError(e.StackTrace);
                    socket = null;
                    return;
                }
                ConnectCloseCallback?.Invoke();
            }
        }

        private void ConnectedCallback(IAsyncResult ar)
        {
            try
            {
                MDebug.Log("Connected");
                socket.EndConnect(ar);
                ConnectSucceededCallback?.Invoke();
                Receive();
            }
            catch (Exception e)
            {
                MDebug.LogError(e.StackTrace);
                ConnectFailedCallback?.Invoke();
                return;
            }

        }

        private void Receive()
        {
            try
            {
                if (socket == null || !socket.Connected)
                {
                    throw new Exception("Socket Error");
                }

                receiveArray = new byte[RECEIVE_MAX_NUM];
                socket.BeginReceive(receiveArray, 0, RECEIVE_MAX_NUM, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
            catch (Exception e)
            {
                MDebug.LogError(e.StackTrace);
                DisConnect();
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            //粘包分包待处理

            int length = socket.EndReceive(ar);
            if (length > 0)
            {
                MDebug.Log(length.ToString());
                MDebug.Log(socket.Available.ToString());

                MDebug.Log($"接收到消息:{Encoding.UTF8.GetString(receiveArray)}");
                Receive();
            }
        }
    }
}
