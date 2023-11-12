using System;
using System.Collections;
using System.Collections.Generic;

namespace tree_3_9
{
    class BinaryTree
    {
        private class TaskData
        {
            public Node root;
            public Node targetNode;
            public int toAdd;
            public int minValue;
            public int maxValue;
            public bool answer;
            public Stack<int> addedValue = new Stack<int>();
            public Queue<int> pathVertices = new Queue<int>();
            public List<string> data = new List<string>();
            public TaskData(int toAdd, ref Node root)
            {
                answer = true;
                Node.Copy(ref this.root, ref root);
                //this.root = root;
                this.toAdd = toAdd;
                targetNode = root;
                minValue = int.MinValue;
                maxValue = int.MaxValue;
            }
            public void Reset()
            {
                targetNode = root;
                pathVertices.Clear();
                minValue = short.MinValue;
                maxValue = short.MaxValue;
            }
            public void AddData()
            {
                string s = "Adding vertice on path: ";
                while (pathVertices.Count != 1)
                {
                    s += pathVertices.Dequeue() + "->";
                }
                s += pathVertices.Dequeue();
                s += " with value in range of [" + minValue + "; " + maxValue + "]. " + toAdd + " left.\n";
                data.Add(s);
            }
        }
        private class Node
        {
            public object inf;
            public int height; //высота
            public uint level; //глубина
            public int counter; //степень узла (counter)
            public Node left;
            public Node right;
            public static Queue<Node> traversalNodes = new Queue<Node>();
            public Node(object inf)
            {
                this.inf = inf;
                height = 1;
                counter = 1;
                left = null;
                right = null;
            }

            public Node(object inf, uint level) : this(inf)
            {
                this.level = level;
            }
            public int Height
            {
                get {
                    return (this != null) ? height : 0;
                }
            }
            //Копирование всех узлов из дерева fromThisTree в дерево toThisTree
            //с помощью модификации прямого обхода дерева (Preorder).
            public static void Copy(ref Node fromThisTree, ref Node toThisTree)
            {
                if (fromThisTree != null)
                {
                    //Если в дереве toThisTree нет узла fromThisTree.inf, то вставим этот узел
                    Node res;
                    Search(toThisTree, fromThisTree.inf, out res);
                    if (res == null)
                    {
                        //добавление узла fromThisTree.inf в toThisTree
                        Add(ref toThisTree, fromThisTree.inf);
                        
                    }
                    Copy(ref fromThisTree.left, ref toThisTree.left);
                    Copy(ref fromThisTree.right, ref toThisTree.right);



                }
            }
            private int BalanceFactor
            {
                get
                {
                    int rh = right != null ? right.height : 0;
                    int lh = left != null ? left.height : 0;
                    return lh - rh;
                }
            }
            
            //TODO: Нужно добавить проверку на идентичный узел
            //TODO: Добавить изменяющийся уровень как static-поле

            public void NewHeight()
            {
                if (this != null)
                {
                    int rh = (right != null) ? right.Height : 0;
                    int lh = (left != null) ? left.Height : 0;
                    height = ((rh > lh) ? rh : lh) + 1;
                }
            }
            public static void RotationRight(ref Node t)
            {
                Node x = t.left;
                t.left = x.right;
                x.right = t;
                t.NewHeight();
                x.NewHeight();
                t = x;
            }
            public static void RotationLeft(ref Node t)
            {
                Node x = t.right;
                t.right = x.left;
                x.left = t;
                t.NewHeight();
                x.NewHeight();
                t = x;
            }

            public static void Rotation(ref Node t)
            {
                t.NewHeight();
                if (t.BalanceFactor == 2)
                {
                    if (t.right.BalanceFactor < 0)
                    {
                        RotationRight(ref t.right);
                    }
                    RotationLeft(ref t);
                }
                if (t.BalanceFactor == -2)
                {
                    if (t.left.BalanceFactor > 0)
                    {
                        RotationLeft(ref t.left);
                    }
                    RotationRight(ref t);
                }
            }
            public static void Add(ref Node r, object inf)
            {
                if (r == null)
                {
                    r = new Node(inf);
                }
                else
                {
                    if (((IComparable)r.inf).CompareTo(inf) > 0)
                    {
                        Add(ref r.left, inf);
                    }
                    else
                    {
                        Add(ref r.right, inf);
                    }
                }
                Rotation(ref r);
            }


            //Добавление
            public static void Add(ref Node currentNode, object nodeData, ref uint level)
            {
                if (currentNode == null)
                {
                    currentNode = new Node(nodeData, level);
                    return;
                }
                level++;
                currentNode.counter++;
                if (((IComparable)(currentNode.inf)).CompareTo(nodeData) > 0)
                {
                    Add(ref currentNode.left, nodeData, ref level);
                }
                else
                {
                    Add(ref currentNode.right, nodeData, ref level);
                }
                RecountHeight(currentNode);
            }

