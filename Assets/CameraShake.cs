using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;

    void Awake() => originalPos = transform.localPosition;

    public void TriggerShake(float magnitude)
    {
        float x = Random.Range(-1f, 1f) * magnitude;
        float y = Random.Range(-1f, 1f) * magnitude;
        transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
    }
}