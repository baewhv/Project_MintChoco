using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureUI : MonoBehaviour
{
    List<GameObject> furnitureList = new List<GameObject>();
    [SerializeField] GridLayoutGroup m_funitureGrid = null;
    [SerializeField] GameObject m_furnitureConfig = null;
    [SerializeField] Text AllertMessage = null;

    GameObject m_furniture = null;
    GameObject m_addFurniture = null;
    DataManager DM = null;
    

    public int GetfurnitureListCount()
    {
        return furnitureList.Count;
    }


    public void CreateFurniture()
    {
        ClearList();
        AllertMessage.gameObject.SetActive(false);
        var obj = GameObject.Find("ObjectManager").GetComponent<ObjectManager>().m_ObjectList;
        for (int i = 0; i < obj.Count; i++)
        {
            if (obj[i].name != "Furniture_Background" && obj[i].name != "Furniture_TV")
            {
                CreateFurnitureIcon(obj[i].name, obj[i]);
            }
        }
        CreateAddFurniture();
    }

    public void CreateFurnitureIcon(string optionKey, GameObject _obj)
    {
        var obj = Instantiate(m_furniture) as GameObject;
        obj.transform.SetParent(m_funitureGrid.transform);
        obj.transform.localScale = Vector3.one;
        obj.name = optionKey;
        obj.GetComponent<Furnituer_Item>().SetPanel(optionKey, _obj);
        obj.GetComponent<Button>().onClick.AddListener(()=> { obj.GetComponent<Furnituer_Item>().OpenFurnitureConfig(m_furnitureConfig); });
        furnitureList.Add(obj);
    }

    public void CreateSpriteList()
    {
        ClearList();
        Sprite[] spr = DM.GetAllImage();
        CreateReturnFurniture();
        if (spr.Length != 0)
        {
            for (int i = 0; i < spr.Length; i++)
            {
                CreateSpriteIcon(spr[i]);
            }
        }
        else
        {
            AllertMessage.gameObject.SetActive(true);
            AllertMessage.text = "사진이 없어요ㅠ";
        }
    }

    public void CreateSpriteIcon(Sprite spr)
    {
        var obj = Instantiate(m_furniture) as GameObject;
        obj.transform.SetParent(m_funitureGrid.transform);
        obj.transform.localScale = Vector3.one;
        obj.name = spr.name;
        obj.GetComponent<Furnituer_Item>().SetPanel(spr);
        obj.GetComponent<Button>().onClick.AddListener(() => { obj.GetComponent<Furnituer_Item>().OpenFurnitureAdd(m_furnitureConfig, spr); });
        furnitureList.Add(obj);
    }

    public void CreateAddFurniture()
    {
        var obj = Instantiate(m_addFurniture) as GameObject;
        obj.transform.SetParent(m_funitureGrid.transform);
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<Button>().onClick.AddListener(() => { OpenSpriteList(); });
        furnitureList.Add(obj);
    }

    public void CreateReturnFurniture()
    {
        var obj = Instantiate(m_addFurniture) as GameObject;
        obj.transform.SetParent(m_funitureGrid.transform);
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<Button>().onClick.AddListener(() => { ReturnToFurnitureList(); });
        Util.FindChildObject(Util.FindChildObject(obj, "Background"), "Text").GetComponent<Text>().text = "취소";
        furnitureList.Add(obj);
    }

    public void ClearList()
    {
        for (int i = 0; i < furnitureList.Count; i++)
        {
            Destroy(furnitureList[i]);
        }
        furnitureList.Clear();
    }

    public void OpenSpriteList()
    {
        CreateSpriteList();
    }

    public void ReturnToFurnitureList()
    {
        CreateFurniture();
    }

    void Awake()
    {
        m_furniture = Resources.Load("Prefab/UI/Furniture_Icon") as GameObject;
        m_addFurniture = Resources.Load("Prefab/UI/Furniture_Add") as GameObject;
        SetFurniturePannel();
    }
    public void SetFurniturePannel()
    {
        AllertMessage.gameObject.SetActive(false);
        m_furnitureConfig.SetActive(false);
    }

    private void Start()
    {
        DM = GameObject.Find("DataManager").GetComponent<DataManager>();
    }
}
