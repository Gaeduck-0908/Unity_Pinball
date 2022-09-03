using UnityEngine;
using System.Collections;

#if EASY_MOBILE
using EasyMobile;
#endif

namespace SgLib
{
    public class ScoreReporter : MonoBehaviour
    {
        [Header("Check to disable automatic score reporting")]
        public bool disable = false;

        [Header("Name of the leaderboard to report score as declared with EasyMobile")]
        public string leaderboardName = "Score";

        #if EASY_MOBILE
        public static ScoreReporter Instance { get; private set; }

        void OnEnable()
        {
            GameServices.UserLoginSucceeded += OnUserAuthenticated;
            GameManager.GameStateChanged += OnGameStateChanged;
        }

        void OnDisable()
        {
            GameServices.UserLoginSucceeded -= OnUserAuthenticated;
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

        void OnGameStateChanged(GameState newState, GameState oldState)
        {
            if (!disable)
            {
                if (newState == GameState.GameOver)
                    ReportScore(ScoreManager.Instance.Score, leaderboardName);
            }
        }

        void OnUserAuthenticated()
        {
            if (!disable)
            {                
                // Load current scores on leaderboards
                GameServices.LoadLocalUserScore(leaderboardName, OnLeaderboardScoreLoaded);
            }
        }

        // Sync local score with the one on leaderboard
        void OnLeaderboardScoreLoaded(string ldbName, UnityEngine.SocialPlatforms.IScore score)
        {
            if (score == null)
            {
                return; 
            }

            int serverScore = (int)score.value;
            int localHighScore = ScoreManager.Instance.HighScore;

            if (localHighScore > serverScore)
            {
                // Local score is better than the submitted one, so reporting the local one.
                ReportScore(localHighScore, leaderboardName);               
            }
            else if (localHighScore < serverScore)
            {
                // Loaded score is better than the local one, so updating the local highscore.
                ScoreManager.Instance.UpdateHighScore(serverScore);
            }
        }

        /// <summary>
        /// Reports score to leaderboard.
        /// </summary>
        public void ReportScore(int score, string ldbName)
        {
            GameServices.ReportScore(score, ldbName);
        }
        #endif
    }
}