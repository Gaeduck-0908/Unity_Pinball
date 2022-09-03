using UnityEngine;
using System.Collections;

#if EASY_MOBILE
using EasyMobile;
#endif

namespace SgLib
{
    public class AdDisplayer : MonoBehaviour
    {
        #if EASY_MOBILE
        public static AdDisplayer Instance { get; private set; }

        public static event System.Action CompleteRewardedAdToRecoverLostGame;
        public static event System.Action CompleteRewardedAdToEarnCoins;

        [Header("BANNER AD DISPLAY CONFIG")]
        [Tooltip("Whether or not to show banner ad")]
        public bool showBannerAd = true;
        public BannerAdPosition bannerAdPosition = BannerAdPosition.Bottom;

        [Header("INTERSTITIAL AD DISPLAY CONFIG")]
        [Tooltip("Whether or not to show interstitial ad")]
        public bool showInterstitialAd = true;
        [Tooltip("Show interstitial ad every [how many] games")]
        public int gamesPerInterstitial = 3;
        [Tooltip("How many seconds after game over that interstitial ad is shown")]
        public float showInterstitialDelay = 2f;


        [Header("REWARDED AD DISPLAY CONFIG")]
        [Tooltip("Check to allow watching ad to continue a lost game")]
        [HideInInspector]
        public bool watchAdToContinueGame = true;
        [Tooltip("Check to allow watching ad to earn coins")]
        [HideInInspector]
        public bool watchAdToEarnCoins = true;
        [Tooltip("How many coins the user earns after watching a rewarded ad")]
        [HideInInspector]
        public int rewardedCoins = 50;

        private static int gameCount = 0;

        void OnEnable()
        {
            GameManager.GameStateChanged += OnGameStateChanged;
        }

        void OnDisable()
        {
            GameManager.GameStateChanged -= OnGameStateChanged;
        }

        void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void Start()
        {
            // Show banner ad
            if (showBannerAd && !Advertising.IsAdRemoved())
            {
                Advertising.ShowBannerAd(bannerAdPosition);
            }
        }

        void OnGameStateChanged(GameState newState, GameState oldState)
        {       
            if (newState == GameState.Playing)
            {
                // This is when a new game starts being played.
            }
            else if (newState == GameState.GameOver)
            {
                // Show interstitial ad
                if (showInterstitialAd && !Advertising.IsAdRemoved())
                {
                    gameCount++;

                    if (gameCount >= gamesPerInterstitial)
                    {
                        if (Advertising.IsInterstitialAdReady())
                        {
                            // Show default ad after some optional delay
                            StartCoroutine(ShowInterstitial(showInterstitialDelay));

                            // Reset game count
                            gameCount = 0;
                        }
                    }
                }
            }
        }

        IEnumerator ShowInterstitial(float delay = 0f)
        {        
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            Advertising.ShowInterstitialAd();
        }

        public bool CanShowRewardedAd()
        {
            return Advertising.IsRewardedAdReady();
        }

        public void ShowRewardedAdToRecoverLostGame()
        {
            if (CanShowRewardedAd())
            {
                Advertising.RewardedAdCompleted += OnCompleteRewardedAdToRecoverLostGame;
                Advertising.ShowRewardedAd();
            }
        }

        void OnCompleteRewardedAdToRecoverLostGame(RewardedAdNetwork adNetwork, AdPlacement location)
        {
            // Unsubscribe
            Advertising.RewardedAdCompleted -= OnCompleteRewardedAdToRecoverLostGame;

            // Fire event
            if (CompleteRewardedAdToRecoverLostGame != null)
            {
                CompleteRewardedAdToRecoverLostGame();
            }
        }

        public void ShowRewardedAdToEarnCoins()
        {
            if (CanShowRewardedAd())
            {
                Advertising.RewardedAdCompleted += OnCompleteRewardedAdToEarnCoins;
                Advertising.ShowRewardedAd();
            }
        }

        void OnCompleteRewardedAdToEarnCoins(RewardedAdNetwork adNetwork, AdPlacement location)
        {
            // Unsubscribe
            Advertising.RewardedAdCompleted -= OnCompleteRewardedAdToEarnCoins;

            // Fire event
            if (CompleteRewardedAdToEarnCoins != null)
            {
                CompleteRewardedAdToEarnCoins();
            }
        }
        #endif
    }
}
