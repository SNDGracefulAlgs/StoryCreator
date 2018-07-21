using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryCreator
{
    public class Node
    {
        public Int16 level;
        public PointF coord;
        public string content;
        public HashSet<Node> parents;
        public List<Heir> heirs;

        public void AddHeir(string branchText, string content)
        {
            if (heirs == null) heirs = new List<Heir>();
            heirs.Add(new Heir { branchText = branchText, heirNode = new Node { content = content, heirs = null,level=(Int16)(level+1) } });
        }
        public void AddHeir(string branchText, Node contentNode)
        {
            if (heirs == null) heirs = new List<Heir>();
            heirs.Add(new Heir { branchText = branchText, heirNode = contentNode});
        }
        public void AddParent(Node parent)
        {
            if (parents == null) parents = new HashSet<Node>();
            parents.Add(parent);
        }
    }

    public class Heir
    {
        public string branchText;
        public Node heirNode;
    }
}
