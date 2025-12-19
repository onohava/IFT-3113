using UnityEngine;

public class lava : MonoBehaviour
{
    private float speed = 0.04f;

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0,speed * Time.deltaTime,0);
        
    }
}
