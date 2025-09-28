using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    [Header("Set in Inspector")]
    public int trialIndex;   // 0 = Door1, 1 = Door2, 2 = Door3
    public int stageIndex;   // 0 = Level1, 1 = Level2
    public string sceneToLoad = "GameScene"; // replace with your scene name

    public void OnClick()
    {
        // Save selection globally
        DataManager.Instance.currentTrial = trialIndex;
        DataManager.Instance.currentStage = stageIndex;

        Debug.Log($"Selected Trial {trialIndex}, Stage {stageIndex}");

        // Load the level
        SceneManager.LoadScene(sceneToLoad);
    }
}
