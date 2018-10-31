using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System;
using GoogleMobileAds.Api;

public class GameManager : MonoBehaviour
{

    int score = 0;
    public TextMeshProUGUI CurrentScoreTextTMPro;
    public TextMeshProUGUI BestScoreTextTMPro;

    public GameObject GameOverPanel;
    public GameObject GameOverEffectPanel;

    public GameObject touchToMoveTextObj;

    public GameObject StartFadeInObj;

    public static int BeforeScore = 0;

    public Button ContinueButton;


    static int PlayCount;
    public string PlayStoreURL;

    void Awake()
    {
        Application.targetFrameRate = 60;


        Time.timeScale = 1.0f;


        BestScoreTextTMPro.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
        StartCoroutine(FadeIn());
    }

    private void Start()
    {
        OneSignal.StartInit("7994bb6b-6d39-4875-9107-6b4f6fc01d20")
  .HandleNotificationOpened(HandleNotificationOpened)
  .EndInit();

        OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
        ReklamScript.RewardedReklamAl();
        ReklamScript.BannerReklamAl();
        ReklamScript.InterstitialReklamAl();
        Invoke("BannerAc", 0.5f);
        if (BeforeScore != 0)
        {
            CurrentScoreTextTMPro.text = BeforeScore.ToString();
            score = BeforeScore;
        }
    }
    void BannerAc()
    {
        ReklamScript.BannerGoster();
    }

    private void HandleNotificationOpened(OSNotificationOpenedResult result)
    {
        throw new NotImplementedException();
    }

    void Update()
    {
        if (touchToMoveTextObj.activeSelf == false) return;
        if (Input.GetMouseButton(0))
        {
            touchToMoveTextObj.SetActive(false);
        }
    }

    IEnumerator FadeIn()
    {
        StartFadeInObj.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        StartFadeInObj.SetActive(false);
        yield break;
    }

    public void addScore()
    {
        score++;
        CurrentScoreTextTMPro.text = score.ToString();

        if (score > PlayerPrefs.GetInt("BestScore", 0))
        {
            PlayerPrefs.SetInt("BestScore", score);
            BestScoreTextTMPro.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
        }
    }

    public void Gameover()
    {
        ContinueButton.interactable = ReklamScript.RewardedReklamHazirMi();
        ReklamScript.InsterstitialGoster();
        StartCoroutine(GameoverCoroutine());
    }

    IEnumerator GameoverCoroutine()
    {
        CurrentScoreTextTMPro.gameObject.GetComponent<Animator>().SetTrigger("GameOver");
        GameOverEffectPanel.SetActive(true);
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.5f);
        GameOverPanel.SetActive(true);
        yield break;
    }

    public void Restart()
    {
        BeforeScore = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ContinueGame()
    {
        ReklamScript.RewardedReklamGoster(odulFonksiyonu);
    }

    private void odulFonksiyonu(Reward odul)
    {
        BeforeScore = Convert.ToInt32(CurrentScoreTextTMPro.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Share()
    {
        StartCoroutine(ShareScreenshot());

    }

    public void RateUs()
    {
        Application.OpenURL(PlayStoreURL);
    }

    private IEnumerator ShareScreenshot()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        //Screenshot'ı geçici bir dosyaya yaz
        string dosyaKonumu = Path.Combine(Application.temporaryCachePath, "geçici resim.png");
        File.WriteAllBytes(dosyaKonumu, ss.EncodeToPNG());

        // Artık screenshot'a ihtiyacımız kalmadı
        Destroy(ss);

        //Dosyayı yazı ve konu eşliğinde paylaş
        new NativeShare().AddFile(dosyaKonumu).SetSubject("Wave").SetText("Best Game For Ever!").Share();
    }
}
