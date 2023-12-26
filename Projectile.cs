using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Projectile : MonoBehaviour
{
    //�ƶ����
    Rigidbody2D m_Rigidbody2d;

    //��Ƶ���
    public AudioClip m_EnemyHittedClip;


    void Awake()
    {
        m_Rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (transform.position.magnitude > 100f)    //���ɵ�����100����λ��ɾ���ɵ�
        {
            Destroy(gameObject);
        }
    }


    public void Launch(Vector2 direction, float force)
    {
        m_Rigidbody2d.AddForce(direction * force);      //������ʩ�ӷ������
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
        if (enemy != null)  //���˺������ú���EnemyController�ű������ʱ
        {
            //enemy.PlaySound(m_EnemyHittedClip);    //���ŵ����ܻ���Ƶ��Դͷ���Ե��ˣ�
            enemy.Fix();
        }
        Destroy(gameObject);
    }
}
