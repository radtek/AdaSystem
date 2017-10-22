using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel.Admin
{
    public class MenuView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public bool? IsLeaf { get; set; }
        public int? Level { get; set; }
        public string TreePath { get; set; }
        public Url Url { get; set; }
        public string LinkUrl { get; set; }
        public string IconCls { get; set; }
        public int? Taxis { get; set; }

    }

    public class Url
    {
        public string Area { get; set; }
        public string Colltroller { get; set; }
        public string Action { get; set; }
        public string LinkUrl { get; set; }
    }
}
