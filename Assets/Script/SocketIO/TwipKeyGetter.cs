    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Networking;

    namespace SocketIO
    {
        public class TwipKeyGetter
        {
            public Action<string> WssHandler;
            public Action<string> AlertBoxHandler;

            public IEnumerator GetWss(string location, string token)
            {
                using (UnityWebRequest www = UnityWebRequest.Get(location))
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        www.SetRequestHeader("Authorization", "Bearer " + token);
                    }
                    yield return www.SendWebRequest();

                    // 구버전 유니티(2019 이하) 호환성 조건문
                    #if UNITY_2020_1_OR_NEWER
                    bool isSuccess = (www.result == UnityWebRequest.Result.Success);
                    #else
                    bool isSuccess = (!www.isNetworkError && !www.isHttpError);
                    #endif

                    if (isSuccess)
                    {
                        string html = www.downloadHandler.text;
                        int wssIndex = html.IndexOf("wss://");
                        if (wssIndex != -1)
                        {
                            int endIndex = html.IndexOf("\"", wssIndex);
                            if (endIndex != -1)
                            {
                                string wssUrl = html.Substring(wssIndex, endIndex - wssIndex);
                                WssHandler?.Invoke(wssUrl);
                                yield break;
                            }
                        }
                    }
                    WssHandler?.Invoke("");
                }
            }

            public IEnumerator GetAlertBoxConfig(string location)
            {
                using (UnityWebRequest www = UnityWebRequest.Get(location))
                {
                    yield return www.SendWebRequest();

                    #if UNITY_2020_1_OR_NEWER
                    bool isSuccess = (www.result == UnityWebRequest.Result.Success);
                    #else
                    bool isSuccess = (!www.isNetworkError && !www.isHttpError);
                    #endif

                    if (isSuccess)
                    {
                        string html = www.downloadHandler.text;
                        AlertBoxHandler?.Invoke(html);
                    }
                    else
                    {
                        AlertBoxHandler?.Invoke("");
                    }
                }
            }
        }
    }