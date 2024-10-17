using System;

namespace GStore
{
    /// <summary>
    /// 游戏单例模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> where T : new()
    {
        private static readonly T ms_instance = Activator.CreateInstance<T>();

        protected Singleton()
        {
        }

        public static T Instance
        {
            get
            {
                return ms_instance;
            }
        }
    }
}
