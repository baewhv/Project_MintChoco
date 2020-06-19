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
    public Queue<int> m_DonateToPresentQ = new Queue<int>();

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

    //int testCurrentQueueCount = 0;

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
            m_DonateToPresentQ.Enqueue(m_donateQueue[_key].value);
            
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

    public void CreatePresentBundle(int value)
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

        Vector3 p_size = Vector3.zero;
        float p_mass = 0;
        selectPresent.ReturnColliderSize(out p_size, out p_mass);

        Vector3 m_randPos = RandomPosition(p_size, selectPresent.m_SpriteSize);
        if (selectPresent.m_sprite != null)
        {
            _present.name = selectPresent.m_sprite.name;
        }
        _present.transform.position = m_randPos;
        _present.SetPresent(selectPresent, value);
        _present.gameObject.SetActive(true);
        m_presentsList.Add(_present);
    }
    public void ResetDropPosition()
    {
        dropPosition.localPosition = new Vector3(-3.7f, dropPosition.localPosition.y, dropPosition.localPosition.z);
        dropPosition.localScale = new Vector3(9.8f, Instance.dropPosition.localScale.y, dropPosition.localScale.z);
        Debug.Log("set");
    }
    public void SetWideDropPosition()
    {
        dropPosition.localPosition = new Vector3(0f, dropPosition.localPosition.y, dropPosition.localPosition.z);
        dropPosition.localScale = new Vector3(16.0f, Instance.dropPosition.localScale.y, dropPosition.localScale.z);
        Debug.Log("unset");
    }

    Vector3 RandomPosition(Vector3 size, Vector3 sprSize)
    {
        int[] array = { -1,0, 1 };
        float posX = Util.Rand(dropPosition.localPosition.x - dropPosition.localScale.x / 2, dropPosition.localPosition.x + dropPosition.localScale.x / 2);
        float posY = Util.Rand(dropPosition.localPosition.y - dropPosition.localScale.y / 2, dropPosition.localPosition.y + dropPosition.localScale.y / 2);
        RaycastHit rayHit;
        bool checkOverlap = false;
        while(!checkOverlap)
        {
            int count = 4;
            for (int i = 0; i < array.Length; i++)
            {
                bool checkOver = false;
                if (!checkOver)
                {
                    for (int j = 0; j < array.Length; j++)
                    {
                        float rayX = posX + (size.x / 2 * array[i]);
                        float rayY = posY + (size.y / 2 * array[j]);

                        Debug.DrawRay(new Vector3(rayX, rayY, dropPosition.localPosition.z + 10), -transform.forward * 10.0f, Color.red, 1.0f);
                        if (Physics.Raycast(new Vector3(rayX, rayY, dropPosition.localPosition.z + 10), -transform.forward, out rayHit, 10.0f))
                        {
                            //Debug.Log(rayHit.transform.name);
                            checkOver = true;
                            break;
                        }
                        count--;
                    }
                }
                else
                {
                    break;
                }
            }

            if(count>0)
            {
                posY += 5;
                //Debug.Log("");
                continue;
            }
            checkOverlap = true;
        }
        
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
            var _obj = new Dictionary<string, string>();
            Util.ParseDictionary(obj.Value, out _obj);
            var newObj = new PresentGroup(obj.Key, _obj);
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
    bool isChromakeyMode = false;
    public void SetChromakeyMode(bool b)
    {
        isChromakeyMode = b;
    }

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
                CreatePresentBundle(1000);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                CreatePresentBundle(5000);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                CreatePresentBundle(10000);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                CreatePresentBundle(50000);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                CreatePresentBundle(100000);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                CreatePresentBundle(1001);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                CreatePresentBundle(1002);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                CreatePresentBundle(1000000);
            }
        }
#else
        if (testDropPresent && Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                CreatePresentBundle(1000);
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                CreatePresentBundle(5000);
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                CreatePresentBundle(10000);
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                CreatePresentBundle(50000);
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                CreatePresentBundle(100000);
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                CreatePresentBundle(1000000);
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                CreatePresentBundle(1001);
            }
            else if (Input.GetKeyDown(KeyCode.Comma))
            {
                CreatePresentBundle(1002);
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
        if(!isChromakeyMode && m_DonateToPresentQ.Count > 0)
        {
            CreatePresentBundle(m_DonateToPresentQ.Dequeue());
        }
        //testCurrentQueueCount = m_donateQueue.Count;
    }
}
