using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public SpriteRenderer body;
    
    [SerializeField] Sprite[] emotion = null;
    [SerializeField] GameObject Weapon = null;
    [SerializeField] float walkSpeed = 10;
    [SerializeField] Vector3 DefaultPosition = Vector3.zero;
    Animator m_anim;
    SpriteRenderer m_Mouth;
    SpriteRenderer m_Eye;


    MicManager Mic;

    float LeftEndZone = -9.0f;
    float RightEndZone = 9.0f;

    //bool isWalk = false;
    //bool isTalk = false;
    bool isDown;
    bool canJump;
    bool canMove; 
    bool equipWeapon; 

    Vector3 m_dir = Vector3.zero;

    Rigidbody m_rigidbody;

    void initProperty()
    {
        //isWalk = false;
        //isTalk = false;
        canMove = true;
        canJump = true;
        equipWeapon = false;
        transform.position = DefaultPosition;
        body.sprite = emotion[0];
        Weapon.SetActive(false);
        m_Mouth = transform.Find("Mouth").GetComponent<SpriteRenderer>();
        m_Eye = transform.Find("Eye").GetComponent<SpriteRenderer>();
        m_rigidbody = GetComponent<Rigidbody>();
        GameObject micGO = GameObject.Find("MicManager");
        Mic = micGO.GetComponent<MicManager>();
        m_anim = GetComponent<Animator>();

    }

    void Start()
    {
        initProperty();
        StartBlink();
    }

    public void StartBlink()
    {
        StartCoroutine(EyeBlink());
    }

    public void ShowFace()
    {
        StartBlink();
        m_Eye.gameObject.SetActive(true);
    }
    public void HideFace()
    {
        StopCoroutine(EyeBlink());
        m_Eye.gameObject.SetActive(false);
    }

    IEnumerator EyeBlink()
    {
        while (true)
        {
            yield return new WaitForSeconds(Util.Rand(3, 5));
            m_anim.SetTrigger("T_Blink"); 
        }
    }

    
    void Jump()
    {
        
        canJump = false;
        m_rigidbody.AddForce(Vector3.up * 15f, ForceMode.Impulse);
    }
    void SetWeapon()
    {
        if (equipWeapon)
        {
            equipWeapon = false;
            Weapon.SetActive(false);
        }
        else if (!equipWeapon)
        {
            equipWeapon = true;
            Weapon.SetActive(true);
        }
    }

    

    public void SetChromaKeyMode()
    {
        Debug.Log(canJump + " Set Chrome " + m_rigidbody.velocity);
        canMove = false;
        canJump = false;
        equipWeapon = true;
        SetWeapon();
        if (body.flipX)
        {
            Weapon.GetComponent<SpriteRenderer>().flipX = false;
            body.flipX = false;
            m_Eye.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            m_Mouth.flipX = false;
            SetFlip();
        }
        body.sprite = emotion[0];
        m_dir = Vector3.zero;
        m_rigidbody.useGravity = false;
        m_rigidbody.velocity = Vector3.zero;
        transform.position = Vector3.zero;
    }
    public void DisableChromaKeyMode()
    {
        canMove = true;
        canJump = true;
        m_rigidbody.useGravity = true;
        transform.position = DefaultPosition;
    }
    void SetFlip()
    {
        Weapon.transform.localPosition = new Vector3(-Weapon.transform.localPosition.x, Weapon.transform.localPosition.y, 0.0f);
        Weapon.transform.localRotation = new Quaternion(0, 0, -Weapon.transform.localRotation.z, Weapon.transform.localRotation.w);
        m_Eye.gameObject.transform.localPosition = new Vector3(-m_Eye.gameObject.transform.localPosition.x, m_Eye.gameObject.transform.localPosition.y, 0.0f);
        m_Mouth.gameObject.transform.localPosition = new Vector3(-m_Mouth.gameObject.transform.localPosition.x, m_Mouth.gameObject.transform.localPosition.y, 0.0f);
    }


    void M_Input()
    {
        if (!canMove)
        {
            return;
        }

        if ( 1 == Input.GetAxisRaw("Horizontal") )
        {
            if (!body.flipX)
            {
                Weapon.GetComponent<SpriteRenderer>().flipX = true;
                body.flipX = true;
                m_Eye.gameObject.GetComponent<SpriteRenderer>().flipX = true;
                m_Mouth.flipX = true;
                SetFlip();
            }

            m_dir = Vector3.right;
        }
        else if(-1 == Input.GetAxisRaw("Horizontal"))
        {
            if (body.flipX)
            {
                Weapon.GetComponent<SpriteRenderer>().flipX = false;
                body.flipX = false;
                m_Eye.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                m_Mouth.flipX = false;
                SetFlip();
            }
            m_dir = Vector3.left;
        }
        else
        {
            m_dir = Vector3.zero;
        }

        if (0 < Input.GetAxisRaw("Vertical") && canJump && m_rigidbody.velocity.y >= 0 && canMove && !Input.GetKeyDown(KeyCode.Semicolon))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SetWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            body.sprite = emotion[0];
            ShowFace();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            body.sprite = emotion[1];
            HideFace();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            body.sprite = emotion[2];
            HideFace();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            body.sprite = emotion[3];
            HideFace();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            body.sprite = emotion[4];
            HideFace();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            body.sprite = emotion[5];
            HideFace();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            body.sprite = emotion[6];
            HideFace();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            body.sprite = emotion[7];
            HideFace();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            body.sprite = emotion[8];
            HideFace();
        }
    }
    void MicInput()
    {
        if (Mic.GetDecibels() > -100)
        {
            m_anim.SetTrigger("T_Mouth");
        }
    }


    void OnTriggerEnter(Collider collision)     //점프 후 착륙 함수
    {
        if (collision.tag.Equals("Wall") && isDown)
        {
            isDown = false;
            canJump = true;
        }
    }
 

    // Update is called once per frame
    void Update()
    {
        M_Input();
        MicInput();
        if (!canJump)
        {
            if (m_rigidbody.velocity.y < 0)
            {
                isDown = true;
            }
        }
        if ((transform.position + walkSpeed * m_dir * Time.deltaTime).x < RightEndZone && (transform.position + walkSpeed * m_dir * Time.deltaTime).x > LeftEndZone && canMove)
        {
            transform.position += walkSpeed * m_dir * Time.deltaTime;
        }
    }
}
