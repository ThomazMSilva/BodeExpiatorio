
using UnityEngine;

public class MovementStopDebugger : MonoBehaviour
{
    public Rigidbody rb;
    private float lastInputReleaseTime;
    private float lastStoppedMovingTime;

    private bool isInputReleased;
    private bool isStoppedMoving;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check for horizontal movement input (you can modify this for other axes or keys)
        float input = Input.GetAxis("Horizontal");

        if (input == 0 && !isInputReleased)
        {
            lastInputReleaseTime = Time.time;
            isInputReleased = true;
            Debug.Log($"Movement input stopped at: {lastInputReleaseTime}");
        }
        else if (input != 0)
        {
            isInputReleased = false;
        }
    }

    void FixedUpdate()
    {
        // Check if the Rigidbody's velocity has almost stopped (threshold to account for small drifts)
        if (rb.velocity.magnitude < 0.01f && !isStoppedMoving)
        {
            lastStoppedMovingTime = Time.time;
            isStoppedMoving = true;
            Debug.Log($"Rigidbody stopped moving at: {lastStoppedMovingTime}");

            // Compare the times
            float timeDifference = lastStoppedMovingTime - lastInputReleaseTime;
            Debug.Log($"Time between input release and rigidbody stopping: {timeDifference} seconds");
        }
        else if (rb.velocity.magnitude >= 0.01f)
        {
            isStoppedMoving = false;
        }
    }
}
