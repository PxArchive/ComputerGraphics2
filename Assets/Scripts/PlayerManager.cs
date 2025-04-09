using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public Vector2 inputDirection;
    public float walkSpeed = 1f;
    public float jumpStrength = 10f;
    public bool grounded = true;
    public bool doJump = false;

    private Rigidbody rb;

    private Vector3 vectorZERO = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += new Vector3(inputDirection.x, 0f, inputDirection.y) * walkSpeed;
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

    public void OnReset(InputValue _input)
    {
        if (_input.isPressed)
        {
            transform.position = new Vector3(0f, 0.5f, 0f);
        }
    }
}
