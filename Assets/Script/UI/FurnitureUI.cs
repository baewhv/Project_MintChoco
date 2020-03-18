using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureUI : MonoBehaviour
{
    List<GameObject> furnitureList = new List<GameObject>();
    [SerializeField] GridLayoutGroup m_furnitureGrid = null;
    [SerializeField] VerticalLayoutGroup m_furnitureList = null;
    [SerializeField] GameObject m_furnitureConfig = null;
    [SerializeField] Text AllertMessage = null;

    GameObject m_furnitureIcon = null;
    GameObject m_furniturelist = null;
    GameObject m_addFurniture = null;
   
    

    //public int GetfurnitureListCount()
    //{
    //    return furnitureList.Count;
    //}


    public void CreateFurniture()
    {
        ClearList();
        m_furnitureList.gameObject.SetActive(true);
        m_furnitureGrid.gameObject.SetActive(false);
        GetComponent<ScrollRect>().content = (RectTransform)m_furnitureList.transform;
        AllertMessage.gameObject.SetActive(false);
        var obj = ObjectManager.Instance.m_ObjectList;
        for (int i = 0; i < obj.Count; i++)
        {
            if (obj[i].name != "Furniture_Background")
            {
                CreateFurnitureListIcon(obj[i].name, obj[i]);
            }
        }
        CreateCharacterPosition();
        CreateAddFurniture();
        SortFurniture();
    }

    public void CreateFurnitureListIcon(string optionKey, GameObject _obj)
    {
        var obj = Instantiate(m_furniturelist) as GameObject;
        obj.transform.SetParent(m_furnitureList.transform);
        obj.transform.localScale = Vector3.one;
        obj.name = optionKey;
        obj.GetComponent<Furniture_List>().SetPanel(optionKey, _obj);
        obj.GetComponent<Furniture_List>().UpButton.onClick.AddListener(() => { _ListUpButton(obj); });
        obj.GetComponent<Furniture_List>().DownButton.onClick.AddListener(() => { _ListDownButton(obj); });
        obj.GetComponent<Button>().onClick.AddListener(()=> { obj.GetComponent<Furniture_List>()._OpenFurnitureConfig(m_furnitureConfig); });
        furnitureList.Add(obj);
    }

    public void CreateSpriteList()
    {
        ClearList();
        m_furnitureGrid.gameObject.SetActive(true);
        m_furnitureList.gameObject.SetActive(false);
        GetComponent<ScrollRect>().content = (RectTransform)m_furnitureGrid.transform;
        Sprite[] spr = DataManager.Instance.GetAllImage();
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
        var obj = Instantiate(m_furnitureIcon) as GameObject;
        obj.transform.SetParent(m_furnitureGrid.transform);
        obj.transform.localScale = Vector3.one;
        obj.name = spr.name;
        obj.GetComponent<Furnituer_Item>().SetPanel(spr);
        obj.GetComponent<Button>().onClick.AddListener(() => { obj.GetComponent<Furnituer_Item>().OpenFurnitureAdd(m_furnitureConfig, spr); });
        furnitureList.Add(obj);
    }

    public void CreateAddFurniture()
    {
        var obj = Instantiate(m_addFurniture) as GameObject;
        obj.name = m_addFurniture.name;
        obj.transform.SetParent(m_furnitureList.transform);
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<Button>().onClick.AddListener(() => { _OpenSpriteList(); });
        furnitureList.Add(obj);
    }

    public void CreateCharacterPosition()
    {
        var obj = Instantiate(m_furniturelist) as GameObject;
        obj.transform.SetParent(m_furnitureList.transform);
        obj.transform.localScale = Vector3.one;
        obj.name = "캐릭터";
        Sprite spr = Resources.Load<Sprite>("Texture/BBangDDuck");
        obj.GetComponent<Furniture_List>().SetPanel(obj.name , spr);
        obj.GetComponent<Furniture_List>().UpButton.interactable = false;
        obj.GetComponent<Furniture_List>().DownButton.interactable = false;
        furnitureList.Add(obj);
    }

    public void CreateReturnFurniture()
    {
        var obj = Instantiate(m_addFurniture) as GameObject;
        obj.transform.SetParent(m_furnitureGrid.transform);
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<Button>().onClick.AddListener(() => { _ReturnToFurnitureList(); });
        Util.FindChildObject(Util.FindChildObject(obj, "Background"), "Text").GetComponent<Text>().text = "취소";
        furnitureList.Add(obj);
    }

    public void ClearList()
    {
        for (int i = 0; i < furnitureList.Count; i++)
        {
            DestroyImmediate(furnitureList[i]);
        }
        furnitureList.Clear();
    }

    public void _OpenSpriteList()
    {
        CreateSpriteList();
    }

    public void _ReturnToFurnitureList()
    {
        CreateFurniture();
    }

    public void _ListUpButton(GameObject obj)
    {
        for (int i = 0; i < furnitureList.Count; i++)
        {
            if (obj == furnitureList[i])
            {
                int sibling = furnitureList[i].transform.GetSiblingIndex();
                if (sibling - 1 < 0)
                {
                    return;
                }
                else if (sibling - 1 == furnitureList.Count - ObjectManager.Instance.GetBackCanvasCount() - 1)
                {
                    ObjectManager.Instance.ObjectSwitchPosition(obj.GetComponent<Furniture_List>().LinkedObject);
                }
                else
                {
                    ObjectManager.Instance.ObjectMoveUp(obj.GetComponent<Furniture_List>().LinkedObject);
                }
                furnitureList[i].transform.SetSiblingIndex(sibling - 1);
                
            }
        }
    }
    public void _ListDownButton(GameObject obj)
    {
        for (int i = 0; i < furnitureList.Count; i++)
        {
            if (obj == furnitureList[i])
            {
                int sibling = furnitureList[i].transform.GetSiblingIndex();
                if (sibling + 1 > furnitureList.Count -2)
                {
                    return;
                }
                else if (sibling + 1 == furnitureList.Count - ObjectManager.Instance.GetBackCanvasCount() - 1)
                {
                    ObjectManager.Instance.ObjectSwitchPosition(obj.GetComponent<Furniture_List>().LinkedObject);
                }
                else
                {
                    ObjectManager.Instance.ObjectMoveDown(obj.GetComponent<Furniture_List>().LinkedObject);
                }
                furnitureList[i].transform.SetSiblingIndex(sibling + 1);
            }
        }
    }
    void SortFurniture()
    {
        for (int  i= 0; i < furnitureList.Count; i++)
        {

            if (furnitureList[i].name == "캐릭터")
            {
                furnitureList[i].transform.SetSiblingIndex(furnitureList.Count - (ObjectManager.Instance.GetBackCanvasCount() + 1));      //-1은 백그라운드가 안들어오기 때문.
                furnitureList[i].SetActive(true);
                continue;
            }
            else if(furnitureList[i].name == m_addFurniture.name)
            {
                furnitureList[i].transform.SetAsLastSibling();
                continue;
            }
            furnitureList[i].transform.SetAsFirstSibling();
            var targetObj = furnitureList[i].GetComponent<Furniture_List>().LinkedObject.GetComponent<FurnitureObject>();
            int sortNum;
            if (targetObj.GetComponent<FurnitureObject>().frontOrder)
            {
                sortNum = furnitureList.Count - (ObjectManager.Instance.GetBackCanvasCount() + targetObj.m_sort + 1);
            }
            else
            {
                sortNum = furnitureList.Count - (targetObj.m_sort + 1);
            }
            furnitureList[i].transform.SetSiblingIndex(sortNum);
            furnitureList[i].SetActive(true);
        }
    }

    void Awake()
    {
        m_furnitureIcon = Resources.Load("Prefab/UI/Furniture_Icon") as GameObject;
        m_furniturelist = Resources.Load("Prefab/UI/Furniture_List") as GameObject;
        m_addFurniture = Resources.Load("Prefab/UI/OptionList_Add") as GameObject;
        m_furnitureGrid.gameObject.SetActive(false);
        m_furnitureList.gameObject.SetActive(false);
        SetFurniturePannel();
    }

    public void SetFurniturePannel()
    {
        AllertMessage.gameObject.SetActive(false);
        m_furnitureConfig.SetActive(false);
    }
}
