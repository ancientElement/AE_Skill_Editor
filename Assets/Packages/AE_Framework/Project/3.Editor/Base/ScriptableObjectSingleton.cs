using UnityEngine;

namespace AE_Framework
{
    public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T>
    {
        private static T m_instance;

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = ScriptableObject.CreateInstance<T>();
                }
                return m_instance;
            }
        }

        private void OnDisable()
        {
            m_instance = null;
        }
    }
}