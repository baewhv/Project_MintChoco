using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Present : MonoBehaviour
{
    SpriteRenderer body;
    public Sprite[] m_sprite;
    // Start is called before the first frame update
    [SerializeField] CapsuleCollider m_collider = null;
    [SerializeField] Rigidbody m_rigidbody = null;
    PresentsManager P_Manager;
    int speed;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag.Equals("DeadLine"))
        {
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
            PresentsManager.Instance.RemovePresents(this);
        }
    }
    public void SetExplosion(Vector3 CharacterV)
    {
        m_rigidbody.velocity = (transform.position - CharacterV) *10;
    }
    public void SetSprite(int Value)
    {
        if(Value >= 100000)
        {
            body.sprite = m_sprite[4];
            m_collider.radius = 0.55f;
            m_collider.height = 3.96f;


        }
        else if(Value >= 50000)
        {
            body.sprite = m_sprite[3];
            m_collider.radius = 0.55f;
            m_collider.height = 1.74f;

        }
        else if (Value >= 10000)
        {
            body.sprite = m_sprite[2];
            m_collider.radius = 0.55f;
            m_collider.height = 1.26f;
        }
        else if (Value >= 5000)
        {
            body.sprite = m_sprite[1];
            m_collider.radius = 0.5f;
            m_collider.height = 0.5f;
        }
        else if (Value >= 1000)
        {
            body.sprite = m_sprite[0];
            m_collider.radius = 0.3f;
            m_collider.height = 0.3f;

        }
        else
        {
            body.sprite = null;
        }
        
    }
   

    private void Awake()
    {
        P_Manager = GameObject.Find("PresentsManager").GetComponent<PresentsManager>();
        body = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<CapsuleCollider>();
        speed = 0;


    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += speed * Vector3.down * Time.deltaTime;
        m_rigidbody.velocity += Vector3.down * 2.5f * Time.deltaTime;
    }
}