            public static void TaskAdd(ref Node currentNode, in TaskData taskData, ref uint level)
            {
                if (currentNode == null)
                {
                    currentNode = new Node((taskData.minValue + taskData.maxValue / 2) / 2, level);
                    taskData.targetNode = currentNode;
                    taskData.addedValue.Push((int)currentNode.inf);
                    taskData.toAdd--;
                    return;
                }
                level++;
                RecountHeight(currentNode);
                if (currentNode.BalanceFactor < 0)
                {
                    taskData.maxValue = Math.Min(taskData.maxValue, (int)currentNode.inf);
                    taskData.pathVertices.Enqueue((int)currentNode.inf);
                    TaskAdd(ref currentNode.left, taskData, ref level);
                }
                else
                {
                    taskData.minValue = Math.Max(taskData.minValue, (int)currentNode.inf);
                    taskData.pathVertices.Enqueue((int)currentNode.inf);
                    TaskAdd(ref currentNode.right, taskData, ref level);
                }
            }

            static void TaskSearch(ref Node currentNode, in TaskData taskData)
            {
                if (((IComparable)currentNode.inf).CompareTo(taskData.targetNode.inf) > 0)
                {
                    taskData.maxValue = Math.Min(taskData.maxValue, (int)currentNode.inf);
                    taskData.pathVertices.Enqueue((int)currentNode.inf);
                    currentNode.height++;
                    TaskSearch(ref currentNode.left, taskData);
                }
                if (((IComparable)currentNode.inf).CompareTo(taskData.targetNode.inf) < 0)
                {
                    taskData.minValue = Math.Max(taskData.minValue, (int)currentNode.inf);
                    taskData.pathVertices.Enqueue((int)currentNode.inf);
                    currentNode.height++;
                    TaskSearch(ref currentNode.right, taskData);
                }
            }

            //Обход в глубину
            public static void PreOrderTraversal(Node currentNode)
            {
                if (currentNode != null)
                {
                    Console.WriteLine(currentNode.inf);
                    PreOrderTraversal(currentNode.left);
                    PreOrderTraversal(currentNode.right);
                }
            }
            public static void InOrderTraversal(Node currentNode)
            {
                if (currentNode != null)
                {
                    InOrderTraversal(currentNode.left);
                    Console.WriteLine(currentNode.inf + "\tlevel: " + currentNode.level +
                        "\theight: " + currentNode.height);
                    InOrderTraversal(currentNode.right);
                }
            }
            public static void PostOrderTraversal(Node currentNode)
            {
                if (currentNode != null)
                {
                    PostOrderTraversal(currentNode.left);
                    PostOrderTraversal(currentNode.right);
                    Console.WriteLine(currentNode.inf);
                }
            }
            //Обход в ширину
            public static void BreadthTraversal(Node currentNode)
            {
                traversalNodes.Enqueue(currentNode);
                while (traversalNodes.Count != 0)
                {
                    if (traversalNodes.Peek() != null)
                    {
                        Console.WriteLine(traversalNodes.Peek().inf);
                        traversalNodes.Enqueue(traversalNodes.Peek().left);
                        traversalNodes.Enqueue(traversalNodes.Peek().right);
                    }
                    traversalNodes.Dequeue();
                }
            }
            //Поиск
            //Q: Можно ли сделать тип возвращаемого значения Node?
            public static void Search(Node currentNode, object keyValue, out Node item)
            {
                if (currentNode == null)
                {
                    item = null;
                    return;
                }
                if (((IComparable)(currentNode.inf)).CompareTo(keyValue) == 0)
                {
                    item = currentNode;
                    return;
                }

                if (((IComparable)(currentNode.inf)).CompareTo(keyValue) > 0)
                {
                    Search(currentNode.left, keyValue, out item);
                }
                else
                {
                    Search(currentNode.right, keyValue, out item);
                }
            }

