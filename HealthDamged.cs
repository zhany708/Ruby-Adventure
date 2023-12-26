using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDamged : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)      //�������Ƿ������
    {
        PlayerController m_Controller = other.GetComponent<PlayerController>();

        if (m_Controller != null)
        {
            m_Controller.ChangeHealth(-1);   //����һ������
        }
    }

}
