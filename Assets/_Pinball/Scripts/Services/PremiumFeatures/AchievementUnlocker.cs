using UnityEngine;
using System.Collections;

#if EASY_MOBILE
using EasyMobile;
#endif

namespace SgLib
{
    public class AchievementUnlocker : MonoBehaviour
    {
        [System.Serializable]
        public struct ScoreAchievement
        {
            public string achievementName;
            public int scoreToUnlock;
        }

        [Header("Check to disable automatic achievement unlocking")]
        public bool disable = false;

        [Header("List of achievements to unlock")]
        public ScoreAchievement[] achievements;

        #if EASY_MOBILE
        public static AchievementUnlocker Instance { get; private set; }

        void OnEnable()
        {
            ScoreManager.ScoreUpdated += OnScoreUpdated;
        }

        void OnDisable()
        {
            ScoreManager.ScoreUpdated -= OnScoreUpdated;
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

        void OnScoreUpdated(int score)
        {
            if (disable)
            {
                return;
            }

            string acmName = null;

            foreach (ScoreAchievement acm in achievements)
            {
                if (score == acm.scoreToUnlock)
                {
                    acmName = acm.achievementName;
                    break;
                }
            }
    
            // Unlock achievement
            if (acmName != null)
                GameServices.UnlockAchievement(acmName);
        }

        #endif
    }
}
