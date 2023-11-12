using System;
using System.IO;
using System.Collections.Generic;
using tree_3_9;

class Program
{
    static void Main()
    {
        char[] splitters = { ' ', '\n', '\t', '\r' };
        StreamReader inputFile = new StreamReader("/Users/DmitryKonorov/Projects/Tree3_9/input1.txt");
        int toAdd = int.Parse(inputFile.ReadLine().ToString());
        string[] s = inputFile.ReadToEnd().Split(splitters, StringSplitOptions.RemoveEmptyEntries);
        List<int> nodesData = new List<int>();
        foreach (string num in s)
        {
            nodesData.Add(int.Parse(num));
        }

        BinaryTree tree = new BinaryTree();
        foreach (int item in nodesData)
        {
            tree.Add(item);
        }

        tree.InOrder();

        tree.task_3_10(toAdd);
        tree.InOrder();
        //tree.task_3_10(toAdd);
        //tree.BreadthSearch();
        Console.ReadKey();
    }   
}