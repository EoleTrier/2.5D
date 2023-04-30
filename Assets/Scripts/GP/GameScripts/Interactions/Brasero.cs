using System.Collections.Generic;
using UnityEngine;
using static ObjectType;

public class Brasero : Interactibles
{
    [SerializeField] private float m_BurnDistance;
    [SerializeField] private bool m_IsBurning;
    private List<ParticleSystem> m_ParticleSystem;
    private bool m_Play;

    private void Start()
    {
        IsOn = m_IsBurning;
        m_AudioManager = FindObjectOfType<AudioManager>();
        m_ParticleSystem = new (GetComponentsInChildren<ParticleSystem>());
    }

    public override void OnOff() {;}

    public void FixedUpdate() 
    {
        WillBeBurning();
        IsOn = m_IsBurning;
        if (IsOn)
        {
            Active();
            m_ParticleSystem.ForEach((flame) => flame.Play());
        }
        else
        {
            Desactive();
            m_ParticleSystem.ForEach((flame) => flame.Stop());
        }

        if (m_IsBurning && m_Play)
        {
            m_AudioManager.Play("fire_on");
            m_AudioManager.Play("fire_burning");
            m_Play = false;
        }

        else if (!m_IsBurning && !m_Play)
        {
            m_AudioManager.Stop("fire_burning");
            m_AudioManager.Play("fire_off");
            m_Play = true;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_BurnDistance);
    }

    private void WillBeBurning()
    {
        Collider[] burn = Physics.OverlapSphere(transform.position, m_BurnDistance);


        foreach (Collider c in burn)
        {
            if (c.TryGetComponent(out BurningObject burningObject))
            {
                if (burningObject.IsBurning)
                    m_IsBurning = true;

                else if (!burningObject.IsBurning && m_IsBurning)
                    burningObject.IsBurning = true;

                else if (c.gameObject.CompareTag("Water"))
                    m_IsBurning = false;
            }
        }
    }
}
