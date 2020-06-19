using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Present : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_image = null;
    // Start is called before the first frame update
    [SerializeField] BoxCollider m_collider = null;
    [SerializeField] Rigidbody m_rigidbody = null;
    Animation specAnim = null;
    Vector3 specPosition;
    GameObject SimpleDonateEffect;
    GameObject DonateRayEffect;
    GameObject GoldBar;
    int speed;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag.Equals("DeadLine"))
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
            m_rigidbody.centerOfMass = Vector3.zero;
            m_rigidbody.useGravity = true;
            specPosition = Vector3.zero;
            SimpleDonateEffect.SetActive(false);
            DonateRayEffect.SetActive(false);
            if(GoldBar != null)
            {
                DisableGoldBar();
            }
            PresentsManager.Instance.RemovePresents(this);
        }
    }


    public void TurnOffEffect()
    {
        SimpleDonateEffect.SetActive(false);
        DonateRayEffect.SetActive(false);
    }

    public void SetExplosion(Vector3 CharacterV)
    {
        if (!specAnim.isPlaying)
        {
            m_rigidbody.useGravity = false;
            //m_rigidbody.drag = 0;
            m_rigidbody.velocity = (transform.position - CharacterV) * 10;
        }
    }
    public void SetPresent(PresentInfo info, int value)
    {
        m_image.transform.localPosition = info.m_SpritePos;
        m_image.transform.localRotation = Quaternion.Euler(info.m_SpriteRot);
        m_image.transform.localScale = info.m_SpriteSize;
        m_image.sprite = info.m_sprite;

        Vector3 p_size = Vector3.zero;
        float p_mass = 0;
        info.ReturnColliderSize(out p_size, out p_mass);
        m_collider.size = p_size;
        m_rigidbody.mass = p_mass;

        if (value >= 50000)
        {
            if (ChromaKeyManager.Instance.GetisHide() == 0)
            {
                SimpleDonateEffect.SetActive(true);
            }
            if (value >= 100000)
            {
                if (ChromaKeyManager.Instance.GetisHide() == 0)
                {
                    DonateRayEffect.SetActive(true);
                }
                specAnim.Play();
                m_rigidbody.useGravity = false;
                m_collider.enabled = false;
                specPosition = new Vector3(transform.position.x, transform.position.y - 7, transform.position.z);
                if (value >= 1000000)
                {
                    SetGoldBar();
                    EffectManager.Instance.SetGoldBar(transform.position.x);
                }
            }
        }
    }

    void SetGoldBar()
    {
        GoldBar = Resources.Load("Prefab/Present/Gold") as GameObject;
        var gold = Instantiate(GoldBar) as GameObject;
        m_image.sprite = null;
        gold.transform.SetParent(m_image.transform);
        gold.transform.localPosition = Vector3.zero;
        GoldBar = gold;
        GoldBar.SetActive(true);
    }

    void DisableGoldBar()
    {
        Destroy(GoldBar.gameObject);
        GoldBar = null;
        
    }

    public void Anim_EndSpecialAnim()
    {
        if (SimpleDonateEffect.activeSelf)
        {
            m_collider.enabled = true;
            m_rigidbody.useGravity = true;
            specAnim.Rewind();
            specAnim.Stop();
            SimpleDonateEffect.SetActive(false);
            DonateRayEffect.SetActive(false);
            m_image.gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    private void Awake()
    {
        m_collider = GetComponent<BoxCollider>();
        specAnim = GetComponent<Animation>();

        SimpleDonateEffect = Resources.Load("Prefab/Effect/SimpleDonateEffect") as GameObject;
        var effA = Instantiate(SimpleDonateEffect) as GameObject;
        effA.transform.SetParent(transform);
        effA.transform.localPosition = Vector3.zero;
        SimpleDonateEffect = effA;
        SimpleDonateEffect.SetActive(false);

        DonateRayEffect = Resources.Load("Prefab/Effect/DonateRayEffect") as GameObject;
        var effB = Instantiate(DonateRayEffect) as GameObject;
        effB.transform.SetParent(transform);
        effB.transform.localPosition = Vector3.zero;
        DonateRayEffect = effB;
        DonateRayEffect.SetActive(false);

        speed = 0;
    }
    void Start()
    {
        //m_rigidbody.AddForce(1, 500, 1);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += speed * Vector3.down * Time.deltaTime;
        if (specAnim.isPlaying)
        {
            transform.position = Vector3.Lerp(transform.position, specPosition, Time.deltaTime);
        }
        else
        {
            //m_rigidbody.velocity += Vector3.down * 2.5f * Time.deltaTime;
            
        }
    }
    private void FixedUpdate()
    {
        if (!specAnim.isPlaying)
        {
            m_rigidbody.AddForce(Vector3.up * 33f * m_rigidbody.mass);
        }
    }
}
