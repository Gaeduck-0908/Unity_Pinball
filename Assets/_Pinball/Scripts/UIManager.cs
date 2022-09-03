using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using SgLib;

#if EASY_MOBILE
using EasyMobile;
#endif

public class UIManager : MonoBehaviour
{
    public static bool firstLoad = true;

    public GameManager gameManager;
    public Text score;
    public Text scoreInScoreBg;
    public Text bestScore;
    public GameObject buttons;
    public Button muteBtn;
    public Button unMuteBtn;


    [Header("Premium Buttons")]
    public GameObject leaderboardBtn;
    public GameObject achievementBtn;
    public GameObject removeAdsBtn;
    public GameObject restorePurchaseBtn;
    public GameObject shareBtn;

    Animator scoreAnimator;
    bool hasCheckedGameOver = false;

    void OnEnable()
    {
        ScoreManager.ScoreUpdated += OnScoreUpdated;
    }

    void OnDisable()
    {
        ScoreManager.ScoreUpdated -= OnScoreUpdated;
    }

    // Use this for initialization
    void Start()
    {
        scoreAnimator = score.GetComponent<Animator>();
        score.gameObject.SetActive(false);
        scoreInScoreBg.text = ScoreManager.Instance.Score.ToString();


        // Show or hide premium buttons
        bool enablePremium = PremiumFeaturesManager.Instance.enablePremiumFeatures;
        leaderboardBtn.SetActive(enablePremium);
        achievementBtn.SetActive(enablePremium);
        removeAdsBtn.SetActive(enablePremium);
        restorePurchaseBtn.SetActive(enablePremium);
        shareBtn.SetActive(false);  // share button only shows when game over
            

        if (!firstLoad)
        {
            HideAllButtons();
        }
    }

    // Update is called once per frame
    void Update()
    {
        score.text = ScoreManager.Instance.Score.ToString();
        bestScore.text = ScoreManager.Instance.HighScore.ToString();
        UpdateMuteButtons();
        if (gameManager.gameOver && !hasCheckedGameOver)
        {
            hasCheckedGameOver = true;
            Invoke("ShowButtons", 1f);
        }
    }

    void OnScoreUpdated(int newScore)
    {
        scoreAnimator.Play("NewScore");
    }

    public void HandlePlayButton()
    {
        if (!firstLoad)
        {
            StartCoroutine(Restart());
        }
        else
        {
            HideAllButtons();
            gameManager.StartGame();
            gameManager.CreateBall();
            firstLoad = false;
        }
    }

    public void ShowButtons()
    {
        buttons.SetActive(true);
        score.gameObject.SetActive(false);
        scoreInScoreBg.text = ScoreManager.Instance.Score.ToString();

        bool enablePremium = PremiumFeaturesManager.Instance.enablePremiumFeatures;
        leaderboardBtn.SetActive(enablePremium);
        achievementBtn.SetActive(enablePremium);
        removeAdsBtn.SetActive(enablePremium);
        restorePurchaseBtn.SetActive(enablePremium);
        shareBtn.SetActive(enablePremium);

    }

    public void HideAllButtons()
    {
        buttons.SetActive(false);
        score.gameObject.SetActive(true);
    }

    public void FinishLoading()
    {
        if (firstLoad)
        {
            ShowButtons();
        }
        else
        {
            HideAllButtons();
        }
    }

    void UpdateMuteButtons()
    {
        if (SoundManager.Instance.IsMuted())
        {
            unMuteBtn.gameObject.SetActive(false);
            muteBtn.gameObject.SetActive(true);
        }
        else
        {
            unMuteBtn.gameObject.SetActive(true);
            muteBtn.gameObject.SetActive(false);
        }
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowLeaderboardUI()
    {
        #if EASY_MOBILE
        if (GameServices.IsInitialized())
        {
            GameServices.ShowLeaderboardUI();
        }
        else
        {
        #if UNITY_IOS
            NativeUI.Alert("Service Unavailable", "The user is not logged in to Game Center.");
        #elif UNITY_ANDROID
            GameServices.Init();
        #endif
        }
        #endif
    }

    public void ShowAchievementUI()
    {
        #if EASY_MOBILE
        if (GameServices.IsInitialized())
        {
            GameServices.ShowAchievementsUI();
        }
        else
        {
        #if UNITY_IOS
            NativeUI.Alert("Service Unavailable", "The user is not logged in to Game Center.");
        #elif UNITY_ANDROID
            GameServices.Init();
        #endif
        }
        #endif
    }

    public void PurchaseRemoveAds()
    {
        #if EASY_MOBILE
        InAppPurchaser.Instance.Purchase(InAppPurchaser.Instance.removeAds);
        #endif
    }

    public void RestorePurchase()
    {
        #if EASY_MOBILE
        InAppPurchaser.Instance.RestorePurchase();
        #endif
    }

    public void ShareScreenshot()
    {
        StartCoroutine("TakeScreenshotAndShare");
    }

    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        new NativeShare().AddFile(filePath)
            .SetSubject("Subject goes here").SetText("Hello world!").SetUrl("https://github.com/yasirkula/UnityNativeShare")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    }

    public void ToggleSound()
    {
        SoundManager.Instance.ToggleMute();
    }

    public void RateApp()
    {
        Utilities.Instance.RateApp();
    }

    public void OpenInstaPage()
    {
        Utilities.Instance.OpenInstaPage();
    }

    public void OpenFacebookPage()
    {
        Utilities.Instance.OpenFacebookPage();
    }
}
