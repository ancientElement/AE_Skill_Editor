namespace AE_Framework
{
    /// <summary>
    /// 不继承monobehaviour的单例
    /// </summary>
    /// <typeparam name="T"></typeparam>

    //约束 T : new() :有一个无参构造函数
    public abstract class BaseManager<T> where T : BaseManager<T>, new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                    instance.Init();
                }
                return instance;
            }
            protected set { instance = value; }
        }

        public virtual void Init()
        { }
    }
}