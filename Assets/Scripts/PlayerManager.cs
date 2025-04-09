using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerManager : MonoBehaviour
{
    public Vector2 inputDirection;
    public float walkSpeed = 1f;
    public float jumpStrength = 10f;
    public bool grounded = true;
    public bool doJump = false;

    public float orbTimer = 0f;
    public float totalOrbActiveTime = 5f;
    private float orbCurrentScale = 0.1f;
    private float orbTargetScale = 0.1f;
    private float orbScaleVelocity  = 0f;
    public float orbScaleSmoothTime = 0.2f;

    float vignetteValue = 0f;
    float vignetteTarget = 0f;
    float vignetteVelocity = 0f;
    public float vignetteSmoothTime = 0.2f;

    private Rigidbody rb;
    private Transform orb;
    private Volume volume;

    Vignette vignette;


    private Vector3 vectorZERO = Vector3.zero;
    private Vector3 vectorONE = Vector3.one;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        orb = GameObject.Find("Player_ORB").transform;
        volume = GameObject.Find("VOLUME").GetComponent<Volume>();
        volume.profile.TryGet(out vignette);

        Application.targetFrameRate = 60;
        Screen.SetResolution(
            Screen.currentResolution.width,
            Screen.currentResolution.height,
            FullScreenMode.Windowed,
            new RefreshRate()
            {
                numerator = 60000,
                denominator = 1001
            });
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += new Vector3(inputDirection.x, 0f, inputDirection.y) * walkSpeed;

        if (orbTimer > 0)
        {
            orbTimer -= Time.deltaTime;

            orbTargetScale = 5f;
            vignetteTarget = 0.45f;
        }
        else
        {
            orbTargetScale = 0.1f;
            vignetteTarget = 0f;
        }

        orbCurrentScale = Mathf.SmoothDamp(orbCurrentScale, orbTargetScale, ref orbScaleVelocity, orbScaleSmoothTime);

        orb.transform.localScale = vectorONE * orbCurrentScale;

        if (vignette)
        {
            vignetteValue = Mathf.SmoothDamp(vignetteValue, vignetteTarget, ref vignetteVelocity, vignetteSmoothTime);
            vignette.intensity.SetValue(new ClampedFloatParameter(vignetteValue, 0f, 1f, true));
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(inputDirection.x, 0f, inputDirection.y) * walkSpeed);
        if (grounded && doJump)
        {
            doJump = false;
            grounded = false;
            rb.AddForce(new Vector3(0f, jumpStrength, 0f));
            //Debug.Log("JUMP");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //Debug.Log("STAY");
        grounded = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("ENTER");
        grounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log("EXIT");
        grounded = false;
    }

    public void OnMove(InputValue _input)
    {
        inputDirection = _input.Get<Vector2>();

        if (!grounded)
        {
            inputDirection = vectorZERO;
        }
    }

    public void OnJump(InputValue _input)
    {
        if (_input.isPressed && grounded)
        {
            doJump = true;
        }
    }

    public void OnInteract(InputValue _input)
    {
        if (orbTimer <= 0)
        {
            orbTimer = totalOrbActiveTime;
        }
    }

    public void OnReset(InputValue _input)
    {
        if (_input.isPressed)
        {
            transform.position = new Vector3(0f, 0.5f, 0f);
        }
    }
}
