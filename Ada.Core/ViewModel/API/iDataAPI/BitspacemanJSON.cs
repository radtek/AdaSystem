using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.API.iDataAPI
{
   public class BitspacemanJSON: iDataJsonResult
    {
        public BitspacemanJSON()
        {
            wordList=new List<Bitspaceman>();
        }
        public List<Bitspaceman> wordList { get; set; }
    }

    public class Bitspaceman
    {
        public int offset { get; set; }
        public int length { get; set; }
        public string pos { get; set; }
        public string word { get; set; }
    }
}
