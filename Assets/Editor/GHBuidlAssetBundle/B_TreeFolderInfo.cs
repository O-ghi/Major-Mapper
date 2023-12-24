using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System;

public class DirInfo
{
    public string Name
    { get; set; }

    public DirInfo Parent
    { get; set; }
    public int Level
    { get; set; }

    public List<bool> Expanded
    { get; set; }

    public int ChildCount
    {
        get
        {
            return childCount;
        }
    }
    int childCount;
    private List<DirInfo> subDir;
    public void AddChild(DirInfo child)
    {
        if (child != null)
        {
            child.Parent = this;
            subDir.Add(child);
            ++childCount;
        }
    }

    public DirInfo(string dirName)
    {
        subDir = new List<DirInfo>();
        Level = -1;
        Expanded = new List<bool>(); ;
        Name = dirName;
        childCount = 0;
        Parent = null;
    }

    public DirInfo GetChild(int index)
    {
        if (index >= 0 && index < childCount)
        {
            return subDir[index];
        }
        return null;
    }

    public List<DirInfo> GetAllChildren()
    {
        return subDir;
    }

    public DirInfo Find(string name)
    {
        if (name == this.Name)
        {
            return this;
        }
        foreach (DirInfo child in subDir)
        {
            if (child.Name == name)
            {
                return child;
            }
            Find(name);
        }
        return null;
    }

    public DirInfo FindChild(string name, bool isAcceptSelf = false)
    {
        if (isAcceptSelf && name == this.Name)
        {
            return this;
        }
        foreach (DirInfo child in subDir)
        {
            if (child.Name == name)
            {
                return child;
            }
        }
        return null;
    }

    public string PrintLogTree(int grade = -1, int maxLength = 1)
    {
        if (grade == maxLength)
        {
            return "";
        }
        StringBuilder log = new StringBuilder();
        for (int i = 0; i < grade; i++)
        {
            log.Append("_/");
        }
        log.Append(Name).Append(Environment.NewLine);
        foreach (DirInfo child in subDir)
        {
            log.Append(child.PrintLogTree(grade + 1, maxLength));
        }
        return log.ToString();
    }

    public void ReCalculateBranch()
    {
        Expanded.Clear();
        foreach (DirInfo child in subDir)
        {
            child.Level = Level + 1;
            Expanded.Add(false);
            child.ReCalculateBranch();
        }
    }

    public int AllLeafCount()
    {
        if (childCount <= 0)
        {
            //leaf.
            return 1;
        }
        int leaf = 0;
        foreach (DirInfo child in subDir)
        {
            leaf += child.AllLeafCount();
        }
        return leaf;
    }
}
public class B_TreeFolderInfo : Editor
{
    private static DirInfo root = new DirInfo("myRoot"); //A SPECIAL NAME THAT CANT BE USE IN ANY CHILD
    [MenuItem("GH Tools/Other/Create B-tree folder info")]
    static public string CreateFolderInfo(string[] lines)
    {
        foreach (string line in lines)
        {
            BuildFolderTree(line);
        }
        return root.PrintLogTree();
        //Debug.Log(root.PrintLogTree());
    }

    static private void BuildFolderTree(string path)
    {
        DirInfo currParent = root;
        DirInfo temp;
        string[] dirs = path.Split('/');
        foreach (string dirName in dirs)
        {
            temp = currParent.FindChild(dirName);
            if (temp == null)
            {
                temp = new DirInfo(dirName);
                currParent.AddChild(temp);
            }
            currParent = temp;
        }
    }

    static public DirInfo CreateFolderTree(string[] lines, string rootName = "myRoot")
    {
        DirInfo root = new DirInfo(rootName);
        foreach (string line in lines)
        {
            BuildFolderTreeWithRoot(line, root);
        }
        return root;
    }

    static private void BuildFolderTreeWithRoot(string path, DirInfo root)
    {
        DirInfo currParent = root;
        DirInfo temp;
        string[] dirs = path.Split('/');
        foreach (string dirName in dirs)
        {
            temp = currParent.FindChild(dirName);
            if (temp == null)
            {
                temp = new DirInfo(dirName);
                currParent.AddChild(temp);
            }
            currParent = temp;
        }
    }
}