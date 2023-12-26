using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDamged : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)      //检查玩家是否触碰尖刺
    {
        PlayerController m_Controller = other.GetComponent<PlayerController>();

        if (m_Controller != null)
        {
            m_Controller.ChangeHealth(-1);   //减少一点生命
        }
    }

}
