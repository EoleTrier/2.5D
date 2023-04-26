using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EoleBehaviour : Golem
{
    [HideInInspector] public List<Collider> listCollider = new();
    [SerializeField] private float m_WindForceHorizontal;
    [SerializeField] private float m_WindForceVertical;
    [SerializeField] private float m_TimeBeforeIdle;
    [HideInInspector] public bool windActive;
    private List<CapsuleCollider> m_WindCollider = new();
    private PlayerSwitch m_PlayerSwitch;
    private float m_IdleTimer;
    private bool forward;
    private ParticleSystem m_Particles;

    // Start is called before the first frame update
    void Start()
    {
        m_Particles = GetComponentInChildren<ParticleSystem>();
        forward = true;
        m_Type = GolemType.EOLE;
        m_CancelAnimator = false;
        windActive = false;
        m_PlayerSwitch = FindObjectOfType<PlayerSwitch>();
        m_PlayerMovement = FindObjectOfType<PlayerMovement>();
        GetComponents(m_WindCollider);
        if (m_WindCollider[0].enabled == true && m_WindCollider[1].enabled == true)
        {
            if (m_WindCollider[0].direction == 2)
            {
                m_WindCollider[0].enabled = true;
                m_WindCollider[1].enabled = false;
            }
            else
            {
                m_WindCollider[0].enabled = false;
                m_WindCollider[1].enabled = true;
            }
        }
        m_InitialJumpStrength = m_JumpStrength;
        m_InitialSpeed = m_Speed;
        CanJump = true;
        if (forward)
            m_PlayerMovement.GetAnimator().Play("EoleGetUpHorizontal");
        else
            m_PlayerMovement.GetAnimator().Play("EoleGetUpVertical");
    }

    private void FixedUpdate()
    {
        BoxCollider c = GetComponent<BoxCollider>();
        if (m_PlayerMovement.IsGrounded)
        {
            c.material = null;
        }
        else
        {
            c.material = PhysicMaterial;
        }
        if(m_Particles != null && m_Particles.isPlaying)
            m_Particles.Stop();
    }
    // Update is called once per frame
    public void EoleUpdate()
    {
        if (m_PlayerSwitch.Rooms[m_PlayerSwitch.m_CurrentRoom].Golems[m_PlayerSwitch.m_CurrentGolem].m_Type != GolemType.EOLE)
        {
            if (m_Particles != null)
                m_Particles.Play();
            else if (forward)
                m_PlayerMovement.GetAnimator().Play("EoleHorizontalKneeled");
            else if (!forward)
                m_PlayerMovement.GetAnimator().Play("EoleVerticalKneeled");
        }

        foreach (Collider collider in listCollider)
        {
            //if there is no rigidbody on collider we leave
            if (collider.attachedRigidbody == null)
                continue;

            Rigidbody rb = collider.attachedRigidbody;
            if (collider.TryGetComponent(out Golem golem) && collider.name == m_PlayerSwitch.Rooms[m_PlayerSwitch.m_CurrentRoom].Golems[m_PlayerSwitch.m_CurrentGolem].name)
            {
                //if wind is in front of eole
                if (forward)
                {
                    if (m_PlayerMovement.GetMoveDirection() == Vector3.zero)
                    {
                        rb.AddForce((transform.forward * m_WindForceHorizontal) * 12);
                    }
                    else
                    {
                        rb.AddForce((transform.forward * m_WindForceHorizontal) * 4);
                    }
                }

                //if wind is above eole
                else
                {
                    if (m_PlayerMovement.GetMoveDirection() == Vector3.zero)
                    {
                        rb.AddForce((transform.up * m_WindForceVertical) * 8);
                    }
                    else
                    {
                        rb.AddForce((transform.up * m_WindForceVertical) * 4);
                    }
                }
            }
            else
            {
                if (forward)
                    rb.AddForce(transform.forward * m_WindForceHorizontal);
                else
                    rb.AddForce(transform.up * m_WindForceVertical);
            }
        }
    }

    public override IEnumerator UseCapacity(double timePressed)
    {
        listCollider.Clear();
        m_WindCollider[0].enabled ^= true;
        m_WindCollider[1].enabled ^= true;
        if (m_PlayerMovement.IsGrounded && m_PlayerMovement.GetMoveDirection() == Vector3.zero)
        {
            forward ^= true;
            if (!forward)
            {
                m_PlayerMovement.GetAnimator().Play("EoleVertical");
                m_PlayerMovement.GetAnimator().SetBool("LookingUP", true);
            }
            else if (forward)
            {
                m_PlayerMovement.GetAnimator().Play("EoleHorizontal");
                m_PlayerMovement.GetAnimator().SetBool("LookingUP", false);
            }
        }
        windActive ^= true;
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!listCollider.Contains(other))
        {
            listCollider.Add(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (listCollider.Contains(other))
        {
            listCollider.Remove(other);
        }
    }
}
