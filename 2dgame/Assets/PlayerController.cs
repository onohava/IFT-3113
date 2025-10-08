using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float jumpForce = 12f;


    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float moveInputHorizontal;
    private float moveInputVertical;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.W))
            moveInputVertical = 1;
        if(Input.GetKey(KeyCode.S))
            moveInputVertical = -1;
        if(Input.GetKey(KeyCode.A))
            moveInputHorizontal = -1;
        if (Input.GetKey(KeyCode.D))
            moveInputHorizontal = 1;

        if (Input.GetKeyUp(KeyCode.W))
            moveInputVertical = 0;
        if (Input.GetKeyUp(KeyCode.S))
            moveInputVertical = 0;
        if (Input.GetKeyUp(KeyCode.A))
            moveInputHorizontal = 0;
        if (Input.GetKeyUp(KeyCode.D))
            moveInputHorizontal = 0;



        if (moveInputHorizontal != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInputHorizontal), 1, 1);
        }
    }

    private void FixedUpdate()
    {       
        moveInput = new Vector2(moveInputHorizontal, moveInputVertical);

        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }
}
