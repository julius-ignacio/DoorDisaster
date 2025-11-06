using UnityEngine;

public class NpcController : MonoBehaviour
{
    public float moveSpeed = 2f;           // movement speed
    public Vector3 moveDirection = Vector3.forward; // direction to move
    public float fadeDuration = 2f;        // seconds to fade out

    private Animator animator;
    private SkinnedMeshRenderer meshRenderer;
    private bool isFading = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    void Update()
    {
        // Move in given direction until fading starts
        if (!isFading)
        {
            transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    public void StartFadeAndDestroy()
    {
        if (!isFading)
        {
            StartCoroutine(FadeOut());
        }
    }

    private System.Collections.IEnumerator FadeOut()
    {
        isFading = true;

        Material mat = meshRenderer.material;
        Color startColor = mat.color;
        float elapsed = 0f;

        // make sure material supports transparency
        mat.SetFloat("_Mode", 2); 
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            mat.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject); // remove NPC
    }
}
