using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Slider HealthSlider;
    private int currentHealth;
    private Coroutine healthRoutine;

    public Slider SpeedSlider;

    public Slider[] TorpSliders;

    public TextMeshProUGUI killText, duckText, timerText, waveText;

    [Header("Start/End Screen")]
    public CanvasGroup cg;
    public TextMeshProUGUI endText, statText, startText;

    private bool gameEnded;


    private float timer;

    void Awake() {
        Instance = this;
        gameEnded = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeBlackCor(false, 3f));
    }

    public void UpdateEnd(bool survived, int waveNum) {
        StartCoroutine(FadeBlackCor(true, 0.1f));

        startText.text = "";

        int totalSeconds = Mathf.FloorToInt(timer);
        int minutes = totalSeconds / 60;
        int secs = totalSeconds % 60;
        string timeString = $"{minutes}:{secs:00}";

        if (survived) {
            endText.text = "All Waves Cleared!";
        } else {
            endText.text = "Waves Survived: " + (waveNum - 1).ToString();
        }
        statText.text = "Time Spent: " + timeString + "\n \n Press any key to restart.";
        gameEnded = true;
    }

    IEnumerator FadeBlackCor(bool fadeToBlack, float startDelay) {
        yield return new WaitForSeconds(startDelay);

        float startAlpha = fadeToBlack ? 0 : 1;
        float endAlpha = fadeToBlack ? 1 : 0;

        float timer = 0;
        while (timer < 3) {
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / 3);

            timer += Time.deltaTime;
            yield return null;
        }
        cg.alpha = endAlpha;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        timerText.text = FormatMinutesSeconds(timer);

        if (gameEnded && Input.anyKeyDown) {
            gameEnded = false;
            BulletPool.Reset();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
    public string FormatMinutesSeconds(float seconds)
    {
        int totalSeconds = Mathf.FloorToInt(seconds);
        int minutes = totalSeconds / 60;
        int secs = totalSeconds % 60;

        return $"{minutes}:{secs:00}";
    }

    public void UpdateDucks( int ducks) {
        duckText.text = ducks.ToString();
    }

    public void UpdateWaveNum( int num) {
        waveText.text = "Wave " + num.ToString();
    }
    public void UpdateKills( int kills) {
        killText.text = kills.ToString();
    }

    public void UpdateTorps(int currNum, float reloadTimer, float reloadTime) {
        for (int i = 0; i < currNum; i++) {
            TorpSliders[i].value = reloadTime;
        }
        if (currNum < 4) {
            TorpSliders[currNum].value = reloadTime - reloadTimer;
        }
        for (int i = currNum + 1; i < 4; i++) {
            TorpSliders[i].value = 0;
        }
    }

    public void UpdateSpeed(float current, float max) {

        SpeedSlider.maxValue = max;
        SpeedSlider.value = current;
    }

    public void UpdateHealth(int current, int max) {
        currentHealth = current;

        HealthSlider.maxValue = max;

        if (healthRoutine != null)
            StopCoroutine(healthRoutine);

        healthRoutine = StartCoroutine(LerpHealth(currentHealth));
    }

    private IEnumerator LerpHealth(float targetValue) {
        float startValue = HealthSlider.value;
        float t = 0f;
        float duration = 0.25f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            HealthSlider.value = Mathf.Lerp(startValue, targetValue, t);
            yield return null;
        }

        HealthSlider.value = targetValue;
    }
}
