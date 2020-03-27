using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text.RegularExpressions;

public class DonateManager : SingletonMonoBehaviour<DonateManager>
{
    int SessionDonationCount { get; set; }
    int RecentDonateAmountTextAccessTime;
    string sessionDonationCountTextRocation;
    string recentDonatorsTextRocation;
    bool canDonate = false;
    LogUI m_massege = null;

    
    public void getTVMessage(LogUI obj)
    {
        m_massege = obj;
    }

    int ReadSessionDonationCount()//현재 세션에서 받은 도네 횟수 카운트.
    {
        FileInfo file = new FileInfo(sessionDonationCountTextRocation);
        if (file.Exists)
        {
            TextReader sr = File.OpenText(sessionDonationCountTextRocation);
            int count = int.Parse(sr.ReadToEnd());
            sr.Close();
            return count;
        }
        else
        {
            Debug.Log("해당 파일이 없습니다. ReadSessionDonationCount");
        }
        return 0;
    }

    int CheckRecentDonateAmountAccessTime()
    {
        FileInfo file = new FileInfo(sessionDonationCountTextRocation);
        if (file.Exists)
        {
            return file.LastAccessTime.Second * 1000 + file.LastAccessTime.Millisecond;
        }
        else
        {
            Debug.Log("해당 파일이 없습니다. CheckRecentDonateAmountAccessTime");
        }
        return 0;
    }

    void GetDonates(int Donaters)
    {
        FileInfo file = new FileInfo(recentDonatorsTextRocation);
        if (file.Exists)
        {
            TextReader sr = File.OpenText(recentDonatorsTextRocation);
            string[] texts = sr.ReadToEnd().Split(' ');
            for (int i = 0; i < Donaters; i++)
            {
                
                int j = 0;
                if(int.TryParse(Regex.Replace(texts[i * 2 + 1], @"\D", ""),out j))
                {

                    if (j >= 1000)
                    {
                        Debug.Log(i +" and "+j);
                        m_massege.SetText(texts[i*2], j);
                        PresentsManager.Instance.CreatePresents(j);
                    }
                }
            }
            sr.Close();
        }
        else
        {
            Debug.Log("해당 파일이 없습니다. recentDonateAmountTextRocation");
        }
    }

    string ReadDefaultPath()
    {
        StreamReader sr = new StreamReader(Application.persistentDataPath + "/Config.txt");
        string str = sr.ReadToEnd();

        DirectoryInfo DI = new DirectoryInfo(str);
        m_massege.SetText("Config 파일에 적힌 폴더 주소가 잘 연결됬는지 볼게요!");
        if (DI.Exists)
        {
            m_massege.SetText("폴더를 찾았어요!");
        }
        else
        {
            m_massege.SetText("폴더를 못찾았어요.");
            m_massege.SetText("현재 찾고있는 폴더 이름 :" + str);
            return null;
        }
        sr.Close();
        canDonate = true;
        return str;
    }



    bool CheckConfigFolder()
    {
        FileInfo file = new FileInfo(sessionDonationCountTextRocation);
        m_massege.SetText("session_donation_count.txt 파일을 찾아봅니다.");
        if (file.Exists)
        {
            m_massege.SetText("파일을 찾았어요!");
        }
        else
        {
            m_massege.SetText("파일을 못찾았어요...");
            return false;
        }

        file = new FileInfo(recentDonatorsTextRocation);
        m_massege.SetText("most_recent_donators.txt 파일을 찾아봅니다.");
        if (file.Exists)
        {
            m_massege.SetText("파일을 찾았어요!");
        }
        else
        {
            m_massege.SetText("파일을 못찾았어요...");
            return false;
        }
        m_massege.SetText("도네를 받을 준비가 되었어요!");
        return true;
    }

    override protected void OnStart()
    {
        base.OnStart();
        string path = DataManager.Instance.GetValues(DataManager.Instance.m_OptionData, "TwippyLogAddress");
        sessionDonationCountTextRocation = path + @"\session_donation_count.txt";
        recentDonatorsTextRocation = path + @"\most_recent_donators.txt";
        if(CheckConfigFolder())
        {
            canDonate = true;
        }
        SessionDonationCount = ReadSessionDonationCount();
        RecentDonateAmountTextAccessTime = CheckRecentDonateAmountAccessTime();
        if(canDonate)
            StartCoroutine(CheckDonate());
    }

    IEnumerator CheckDonate()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.1f);
            int time = CheckRecentDonateAmountAccessTime();
            if (RecentDonateAmountTextAccessTime != time)
            {
                RecentDonateAmountTextAccessTime = time;
                int donateCount = ReadSessionDonationCount();
                if (donateCount - SessionDonationCount >= 1) //1개의 도네가 왔다.
                {
                    GetDonates(donateCount - SessionDonationCount);
                    SessionDonationCount = donateCount;
                }

            }
        }
    }

}
