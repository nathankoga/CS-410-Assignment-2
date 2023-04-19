using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float turnSpeed = 20f;
    public float speedPerGhost = 0.25f;

    public float eatDistance = 1.25f;
    public float eatAngle = 0.72f;

    int ghostsConsumed = 0;
    
    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    AudioSource m_EatAudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;  // Quaternions store rotation data better than 3D vectors

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
        m_EatAudioSource = GameObject.Find("EatNode").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();


        bool hasHorizontalInput = !Mathf.Approximately (horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately (vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool ("IsWalking", isWalking);

        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop();
        }
        Vector3 desiredForward = Vector3.RotateTowards (transform.forward, m_Movement, GetTurnSpeed() * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);

        AttemptEat();
    }

    void OnAnimatorMove ()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + (m_Movement * m_Animator.deltaPosition.magnitude * GetMovementSpeed()));
        m_Rigidbody.MoveRotation(m_Rotation);
    }

    float GetMovementSpeed()
    {
        return moveSpeed + (speedPerGhost * ghostsConsumed);
    }

    float GetTurnSpeed()
    {
        return turnSpeed * GetMovementSpeed();
    }

    /*
     * Ghost Eating
     */

    void AttemptEat()
    {
        // Calculate the dot product and distance of the nearest ghost.
        GameObject closestGhost = FindSilliestGuy();
        if (closestGhost == null) return;

        Vector3 lookVector = transform.TransformDirection(Vector3.forward);
        Vector3 ghostLookVector = closestGhost.transform.TransformDirection(Vector3.forward);
        lookVector.Normalize();
        ghostLookVector.Normalize();

        float distance = Vector3.Distance(closestGhost.transform.position, transform.position);
        float dot = Vector3.Dot(lookVector, ghostLookVector);

        // If we are close enough and can eat, do it.
        if (distance < eatDistance && dot > eatAngle)
        {
            m_EatAudioSource.Play();
            closestGhost.GetComponent<WaypointPatrol>().AttemptDestroy();
            closestGhost.SetActive(false);
            ghostsConsumed += 1;
        }
    }

    GameObject FindSilliestGuy()
    {
        // Locates the silliest guy.
        GameObject silly = null;
        float bestDistance = Mathf.Infinity;
        foreach (GameObject guy in GameObject.FindGameObjectsWithTag("Ghost"))
        {
            float distance = Vector3.Distance(guy.transform.position, transform.position);
            if (bestDistance > distance)
            {
                bestDistance = distance;
                silly = guy;
            }
        }
        return silly;
    }
}
