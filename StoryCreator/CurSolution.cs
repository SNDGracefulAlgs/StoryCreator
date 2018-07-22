using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryCreator
{
    public static class CurSolution
    {
        public static Node root;
        public static UInt16 height;
        public static UInt16 width;
        static UInt16 maxCountLevel;
        public static Dictionary<UInt16,HashSet<Node>> levelDict;

        public static void CalcTreeLayout()
        {
            ParentsSet(root, null);
            levelDict = null;
            levelDict = new Dictionary<ushort, HashSet<Node>>();
            FillLevelDict(root);
            CalcCoords();
        }
        
        //распределение узлов по уровням
        static void FillLevelDict(Node curNode)
        {
            if (!levelDict.ContainsKey((UInt16)curNode.level)) levelDict.Add((UInt16)curNode.level,new HashSet<Node>());
            levelDict[(UInt16)curNode.level].Add(curNode);
            if(curNode.heirs!=null)
            foreach (Heir heir in curNode.heirs) FillLevelDict(heir.heirNode);
        }

        static void CalcCoords() {
            height = (UInt16)levelDict.Count;
            maxCountLevel = levelDict.First(el => el.Value.Count==levelDict.Values.Select(p => p.Count).Max()).Key;
            width = (UInt16)levelDict[maxCountLevel].Count;
            FillTreeCoords();
        }

        static void FillTreeCoords()
        {
            List<Node> tmpNodesList = levelDict[maxCountLevel].ToList();
            for (int i = 0; i < tmpNodesList.Count; i++)
            {
                tmpNodesList[i].coord.X = i;
                tmpNodesList[i].coord.Y = tmpNodesList[i].level;
            }
            UpperStep(tmpNodesList);
            LowerStep(tmpNodesList);
        }

        static void UpperStep(List<Node> level)
        {
            HashSet<Node> allLevelParents = ObtainLevelParents(level);
            if (allLevelParents == null) return;

            if (allLevelParents.Count < levelDict[(UInt16)level[0].level].Count)
            {
                List<Node> nonParentNodes = levelDict[(UInt16)level[0].level].Where(el => !allLevelParents.Contains(el)).ToList();
            }

            foreach (Node parent in allLevelParents)
            {
                parent.coord.X = parent.heirs.Select(h => h.heirNode.coord.X).Sum() / (float)parent.heirs.Count;
                parent.coord.Y = parent.level;
            }

            UpperStep(allLevelParents.ToList());
        }

        static void LowerStep(List<Node> level)
        {
            HashSet<Node> allLevelHeirs = ObtainLevelheirs(level);
            if (allLevelHeirs == null) return;
           
            foreach (Node heir in allLevelHeirs)
            {
                heir.coord.X = heir.parents.Select(p => p.coord.X).Sum() / (float)heir.parents.Count;
                heir.coord.Y = heir.level;
            }

            LowerStep(allLevelHeirs.ToList());
        }

        static HashSet<Node> ObtainLevelParents(List<Node> level)
        {
            IEnumerable<HashSet<Node>> tmpCollect = level.Select(node => node.parents);
            tmpCollect = tmpCollect.Where(el => el != null);
            if (tmpCollect.Count() == 0) return null;
            HashSet<Node> allLevelParents = new HashSet<Node>();
            foreach (HashSet<Node> parents in tmpCollect)
                foreach (Node parent in parents)
                    allLevelParents.Add(parent);
            return allLevelParents;
        }

        static HashSet<Node> ObtainLevelheirs(List<Node> level)
        {
            IEnumerable<List<Heir>> tmpCollect = level.Select(node => node.heirs);
            tmpCollect = tmpCollect.Where(el => el != null);
            if (tmpCollect.Count() == 0) return null;
            HashSet<Node> allLevelHeirs = new HashSet<Node>();
            foreach (List<Heir> heirs in tmpCollect)
                foreach (Heir heir in heirs)
                    allLevelHeirs.Add(heir.heirNode);
            return allLevelHeirs;
        }

        public static void FillTestTree()
        {
            Node tmpNode;
            root = new Node { content = "tree root", level = 0 };
            root.AddHeir("branch1","heir1"); root.AddHeir("branch2", "heir2"); root.AddHeir("branch3", "heir3");//lvl 1

            for (int i = 0; i < 4; i++) root.heirs[0].heirNode.AddHeir("branch"+i+1,"heir"+i+1);//lvl 2
            root.heirs[1].heirNode.AddHeir("branch1", "heir1");
            root.heirs[2].heirNode.AddHeir("branch1", "heir1");

            tmpNode = root.heirs[0].heirNode.heirs[2].heirNode;//lvl3
            tmpNode.AddHeir("branch1", "heir1");
            tmpNode = root.heirs[1].heirNode.heirs[0].heirNode;
            for (int i = 0; i < 3; i++) tmpNode.AddHeir("branch" + i + 1, "heir" + i + 1);
            tmpNode = root.heirs[2].heirNode.heirs[0].heirNode;
            tmpNode.AddHeir("branch1", "heir1");

            tmpNode = root.heirs[0].heirNode.heirs[2].heirNode.heirs[0].heirNode;//lvl4
            tmpNode.AddHeir("branch1", "heir1"); tmpNode.heirs[0].heirNode.AddParent(root.heirs[1].heirNode.heirs[0].heirNode.heirs[0].heirNode);
            root.heirs[1].heirNode.heirs[0].heirNode.heirs[0].heirNode.AddHeir("branch1",root.heirs[0].heirNode.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode);
            tmpNode = root.heirs[1].heirNode.heirs[0].heirNode.heirs[1].heirNode;
            tmpNode.AddHeir("branch1", "heir1");
            tmpNode = root.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode;
            for (int i = 0; i < 3; i++) tmpNode.AddHeir("branch" + i + 1, "heir" + i + 1);

            tmpNode = root.heirs[0].heirNode.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode;//lvl5
            for (int i = 0; i < 3; i++) tmpNode.AddHeir("branch" + i + 1, "heir" + i + 1);
            tmpNode = root.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode;
            for (int i = 0; i < 3; i++) tmpNode.AddHeir("branch" + i + 1, "heir" + i + 1);

            tmpNode = root.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode.heirs[0].heirNode;//lvl6
            for (int i = 0; i < 2; i++) tmpNode.AddHeir("branch" + i + 1, "heir" + i + 1);
            tmpNode = root.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode.heirs[1].heirNode;
            tmpNode.AddHeir("branch1", "heir1");
            tmpNode = root.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode.heirs[2].heirNode;
            for (int i = 0; i < 2; i++) tmpNode.AddHeir("branch" + i + 1, "heir" + i + 1);
            tmpNode = root.heirs[0].heirNode.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[0].heirNode;
            tmpNode.AddHeir("branch1", "heir1");
            tmpNode = root.heirs[0].heirNode.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode;
            for (int i = 0; i < 2; i++) tmpNode.AddHeir("branch" + i + 1, "heir" + i + 1);

            tmpNode = root.heirs[0].heirNode.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[0].heirNode;//lvl7
            tmpNode.AddHeir("branch1", "heir1");
            tmpNode.heirs[0].heirNode.AddParent(root.heirs[0].heirNode.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode.heirs[0].heirNode);
            tmpNode = root.heirs[0].heirNode.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode.heirs[1].heirNode;
            tmpNode.AddHeir("branch1", "heir1");
            root.heirs[0].heirNode.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode.heirs[1].heirNode.heirs[0].heirNode.AddParent(root.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode.heirs[0].heirNode.heirs[0].heirNode);
            root.heirs[0].heirNode.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode.heirs[1].heirNode.heirs[0].heirNode.AddParent(root.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode.heirs[0].heirNode.heirs[1].heirNode);
            tmpNode = root.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode.heirs[1].heirNode.heirs[0].heirNode;
            tmpNode.AddHeir("branch1", "heir1");
            tmpNode.heirs[0].heirNode.AddParent(root.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode.heirs[2].heirNode.heirs[0].heirNode);
            tmpNode.heirs[0].heirNode.AddParent(root.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode.heirs[2].heirNode.heirs[1].heirNode);
           
        }

        static void ParentsSet(Node curNode,Node parentNode)
        {
            if (parentNode != null)
            {
                if (curNode.parents == null) curNode.parents = new HashSet<Node>();
                curNode.parents.Add(parentNode);
            }

            if (curNode.heirs != null)
                foreach (Heir heir in curNode.heirs) ParentsSet(heir.heirNode,curNode);
        }
    }
}
