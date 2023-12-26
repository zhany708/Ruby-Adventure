using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //�ƶ����
    public InputAction moveAction;

    float moveSpeed = 3f;

    Vector2 m_Position;
    Vector2 m_Move;
    Rigidbody2D m_Rigidbody2d;

    //����ֵ���    
    public int health { get { return m_CurrentHealth; } }   //��ȡ��ǰ����ֵ
    public int maxHealth = 5;

    public int m_CurrentHealth;

    //�˺���ȴ���
    float timeInvincible = 2f;
    bool m_IsInvincible;
    float m_DamageCooldown;

    //�������
    Animator m_Animator;
    Vector2 m_MoveDirection = new Vector2(1, 0);

    //�������
    public GameObject projectilePrefab;
    public InputAction launchAction;

    //NPC���
    public InputAction talkAction;

    //�������
    public AudioClip m_AttackClip;
    public AudioClip m_HittedClip;

    AudioSource[] m_Audiosource;    //��һ������ж��������ʱ����Ҫ�����������ø���������
    AudioSource walkingAudioSource;     //��������·��Ƶ�Ĳ������벥��������Ƶ�Ĳ��������ֿ�����ֹ���ֲ��Ŵ���
    AudioSource actionAudioSource;      //�����һ����Դ�Ļ�����ֹͣ������·��Чʱ��ͨ��playoneshot����������Ч��Ч�������



    void Start()
    {
        moveAction.Enable();

        talkAction.Enable();
        talkAction.performed += FindFriend;

        launchAction.Enable();
        launchAction.performed += Launch;      //���´�ϵͳ�еİ���ʱ������Launch����

        m_Rigidbody2d = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();

        m_Audiosource = GetComponents<AudioSource>();
        walkingAudioSource = m_Audiosource[0];
        actionAudioSource = m_Audiosource[1];

        m_CurrentHealth = maxHealth;
    }

    void Update()
    {
        CheckInvincible();
        SetAnimator();

        
    }

    void FixedUpdate()
    {       
        Run();
        PlayWalkingSound();
    }



    private void Run()      //��ɫ�ƶ�
    {
        //����Input system���µ�inputϵͳ��������ֵ
        m_Move = moveAction.ReadValue<Vector2>();

        //ͨ����ɫ��Rigidbody 2D�����ƶ�����ֹ������ײ����
        m_Position = (Vector2)m_Rigidbody2d.position + m_Move * moveSpeed * Time.deltaTime;
        m_Rigidbody2d.MovePosition(m_Position);
    }

    private void Launch(InputAction.CallbackContext context)    //��ɫ�Ĺ�����Ͷ����Ʒ��
    {
        GameObject projectileObject = Instantiate(projectilePrefab, m_Rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);   //�ڶ���������ʾλ�ã�������������ʾ��ת�������ʾ����ת��

        Projectile projectile = projectileObject.GetComponent<Projectile>();    //����Projectile�ű�
        projectile.Launch(m_MoveDirection, 300);

        m_Animator.SetTrigger("Launch");
        PlaySound(m_AttackClip);    //���Ź�����Ƶ
    }


    private void FindFriend(InputAction.CallbackContext context)   //��������ʱ�����Ի�
    {
        //��һ��������ʾ���߷��򣬵ڶ����ǽ�ɫ����ķ��򣬵����������ߵ������룬���ĸ��Ǵ�������layer
        RaycastHit2D hit = Physics2D.Raycast(m_Rigidbody2d.position + Vector2.up * 0.2f, m_MoveDirection, 1.5f, LayerMask.GetMask("NPC"));

        if (hit.collider != null)
        {
            NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();     //���ú���NonPlayerCharacter�ű������

            if (character != null)
            {
                UIHandler.instance.DisplayDialogue(character.dialogText);     //���ݲ�ͬ��NPC��ʾ�Ի�
            }
        }
    }


    private void SetAnimator()      //������ɫ�ƶ�ʱ��ͬ����Ķ���
    {
        if (!Mathf.Approximately(m_Move.x, 0f) || !Mathf.Approximately(m_Move.y, 0f))   //���X��Y��ֵ�Ƿ�Ϊ0��mathf.Approximately���ڷ�ֹfloat��������
        {
            m_MoveDirection.Set(m_Move.x, m_Move.y);     //��ɫ�ƶ�ʱ������ɫ�����괫��m_Direction
            m_MoveDirection.Normalize();      //Normalize���ڴ��淽������������Ὣ�κβ�Ϊ0�ĳ�������Ϊ1����Ϊ���ڷ�����˵�����Ȳ���Ҫ
        }

        m_Animator.SetFloat("Move X", m_MoveDirection.x);
        m_Animator.SetFloat("Move Y", m_MoveDirection.y);
        m_Animator.SetFloat("Speed", m_Move.magnitude);
    }



    public void ChangeHealth(int amount)        //���Ľ�ɫ����ֵ
    {
        if (amount < 0)     //����ʱ�����޵�ʱ��
        {
            if (m_IsInvincible)
            {
                return;
            }
            m_IsInvincible = true;      //����ɫ�����Ҳ����޵�ʱ��ʱ�����޵�ʱ��
            m_DamageCooldown = timeInvincible;

            m_Animator.SetTrigger("Hit");       //�ĳ��ܻ�����״̬
            PlaySound(m_HittedClip);    //�����ܻ���Ч
        }

        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth + amount, 0, maxHealth);  //���Ƶ�һ�������ķ�Χ����СΪ�ڶ������������Ϊ����������ɫ����ֵ����Ϊ0-5��

        UIHandler.instance.SetHealthValue(m_CurrentHealth / (float)maxHealth);      //�������float��֤��������������ֵҲ��float����ʹ���Ƕ���int��
    }

    private void CheckInvincible()      //����ɫ�Ƿ����޵�״̬
    {
        if (m_IsInvincible)     //���������޵�ʱ��
        {
            m_DamageCooldown -= Time.deltaTime;
            if (m_DamageCooldown < 0)
            {
                m_IsInvincible = false;
            }
        }
    }


    private void PlayWalkingSound()
    {
        if (m_Move.magnitude > Mathf.Epsilon && !walkingAudioSource.isPlaying)      //���κ���Ҫ������ڲ��Ź������ظ����Ż��е�������
        {
            walkingAudioSource.Play();        
        }

        else if (m_Move.magnitude <= Mathf.Epsilon && walkingAudioSource.isPlaying)
        {
            walkingAudioSource.Stop();
        }
    }

    public void PlaySound(AudioClip clip)       //ͨ����ɫ������Ƶ��ʹ������·��Դ��ͬ����Դ
    {
        actionAudioSource.PlayOneShot(clip);
    }
}
