using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpPower = 10f;
    public float jumpTime = 1f;
    public float minJumpPower = 5f;
    public float maxJumpPower = 15f;

    private Rigidbody rb;
    private float jumpTimer;
    private bool isJumping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            isJumping = true;
            jumpTimer = 0f;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            jumpTimer += Time.deltaTime;
            if (jumpTimer < jumpTime)
            {
                float jumpHeightRatio = jumpTimer / jumpTime;
                float currentJumpPower = Mathf.Lerp(minJumpPower, maxJumpPower, jumpHeightRatio);
                rb.velocity = new Vector3(rb.velocity.x, currentJumpPower, rb.velocity.z);
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && isJumping)
        {
            isJumping = false;
        }
    }
}