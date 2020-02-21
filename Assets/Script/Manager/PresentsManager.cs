using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PresentsManager : SingletonMonoBehaviour<PresentsManager>
{

    List<Present> m_presentsList = new List<Present>();
    GameObjectPool<Present> m_PresentsPool;
    GameObject m_presents;
    [SerializeField] Transform dropPosition;

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
            var _present = m_PresentsPool.pop();
            Vector3 m_randPos = RandomPosition();
            _present.SetSprite(value);
            _present.transform.position = m_randPos;
            _present.gameObject.SetActive(true);
            m_presentsList.Add(_present);
        }
    }

    Vector3 RandomPosition()
    {
        float posX = Util.Rand(dropPosition.localPosition.x - dropPosition.localScale.x / 2, dropPosition.localPosition.x + dropPosition.localScale.x / 2);
        float posY = Util.Rand(dropPosition.localPosition.y - dropPosition.localScale.y / 2, dropPosition.localPosition.y + dropPosition.localScale.y / 2);
        float posZ = Util.Rand(dropPosition.localPosition.z - dropPosition.localScale.z / 2, dropPosition.localPosition.z + dropPosition.localScale.z / 2);
        return new Vector3(posX, posY, posZ);
    }

    // Start is called before the first frame update
    override protected void OnStart()
    {
        base.OnStart();
        m_presents = Resources.Load("Prefab/Present") as GameObject;
        m_PresentsPool = new GameObjectPool<Present>(5, () =>
        {
            var obj = Instantiate(m_presents) as GameObject;
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            var presents = obj.GetComponent<Present>();
            return presents;
        });
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(dropPosition.position.x, dropPosition.position.y - dropPosition.localScale.y * 5), new Vector3(dropPosition.localScale.x, dropPosition.localScale.y * 10));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha6))
            {
                Debug.Log("1");
                CreatePresents(1000);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("2");
                CreatePresents(1000);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("3");
                CreatePresents(1000);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Debug.Log("4");
                CreatePresents(1000);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Debug.Log("5");
                CreatePresents(1000);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            CreatePresents(1000);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            CreatePresents(5000);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            CreatePresents(10000);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            CreatePresents(50000);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            CreatePresents(100000);
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.End))
        {

        }
#endif

    }
}
