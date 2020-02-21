using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text.RegularExpressions;

public class DonateManager : SingletonMonoBehaviour<DonateManager>
{
    int SessionDonationCount;
    int RecentDonateAmountTextAccessTime;
    string sessionDonationCountTextRocation;
    string recentDonateAmountTextRocation;
    string recentDonatorsTextRocation;
    bool canDonate = false;
    PresentsManager m_presentManger;
    TVMassege m_massege;

    //현재 세션에서 받은 도네 횟수 카운트.
    int ReadSessionDonationCount()
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

    void GetDonate()
    {
        FileInfo file = new FileInfo(recentDonateAmountTextRocation);
        if (file.Exists)
        {
            TextReader sr = File.OpenText(recentDonateAmountTextRocation);
            int j = 0;
            if (int.TryParse(Regex.Replace(sr.ReadToEnd(), @"\D", ""), out j))
            {
                m_presentManger.CreatePresents(j);
            }
            //Debug.Log("RecentDonateAmountTextRocation : " + Regex.Replace(sr.ReadToEnd(), @"\D", ""));
            sr.Close();
        }
        else
        {
            Debug.Log("해당 파일이 없습니다. recentDonateAmountTextRocation");
        }
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
                        m_massege.setText(texts[i*2], j);
                        m_presentManger.CreatePresents(j);
                    }
                }
                //Debug.Log("RecentDonateAmountTextRocation : " + int.Parse(Regex.Replace(texts[i*2+1], @"\D", "")));
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
        if (File.Exists(Application.streamingAssetsPath + "/Config.txt"))
        {
            m_massege.setText("Config txt파일을 찾았어요!");
            StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/Config.txt");
            string str = sr.ReadToEnd();
            
            DirectoryInfo DI = new DirectoryInfo(str);
            m_massege.setText("Config 파일에 적힌 폴더 주소가 잘 연결됬는지 볼게요...");
            if(DI.Exists)
            {
                m_massege.setText("폴더를 찾았어요!");
            }
            else
            {
                m_massege.setText("폴더를 못찾았어요...");
                m_massege.setText("현재 찾고있는 폴더 이름 :" + str);
                return null;
            }
            sr.Close();
            canDonate = true;
            return str;
        }
        else
        {
            canDonate = false;
        }
        return "";
    }

    bool CheckConfigFolder()
    {
        FileInfo file = new FileInfo(sessionDonationCountTextRocation);
        m_massege.setText("session_donation_count.txt 파일을 찾아봅니다...");
        if (file.Exists)
        {
            m_massege.setText("파일을 찾았어요!");
        }
        else
        {
            m_massege.setText("파일을 못찾았어요...");
            return false;
        }
        file = new FileInfo(recentDonateAmountTextRocation);
        m_massege.setText("most_recent_donate_amount.txt 파일을 찾아봅니다...");
        if (file.Exists)
        {
            m_massege.setText("파일을 찾았어요!");
        }
        else
        {
            m_massege.setText("파일을 못찾았어요...");
            return false;
        }
        file = new FileInfo(recentDonatorsTextRocation);
        m_massege.setText("most_recent_donators.txt 파일을 찾아봅니다...");
        if (file.Exists)
        {
            m_massege.setText("파일을 찾았어요!");
        }
        else
        {
            m_massege.setText("파일을 못찾았어요...");
            return false;
        }
        m_massege.setText("도네를 받을 준비가 되었어요!");
        return true;
    }

    override protected void OnStart()
    {
        base.OnStart();
        m_presentManger = GameObject.Find("PresentsManager").GetComponent<PresentsManager>();
        m_massege = GameObject.Find("TVMessage").GetComponent<TVMassege>();
        string path = ReadDefaultPath();
        sessionDonationCountTextRocation = path + @"\session_donation_count.txt";
        recentDonateAmountTextRocation = path + @"\most_recent_donate_amount.txt";
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

    IEnumerator CoroutineTest()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);

        }
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
                    //GetDonate();
                    GetDonates(donateCount - SessionDonationCount);
                    SessionDonationCount = donateCount;

                }
                //else if (donateCount - SessionDonationCount > 1) //2개 이상의 도네가 와서 
                //{
                //    GetDonates(donateCount - SessionDonationCount);
                //    SessionDonationCount = donateCount;
                //}
            }
        }
    }

}
