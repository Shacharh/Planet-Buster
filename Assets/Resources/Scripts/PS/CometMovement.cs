using UnityEngine;

public class CometMovement : MonoBehaviour
{
    [Header("Settings")]
    public float minSpeed = 3.0f;
    public float maxSpeed = 6.0f;
    public float curveStrength = 1.5f; // How much it deviates from the straight line

    private Vector3 moveDirection;
    private float speed;
    private float startTime;
    private float curveOffsetSeed; // Random seed for unique curves

    public void Initialize(Vector3 start, Vector3 target)
    {
        startTime = Time.time;
        speed = Random.Range(minSpeed, maxSpeed);
        
        // Calculate the straight line direction
        moveDirection = (target - start).normalized;
        
        // Rotate sprite to face the target (optional)
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        // Random offset so they don't all curve the same way
        curveOffsetSeed = Random.Range(0f, 100f);
    }

    void Update()
    {
        // 1. Move forward in the main direction
        transform.position += moveDirection * speed * Time.deltaTime;

        // 2. Add "Noise" or Curve perpendicular to movement
        // We calculate a 'right' vector relative to our movement
        Vector3 perpendicular = new Vector3(-moveDirection.y, moveDirection.x, 0);
        
        // Sine wave based on time
        float wave = Mathf.Sin((Time.time * 2.0f) + curveOffsetSeed) * curveStrength * Time.deltaTime;
        
        transform.position += perpendicular * wave;

        // 3. Destroy if too far off screen (simple distance check or viewport check)
        if (!IsVisible())
        {
            Destroy(gameObject);
        }
    }

    bool IsVisible()
    {
        // Simple check: Convert to viewport. If x or y is WAY off, destroy.
        // We use a wide buffer (-0.2 to 1.2) because spawning happens outside too.
        // We only really care if it's gone off the BOTTOM.
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        return viewPos.y > -0.5f; 
    }
}