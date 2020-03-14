using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaApi.Models
{
    public class Test
    {
        public Test(string nombre)
        {
            Nombre = nombre;
        }
        public int Id{ get; set; }
        public string Nombre{ get; set; }
    }
}
