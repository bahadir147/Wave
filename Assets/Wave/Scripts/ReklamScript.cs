using UnityEngine;
using System.Collections;
using System;
using GoogleMobileAds.Api;

public class ReklamScript : MonoBehaviour
{
    private static ReklamScript instance = null;

    [Header("Kimlikler")]
    public string uygulamaKimligi;
    public string bannerKimligi;
    public string interstitialKimligi;
    public string rewardedVideoKimligi;

    [Header("Test Modu")]
    public bool testModu = false;
    public string testDeviceID;

    [Header("Diğer Ayarlar")]
    public bool cocuklaraYonelikReklamGoster = false;
    public AdPosition bannerPozisyonu = AdPosition.Top;

    private BannerView bannerReklam;
    private InterstitialAd interstitialReklam;

    private float interstitialIstekTimeoutZamani;
    private float rewardedVideoIstekTimeoutZamani;

    private IEnumerator interstitialGosterCoroutine;
    private IEnumerator rewardedVideoGosterCoroutine;

    public delegate void RewardedVideoOdul(Reward odul);
    private RewardedVideoOdul odulDelegate;


    private static int count = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);

            MobileAds.Initialize(uygulamaKimligi);

            BannerReklamYukle();
            InterstitialReklamYukle();
            RewardedReklamYukle();
        }
        else if (this != instance)
            Destroy(this);
    }

    private void BannerReklamYukle()
    {
        if (string.IsNullOrEmpty(bannerKimligi))
            return;

        if (bannerReklam != null)
            bannerReklam.Destroy();

        if (testModu && string.IsNullOrEmpty(testDeviceID))
        {
#if UNITY_ANDROID
            bannerReklam = new BannerView("ca-app-pub-3940256099942544/6300978111", AdSize.SmartBanner, bannerPozisyonu);
#elif UNITY_IOS
            bannerReklam = new BannerView( "ca-app-pub-3940256099942544/2934735716", AdSize.SmartBanner, bannerPozisyonu );
#endif
        }
        else
            bannerReklam = new BannerView(bannerKimligi, AdSize.SmartBanner, bannerPozisyonu);

        bannerReklam.OnAdFailedToLoad += BannerYuklenemedi;
        bannerReklam.LoadAd(ReklamIstegiOlustur());
        bannerReklam.Hide();
    }

    private void InterstitialReklamYukle()
    {
        if (string.IsNullOrEmpty(interstitialKimligi))
            return;

        if (interstitialReklam != null)
            interstitialReklam.Destroy();

        if (testModu && string.IsNullOrEmpty(testDeviceID))
        {
#if UNITY_ANDROID
            interstitialReklam = new InterstitialAd("ca-app-pub-3940256099942544/1033173712");
#elif UNITY_IOS
            interstitialReklam = new InterstitialAd( "ca-app-pub-3940256099942544/4411468910" );
#endif
        }
        else
            interstitialReklam = new InterstitialAd(interstitialKimligi);

        interstitialReklam.OnAdClosed += InterstitialDelegate;
        interstitialReklam.OnAdFailedToLoad += InterstitialYuklenemedi;
        interstitialReklam.LoadAd(ReklamIstegiOlustur());

        interstitialIstekTimeoutZamani = Time.realtimeSinceStartup + 10f;
    }

    private void RewardedReklamYukle()
    {
        if (string.IsNullOrEmpty(rewardedVideoKimligi))
            return;

        RewardBasedVideoAd rewardedReklam = RewardBasedVideoAd.Instance;
        rewardedReklam.OnAdClosed -= RewardedVideoDelegate;
        rewardedReklam.OnAdClosed += RewardedVideoDelegate;
        rewardedReklam.OnAdRewarded -= RewardedVideoOdullendir;
        rewardedReklam.OnAdRewarded += RewardedVideoOdullendir;

        if (testModu && string.IsNullOrEmpty(testDeviceID))
        {
#if UNITY_ANDROID
            rewardedReklam.LoadAd(ReklamIstegiOlustur(), "ca-app-pub-3940256099942544/5224354917");
#elif UNITY_IOS
            rewardedReklam.LoadAd( ReklamIstegiOlustur(), "ca-app-pub-3940256099942544/1712485313" );
#endif
        }
        else
            rewardedReklam.LoadAd(ReklamIstegiOlustur(), rewardedVideoKimligi);

        rewardedVideoIstekTimeoutZamani = Time.realtimeSinceStartup + 30f;
    }

    private AdRequest ReklamIstegiOlustur()
    {
        AdRequest.Builder reklamIstegi = new AdRequest.Builder();

        if (testModu && !string.IsNullOrEmpty(testDeviceID))
            reklamIstegi.AddTestDevice(testDeviceID);

        if (cocuklaraYonelikReklamGoster)
            reklamIstegi.TagForChildDirectedTreatment(true);

        return reklamIstegi.Build();
    }

    private void InterstitialDelegate(object sender, EventArgs args)
    {
        InterstitialReklamYukle();
    }

    private void RewardedVideoDelegate(object sender, EventArgs args)
    {
        RewardedReklamYukle();
    }

    private void BannerYuklenemedi(object sender, AdFailedToLoadEventArgs args)
    {
        if (bannerReklam != null)
        {
            bannerReklam.Destroy();
            bannerReklam = null;
        }
    }

    private void InterstitialYuklenemedi(object sender, AdFailedToLoadEventArgs args)
    {
        if (interstitialReklam != null)
        {
            interstitialReklam.Destroy();
            interstitialReklam = null;
        }
    }

    //private void OnGUI()
    //{
    //  Color c = GUI.color;

    //  if( GUI.Button( new Rect( Screen.width / 2 - 150, 0, 300, 120 ), "Banner Goster" ) )
    //      ReklamScript.BannerGoster();

    //  if( GUI.Button( new Rect( Screen.width / 2 - 150, 120, 300, 120 ), "Banner Gizle" ) )
    //      ReklamScript.BannerGizle();

    //  GUI.color = InterstitialHazirMi() ? Color.green : Color.red;
    //  if( GUI.Button( new Rect( Screen.width / 2 - 150, 240, 300, 120 ), "Interstitial Goster" ) )
    //      ReklamScript.InsterstitialGoster();

    //  GUI.color = RewardedReklamHazirMi() ? Color.green : Color.red;
    //  if( GUI.Button( new Rect( Screen.width / 2 - 150, 360, 300, 120 ), "Rewarded Goster" ) )
    //      ReklamScript.RewardedReklamGoster( null );

    //  GUI.color = c;
    //}

    public static void BannerReklamAl()
    {
        if (instance == null)
            return;

        instance.BannerReklamYukle();
    }

    public static void BannerGoster()
    {
        if (instance == null)
            return;

        if (instance.bannerReklam == null)
        {
            instance.BannerReklamYukle();
            if (instance.bannerReklam == null)
                return;
        }

        instance.bannerReklam.Show();
    }

    public static void BannerGizle()
    {
        if (instance == null)
            return;

        if (instance.bannerReklam == null)
            return;

        instance.bannerReklam.Hide();
    }

    public static bool InterstitialHazirMi()
    {
        if (instance == null)
            return false;

        if (instance.interstitialReklam == null)
            return false;

        return instance.interstitialReklam.IsLoaded();
    }

    public static void InterstitialReklamAl()
    {
        if (instance == null)
            return;

        if (instance.interstitialReklam != null && instance.interstitialReklam.IsLoaded())
            return;

        instance.InterstitialReklamYukle();
    }

    public static void InsterstitialGoster()
    {
        count++;
        if (count != 3)
            return;
        else count = 0;

        if (instance == null)
            return;

        if (instance.interstitialReklam == null)
        {
            instance.InterstitialReklamYukle();
            if (instance.interstitialReklam == null)
                return;
        }

        if (instance.interstitialGosterCoroutine != null)
        {
            instance.StopCoroutine(instance.interstitialGosterCoroutine);
            instance.interstitialGosterCoroutine = null;
        }

        if (instance.interstitialReklam.IsLoaded())
            instance.interstitialReklam.Show();
        else
        {
            if (Time.realtimeSinceStartup >= instance.interstitialIstekTimeoutZamani)
                instance.InterstitialReklamYukle();

            instance.interstitialGosterCoroutine = instance.InsterstitialGosterCoroutine();
            instance.StartCoroutine(instance.interstitialGosterCoroutine);
        }
    }

    public static bool RewardedReklamHazirMi()
    {
        if (instance == null)
            return false;

        return RewardBasedVideoAd.Instance.IsLoaded();
    }

    public static void RewardedReklamAl()
    {
        if (instance == null)
            return;

        if (RewardBasedVideoAd.Instance.IsLoaded())
            return;

        instance.RewardedReklamYukle();
    }

    public static void RewardedReklamGoster(RewardedVideoOdul odulFonksiyonu)
    {
        if (instance == null)
            return;

        if (string.IsNullOrEmpty(instance.rewardedVideoKimligi))
            return;

        if (instance.rewardedVideoGosterCoroutine != null)
        {
            instance.StopCoroutine(instance.rewardedVideoGosterCoroutine);
            instance.rewardedVideoGosterCoroutine = null;
        }

        instance.odulDelegate = odulFonksiyonu;

        RewardBasedVideoAd rewardedReklam = RewardBasedVideoAd.Instance;
        if (rewardedReklam.IsLoaded())
            rewardedReklam.Show();
        else
        {
            if (Time.realtimeSinceStartup >= instance.rewardedVideoIstekTimeoutZamani)
                instance.RewardedReklamYukle();

            instance.rewardedVideoGosterCoroutine = instance.RewardedVideoGosterCoroutine();
            instance.StartCoroutine(instance.rewardedVideoGosterCoroutine);
        }
    }

    private IEnumerator InsterstitialGosterCoroutine()
    {
        float istekTimeoutAni = Time.realtimeSinceStartup + 2.5f;
        while (!interstitialReklam.IsLoaded())
        {
            if (Time.realtimeSinceStartup > istekTimeoutAni)
                yield break;

            yield return null;

            if (interstitialReklam == null)
                yield break;
        }

        interstitialReklam.Show();
    }

    private IEnumerator RewardedVideoGosterCoroutine()
    {
        RewardBasedVideoAd rewardedReklam = RewardBasedVideoAd.Instance;
        float istekTimeoutAni = Time.realtimeSinceStartup + 10f;
        while (!rewardedReklam.IsLoaded())
        {
            if (Time.realtimeSinceStartup > istekTimeoutAni)
                yield break;

            yield return null;
        }

        rewardedReklam.Show();
    }

    private void RewardedVideoOdullendir(object sender, Reward odul)
    {
        if (odulDelegate != null)
            odulDelegate(odul);
    }
}