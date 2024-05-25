using TrailsFX;
using UnityEngine;

public class HandleVisual : MonoBehaviour
{
    [SerializeField, Multiline] private string notes;

    [Header("Components")]
    private AllowInput input;
    private SwitchDimension dimension;
    
    private PlayerMovement3D movement3D;
    private PlayerMovement2D movement2D;
    private SnappableCheck _snappableCheck;
    [SerializeField] private SquashAndStretch[] snsScript;
    [SerializeField] private TrailEffect[] trailEffect;
    
    [SerializeField] private MeshRenderer[] renderers;
    [SerializeField] private MeshRenderer[] eyeRenderers;
    
    private Transform legTransform;
    private Transform headTransform;
    
    [Header("Appearance")]
    [SerializeField] private Color playerColor = Color.white;
    [SerializeField] private Color eyesColor;
    [SerializeField] private bool useDarkenedInvertedColorForEyes;
    
    [Header("Animation")]
    [SerializeField] private Transform[] objectToUnparent;
    [Space] [SerializeField] private float headRotationSpeed = 66f;
    
    [Header("Others")]
    [SerializeField] private bool is3D;
    
    private bool hasRun = false;

    private void Awake()
    {
        input = GameObject.Find("Game Manager").GetComponent<AllowInput>();
        dimension = GameObject.Find("Game Manager").GetComponent<SwitchDimension>();
        
        movement3D = GetComponentInParent<PlayerMovement3D>();
        movement2D = GetComponentInParent<PlayerMovement2D>();
        _snappableCheck = GameObject.Find("Player3D").GetComponent<SnappableCheck>();
        //snsScript = GetComponent<SquashAndStretch>();
        
        legTransform = transform.Find("Leg");
        headTransform = transform.Find("Head");
        renderers = new[] { legTransform.GetComponent<MeshRenderer>(), headTransform.GetComponent<MeshRenderer>() };
        eyeRenderers = new[] { transform.Find("LEye").GetComponent<MeshRenderer>(), transform.Find("REye").GetComponent<MeshRenderer>() };
        trailEffect = new[] { legTransform.GetComponent<TrailEffect>(), headTransform.GetComponent<TrailEffect>() };
        
        if (movement3D != null) is3D = true;
        else if (movement2D != null) is3D = false;
        else Debug.LogError("The fawk did you attach this script to? Make sure parent object has either 'PlayerMovement3D' or '--2D'");
        
        if (useDarkenedInvertedColorForEyes) eyesColor = new Color((1 - playerColor.r) / 12, (1 - playerColor.g) / 12, (1 - playerColor.b) / 12, playerColor.a);
    }

    private void Start()
    {
        DetachObjectsFromParentAndDisable();
        
        ApplyColor();
    }

    private void OnEnable()
    {
        ReenableDetachedObjectWhenSwitchingDimension();
    }
    
    void Update()
    {
        RotateHeadToLeg();
        ApplyFallSnS();

        DisableDetachedObjectWhenSwitchingDimension();
    }

    void RotateHeadToLeg()
    {
        Vector3 targetDirection = legTransform.position - headTransform.position;

        Quaternion targetRotation = Quaternion.FromToRotation(-legTransform.up, targetDirection);

        headTransform.rotation = Quaternion.RotateTowards(headTransform.rotation, targetRotation * headTransform.rotation, headRotationSpeed * Time.deltaTime);
    }

    public void ApplyJumpSnS()
    {
        snsScript[0].PlaySquashAndStretch();
        
        /*if (is3D ? movement3D.isGrounded : movement2D.isGrounded)
        {
            snsScript[0].PlaySquashAndStretch();
        }*/
    }
    
    void ApplyFallSnS()
    {
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
            if (is3D && Input.GetButtonDown("Switch Dimension") && input.allowInput && _snappableCheck.allowSnap)
            {
                if (dimension.currentState == SwitchDimension.GameState.ThreeDimension) objectToUnparent[i].gameObject.SetActive(false);
            }
            else if (!is3D && Input.GetButtonDown("Switch Dimension"))
            {
                if (dimension.currentState == SwitchDimension.GameState.TwoDimension) objectToUnparent[i].gameObject.SetActive(false);
            }
        }
    }

    void DetachObjectsFromParentAndDisable()
    {
        for (int i = 0; i < objectToUnparent.Length; i++)
        {
            objectToUnparent[i].SetParent(null);
            
            // Be sure to have this game object re-enabled somewhere later
            if (is3D && dimension.currentState == SwitchDimension.GameState.TwoDimension) objectToUnparent[i].gameObject.SetActive(false);
            else if (!is3D && dimension.currentState == SwitchDimension.GameState.ThreeDimension) objectToUnparent[i].gameObject.SetActive(false);
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
            }
            else if (!is3D)
            {
                if (dimension.currentState == SwitchDimension.GameState.TwoDimension) objectToUnparent[i].gameObject.SetActive(true);
            }
        }
    }

    void ApplyColor()
    {
        // To Mesh Renderer-s
        for (int i = 0; i < renderers.Length; i++)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            renderers[i].GetPropertyBlock(propBlock, 0);
            
            propBlock.SetColor("_Color", playerColor);
            
            renderers[i].SetPropertyBlock(propBlock, 0);
        }
        
        // To TrailRenderer.cs-es
        for (int i = 0; i < trailEffect.Length; i++)
        {
            trailEffect[i].trailTint = playerColor;

            Color newColor = playerColor;
            newColor.a = 0.5f;
            trailEffect[i].color = newColor;
        }
        
        // To eyes' MeshRenderer-s
        for (int i = 0; i < eyeRenderers.Length; i++)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            eyeRenderers[i].GetPropertyBlock(propBlock, 0);
            
            propBlock.SetColor("_Color", eyesColor);
            
            eyeRenderers[i].SetPropertyBlock(propBlock, 0);
        }
    }
}