using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMVCtest.Models
{
    public class TestViewModel
    {
        public string dataSource { get; set; }
        public string data1 { get; set; }
    }


    public class Product
    {
        public string type { get; set; }
        public string name { get; set; }
    }
}