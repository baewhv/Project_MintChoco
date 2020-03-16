using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PresentsManager : SingletonMonoBehaviour<PresentsManager>
{
    public PresentInfo[] m_presents;
    public PresentGroup[] m_presentGroup;

    //objectPool
    List<Present> m_presentsList = new List<Present>();
    GameObjectPool<Present> m_PresentsPool;
    GameObject m_present;


    GameObject m_character;
    [SerializeField] Transform dropPosition = null;

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
                Debug.Log(randCount);
                CreatePresent(m_presents[selectGroup.m_presentList[randCount]]);
            }

        }
    }
    public PresentGroup SelectPresentGroup(int value)
    {
        PresentGroup select = null;
        for(int i = 0; i < m_presentGroup.Length; i++)
        {
            if(m_presentGroup[i].m_price == value && m_presentGroup[i].m_isSame)
            {
                return m_presentGroup[i];
            }
            else if (m_presentGroup[i].m_price <= value)
            {
                if (select == null)
                {
                    select = m_presentGroup[i];
                    continue;
                }
                if (select.m_price < m_presentGroup[i].m_price)
                {
                    select = m_presentGroup[i];
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

    
    override protected void OnStart()
    {
        base.OnStart();
        m_character = GameObject.Find("Character");
        m_present = Resources.Load("Prefab/Present/Present") as GameObject;
        m_PresentsPool = new GameObjectPool<Present>(5, () =>
        {
            var obj = Instantiate(m_present) as GameObject;
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            var presents = obj.GetComponent<Present>();
            return presents;
        });
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(dropPosition.position.x, dropPosition.position.y - dropPosition.localScale.y * 5), new Vector3(dropPosition.localScale.x, dropPosition.localScale.y * 10));
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

    }
}
