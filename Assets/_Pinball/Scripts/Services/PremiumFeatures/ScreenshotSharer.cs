using UnityEngine;
using System.Collections;

#if EASY_MOBILE
using EasyMobile;
#endif

namespace SgLib
{
    public class ScreenshotSharer : MonoBehaviour
    {
        [Header("Sharing Config")]
        [Tooltip("Any instances of [score] will be replaced by the actual score achieved in the last game")]
        [TextArea(3, 3)]
        public string shareMessage = "Awesome! I've just scored [score] in Bridges! #bridges";
        public string screenshotFilename = "screenshot.png";

        #if EASY_MOBILE
        public static ScreenshotSharer Instance { get; private set; }

        GameManager gameManager;

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
            gameManager = GameObject.FindObjectOfType<GameManager>();
        }

        void OnGameStateChanged(GameState newState, GameState oldState)
        {
            if (newState == GameState.GameOver && PremiumFeaturesManager.Instance.enablePremiumFeatures)
            {
                ScreenCapture.CaptureScreenshot(screenshotFilename);
            }
        }

        public void ShareScreenshot()
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, screenshotFilename);
            string msg = shareMessage;
            msg = msg.Replace("[score]", ScoreManager.Instance.Score.ToString()); 
            Sharing.ShareImage(path, screenshotFilename, msg);
        }

        #endif
    }
}
