#define SOCKET_IO_DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace SocketIO
{
    public class SocketIOManager : MonoBehaviour
    {
        string url = "";
        public WebSocket ws;
        public Action<string> debugMethod;
        public float pingDelay = 25;
        public float reconnectDelay = 5;
        private Parser parser;
        private Decoder decoder;
        [SerializeField] int donateQueueCount = 0;

        public bool doReconnect;
        public bool rightConnection;

        private object donateQueueLock = new object();
        private Queue<SocketIOEvent> donateQueue = new Queue<SocketIOEvent>();

        private object pongLock = new object();
        private Queue<Blinker.BLINK_TYPE> pongQueue = new Queue<Blinker.BLINK_TYPE>();

        IEnumerator SendPing;

        IEnumerator TryReconnect;

        public void Awake()
        {
            rightConnection = false;
            parser = new Parser();
            decoder = new Decoder();
            SetWebSocket();
            doReconnect = false;
            SendPing = SendPing_Base(pingDelay);
            TryReconnect = TryReconnect_Base(reconnectDelay);
            if (debugMethod == null)
            {
                debugMethod = Debug.Log;
            }
        }

        public void Start()
        {
            ws.Connect();
            if (ws.IsConnected)
            {
                StartCoroutine(SendPing);
            }
        }

        public void Update()
        {
            lock (donateQueueLock)
            {
                while (donateQueue.Count > 0)
                {
                    EmitDonate(donateQueue.Dequeue());
                }
            }

            lock (pongLock)
            {
                while (pongQueue.Count > 0)
                {
                    Blinker.Instance.SetColor(pongQueue.Dequeue());
                }
            }

            CheckConnection();
            donateQueueCount = donateQueue.Count;
        }

        void SetWebSocket()
        {
            url = DataManager.Instance.GetValues(DataManager.Instance.m_OptionData, "TwipWssAddress");
            url = "wss://io.mytwip.net/socket.io/?alertbox_key="+ url + "&version=1.1.54&EIO=3&transport=websocket";
            ws = new WebSocket(url);
            ws.OnOpen += OnOpen;
            ws.OnMessage += OnMessage;
            ws.OnError += OnError;
            ws.OnClose += OnClose;
        }

        public void CloseWebSocket()
        {
            ws.Close();
        }

        void CheckConnection()
        {
            if (!ws.IsConnected && !doReconnect)
            {
                Debug.Log("연결 시도");
                StopCoroutine(SendPing);
                SendPing = SendPing_Base(pingDelay);
                StartCoroutine(TryReconnect);
            }
            else if(ws.IsConnected && doReconnect)
            {
                doReconnect = false;
                StopCoroutine(TryReconnect);
                TryReconnect = TryReconnect_Base(reconnectDelay);
                StartCoroutine(SendPing);
            }
        }

        IEnumerator SendPing_Base(float PingDelay)
        {
            yield return new WaitForSeconds(5);
            while (ws.IsConnected)
            {
                if (!rightConnection)
                {
                    LogUI.Instance.SetText("연결된 키에서 사용자 정보를 찾을 수 없습니다.");
                    CloseWebSocket();
                }
                ws.Send("2");
                yield return new WaitForSeconds(PingDelay);
            }
        }
        IEnumerator TryReconnect_Base(float ReconnectDelay)
        {
            doReconnect = true;
            while (!ws.IsConnected)
            {
                Blinker.Instance.SetColor(Blinker.BLINK_TYPE.Error);
                yield return new WaitForSeconds(ReconnectDelay);
                SetWebSocket();
                ws.Connect();
                LogUI.Instance.SetText("소켓서버와 다시 연결중입니다.");
            }
            LogUI.Instance.SetText("연결되었습니다.");
        }

        private void OnOpen(object sender, EventArgs e)
        {
#if SOCKET_IO_DEBUG
            debugMethod.Invoke("[SocketIO]Open: ");
#endif
            LogUI.Instance.SetText("소켓서버와 연결되었습니다!");
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
#if SOCKET_IO_DEBUG
            //debugMethod.Invoke("[SocketIO] Raw message: " + e.Data);
#endif
            Packet packet = decoder.Decode(e);
            if (packet.json != null && packet.socketPacketType == SocketPacketType.EVENT)
            {
                try
                {
                    SocketIOEvent ev = parser.Parse(packet.json);
                    if (ev.name == "alertbox config" || ev.name == "new donate" || ev.name == "now")
                    {
                        lock (donateQueueLock)
                        {
                            try
                            {
                                donateQueue.Enqueue(ev);
                                Debug.Log("파싱 성공" + ev.ToString());
                            }
                            catch (Exception ex)
                            {
                                Debug.Log("파싱 실패 : 기타 사유 - " + ex.Message);
                                Debug.Log("파싱명" + ev.name);
                            }
                        }
                    }
                }
                catch (SocketIOException ex)
                {
                    Debug.Log("파싱 실패" + ex.ToString());
                }
                catch(Exception ex)
                {
                    Debug.Log("파싱 실패 : " + ex.ToString());
                }
            }
            else if (packet.enginePacketType == EnginePacketType.PONG)
            {
                lock (pongLock)
                {
                    try
                    {
                        pongQueue.Enqueue(Blinker.BLINK_TYPE.Pong);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("pong error : " + ex.Message);
                    }

                }
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
#if SOCKET_IO_DEBUG
            debugMethod.Invoke("[SocketIO]error: " + e.Message);
#endif
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
#if SOCKET_IO_DEBUG
            debugMethod.Invoke("[SocketIO]close: " + e.Reason);
#endif
        }

        private void EmitDonate(SocketIOEvent ev)
        {
            switch (ev.name)
            {
                case "new donate":
                    Debug.Log("socket - new donate");
                    if (ev.data.HasField("_id"))
                    {
                        string _id = "";
                        ev.data.GetField(ref _id, "_id");
                        if (_id == "TEST")
                        {
                            Debug.Log("이건 테스트 도네입니다.");
                            int a = 0;
                            ev.data.GetField(ref a, "amount");
                            LogUI.Instance.SetText("[Socket]테스트", a);
                            PresentsManager.Instance.CreatePresents(a);
                            Blinker.Instance.SetColor(Blinker.BLINK_TYPE.Donate);
                        }
                        else
                        {
                            #if !UNITY_EDITOR
                            bool isRepeat = false;
                            ev.data.GetField(ref isRepeat, "repeat");
                            if (isRepeat)
                            {
                                Debug.Log("이건 반복된 도네입니다.");
                                break;
                            }
                            #endif
                            int a = 0;
                            string name = "";
                            ev.data.GetField(ref a, "amount");
                            ev.data.GetField(ref name, "nickname");
                            LogUI.Instance.SetText("[Socket]" + name, a);
                            PresentsManager.Instance.AddDonateQueue(_id, a);
                            Blinker.Instance.SetColor(Blinker.BLINK_TYPE.Donate);
                        }
                    }
                    break;
                case "alertbox config":
                    if(ev.data.HasField("name"))
                    {
                        string name = "";
                        ev.data.GetField(ref name, "name");
                        if(name == "mint_luv" || name == "baewhv")
                        {
                            rightConnection = true;
                        }
                        else
                        {
                            LogUI.Instance.SetText("닉네임이 일치하지 않아 연결을 종료합니다.");
                            LogUI.Instance.SetText("연결한 곳의 닉네임 : " +name);
                            CloseWebSocket();
                        }
                        int alert_delay = 0;
                        int donation_duration = 0;
                        ev.data.GetField(ref alert_delay, "alert_delay");
                        ev.data.GetField(ref donation_duration, "donation_duration");
                        PresentsManager.Instance.SetDonateDelayTime(alert_delay + donation_duration);
                    }
                    break;
                case "now":
                    Debug.Log("socket - now");
                    if (ev.data.HasField("_id"))
                    {
                        string _id = "";
                        ev.data.GetField(ref _id, "_id");
                        PresentsManager.Instance.DonateSetNow(_id);

                    }
                    break;

            }
        }

        private void OnApplicationQuit()
        {
            CloseWebSocket();
        }
    }
}

