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
        static UInt16 height;
        static UInt16 width;
        static UInt16 maxCountLevel;
        public static Dictionary<UInt16,HashSet<Node>> levelDict;

        public static void CalcTreeLayout()
        {
            FillLevelDict(root);
            CalcCoords();
        }
        
        static void FillLevelDict(Node curNode)
        {
            levelDict = null;
            levelDict = new Dictionary<ushort, HashSet<Node>>();
            if (!levelDict.ContainsKey((UInt16)curNode.level)) levelDict.Add((UInt16)curNode.level,new HashSet<Node>());
            levelDict[(UInt16)curNode.level].Add(curNode);
            if(curNode.heirs!=null)
            foreach (Heir heir in curNode.heirs) FillLevelDict(heir.heirNode);
        }

        static void CalcCoords() {
            height = (UInt16)levelDict.Count;
            maxCountLevel = levelDict.First(el => el.Value.Count==levelDict.Values.Select(p => p.Count).Max()).Key;
            width = (UInt16)levelDict[maxCountLevel].Count;
        }

        static void FillTreeCoords(HashSet<Node> level)
        {

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


    }
}
