using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;            
    [SerializeField] float smoothTime = 0.15f;    
    [SerializeField] Vector2 offset;              
    [SerializeField] BoxCollider2D worldBounds;   
    [SerializeField] Camera cam;                 

    Vector3 velocity;

    void Awake()
    {
        if (cam == null) cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(target.position.x + offset.x,
                                      target.position.y + offset.y,
                                      transform.position.z);

        // Smooth follow
        Vector3 pos = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);

        // Optional clamp to level bounds
        if (worldBounds != null && cam.orthographic)
        {
            Bounds b = worldBounds.bounds;
            float vertExtent = cam.orthographicSize;
            float horzExtent = vertExtent * cam.aspect;

            float minX = b.min.x + horzExtent;
            float maxX = b.max.x - horzExtent;
            float minY = b.min.y + vertExtent;
            float maxY = b.max.y - vertExtent;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }

        transform.position = pos;
    }
}
