using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField] private Transform BridgeTarget;
    [SerializeField] private GameObject AnchorChain;
    private Vector2 initialPos;
    private float speed = 0.3f;

    private void Start()
    {
        initialPos = AnchorChain.transform.position;
    }

    private void Update()
    {
        if(GetComponent<Activable>() != null && GetComponent<Activable>().isActivated)
        {
            AnchorChain.transform.position = Vector2.MoveTowards(AnchorChain.transform.position, BridgeTarget.position, speed * Time.deltaTime);
            //AnchorChain.transform.position = Vector2.Lerp(AnchorChain.transform.position, BridgeTarget.position,Time.deltaTime * 1);
        }
        else if(GetComponent<Activable>() != null)
        {
            AnchorChain.transform.position = Vector2.MoveTowards(AnchorChain.transform.position, initialPos, speed * Time.deltaTime);
            //AnchorChain.transform.position = Vector2.Lerp(AnchorChain.transform.position, initialPos, Time.deltaTime * 1);
        }
    }


}
