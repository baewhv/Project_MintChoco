using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Present : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_image = null;
    // Start is called before the first frame update
    [SerializeField] BoxCollider m_collider = null;
    [SerializeField] Rigidbody m_rigidbody = null;
    int speed;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag.Equals("DeadLine"))
        {
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
            m_rigidbody.centerOfMass = Vector3.zero;
            PresentsManager.Instance.RemovePresents(this);
        }
    }
    public void SetExplosion(Vector3 CharacterV)
    {
        m_rigidbody.velocity = (transform.position - CharacterV) *10;
    }
    public void SetPresent(Vector3 pos, Vector3 rot, Vector3 size, Sprite image, PresentInfo.PRESENT_TYPE type)
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
                m_rigidbody.drag = 10;
                m_collider.size = new Vector3(2f, 2.8f, 0.2f);
                break;
            case PresentInfo.PRESENT_TYPE.Long_Coke:
                m_collider.size = new Vector3(0.8f, 2.6f, 0.2f);
                break;
        }
    }
   

    private void Awake()
    {
        m_collider = GetComponent<BoxCollider>();
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
