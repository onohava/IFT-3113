using UnityEngine;

public class PlayerController2 : MonoBehaviour
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

        if (Input.GetKey(KeyCode.UpArrow))
            moveInputVertical = 1;

        if (Input.GetKey(KeyCode.DownArrow))
            moveInputVertical = -1;

        if (Input.GetKey(KeyCode.LeftArrow))
            moveInputHorizontal = -1;

        if (Input.GetKey(KeyCode.RightArrow))
            moveInputHorizontal = 1;
            
        if(Input.GetKeyUp(KeyCode.UpArrow))
            moveInputVertical = 0;
        if (Input.GetKeyUp(KeyCode.DownArrow))
            moveInputVertical = 0;
        if (Input.GetKeyUp(KeyCode.LeftArrow))
            moveInputHorizontal = 0;
        if (Input.GetKeyUp(KeyCode.RightArrow))
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
