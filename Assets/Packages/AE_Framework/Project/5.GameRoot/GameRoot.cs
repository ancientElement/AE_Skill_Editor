using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AE_Framework
{
    public class GameRoot : SingletonMonobehaviour<GameRoot>
    {
        /// <summary>
        /// 框架设置
        /// </summary>
        [LabelText("框架设置")][SerializeField] private GameSettings gameSetting;

        public GameSettings GameSetting
        {
            get { return gameSetting; }
        }

        protected override void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            base.Awake();
            DontDestroyOnLoad(gameObject);
            // 初始化所有管理器
            InitManager();
        }

        private void InitManager()
        {
            SingletonMonoMgr[] managers = GetComponents<SingletonMonoMgr>();
            for (int i = 0; i < managers.Length; i++)
            {
                managers[i].Init();
            }
        }

#if UNITY_EDITOR

        static GameRoot()
        {
            EditorApplication.update += () => { InitForEditor(); };
        }

        [InitializeOnLoadMethod]
        public static void InitForEditor()
        {
            // 当前是否要进行播放或准备播放中
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if (Instance == null && GameObject.Find("GameRoot") != null)
            {
                Instance = GameObject.Find("GameRoot").GetComponent<GameRoot>();
                // 清空事件
                EventCenter.Clear();
                Instance.InitManager();
                Instance.GameSetting.InitForEditor();
            }
        }

#endif
    }
}