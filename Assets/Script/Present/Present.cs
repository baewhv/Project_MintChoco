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

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.collider.tag == "Present")
    //    {
    //        CollisionOverlaped++;
    //        Debug.Log(name + " to " + collision.collider.name  + " / " + CollisionOverlaped);
    //    }
        
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.collider.tag == "Present")
    //    {
    //        CollisionOverlaped--;
    //        Debug.Log(name + " to " + collision.collider.name + " / " + CollisionOverlaped);
    //    }
    //}

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
    public void SetPresent(PresentInfo info, int value)
    {
        m_image.transform.localPosition = info.m_SpritePos;
        m_image.transform.localRotation = Quaternion.Euler(info.m_SpriteRot);
        m_image.transform.localScale = info.m_SpriteSize;
        m_image.sprite = info.m_sprite;
        m_collider.size = info.ReturnColliderSize();
        if (value >= 50000)
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
        if (SimpleDonateEffect.activeSelf)
        {
            m_collider.enabled = true;
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
