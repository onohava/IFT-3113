using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Sprite OpenedDoor;
    [SerializeField] private Sprite ClosedDoor;
    private SpriteRenderer sr;

    private void Start()
    {        
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GetComponent<Activable>())
        {
            if(GetComponent<Activable>().isActivated)
            {
                sr.sprite = OpenedDoor;
                GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                sr.sprite = ClosedDoor;
                GetComponent<BoxCollider2D>().enabled = true;
            }
        } 
    }

}
