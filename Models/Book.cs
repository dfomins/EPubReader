using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPubReader.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string AddingDate { get; set; }
    }
}
