using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public SpriteRenderer body;
    
    [SerializeField] Sprite[] emotion = null;
    [SerializeField] SpriteRenderer Weapon = null;
    [SerializeField] float walkSpeed = 10;
    [SerializeField] Vector3 DefaultPosition = Vector3.zero;
    [SerializeField] Sprite[] WeaponImage = null;

    Animator m_anim;
    [SerializeField] SpriteRenderer m_Mouth = null;
    [SerializeField] SpriteRenderer m_Eye = null;
    [SerializeField] GameObject spine = null;

    IEnumerator SDown;
    IEnumerator SUp;

    IEnumerator EyeBlinker;

    MicManager Mic;

    float LeftEndZone = -9.0f;
    float RightEndZone = 9.0f;
    [SerializeField] float SpineSpeed = 0;
    [SerializeField] float SpineSlowSpeed = 10;

    int CurrentWeaponNum = 0;

    //bool isWalk = false;
    //bool isTalk = false;
    bool isDown;
    bool canJump;
    bool canMove; 
    bool equipWeapon;
    bool canEmoticon;
    bool canBlink;
    bool isBurrow;
    bool canBurrow;

    Vector3 m_dir = Vector3.zero;
    Vector3 m_spineDir = Vector3.zero;

    Rigidbody m_rigidbody;

    void initProperty()
    {
        //isWalk = false;
        //isTalk = false;
        canMove = true;
        canJump = true;
        canBlink = true;
        EyeBlinker = EyeBlink();
        canEmoticon = true;
        equipWeapon = false;
        isBurrow = false;
        canBurrow = true;
        transform.position = DefaultPosition;
        body.sprite = emotion[0];
        Weapon.gameObject.SetActive(false);
        m_rigidbody = GetComponent<Rigidbody>();
        GameObject micGO = GameObject.Find("MicManager");
        Mic = micGO.GetComponent<MicManager>();
        m_anim = GetComponent<Animator>();

    }

    void Start()
    {
        initProperty();
        StartCoroutine(EyeBlinker);
    }


    public void ShowFace()
    {
        canBlink = true;
        m_Eye.gameObject.SetActive(true);
    }
    public void HideFace()
    {
        canBlink = false;
        m_Eye.gameObject.SetActive(false);
    }

    IEnumerator EyeBlink()
    {
        while (true)
        {
            yield return new WaitForSeconds(Util.Rand(3, 5));
            if (canBlink)
            {
                m_anim.SetTrigger("T_Blink");
            }
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
            Weapon.gameObject.SetActive(false);
        }
        else if (!equipWeapon)
        {
            equipWeapon = true;
            Weapon.gameObject.SetActive(true);
        }
    }

    public void SwitchWeapon()
    {
        if (!Weapon.gameObject.activeSelf)
        {
            return;
        }
        if (WeaponImage.Length > 1)
        {
            if (CurrentWeaponNum + 1 > WeaponImage.Length - 1)
            {
                CurrentWeaponNum = 0;
            }
            else
            {
                CurrentWeaponNum++;
            }
            Weapon.sprite = WeaponImage[CurrentWeaponNum];
        }
    }

    public bool CheckEffect(out int i)
    {
        if (!Weapon.gameObject.activeSelf)
        {
            i = 0;
            return false;
        }

        if(CurrentWeaponNum == 0)
        {
            i = 0;
            return false;
        }

        i = 1;
        return true;
    }

    public void StopMovement()
    {
        canMove = false;
        canJump = false;
        canEmoticon = false;
    }
    public void ReplayMovement()
    {
        canMove = true;
        canJump = true;
        canEmoticon = true;
    }

    public void SetChromaKeyMode()
    {
        canMove = false;
        canJump = false;
        canBurrow = false;
        equipWeapon = true;
        SetWeapon();
        spine.transform.localPosition = Vector3.zero;
        m_spineDir = Vector3.zero;
        if (body.flipX)
        {
            Weapon.GetComponent<SpriteRenderer>().flipX = false;
            body.flipX = false;
            m_Eye.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            m_Mouth.flipX = false;
            SetFlip();
        }
        m_dir = Vector3.zero;
        m_rigidbody.useGravity = false;
        m_rigidbody.velocity = Vector3.zero;
        transform.position = Vector3.zero;
    }
    public void SetCamBroadMode()
    {
        canMove = false;
        canJump = false;
        canBurrow = false;
        equipWeapon = true;
        spine.transform.localPosition = Vector3.zero;
        m_dir = Vector3.zero;
        m_rigidbody.velocity = Vector3.zero;
        transform.position = Vector3.zero;
        StopCoroutine(EyeBlinker);
        gameObject.SetActive(false);
    }

    public void DisableChromaKeyMode()
    {
        gameObject.SetActive(true);
        StartCoroutine(EyeBlinker);
        canBurrow = true;
        ReplayMovement();
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

        if (0 < Input.GetAxisRaw("Vertical") && canJump && m_rigidbody.velocity.y >= 0 && canMove && !Input.GetKeyDown(KeyCode.Semicolon) && spine.transform.localPosition.y == 0)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SetWeapon();
        }

        if(Input.GetKeyDown(KeyCode.Home) && !isBurrow)
        {
            isBurrow = true;
            m_spineDir = Vector3.up;
        }
        if (Input.GetKeyDown(KeyCode.End) && !isBurrow)
        {
            isBurrow = true;
            m_spineDir = Vector3.down;
        }

        if (Input.GetKey(KeyCode.PageUp) && canBurrow && !isBurrow)
        {
            m_spineDir = Vector3.up;
        }
        else if(Input.GetKeyUp(KeyCode.PageUp) && !isBurrow)
        {
            m_spineDir = Vector3.zero;
        }
        if (Input.GetKeyDown(KeyCode.PageDown) && canBurrow && !isBurrow)
        {
            m_spineDir = Vector3.down;
        }
        else if (Input.GetKeyUp(KeyCode.PageDown) && !isBurrow)
        {
            m_spineDir = Vector3.zero;
        }

    }
    void M_EmoticonInput()
    {
        if(!canEmoticon)
        {
            return;
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
    }

    void MicInput()
    {
        if (Mic.GetDecibels() > Mic.GetMinValue())
        {
            m_anim.SetBool("B_Mouth" , true);
        }
        else
        {
            m_anim.SetBool("B_Mouth", false);
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
        M_EmoticonInput();
        MicInput();
        if (!canJump )
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

        if (canBurrow)
        {
            float _speed = 0;
            if(isBurrow)
            {
                _speed = SpineSpeed;
            }
            else
            {
                _speed = SpineSlowSpeed;
            }
            if ((spine.transform.localPosition + (_speed * m_spineDir * Time.deltaTime)).y > 0 )
            {
                spine.transform.localPosition = Vector3.zero;
                isBurrow = false;
                m_spineDir = Vector3.zero;
            }
            else if ((spine.transform.localPosition + (_speed * m_spineDir * Time.deltaTime)).y < -7)
            {
                spine.transform.localPosition = Vector3.down * 7;
                isBurrow = false;
                m_spineDir = Vector3.zero;
            }
            else
            {
                spine.transform.position += _speed * m_spineDir * Time.deltaTime;
            }
        }
    }
}
