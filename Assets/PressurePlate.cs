using System.IO;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    private bool isActivated = false;
    [SerializeField] Activable ConnectedObject;
    private Vector2 initialPos;
    private Vector2 targetPos;
    private GameObject pressurePlate;


    private void Start()
    {
        pressurePlate = GetComponentInChildren<SpringJoint2D>().gameObject;
        initialPos = new Vector2(pressurePlate.transform.position.x, pressurePlate.transform.position.y);
        targetPos = new Vector2(initialPos.x,initialPos.y-0.06f);
    }

    private void Update()
    {
        if(pressurePlate.transform.position.y <= targetPos.y && !isActivated && ConnectedObject != null)
        {
            isActivated = true;
            ConnectedObject.Activate();
        }
        else if (pressurePlate.transform.position.y > targetPos.y && isActivated && ConnectedObject != null)
        {
            isActivated = false;
            ConnectedObject.Deactivate();
        }
    }
}
