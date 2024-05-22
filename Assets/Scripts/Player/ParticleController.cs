using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ParticleController : MonoBehaviour
{
    [Header("Components")]
    private PlayerMovement2D movement2D;
    private PlayerMovement3D movement3D;
    
    private Rigidbody rb;
    private CharacterController controller;
    [SerializeField] private ParticleSystem walkParticle;
    [SerializeField] private ParticleSystem landParticle;
    
    [Header("Parameters")]
    [SerializeField] private float walkParticleCooldown = 0.1f;

    private Vector3 movementDirection;
    private Vector3 lastPosition;
    private float elapsedTime; // Used for PlayWalkParticle()
    private bool hasPlayed = false; // Used for PlayLandParticle()
    private float previousYVelocity;

    private void OnEnable()
    {
        movement2D = GetComponent<PlayerMovement2D>();
        movement3D = GetComponent<PlayerMovement3D>();
        
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        CheckMovement();

        PlayWalkParticle();
        PlayLandParticle();
    }

    private void LateUpdate()
    {
        RecordYVelocity();
    }

    void CheckMovement()
    {
        movementDirection = Vector3.zero;
        if (rb != null && rb.velocity.magnitude > 0.1f)
        {
            movementDirection = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }
        else if (controller != null && (transform.position - lastPosition).magnitude > 0.1f)
        {
            Vector3 position = transform.position - lastPosition;
            movementDirection = new Vector3(position.x, 0f, position.z).normalized;
            
            lastPosition = transform.position;
        }
    }

    void PlayWalkParticle()
    {
        elapsedTime += Time.deltaTime;
        
        if (IsGround() && movementDirection != Vector3.zero && elapsedTime >= walkParticleCooldown)
        {
            walkParticle.Emit(2);
            elapsedTime = 0;
        }
    }

    void PlayLandParticle()
    {
        if (IsGround() && !hasPlayed)
        {
            landParticle.Emit(12);
            hasPlayed = true;
        }
        else if (!IsGround()) hasPlayed = false;
    }

    bool IsGround()
    {
        if (movement2D != null && movement2D.isGrounded)
        {
            return true;
        } 
        else if (movement3D != null && movement3D.isGrounded)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    int LandParticleAmount()
    {
        const int maxParticles = 12;
        const int minParticles = 0;
        const float maxVelocityY = -20f;
        
        if (rb != null)
        {
            previousYVelocity = Mathf.Clamp(previousYVelocity, maxVelocityY, 0);

            float normalizedVelocity = Mathf.InverseLerp(0, maxParticles, previousYVelocity);
            
            int particles = Mathf.RoundToInt(Mathf.Lerp(minParticles, maxParticles, normalizedVelocity));
            Debug.Log(particles);
            return particles;
        }
        else if (controller != null)
        {
            previousYVelocity = Mathf.Clamp(previousYVelocity, maxVelocityY, 0);

            float normalizedVelocity = Mathf.InverseLerp(0, maxParticles, previousYVelocity);
            
            int particles = Mathf.RoundToInt(Mathf.Lerp(minParticles, maxParticles, normalizedVelocity));
            Debug.Log(particles);
            return particles;
        }

        return 0;
    }

    void RecordYVelocity()
    {
        if (rb!= null) previousYVelocity = rb.velocity.y;
        else if (controller != null) previousYVelocity = controller.velocity.y;
    }
}