using UnityEngine;

public class Activable : MonoBehaviour
{
    private enum ActivationType {AND, OR, XOR};
    [SerializeField] ActivationType activationType;
    public bool isActivated = false;
    [Header("Change this only for AND logic, else it should remains at 1")]
    public int minActivations = 1;

    private int activationCounter = 0;

    public void Activate()
    {
        activationCounter++;
    }

    public void Deactivate()
    {
        activationCounter--;
    }

    private void Update()
    {
        if (activationType == ActivationType.AND) {
            if (activationCounter == minActivations)
                isActivated = true;
            else
                isActivated = false;
        }
        else if (activationType == ActivationType.OR)
        {
            if (activationCounter >= 1)
                isActivated = true;
            else
                isActivated = false;
        }
        else if(activationType == ActivationType.XOR)
        {
            if (activationCounter == 1)
                isActivated = true;
            else
                isActivated = false;
        }


    }
}
