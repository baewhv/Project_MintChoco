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
    int speed;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag.Equals("DeadLine"))
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
            m_rigidbody.centerOfMass = Vector3.zero;
            specPosition = Vector3.zero;
            SimpleDonateEffect.SetActive(false);
            DonateRayEffect.SetActive(false);
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
            m_rigidbody.velocity = (transform.position - CharacterV) * 10;
        }
    }
    public void SetPresent(Vector3 pos, Vector3 rot, Vector3 size, Sprite image, PresentInfo.PRESENT_TYPE type, int value)
    {
        m_image.transform.localPosition = pos;
        m_image.transform.localRotation = Quaternion.Euler(rot);
        m_image.transform.localScale = size;
        m_image.sprite = image;
        switch(type)
        {
            case PresentInfo.PRESENT_TYPE.CandyBall:
                m_collider.size = new Vector3(0.25f, 0.25f, 0.2f);
                break;
            case PresentInfo.PRESENT_TYPE.Candy:
                m_collider.size = new Vector3(1f, 0.4f, 0.2f);
                break;
            case PresentInfo.PRESENT_TYPE.ThinCandy:
                m_collider.size = new Vector3(1.1f, 0.3f, 0.2f);
                break;
            case PresentInfo.PRESENT_TYPE.Small:
                m_collider.size = new Vector3(1f, 1f, 0.2f);
                break;
            case PresentInfo.PRESENT_TYPE.Small_Macaroon:
                m_collider.size = new Vector3(1f, 0.7f, 0.2f);
                break;
            case PresentInfo.PRESENT_TYPE.Long:
                m_collider.size = new Vector3(1f, 1.7f, 0.2f);
                break;
            case PresentInfo.PRESENT_TYPE.Wide:
                m_collider.size = new Vector3(2f, 1.4f, 0.2f);
                break;
            case PresentInfo.PRESENT_TYPE.Big:
                m_collider.size = new Vector3(2f, 2.5f, 0.2f);
                break;
            case PresentInfo.PRESENT_TYPE.Huge:
                m_rigidbody.mass = 10;
                m_collider.size = new Vector3(1.3f, 1.3f, 0.2f);
                break;
            case PresentInfo.PRESENT_TYPE.Long_Coke:
                m_collider.size = new Vector3(0.8f, 2.6f, 0.2f);
                break;
        }
        if(value >= 50000)
        {
            SimpleDonateEffect.SetActive(true);
            //effect a
            if (value >= 100000)
            {
                DonateRayEffect.SetActive(true);
                //effect b
                specAnim.Play();
                m_collider.enabled = false;
                specPosition = new Vector3(transform.position.x, transform.position.y - 7, transform.position.z);
            }
        }
    }

    public void Anim_EndSpecialAnim()
    {
        m_collider.enabled = true;
        specAnim.Rewind();
        specAnim.Stop();
        m_image.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        Debug.Log("enter here" + name);
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
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += speed * Vector3.down * Time.deltaTime;
        if (specAnim.isPlaying)
        {
            transform.position = Vector3.Lerp(transform.position, specPosition, Time.deltaTime);
        }
        else
        {
            m_rigidbody.velocity += Vector3.down * 2.5f * Time.deltaTime;
        }

    }
}
