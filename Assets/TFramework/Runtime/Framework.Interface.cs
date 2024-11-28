using UnityEngine;

namespace TFramework.Runtime
{
    public delegate void TAction<T>();
    // public interface ISystem
    // {
    //     public T GetModule<T>() where T : class,IModule;
    //     public T GetSystem<T>() where T : MonoBehaviour,ISystem;
    //
    //     #region Event
    //
    //     public void Call<T>(T value);
    //
    //     #endregion
    // }
    //
    // public interface IModule
    // {
    //     public T GetSystem<T>() where T : MonoBehaviour,ISystem;
    //
    //     #region Event
    //
    //     public void Register<T>(TAction<T> action);
    //     public void UnRegister<T>(TAction<T> action);
    //
    //     #endregion
    // }
}