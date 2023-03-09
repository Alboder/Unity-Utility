using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//实现普通的单例模式
//where 限制模板的类型，new()表示这个类型必须要能被实例化
public abstract class NormalSingleton<T> where T : new()
{
    private static T _instance;
    //互斥锁，保证线程安全
    private static object mutex = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (mutex)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }
}

//Unity单例

/// <summary>
/// 这是一个单例的基类，派生自该类的子类全部都是单例,需要挂在一个对象下,如果子类要在awake书写代码，要先使用override来继承并重写base；使用时机：如果一个类需要维持实例的状态，则选择单例；
///如果这个类作为工具类只提供全局的访问，不用考虑状态的维持，则使用静态类速度更快；
/// </summary>
/// <typeparam name="T">T只可是继承自该类</typeparam>
public class BaseSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T Instance { get { return _instance; } }

    public virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);
    }
}
