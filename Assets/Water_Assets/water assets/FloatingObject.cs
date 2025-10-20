using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [Header("Water Settings")]
    public Transform waterPlane;
    public float waveFrequency = 1f;
    public float waveAmplitude = 0.5f;
    public float waveSpeed = 1f;
    public Vector2 waveDirection = new Vector2(1f, 0f); // base direction

    [Header("Buoyancy Settings")]
    public float floatStrength = 10f;
    public float damping = 2f;
    public float waterDrag = 1f;

    [Header("Random Scatter Settings")]
    public float heightOffsetRange = 0.3f;
    public float phaseOffsetRange = 2f;

    [Header("Random Direction Settings")]
    public bool randomizeDirection = true;
    private Vector2 randomizedDirection;

    [Header("Drift Settings 🌊")]
    public float driftSpeed = 0.2f;           // how fast it drifts away
    public float driftAcceleration = 0.01f;   // how fast drift increases over time
    public float maxDriftSpeed = 1.5f;        // maximum drift speed cap
    private Vector3 driftDirection;
    private float currentDriftSpeed = 0f;

    private Rigidbody rb;
    private bool floatingActivated = false;
    private float heightOffset;
    private float phaseOffset;

    void Awake()
    {
        MeshCollider mc = GetComponent<MeshCollider>();
        if (mc != null)
            mc.convex = true;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        // 🎲 Random offsets
        heightOffset = Random.Range(-heightOffsetRange, heightOffsetRange);
        phaseOffset = Random.Range(-phaseOffsetRange, phaseOffsetRange);

        // 🎲 Randomize wave direction per object
        if (randomizeDirection)
        {
            float randomAngle = Random.Range(0f, 360f);
            randomizedDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
        }
        else
        {
            randomizedDirection = waveDirection.normalized;
        }

        // 🎲 Random drift direction (slow spread)
        float driftAngle = Random.Range(0f, 360f);
        driftDirection = new Vector3(Mathf.Cos(driftAngle * Mathf.Deg2Rad), 0f, Mathf.Sin(driftAngle * Mathf.Deg2Rad));
    }

    void FixedUpdate()
    {
        if (waterPlane == null) return;

        Vector3 pos = transform.position;

        // 🌊 Wave motion (same as before)
        float wave = Mathf.Sin(
            Vector2.Dot(new Vector2(pos.x, pos.z), randomizedDirection) * waveFrequency
            + Time.time * waveSpeed
            + phaseOffset
        ) * waveAmplitude;

        float waterHeight = waterPlane.position.y + wave + heightOffset;

        if (!floatingActivated && pos.y < waterHeight)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            floatingActivated = true;
        }

        if (floatingActivated && pos.y < waterHeight)
        {
            float depth = waterHeight - pos.y;
            Vector3 force = Vector3.up * depth * floatStrength;
            force -= rb.linearVelocity * damping;
            rb.AddForce(force, ForceMode.Acceleration);

            rb.linearDamping = waterDrag;

            // 🌬 Gradually drift apart over time
            currentDriftSpeed = Mathf.Min(currentDriftSpeed + driftAcceleration * Time.fixedDeltaTime, maxDriftSpeed);
            rb.AddForce(driftDirection * currentDriftSpeed, ForceMode.Acceleration);
        }
        else
        {
            rb.linearDamping = 0f;
        }
    }
}
