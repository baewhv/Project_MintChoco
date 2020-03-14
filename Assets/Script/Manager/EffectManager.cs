using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : SingletonMonoBehaviour<EffectManager>
{
    List<Effect> m_effectList = new List<Effect>();
    GameObjectPool<Effect> m_EffectPool;
    GameObject Effect;
    int ActiveEffects;

    public void CreateEffect(Vector3 Pos, int i)
    {
        if(ActiveEffects >= 5)
        {
            return;
        }
        var _effect = m_EffectPool.pop();
        _effect.transform.position = new Vector3(Pos.x, Pos.y, 5);
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
        Effect = Resources.Load("Prefab/Effect/CandyEffect") as GameObject;
    }

    override protected void OnStart()
    {
        base.OnStart();
        m_EffectPool = new GameObjectPool<Effect>(5, () =>
        {
            var obj = Instantiate(Effect) as GameObject;
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            
            return obj.GetComponent<Effect>();
        });
    }

        void Update()
    {
        
    }
}
