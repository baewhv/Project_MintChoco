using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : SingletonMonoBehaviour<EffectManager>
{
    List<Effect> m_effectList = new List<Effect>();
    List<Effect> m_goldbarList = new List<Effect>();
    GameObjectPool<Effect> m_EffectPool;
    GameObjectPool<Effect> m_GoldBarPool;
    GameObject EffectObj;
    GameObject GoldBarObj;
    GameObject EffectPoolObject;
    [SerializeField] GameObject Certain;

    int ActiveEffects;

    public void CreateEffect(Vector3 Pos, int i)
    {
        if(ActiveEffects >= 5)
        {
            return;
        }
        var _effect = m_EffectPool.pop();
        _effect.transform.position = new Vector3(Pos.x, Pos.y, -10);
        _effect.gameObject.SetActive(true);
        _effect.SetTimer(5);
        m_effectList.Add(_effect);
        ActiveEffects++;
    }

    public void RemoveEffect(Effect effect)
    {
        if (m_effectList.Remove(effect))
        {
            effect.gameObject.SetActive(false);
            m_EffectPool.push(effect);
            ActiveEffects--;
        }
    }

    override protected void OnAwake()
    {
        base.OnAwake();
        EffectObj = Resources.Load("Prefab/Effect/CandyEffect") as GameObject;
        GoldBarObj = Resources.Load("Prefab/Effect/E_SpotLight") as GameObject;
        
        EffectPoolObject = transform.Find("EffectPool").gameObject;
    }

    override protected void OnStart()
    {
        base.OnStart();
        m_EffectPool = new GameObjectPool<Effect>(5, () =>
        {
            var obj = Instantiate(EffectObj) as GameObject;
            obj.SetActive(false);
            obj.transform.SetParent(EffectPoolObject.transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            
            return obj.GetComponent<Effect>();
        });

        m_GoldBarPool = new GameObjectPool<Effect>(5, () =>
        {
            var obj = Instantiate(GoldBarObj) as GameObject;
            obj.SetActive(false);
            obj.transform.SetParent(EffectPoolObject.transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;

            return obj.GetComponent<Effect>();
        });

        Certain.SetActive(false);
    }

    public void SetGoldBar(float PosX)
    {
        if (CheckGoldBarEff() < 5)
        {
            var _goldBar = m_GoldBarPool.pop();
            _goldBar.transform.position = new Vector3(PosX, 0, -10);
            _goldBar.gameObject.SetActive(true);
            _goldBar.SetTimer(15);
            m_effectList.Add(_goldBar);
        }

        if(Certain.activeSelf)
        {
            Certain.GetComponent<Animation>().Rewind();
            Certain.GetComponent<Animation>().Play();
            Certain.GetComponent<CertainTimer>().ResetTimer();
        }
        else
        {
            Certain.SetActive(true);
            Certain.GetComponent<CertainTimer>().SetTimer(15);
        }
    }

    public void RemoveGoldBar(Effect effect)
    {
        if (m_effectList.Remove(effect))
        {
            GoldBarObj.gameObject.SetActive(false);
            m_GoldBarPool.push(effect);
        }
    }

    int CheckGoldBarEff()
    {
        int countActive = 0;
        for (int i = 0; i < m_goldbarList.Count; i++)
        {
            if(m_goldbarList[i].gameObject.activeSelf)
            {
                countActive++;
            }
        }
        return countActive;
    }

    void Update()
    {
        
    }
}
