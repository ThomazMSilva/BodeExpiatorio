using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TesteThwompMovel : MonoBehaviour
{
    private bool isFalling;
    private Vector3 gravity;
    private Vector3 risingForce = Vector3.zero;
    private Rigidbody rb;
    [SerializeField] private float fallingVelocity = 5f;
    [SerializeField] private float risingVelocity = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravity = Physics.gravity;
    }

    void FixedUpdate()
    {
        if (isFalling)
        {
            rb.AddForce(gravity * fallingVelocity, ForceMode.Acceleration);
        }
        else
        {
            risingForce.Set(0, -gravity.y * risingVelocity - rb.velocity.y, 0);
            rb.AddForce(risingForce, ForceMode.VelocityChange);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))isFalling = !isFalling;
    }
}
