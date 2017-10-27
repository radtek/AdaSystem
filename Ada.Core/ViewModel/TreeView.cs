using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ada.Core.ViewModel
{
    public class TreeView
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string ParentId { get; set; }
        public bool IsChecked { get; set; }
        public List<TreeView> Children { get; set; }
    }
}
