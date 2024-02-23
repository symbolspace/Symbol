using System.ComponentModel;

namespace Symbol;

/// <summary>
/// 接口：可观察对象。
/// </summary>
public interface IObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
{
}