using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.Enums.Framework {
    internal class Program {
        static void Main(string[] args) {


            JSONListT();
        }

        static void New_Normal() {
            var info = new BookInfo();
        }
        static void New_Generic() {
            var list = new List<BookInfo>();
        }
        static void New_Generic_Any<T>() {
            var list = new List<T>();
        }

        static void JSONListT() {
            var userBook = new UserBook() {
                User = new User() { Name = "张三" },
                Books = new System.Collections.Generic.List<BookInfo>() {
                     new BookInfo() {
                          Name="人文英语",
                           Count=1
                     }
                 }
            };
            userBook.Books[0].Attributes.Add("color", "red");

            var json = JSON.ToNiceJSON(userBook);
            var userBook2 = JSON.ToObject<UserBook>(json, true);
            Console.WriteLine(userBook2);
        }
    }
}
