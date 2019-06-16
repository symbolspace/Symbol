using System;
using Symbol.Data;

namespace Examples.Data {
    public class t_User {

        public long id { get; set; }
        public string account { get; set; }
        public string password { get; set; }

        public UserTypes type { get; set; }

        public object data { get; set; }
    }
}
