using UnityEngine;

public class ItemInteract_Water : MonoBehaviour
{
    public FloodQuizSet quizSet;       // optional quiz for this item
    public string itemName;            // optional: name of the item
    public GameObject pickupEffect;    // optional: particle or VFX when picked

    private bool isCollected = false;
    private Renderer itemRenderer;
    private Collider itemCollider;

    private void Awake()
    {
        itemRenderer = GetComponentInChildren<Renderer>();
        itemCollider = GetComponent<Collider>();
    }

    public void Interact()
    {
        if (isCollected) return;

        if (quizSet != null && quizSet.questions.Count > 0)
        {
            var quiz = FindObjectOfType<FloodQuiz>();
            if (quiz != null)
            {
                quiz.BeginQuiz(quizSet.questions);
                quiz.OnQuizComplete += OnQuizFinished;
            }
        }
        else
        {
            CollectItem();
        }
    }

    private void OnQuizFinished(bool correct)
    {
        var quiz = FindObjectOfType<FloodQuiz>();
        if (quiz != null)
            quiz.OnQuizComplete -= OnQuizFinished;

        if (correct)
        {
            CollectItem();
        }
        else
        {
            Debug.Log("❌ Wrong answer! Item not collected.");
        }
    }

    void CollectItem()
    {
        isCollected = true;
        Debug.Log("✅ Collected item: " + itemName);

        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        // 🔹 Instead of destroying the object:
        //    Hide the mesh + disable collider, so it can’t be picked again
        if (itemRenderer != null)
            itemRenderer.enabled = false;

        if (itemCollider != null)
            itemCollider.enabled = false;

        // Optionally move it out of sight or mark collected in inventory
        // transform.position += Vector3.down * 100f;
    }
}
