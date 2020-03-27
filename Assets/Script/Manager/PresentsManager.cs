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

    }
}
