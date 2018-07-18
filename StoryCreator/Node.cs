using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryCreator
{
    class Node
    {
        public string content;
        public List<Heir> heirs;

        public void AddHeir(string branchText, string content)
        {
            if (heirs == null) heirs = new List<Heir>();
            heirs.Add(new Heir { branchText = branchText, heir = new Node { content = content, heirs = null } });
        }
    }

    class Heir
    {
        public string branchText;
        public Node heir;
    }
}
