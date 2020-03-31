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
    Dictionary<string, int> m_donateQueue = new Dictionary<string, int>();

    public float startTime = 0.0f;
    public float delayTime = 10.0f;
    public bool doDonateToPresent;
    public int currentDonateQueue = 0;

    public void SetDonateDelayTime(float _delayTime)
    {
        if(_delayTime != 0)
        {
            delayTime = _delayTime;
        }
    }
    public void AddDonateQueue(string _key, int _value)//통신 중 NewDonate명령이 들어올 때
    {
        if (!m_donateQueue.ContainsKey(_key))
        {
            m_donateQueue.Add(_key, _value);
        }
    }
    public void DonateSetNow(string _key)//통신 중 Now 명령이 들어올 때
    {
        //Debug.Log("나우");
        SetDonateToPresent(_key);
    }

    void DonateQueueActive()
    {
        if(!doDonateToPresent && m_donateQueue.Count !=0)
        {
            Debug.Log("도네받아라");
            string firstKey = null;
            foreach(KeyValuePair<string, int> pair in m_donateQueue)
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
        //Debug.Log("키 : " +  _key);
        if (m_donateQueue.ContainsKey(_key))
        {
            //Debug.Log("도네발사");
            CreatePresents(m_donateQueue[_key]);
            startTime = 0.0f;
            doDonateToPresent = true;
            m_donateQueue.Remove(_key);
        }
    }
    #endregion


    #region DropPresentFunction
    public bool testDropPresent = true;

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
                CreatePresent(m_presents[selectGroup.m_presentList[randCount]]);
            }

        }
    }
    public PresentGroup SelectPresentGroup(int value)
    {
        PresentGroup select = null;
        for(int i = 0; i < m_presentGroupList.Count; i++)
        {
            if(m_presentGroupList[i].m_price == value && m_presentGroupList[i].m_isSame)
            {
                return m_presentGroupList[i];
            }
            else if (m_presentGroupList[i].m_price <= value)
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
        return select;
    }

    public void CreatePresent(PresentInfo selectPresent)
    {
        var _present = m_PresentsPool.pop();
        Vector3 m_randPos = RandomPosition();
        _present.name = selectPresent.m_sprite.name;
        _present.SetPresent(selectPresent.m_SpritePos, selectPresent.m_SpriteRot, selectPresent.m_SpriteSize, selectPresent.m_sprite, selectPresent.m_PresentType);
        _present.transform.position = m_randPos;
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
        foreach (KeyValuePair<string, string> obj in DataManager.Instance.m_PresentGroupData)
        {
            var _obj = new Dictionary<string,string>();
            Util.ParseDictionary(obj.Value , out _obj);
            var newOjb = new PresentGroup(obj.Key, _obj);
            m_presentGroupList.Add(newOjb);
        }
    }

    public void SavePresentGroupData(PresentGroup obj)
    {
        DataManager.Instance.SaveValues(DataManager.Instance.m_PresentGroupData, obj.m_key, obj.ReturnValue());
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

        }

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
        currentDonateQueue = m_donateQueue.Count;
    }
}
