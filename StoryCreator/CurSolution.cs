using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryCreator
{
    public static class CurSolution
    {
        public static Node curTree;

        public static void FillTestTree()
        {
            Node tmpNode;
            curTree = new Node { content = "tree root", level = 0 };
            curTree.AddHeir("branch1","heir1"); curTree.AddHeir("branch2", "heir2"); curTree.AddHeir("branch3", "heir3");//lvl 1

            for (int i = 0; i < 4; i++) curTree.heirs[0].heirNode.AddHeir("branch"+i+1,"heir"+i+1);//lvl 2
            curTree.heirs[1].heirNode.AddHeir("branch1", "heir1");
            curTree.heirs[2].heirNode.AddHeir("branch1", "heir1");

            tmpNode = curTree.heirs[0].heirNode.heirs[2].heirNode;//lvl3
            tmpNode.AddHeir("branch1", "heir1");
            tmpNode = curTree.heirs[1].heirNode.heirs[0].heirNode;
            for (int i = 0; i < 3; i++) tmpNode.AddHeir("branch" + i + 1, "heir" + i + 1);
            tmpNode = curTree.heirs[2].heirNode.heirs[0].heirNode;
            tmpNode.AddHeir("branch1", "heir1");

            tmpNode = curTree.heirs[0].heirNode.heirs[2].heirNode.heirs[0].heirNode;//lvl4
            tmpNode.AddHeir("branch1", "heir1"); tmpNode.heirs[0].heirNode.AddParent(curTree.heirs[1].heirNode.heirs[0].heirNode.heirs[0].heirNode);
            curTree.heirs[1].heirNode.heirs[0].heirNode.heirs[0].heirNode.AddHeir("branch1",curTree.heirs[0].heirNode.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode);
            tmpNode = curTree.heirs[1].heirNode.heirs[0].heirNode.heirs[1].heirNode;
            tmpNode.AddHeir("branch1", "heir1");
            tmpNode = curTree.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode;
            for (int i = 0; i < 3; i++) tmpNode.AddHeir("branch" + i + 1, "heir" + i + 1);

            tmpNode = curTree.heirs[0].heirNode.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode;//lvl5
            for (int i = 0; i < 3; i++) tmpNode.AddHeir("branch" + i + 1, "heir" + i + 1);
            tmpNode = curTree.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode;
            for (int i = 0; i < 3; i++) tmpNode.AddHeir("branch" + i + 1, "heir" + i + 1);

            tmpNode = curTree.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode.heirs[0].heirNode;//lvl6
            for (int i = 0; i < 2; i++) tmpNode.AddHeir("branch" + i + 1, "heir" + i + 1);
            tmpNode = curTree.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode.heirs[1].heirNode;
            tmpNode.AddHeir("branch1", "heir1");
            tmpNode = curTree.heirs[2].heirNode.heirs[0].heirNode.heirs[0].heirNode.heirs[1].heirNode.heirs[2].heirNode;
            for (int i = 0; i < 2; i++) tmpNode.AddHeir("branch" + i + 1, "heir" + i + 1);
        }


    }
}
