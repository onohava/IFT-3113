using UnityEngine;
using UnityEngine.UI;

public class AutoSplit : MonoBehaviour
{
    [Header("Players")]
    public Transform player1;
    public Transform player2;

    [Header("Cameras")]
    public Camera sharedCam;   // full screen when merged
    public Camera leftCam;     // left half when split
    public Camera rightCam;    // right half when split

    [Header("UI")]
    public Image dividerLine;  

    [Header("Distances / Timing")]
    public float splitDistance   = 12f;   
    public float hysteresis      = 1.5f; 
    public float followSmoothTime = 0.15f; 
    public float transitionTime   = 0.6f;  

    float splitBlend = 0f;  
    float targetBlend = 0f;

    Vector3 velLeft, velRight, velShared; 

    void Start()
    {
        if (leftCam)   leftCam.gameObject.SetActive(true);   // keep active so we can animate rect
        if (rightCam)  rightCam.gameObject.SetActive(true);
        if (sharedCam) sharedCam.gameObject.SetActive(true);

        ApplyRects(0f);
        SetDividerAlpha(0f);
    }

    void LateUpdate()
    {
        if (!player1 || !player2 || !sharedCam || !leftCam || !rightCam) return;

        float d = Vector2.Distance(player1.position, player2.position);
        if (splitBlend < 0.5f)
            targetBlend = (d > splitDistance) ? 1f : 0f;
        else
            targetBlend = (d > (splitDistance - hysteresis)) ? 1f : 0f;

        // animate splitBlend towards target
        float step = (transitionTime > 0f) ? (Time.deltaTime / transitionTime) : 1f;
        splitBlend = Mathf.MoveTowards(splitBlend, targetBlend, step);

        Vector3 mid = (player1.position + player2.position) * 0.5f;

        Vector3 leftTarget  = new Vector3(player1.position.x, player1.position.y, sharedCam.transform.position.z);
        Vector3 rightTarget = new Vector3(player2.position.x, player2.position.y, sharedCam.transform.position.z);
        Vector3 sharedTarget= new Vector3(mid.x,            mid.y,            sharedCam.transform.position.z);

        Vector3 lp = leftCam.transform.position;
        Vector3 rp = rightCam.transform.position;
        Vector3 sp = sharedCam.transform.position;

        leftCam.transform.position   = Vector3.SmoothDamp(lp, leftTarget,   ref velLeft,   followSmoothTime);
        rightCam.transform.position  = Vector3.SmoothDamp(rp, rightTarget,  ref velRight,  followSmoothTime);
        sharedCam.transform.position = Vector3.SmoothDamp(sp, sharedTarget, ref velShared, followSmoothTime);
        ApplyRects(splitBlend);
        SetDividerAlpha(splitBlend);
    }

    void ApplyRects(float t)
    {
        // t=0  => shared full, split cams width 0
        // t=1  => shared hidden, split cams 0.5 each

        float halfW = 0.5f * t;
        leftCam.rect  = new Rect(0.5f - halfW, 0f, halfW, 1f);
        rightCam.rect = new Rect(0.5f,         0f, halfW, 1f);

        float sharedW = 1f - t;
        float sharedX = (1f - sharedW) * 0.5f;
        sharedCam.rect = new Rect(sharedX, 0f, sharedW, 1f);

        // Enable/disable renderers only at extremes
        bool fullyMerged = t <= 0.0001f;
        bool fullySplit  = t >= 0.999f;

        sharedCam.enabled = !fullySplit;
        leftCam.enabled   = !fullyMerged;
        rightCam.enabled  = !fullyMerged;
    }

    void SetDividerAlpha(float t)
    {
        if (!dividerLine) return;
        Color c = dividerLine.color;
        c.a = Mathf.SmoothStep(0f, 1f, t); 
        dividerLine.color = c;
    }
}