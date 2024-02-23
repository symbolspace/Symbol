using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Symbol.Tests;

[TestClass()]
public class ObservableObjectExtensionsTests
{
    [TestMethod()]
    public void RaisePropertyChangingTest()
    {
        var o = new EmptyObservableObject();
        o.PropertyChanging += (p1, p2) =>
        {
            Console.WriteLine($"[{p1.GetHashCode()}]{p1.GetType().Name}.{p2.PropertyName} changing ...");
        };
        o.PropertyChanged += (p1, p2) =>
        {
            Console.WriteLine($"[{p1.GetHashCode()}]{p1.GetType().Name}.{p2.PropertyName} changed");
        };

        o.RaisePropertyChanging("Name");
        o.RaisePropertyChanged("Name");

        Assert.ThrowsException<NotSupportedException>(() => o.RaiseEvent("Click"));
    }


    class EmptyObservableObject : IObservableObject
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangingEventHandler? PropertyChanging;
    }

    [TestMethod()]
    public void SetPropertyTest()
    {
        var o = new Book();
        o.PropertyChanging += (p1, p2) =>
        {
            Console.WriteLine($"[{p1.GetHashCode()}]{p1.GetType().Name}.{p2.PropertyName} changing ...");
        };
        o.PropertyChanged += (p1, p2) =>
        {
            Console.WriteLine($"[{p1.GetHashCode()}]{p1.GetType().Name}.{p2.PropertyName} changed");
        };

        o.Name = "test";
        o.Name = null;

    }
    class Book : EmptyObservableObject
    {

        private string? _name;
        public string? Name
        {
            get { return _name; }
            set
            {
                this.SetProperty(ref _name, value);
            }
        }
    }
}