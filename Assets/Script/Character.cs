using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public SpriteRenderer body;
    [SerializeField] Sprite[] walk = null;
    [SerializeField] Sprite[] talk = null;
    [SerializeField] Sprite idle = null;

    [SerializeField] Sprite[] emotion = null;
    [SerializeField] GameObject Weapon = null;

    [SerializeField] float walkSpeed = 10;
    [SerializeField] float talkSpeed = 10;
    [SerializeField] Vector3 DefaultPosition = Vector3.zero;

    MicManager Mic;

    float LeftEndZone = -9.0f;
    float RightEndZone = 9.0f;

    //bool isWalk = false;
    //bool isTalk = false;
    bool isDown;
    bool isEmotion;
    bool canJump;
    bool canMove; 
    bool equipWeapon; 


    float motionTime = 0.0f;
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
        body.sprite = idle;
        Weapon.SetActive(false);
    }

    void Start()
    {
        initProperty();
        GameObject micGO = GameObject.Find("MicManager");
        Mic = micGO.GetComponent<MicManager>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    void Walk(float value)
    {
        if(isEmotion)
        {
            return;
        }
        value *= walkSpeed;
        switch ((int)value % walk.Length)
        {
            case 0:
                body.sprite = walk[0];
                break;
            case 1:
                body.sprite = walk[1];
                break;
            case 2:
                body.sprite = walk[2];
                break;
            case 3:
                body.sprite = walk[3];
                break;
        }
    }

    void Talk(float value)
    {
        if(isEmotion)
        {
            return;
        }
        value *= talkSpeed;
        switch ((int)value % talk.Length)
        {
            case 0:
                body.sprite = talk[0];
                break;
            case 1:
                body.sprite = talk[1];
                break;
        }
    }
    void SetIdle()
    {
        if (isEmotion)
        {
            return;
        }
        body.sprite = idle;
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
    void M_Input()
    {
        if (!canMove)
        {
            return;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //isWalk = true;
            if (body.flipX)
            {
                Weapon.transform.localPosition = new Vector3(-Weapon.transform.localPosition.x, Weapon.transform.localPosition.y, 0.0f);
                Weapon.transform.localRotation = new Quaternion(0, 0, -Weapon.transform.localRotation.z, Weapon.transform.localRotation.w);
                Weapon.GetComponent<SpriteRenderer>().flipX = false;
                body.flipX = false;
            }
            motionTime += Time.deltaTime;
            m_dir = Vector3.left;
            Walk(motionTime);

        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            //isWalk = false;
            motionTime = 0;
            m_dir = Vector3.zero;
            SetIdle();
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            //isWalk = true;

            if (!body.flipX)
            {
                Weapon.transform.localPosition = new Vector3(-Weapon.transform.localPosition.x, Weapon.transform.localPosition.y, 0.0f);
                Weapon.transform.localRotation = new Quaternion(0, 0, -Weapon.transform.localRotation.z, Weapon.transform.localRotation.w);
                Weapon.GetComponent<SpriteRenderer>().flipX = true;
                body.flipX = true;
            }
            motionTime += Time.deltaTime;
            m_dir = Vector3.right;
            Walk(motionTime);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            //isWalk = false;
            motionTime = 0;
            m_dir = Vector3.zero;
            SetIdle();
        }

        if (0 < Input.GetAxisRaw("Vertical") && canJump && m_rigidbody.velocity.y >= 0)
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
            isEmotion = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            body.sprite = emotion[1];
            isEmotion = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            body.sprite = emotion[2];
            isEmotion = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            body.sprite = emotion[3];
            isEmotion = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            body.sprite = emotion[4];
            isEmotion = true;
        }
    }
    void MicInput()
    {
        if (isEmotion)
        {
            return;
        }
        if (Mic.GetDecibels() > -110)
        {
            motionTime += Time.deltaTime;
            Talk(motionTime);
            Debug.Log(Mic.GetDecibels());
        }
        else
        {
            motionTime = 0;
            SetIdle();
        }
    }

    public void SetChromaKeyMode()
    {
        canMove = false;
        equipWeapon = true;
        SetWeapon();
        m_rigidbody.useGravity = false;
        body.flipX = false;
        body.sprite = emotion[0];
        m_dir = Vector3.zero;
        transform.position = Vector3.zero;
    }
    public void DisableChromaKeyMode()
    {
        canMove = true;
        m_rigidbody.useGravity = true;
        transform.position = DefaultPosition;
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
