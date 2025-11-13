using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public enum MovementAxis { Horizontal, Vertical }
    
    [Tooltip("Choose whether the platform moves horizontally or vertically.")]
    public MovementAxis movementAxis = MovementAxis.Horizontal;
    
    [Tooltip("How far the object will move from its start.")]
    public float moveDistance = 5.0f;
    
    [Tooltip("The speed at which the object moves.")]
    public float moveSpeed = 2.0f;
    
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMovingForward = true;
    
    void Start()
    {
        startPosition = transform.position;
        SetTargetPosition(isMovingForward);
    }
    
    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetPosition, 
            moveSpeed * Time.deltaTime
        );
        
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMovingForward = !isMovingForward;
            SetTargetPosition(isMovingForward);
        }
    }
    
    private void SetTargetPosition(bool movingForward)
    {
        Vector3 direction = GetDirectionVector();
        
        if (movingForward)
        {
            targetPosition = startPosition + (direction * moveDistance);
        }
        else
        {
            targetPosition = startPosition;
        }
    }
    
    private Vector3 GetDirectionVector()
    {
        switch (movementAxis)
        {
            case MovementAxis.Horizontal:
                return Vector3.right;
            case MovementAxis.Vertical:
                return Vector3.up;
            default:
                return Vector3.right;
        }
    }
}