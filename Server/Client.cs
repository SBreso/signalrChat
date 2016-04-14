using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Client
    {
        public String Name { get; set; }
        public String Id { get; set; }
        public Client(string n, string i)
        {
            Name = n;
            Id = i;
        }
        public override string ToString()
        {
            return String.Format("{0}: {1}",Name,Id);
        }
    }
}
