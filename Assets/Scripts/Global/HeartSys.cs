using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class HeartSys : MonoBehaviour
{
    public Image[] hearts;
    public int currentHearts = 8;
    public bool isHelmetUsed = false;
    public GameNotifier gameNotifier;

    public Sprite yellowHeart;



    void Start()
    {
        hearts[8].gameObject.SetActive(false);
        hearts[9].gameObject.SetActive(false);
        hearts[10].gameObject.SetActive(false);

        // foreach (var heart in hearts)
        // {
        //     heart.sprite = yellowHeart;
        // }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) { TakeDamage(1); }
        if (Input.GetKeyDown(KeyCode.Alpha9)) { HelmetUsed(); }
    }

    public void HelmetUsed()
    {
        isHelmetUsed = true;
        Debug.Log("Helmet used, increasing hearts to 11");

        // currentHearts += 3;
        // hearts[8].gameObject.SetActive(true);
        // hearts[9].gameObject.SetActive(true);
        // hearts[10].gameObject.SetActive(true);

        if (currentHearts == 8)
        {
            currentHearts += 3;
            hearts[8].gameObject.SetActive(true);
            hearts[9].gameObject.SetActive(true);
            hearts[10].gameObject.SetActive(true);
        }
        else if (currentHearts == 7)
        {
            currentHearts += 3;
            hearts[7].sprite = yellowHeart;
            hearts[8].sprite = yellowHeart;
            hearts[9].sprite = yellowHeart;
            hearts[8].gameObject.SetActive(true);

        }
        else if (currentHearts == 6)
        {
            currentHearts += 3;
            hearts[6].sprite = yellowHeart;
            hearts[7].sprite = yellowHeart;
            hearts[8].sprite = yellowHeart;

        }

        else if (currentHearts == 5)
        {
            currentHearts += 3;
            hearts[5].sprite = yellowHeart;
            hearts[6].sprite = yellowHeart;
            hearts[7].sprite = yellowHeart;
        }


        else if (currentHearts == 4)
        {
            currentHearts += 3;
            hearts[4].sprite = yellowHeart;
            hearts[5].sprite = yellowHeart;
            hearts[6].sprite = yellowHeart;
        }

        else if (currentHearts == 3)
        {
            currentHearts += 3;
            hearts[3].sprite = yellowHeart;
            hearts[4].sprite = yellowHeart;
            hearts[5].sprite = yellowHeart;
        }

        else if (currentHearts == 2)
        {
            currentHearts += 3;
            hearts[2].sprite = yellowHeart;
            hearts[3].sprite = yellowHeart;
            hearts[4].sprite = yellowHeart;
        }

        else if (currentHearts == 1)
        {
            currentHearts += 3;
            hearts[1].sprite = yellowHeart;
            hearts[2].sprite = yellowHeart;
            hearts[3].sprite = yellowHeart;
        }


        AudioManager.Instance.PlaySFX(23);
        UpdateHearts();
    }


    public void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < currentHearts;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHearts -= amount;
        AudioManager.Instance.PlaySFX(16);

        currentHearts = Mathf.Clamp(currentHearts, 0, hearts.Length);

        //Plays break sfx and game notif
        if (isHelmetUsed && currentHearts <= 8) { AudioManager.Instance.PlaySFX(24); gameNotifier.HelmetBreak(); isHelmetUsed = false; }

        UpdateHearts();
    }

    public void Heal(int amount)
    {
        currentHearts += amount;
        currentHearts = Mathf.Clamp(currentHearts, 0, hearts.Length);
        UpdateHearts();
    }
}
