using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if EASY_MOBILE
using EasyMobile;
#endif

namespace SgLib
{
    public class RatingRequester : MonoBehaviour
    {
        public enum RequestMode
        {
            GameBased,
            TimeBased

        }

        [Header("Select rating request mode")]
        public RequestMode requestMode;

        [Header("Game-based rating request settings")]
        [Range(3, 500)]
        public int gamesPlayedAfterInstall = 2;
        [Range(3, 500)]
        public int gamesPlayedBetweenRequests = 10;

        [Header("Time-based rating request settings")]
        [Range(3, 300)]
        public int daysAfterInstall = 14;
        [Range(3, 300)]
        public int daysBetweenRequests = 14;


        #if EASY_MOBILE
        public static RatingRequester Instance { get; private set; }

        private const string GAMES_PLAYED_PPK = "SGLIB_GAMES_PLAYED";
        private const string INSTALL_TIMESTAMP_PPK = "SGLIB_INSTALL_TIMESTAMP";
        private const string LAST_REQUEST_GAMES_PLAYED_PPK = "SGLIB_LAST_REQUEST_GAMES_PLAYED";
        private const string LAST_REQUEST_TIME_PPK = "SGLIB_LAST_REQUEST_TIME";

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

        void OnEnable()
        {
            GameManager.GameStateChanged += GameManager_GameStateChanged;
        }

        void OnDisable()
        {
            GameManager.GameStateChanged -= GameManager_GameStateChanged;
        }

        void Start()
        {
            // Record install time
            DateTime defaultTime = new DateTime(1970, 1, 1);
            if (Utilities.GetTime(INSTALL_TIMESTAMP_PPK, defaultTime).Equals(defaultTime))
            {
                Utilities.StoreTime(INSTALL_TIMESTAMP_PPK, DateTime.Now);
            }
        }

        void GameManager_GameStateChanged(GameState newState, GameState oldState)
        {
            if (newState == GameState.Playing)
            {
                // A new game has started.
                SetGamesPlayed(GetGamesPlayed() + 1);
            }
            else if (newState == GameState.GameOver)
            {
                if (CanRequestNow())
                    StartCoroutine(MakeRatingRequest(3f));
            }
        }

        int GetGamesPlayed()
        {
            return PlayerPrefs.GetInt(GAMES_PLAYED_PPK, 0);
        }

        void SetGamesPlayed(int games)
        {
            PlayerPrefs.SetInt(GAMES_PLAYED_PPK, games);
        }

        DateTime GetInstallTime()
        {
            return Utilities.GetTime(INSTALL_TIMESTAMP_PPK, DateTime.Now);
        }

        bool CanRequestNow()
        {
            bool canRequest = StoreReview.CanRequestRating();

            if (canRequest)
            {
                if (requestMode == RequestMode.GameBased)
                {
                    bool hasPlayedEnoughGames = false;
                    int gamesPlayed = GetGamesPlayed();

                    if (gamesPlayed >= gamesPlayedAfterInstall)
                    {
                        int lastRequestGamesPlayed = PlayerPrefs.GetInt(LAST_REQUEST_GAMES_PLAYED_PPK, -9999);

                        if (gamesPlayed - lastRequestGamesPlayed >= gamesPlayedBetweenRequests)
                        {
                            hasPlayedEnoughGames = true;
                        }
                    }

                    canRequest &= hasPlayedEnoughGames;
                }
                else if (requestMode == RequestMode.TimeBased)
                {
                    bool isGoodTiming = false;
                    DateTime installTime = GetInstallTime();

                    if ((DateTime.Now - installTime).Days >= daysAfterInstall)
                    {
                        DateTime lastRequestTime = Utilities.GetTime(LAST_REQUEST_TIME_PPK, new DateTime(1970, 1, 1));

                        if ((DateTime.Now - lastRequestTime).Days >= daysBetweenRequests)
                        {
                            isGoodTiming = true;
                        }
                    }

                    canRequest &= isGoodTiming;
                }
            }

            return canRequest;
        }

        IEnumerator MakeRatingRequest(float delay = 0f)
        {
            yield return new WaitForSeconds(delay);

            StoreReview.RequestRating();

            if (requestMode == RequestMode.GameBased)
            {
                PlayerPrefs.SetInt(LAST_REQUEST_GAMES_PLAYED_PPK, GetGamesPlayed());
            }
            else if (requestMode == RequestMode.TimeBased)
            {
                Utilities.StoreTime(LAST_REQUEST_TIME_PPK, DateTime.Now);    
            }
        }

        #endif
    }
}
