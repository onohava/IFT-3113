using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SharedViewCamera : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    [Header("Margins (how far player can move before camera follows)")]
    public float xMargin = 3f;
    public float yMargin = 2f;
    public float playerDistance = 2f;

    [Header("Smoothness")]
    public float smoothTime = 0.2f;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (player1 == null) return;

        Vector3 targetPos = transform.position;
        //If the players are together, use shared-view camera logic
        if(Vector2.Distance(player1.position, player2.position) < playerDistance)
        {
            // Check X margin
            if (Mathf.Abs(transform.position.x - player1.position.x) > xMargin)
            {
                targetPos.x = Mathf.Lerp(transform.position.x, player1.position.x, smoothTime);
            }

            // Check Y margin
            if (Mathf.Abs(transform.position.y - player1.position.y) > yMargin)
            {
                targetPos.y = Mathf.Lerp(transform.position.y, player1.position.y, smoothTime);
            }
        }


        // Keep the same Z (camera depth)
        targetPos.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }
}
