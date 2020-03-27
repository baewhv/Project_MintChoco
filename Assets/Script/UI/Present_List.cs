using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Present_List : MonoBehaviour
{
    string KeyValue;
    [SerializeField] Image Icon = null;
    [SerializeField] Text objName = null;
    public PresentGroup m_PresentGroup;

    public void SetPanel(PresentGroup pg)
    {
        Icon.sprite = PresentsManager.Instance.m_presents[pg.m_presentList[0]].m_sprite;
        RectTransform rt = (RectTransform)Icon.transform;
        m_PresentGroup = pg;
        //objName.text = string.Format("{0}원 {1}도네", m_PresentGroup.m_price, m_PresentGroup.m_isSame ? "" : "이상 ");
        objName.text = pg.m_name;
        if (Icon.sprite.rect.size.x > Icon.sprite.rect.size.y)
        {
            rt.sizeDelta = new Vector2(100, (Icon.sprite.rect.size.y / Icon.sprite.rect.size.x) * 100);
        }
        else
        {
            rt.sizeDelta = new Vector2((Icon.sprite.rect.size.x / Icon.sprite.rect.size.y) * 100, 100);
        }
    }

    public void _OpenPresentConfig(GameObject obj , GameObject parent)
    {
        var _obj = Instantiate(obj) as GameObject;
        _obj.transform.SetParent(parent.transform);
        _obj.transform.localPosition = Vector3.zero;
        _obj.transform.localScale = Vector3.one;
        ((RectTransform)_obj.transform).sizeDelta = Vector2.zero;
        _obj.GetComponent<Present_Config>().SetPresentGroupConfig(m_PresentGroup);
        parent.GetComponent<PresentUI>().m_PresentConfigObject = _obj;
        _obj.SetActive(true);
    }
    public void _OpenPresentGroupAdd(GameObject obj)
    {
        //obj.GetComponent<Present_Config>().SetNewGroup();
        obj.SetActive(true);
    }
}
