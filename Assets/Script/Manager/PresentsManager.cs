using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PresentsManager : SingletonMonoBehaviour<PresentsManager>
{
    public PresentInfo[] m_presents;
    public PresentGroup[] m_presentGroup;


    int addedGroupNum = 0;
    

    public List<PresentInfo> m_presentsDataList = new List<PresentInfo>();
    public List<PresentGroup> m_presentGroupList = new List<PresentGroup>();

    //objectPool
    List<Present> m_presentsList = new List<Present>();
    GameObjectPool<Present> m_PresentsPool;
    GameObject m_presentPrefab;


    GameObject m_character;
    [SerializeField] Transform dropPosition = null;

    #region SocketIO to Donate
    Dictionary<string, DonateData> m_donateQueue = new Dictionary<string, DonateData>();

    public enum DONATE_TYPE
    {
        Donate = 0,
        Media,
        Sub,
        Hosting,
        Cheer
    }

    public struct DonateData
    {
        public int value;
        public DONATE_TYPE type;
    }

    float startTime = 0.0f;
    float delayTime = 10.0f;

    float alertDelayTime = 0.0f;
    float donationDuration = 0.0f;
    float subDuration = 0.0f;
    float hostingDuration = 0.0f;
    float cheerduration = 0.0f;
    bool doDonateToPresent;

    int testCurrentQueueCount = 0;

    public void SetDelayTimes(float _alertDelayTime, float _donationDuration, float _subDuration, float _hostingDuration, float _cheerduration)
    {
        alertDelayTime = _alertDelayTime;
        donationDuration = _donationDuration;
        subDuration = _subDuration;
        hostingDuration = _hostingDuration;
        cheerduration = _cheerduration;

    }
    public void AddDonateQueue(string _key, int _value, DONATE_TYPE _type)//통신 중 NewDonate명령이 들어올 때
    {
        if (!m_donateQueue.ContainsKey(_key))
        {
            DonateData data = new DonateData();
            data.value = _value;
            data.type = _type;
            m_donateQueue.Add(_key, data);
        }
    }
    public void DonateSetNow(string _key, bool _isKey = true)//통신 중 Now 명령이 들어올 때
    {
        if (_isKey)
        {
            SetDonateToPresent(_key);
        }
        else
        {
            SetDelayToPresent(_key);
        }
    }

    void DonateQueueActive()
    {
        if(!doDonateToPresent && m_donateQueue.Count !=0)   //쿨타임이 되면 자동으로 내보냄.
        {
            string firstKey = null;
            foreach(KeyValuePair<string, DonateData> pair in m_donateQueue)
            {
                firstKey = pair.Key;
                break;
            }
            SetDonateToPresent(firstKey);
        }

        if (doDonateToPresent)  //타이머 가동
        {
            startTime += Time.deltaTime;
            if(startTime >= delayTime)
            {
                doDonateToPresent = false;
                startTime = 0.0f;
            }
        }
    }

    void SetDonateToPresent(string _key)
    {
        if (m_donateQueue.ContainsKey(_key))
        {
            //Debug.Log("!make Present" + _key);
            CreatePresents(m_donateQueue[_key].value);
            startTime = 0.0f;
            SetOptions(m_donateQueue[_key].type, m_donateQueue[_key].value);
            doDonateToPresent = true;
            m_donateQueue.Remove(_key);
        }
    }
    void SetDelayToPresent(string _key)
    {
        string firstKey = null;
        foreach (KeyValuePair<string, DonateData> pair in m_donateQueue)
        {
            if (pair.Key.Contains(_key))
            {
                firstKey = pair.Key;
                break;
            }
        }
        if(firstKey != null)
        {
            SetDonateToPresent(firstKey);
        }
    }

    void SetOptions(DONATE_TYPE _type, int extraDelay) //딜레이만 있는 값을 설정해야 할 때.
    {
        switch (_type)
        {
            case DONATE_TYPE.Donate:
                delayTime = alertDelayTime + donationDuration;
                break;
            case DONATE_TYPE.Media:
                delayTime = extraDelay;
                break;
            case DONATE_TYPE.Sub:
                delayTime = alertDelayTime + subDuration;
                break;
            case DONATE_TYPE.Cheer:
                delayTime = alertDelayTime + subDuration;
                break;
            case DONATE_TYPE.Hosting:
                delayTime = alertDelayTime + hostingDuration;
                break;
        }
    }

    #endregion


    #region DropPresentFunction
    public bool testDropPresent = true;

    public void DisableSpecPresent()
    {
        for (int i = 0; i < m_presentsList.Count; i++)
        {
            if (m_presentsList[i].gameObject.activeSelf)
            {
                m_presentsList[i].Anim_EndSpecialAnim();
                m_presentsList[i].TurnOffEffect();
            }
        }
    }

    public void RemovePresents(Present present)
    {
        if(m_presentsList.Remove(present))
        {
            present.gameObject.SetActive(false);
            m_PresentsPool.push(present);
        }
    }

    public void CreatePresents(int value)
    {
        if (value >= 1000)
        {
            PresentGroup selectGroup = SelectPresentGroup(value);
            if(selectGroup == null)
            {
                return;
            }
            //check drop type ->비용에 맞춰 어떤 그룹을 선택할지.
            int addExtract = 1;
            if (selectGroup.m_addPrice != 0)
            {
                addExtract += ((value - selectGroup.m_price) / selectGroup.m_addPrice); //추가 사출
            }
            int dropCount = selectGroup.m_extractCount * addExtract;
            //그룹에서 몇개나 내보낼지
            for(int  i= 0; i < dropCount; i++)
            {
                int randCount = Random.Range(0, selectGroup.m_presentList.Length);
                CreatePresent(m_presents[selectGroup.m_presentList[randCount]], value);
            }

        }
    }
    public PresentGroup SelectPresentGroup(int value)
    {
        PresentGroup select = null;
        for(int i = 0; i < m_presentGroupList.Count; i++)
        {
            if(m_presentGroupList[i].m_isSame)
            {
                if(m_presentGroupList[i].m_price == value)
                {
                    return m_presentGroupList[i];
                }
            }
            else
            {
                if (m_presentGroupList[i].m_price <= value)
                {
                    if (select == null)
                    {
                        select = m_presentGroupList[i];
                        continue;
                    }
                    if (select.m_price < m_presentGroupList[i].m_price)
                    {
                        select = m_presentGroupList[i];
                    }
                }
            }
        }
        return select;
    }

    public void CreatePresent(PresentInfo selectPresent, int value)
    {
        var _present = m_PresentsPool.pop();
        Vector3 m_randPos = RandomPosition();
        _present.name = selectPresent.m_sprite.name;
        _present.transform.position = m_randPos;
        _present.SetPresent(selectPresent.m_SpritePos, selectPresent.m_SpriteRot, selectPresent.m_SpriteSize, selectPresent.m_sprite, selectPresent.m_PresentType, value);
        _present.gameObject.SetActive(true);
        m_presentsList.Add(_present);
    }

    Vector3 RandomPosition()
    {
        float posX = Util.Rand(dropPosition.localPosition.x - dropPosition.localScale.x / 2, dropPosition.localPosition.x + dropPosition.localScale.x / 2);
        float posY = Util.Rand(dropPosition.localPosition.y - dropPosition.localScale.y / 2, dropPosition.localPosition.y + dropPosition.localScale.y / 2);
        //float posZ = Util.Rand(dropPosition.localPosition.z - dropPosition.localScale.z / 2, dropPosition.localPosition.z + dropPosition.localScale.z / 2);
        return new Vector3(posX, posY, dropPosition.localPosition.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(dropPosition.position.x, dropPosition.position.y - dropPosition.localScale.y * 5), new Vector3(dropPosition.localScale.x, dropPosition.localScale.y * 10));
    }


    #endregion

    #region PresentDataFunction
    void GetDatas()
    {
        bool[] changeValue = { false, false };
        foreach (KeyValuePair<string, string> obj in DataManager.Instance.m_PresentGroupData)
        {
            var _obj = new Dictionary<string,string>();
            Util.ParseDictionary(obj.Value , out _obj);
            var newObj = new PresentGroup(obj.Key, _obj); 
            if(newObj.m_price == 100000)
            {
                if (newObj.m_presentList[0] == 19)
                {
                    int[] a = new int[1];
                    a[0] = 21;
                    newObj.m_presentList = a;
                    changeValue[0] = true;
                }
            }
            else if(newObj.m_price == 50000)
            {
                if (newObj.m_presentList[0] == 17)
                {
                    int[] a = new int[1];
                    a[0] = 20;
                    newObj.m_presentList = a;
                    changeValue[1] = true;
                }
            }
                m_presentGroupList.Add(newObj);
        }
        for(int i = 0; i < changeValue.Length; i++)
        {
            if(changeValue[i])
            {
                SavePresentGroupData();
                break;
            }
        }
    }

    public void SavePresentGroupData(PresentGroup obj)
    {
        DataManager.Instance.SaveValues(DataManager.Instance.m_PresentGroupData, obj.m_key, obj.ReturnValue());
        DataManager.Instance.SaveDatas(DataManager.Instance.m_PresentGroupData, DataManager.Instance.PresentGroupConfigTXT);
    }
    public void SavePresentGroupData()//save all
    {
        for(int  i= 0; i< m_presentGroupList.Count; i++)
        {
            DataManager.Instance.SaveValues(DataManager.Instance.m_PresentGroupData, m_presentGroupList[i].m_key, m_presentGroupList[i].ReturnValue());
        }
        DataManager.Instance.SaveDatas(DataManager.Instance.m_PresentGroupData, DataManager.Instance.PresentGroupConfigTXT);
    }
    public void AddPresentGroupData(PresentGroup obj)
    {
        DataManager.Instance.AddValues(DataManager.Instance.m_PresentGroupData, "presentGroup_add_" + addedGroupNum, obj.ReturnValue());
        DataManager.Instance.SaveDatas(DataManager.Instance.m_PresentGroupData, DataManager.Instance.PresentGroupConfigTXT);
    }
    public void DeletePresentGroupData(PresentGroup obj)
    {
        DataManager.Instance.DeleteValue(DataManager.Instance.m_FurnitureData, obj.m_key);
        DataManager.Instance.SaveDatas(DataManager.Instance.m_PresentGroupData, DataManager.Instance.PresentGroupConfigTXT);
    }

    #endregion

    override protected void OnAwake()
    {
        base.OnAwake();
        doDonateToPresent = false;
    }

    override protected void OnStart()
    {
        base.OnStart();
        GetDatas();
        m_character = GameObject.Find("Character");
        m_presentPrefab = Resources.Load("Prefab/Present/Present") as GameObject;
        m_PresentsPool = new GameObjectPool<Present>(5, () =>
        {
            var obj = Instantiate(m_presentPrefab) as GameObject;
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            var presents = obj.GetComponent<Present>();
            return presents;
        });
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (testDropPresent )
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                CreatePresents(1000);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                CreatePresents(5000);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                CreatePresents(10000);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                CreatePresents(50000);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                CreatePresents(100000);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                CreatePresents(1001);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                CreatePresents(1002);
            }
        }
#else
        if (testDropPresent && Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                CreatePresents(1000);
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                CreatePresents(5000);
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                CreatePresents(10000);
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                CreatePresents(50000);
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                CreatePresents(100000);
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                CreatePresents(1001);
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                CreatePresents(1002);
            }
        }
#endif

        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (Present P in m_presentsList)
            {
                if (P.gameObject.activeSelf)
                {
                    P.SetExplosion(new Vector3(m_character.transform.position.x, -5f));
                }
            }
        }
        DonateQueueActive();
        testCurrentQueueCount = m_donateQueue.Count;
    }
}
