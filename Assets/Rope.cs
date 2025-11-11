using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rope : MonoBehaviour
{
    private HingeJoint2D playerGrabPoint;
    private HingeJoint2D hinge;
    private bool playerOnRope = false;

    Rope[] otherRopes;

    private float speed = 100f;
    private float torque = 30f;
    private float inputForce = 1f;
    private float mass = 5f;

    private void Start()
    {
        hinge = GetComponent<HingeJoint2D>();
        hinge.useMotor = true;
        JointAngleLimits2D jointAngle = new JointAngleLimits2D();
        jointAngle.max = 10;
        jointAngle.min = -10;
        hinge.limits = jointAngle;
        otherRopes = transform.parent.GetComponentsInChildren<Rope>();
        gameObject.GetComponent<Rigidbody2D>().mass = mass;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !playerOnRope)
        {
            Debug.Log("Player attached to rope");
            collision.gameObject.GetComponentInParent<PlayerMovementInputSystem>().GrabRope(collision.gameObject, gameObject);
            playerGrabPoint = gameObject.AddComponent<HingeJoint2D>();
            playerGrabPoint.connectedBody = collision.attachedRigidbody;
            playerGrabPoint.anchor = Vector2.zero;
            playerGrabPoint.connectedAnchor = Vector2.zero;
            
            foreach(Rope rope in otherRopes)
            {
                if (rope != this)
                    rope.playerOnRope = true;
            }

            playerOnRope = true;
        }

    }

    private void Update()
    {

    }

    public void ControlRope(float input)
    {
        foreach(Rope rope in otherRopes)
        {
            rope.hinge.useMotor = true;
            JointMotor2D motor = rope.hinge.motor;
            motor.motorSpeed = -input * rope.speed;
            motor.maxMotorTorque = rope.torque;
            rope.hinge.motor = motor;
        }
    }

    public void LetGoRope()
    {
        foreach (Rope rope in otherRopes)
        {
            rope.playerOnRope = false;
            JointMotor2D motor = rope.hinge.motor;
            motor.motorSpeed = 0;
            motor.maxMotorTorque = 0;
            rope.hinge.motor = motor;
            rope.hinge.useMotor = false;
        }
        Destroy(playerGrabPoint);
        StartCoroutine(DisableCollider());
    }

    IEnumerator DisableCollider()
    {
        foreach (Rope rope in otherRopes)
        {
            rope.gameObject.GetComponent<Collider2D>().enabled = false;
        }
        yield return new WaitForSeconds(0.5f);
        foreach (Rope rope in otherRopes)
        {
            rope.gameObject.GetComponent<Collider2D>().enabled = true;
        }
    }
}