            //Удаление
            public static void Delete(ref Node t, object keyValue)
            {
                if (t == null)
                {
                    Console.WriteLine("No item to delete");
                    return;
                }

                if (((IComparable)t.inf).CompareTo(keyValue) > 0)
                {
                    Delete(ref t.left, keyValue);
                    RecountCounter(t);
                    RecountHeight(t);
                    return;
                }
                if (((IComparable)(t.inf)).CompareTo(keyValue) < 0)
                {
                    Delete(ref t.right, keyValue);
                    RecountCounter(t);
                    RecountHeight(t);
                    return;
                }
                if (t.left == null)
                {
                    t = t.right;
                    return;
                }
                if (t.right == null)
                {
                    t = t.left;
                    return;
                }
                FindReplacement(t, ref t.right);
                RecountCounter(t);
                RecountHeight(t);
            }
            private static void FindReplacement(Node toDelete, ref Node replacement)
            {
                if (replacement.left == null)
                {
                    toDelete.inf = replacement.inf;
                    replacement = replacement.right;
                    RecountLevel(replacement);
                    RecountHeight(replacement);
                }
                else
                {
                    FindReplacement(toDelete, ref replacement.left);
                }
                RecountHeight(replacement);
                RecountCounter(replacement);
            }
            //Пересчёт уровня
            private static void RecountLevel(Node replacement)
            {
                if (replacement != null)
                {
                    replacement.level--;
                    RecountLevel(replacement.right);
                    RecountLevel(replacement.left);
                }
            }
            //Пересчёт количества узлов
            private static void RecountCounter(Node target)
            {
                if (target != null)
                {
                    target.counter = 0;
                    target.counter += target.right == null ? 0 : target.right.counter;
                    target.counter += target.left == null ? 0 : target.left.counter;
                    target.counter++;
                }
            }
            //Пересчёт высоты
            private static void RecountHeight(Node target)
            {
                if (target != null)
                {
                    int rightHeight = target.right != null ? target.right.height : 0;
                    int leftHeight = target.left != null ? target.left.height : 0;
                    target.height = Math.Max(rightHeight, leftHeight) + 1;
                }
                
            }

            //task 3_10
            public static void task_3_10(Node currentNode, in TaskData taskData)
            {
                uint level = 1;
                FindDisbalance(currentNode, taskData, ref level);
            }

            private static void FindDisbalance(Node currentNode, in TaskData taskData, ref uint level)
            {
                if (currentNode == null)
                { return; }
                if (Math.Abs(currentNode.BalanceFactor) > taskData.toAdd || taskData.toAdd < 0)
                {
                    TerminateFixing(currentNode, taskData);
                    return;
                }
                if (currentNode.BalanceFactor >= 2 || currentNode.BalanceFactor <= -2)
                {
                    FixDisbalance(currentNode, taskData, ref level);
                    return;
                }
                else
                {
                    FindDisbalance(currentNode.right, taskData, ref level);
                    FindDisbalance(currentNode.left, taskData, ref level);
                }
            }
            private static void FixDisbalance(Node currentNode, in TaskData taskData, ref uint level)
            {
                taskData.targetNode = currentNode;
                TaskSearch(ref taskData.root, in taskData);
                TaskAdd(ref taskData.targetNode, in taskData, ref level);
                taskData.AddData();
                taskData.Reset();
                level = 1;
                FindDisbalance(taskData.root, taskData, ref level);
            }
            private static void TerminateFixing(Node currentNode, in TaskData taskData)
            {
                taskData.answer = false;
                foreach (int node in taskData.addedValue)
                {
                    Node root = taskData.root;
                    Delete(ref root, node);
                }
            }
        }

        private Node tree;
        public object Inf
        {
            get { return tree.inf; }
        }
        public uint Level
        {
            get { return tree.level; }
        }

        //открытый конструктор
        public BinaryTree()
        {
            tree = null;
        }

        //открытый нельзя, т.к. возникнет небезопасное присваивание вида BinaryTree b = new BinaryTree(a)
        //2 разных дерева ссылаются на один и тот же узел (один из которых приватный)
        //закрытый конструктор
        private BinaryTree(Node node)
        {
            tree = node;
        }

        //добавление узла в дерево
        public void Add(object nodeInf)
        {
            uint level = 1;
            Node.Add(ref tree, nodeInf, ref level);
        }
        //организация различных способов обхода дерева
        public void PreOrder()
        {
            Node.PreOrderTraversal(tree);
        }
        public void InOrder()
        {
            Node.InOrderTraversal(tree);
        }
        public void PostOrder()
        {
            Node.PostOrderTraversal(tree);
        }
        public void BreadthSearch()
        {
            Node.BreadthTraversal(tree);
        }

        //Поиск
        public BinaryTree Search(object key)
        {
            Node toFind;
            Node.Search(tree, key, out toFind);
            BinaryTree targetNode = new BinaryTree(toFind);
            return targetNode;
        }
        public void Delete(object key)
        {
            Node.Delete(ref tree, key);
        }



        //Задание 3_10
        public void task_3_10(int toAdd)
        {
            TaskData taskData = new TaskData(toAdd, ref tree);
            Node.task_3_10(tree, taskData);
            if (taskData.answer)
            {
                Console.WriteLine("Balancing is possible adding following nodes:");
                foreach (string message in taskData.data)
                {
                    Console.Write(message);
                }
            }
            else
            {
                Console.WriteLine("Balancing is not possible");
            }
        }
    }
}
