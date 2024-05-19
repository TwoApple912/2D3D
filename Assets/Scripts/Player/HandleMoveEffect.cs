using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class HandleMoveEffect : MonoBehaviour
{
    [SerializeField, Multiline] private string notes;
    
    [Header("Components")]
    [SerializeField] private SwitchDimension dimension;
    private PlayerMovement3D movement3D;
    private PlayerMovement2D movement2D;
    [SerializeField] private SquashAndStretch[] snsScript;
    
    private Transform legTransform;
    private Transform headTransform;
    
    [Header("Parameters")]
    [SerializeField] private Transform[] objectToUnparent;
    [Space] [SerializeField] private float headRotationSpeed = 66f;
    
    [Header("Others")]
    [SerializeField] private bool is3D;
    
    private bool hasRun = false;

    private void Awake()
    {
        dimension = GameObject.Find("Game Manager").GetComponent<SwitchDimension>();
        
        movement3D = GetComponentInParent<PlayerMovement3D>();
        movement2D = GetComponentInParent<PlayerMovement2D>();
        //snsScript = GetComponent<SquashAndStretch>();
        
        legTransform = transform.Find("Leg");
        headTransform = transform.Find("Head");

        if (movement3D != null) is3D = true;
        else if (movement2D != null) is3D = false;
        else Debug.LogError("The fawk did you attach this script to? Make sure parent object has either 'PlayerMovement3D' or '--2D'");
        
        DetachObjectFromParent();
    }

    private void OnEnable()
    {
        ReenableDetachedObjectWhenSwitchingDimension();
    }
    
    void Update()
    {
        RotateHeadToLeg();
        ApplySnS();

        DisableDetachedObjectWhenSwitchingDimension();
    }

    void DetachObjectFromParent()
    {
        for (int i = 0; i < objectToUnparent.Length; i++)
        {
            objectToUnparent[i].SetParent(null);
        }
    }

    void RotateHeadToLeg()
    {
        Vector3 targetDirection = legTransform.position - headTransform.position;

        Quaternion targetRotation = Quaternion.FromToRotation(-legTransform.up, targetDirection);

        headTransform.rotation = Quaternion.RotateTowards(headTransform.rotation, targetRotation * headTransform.rotation, headRotationSpeed * Time.deltaTime);
    }

    void ApplySnS()
    {
        if (Input.GetButtonDown("Jump") && (is3D ? movement3D.isGrounded : movement2D.isGrounded) )
        {
            snsScript[0].PlaySquashAndStretch();
        }

        if ( (is3D ? movement3D.isGrounded : movement2D.isGrounded) && !hasRun)
        {
            snsScript[1].PlaySquashAndStretch();
            headTransform.GetComponent<SquashAndStretch>().PlaySquashAndStretch();
            hasRun = true;
        }
        else if (!(is3D ? movement3D.isGrounded : movement2D.isGrounded)) hasRun = false;
    }

    void DisableDetachedObjectWhenSwitchingDimension()
    {
        for (int i = 0; i < objectToUnparent.Length; i++)
        {
            /* The reason why the if-s compare with ThreeDimension and TwoDimension respectively is because it check for
             currentState before SwitchDimension.cs' LateUpdate() takes place to change the currentState */
            if (is3D && Input.GetButtonDown("Switch Dimension"))
            {
                if (dimension.currentState == SwitchDimension.GameState.ThreeDimension) objectToUnparent[i].gameObject.SetActive(false);
                Debug.Log("Nega");
            }
            else if (!is3D && Input.GetButtonDown("Switch Dimension"))
            {
                if (dimension.currentState == SwitchDimension.GameState.TwoDimension) objectToUnparent[i].gameObject.SetActive(false);
            }
        }
    }

    void ReenableDetachedObjectWhenSwitchingDimension()
    {
        for (int i = 0; i < objectToUnparent.Length; i++)
        {
            /* Same reasoning as above, except this happens after the script has been re-enabled by SwitchDimension.cs'
             LateUpdate() */
            if (is3D)
            {
                if (dimension.currentState == SwitchDimension.GameState.ThreeDimension) objectToUnparent[i].gameObject.SetActive(true);
                Debug.Log("Black");
            }
            else if (!is3D)
            {
                if (dimension.currentState == SwitchDimension.GameState.TwoDimension) objectToUnparent[i].gameObject.SetActive(true);
            }
        }
    }
}
