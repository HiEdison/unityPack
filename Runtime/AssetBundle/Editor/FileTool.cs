using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FileTool : Editor
{
    #region 文件清理\迁移

    private static void DelFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static void DelDirectory(string path)
    {
        Debug.Log("DelDirectory=>" + path);
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    public static void CopyFilesToDirKeepSrcDirName(string srcPath, string destDir, string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        Debug.Log("copy=>" + srcPath + " to " + destDir);
        if (Directory.Exists(srcPath))
        {
            DirectoryInfo srcDirectory = new DirectoryInfo(srcPath);
            CopyDirectory(srcPath, destDir + @"\" + srcDirectory.Name, searchPattern, searchOption);
        }
        else
        {
            CopyFile(srcPath, destDir);
        }
    }

    public static void CopyFilesToDir(string srcPath, string destDir,string searchPattern = "*", SearchOption searchOption= SearchOption.TopDirectoryOnly)
    {
        if (Directory.Exists(srcPath))
        {
            CopyDirectory(srcPath, destDir, searchPattern, searchOption);
        }
        else
        {
            CopyFile(srcPath, destDir);
        }
    }

    public static FileInfo[] GetFiles(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        DirectoryInfo srcDirectory = new DirectoryInfo(path);
        if (!srcDirectory.Exists)
        {
            return new FileInfo[0];
        }

        FileInfo[] files = srcDirectory.GetFiles(searchPattern, searchOption);
        return files;
    }

    private static void CopyDirectory(string srcDir, string destDir, string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        DirectoryInfo srcDirectory = new DirectoryInfo(srcDir);
        DirectoryInfo destDirectory = new DirectoryInfo(destDir);
        if (destDirectory.FullName.StartsWith(srcDirectory.FullName, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new Exception("cannot copy parent to child directory.");
        }

        if (!srcDirectory.Exists)
        {
            return;
        }

        if (!destDirectory.Exists)
        {
            destDirectory.Create();
        }

        FileInfo[] files = GetFiles(srcDir, searchPattern, searchOption);
        string saveFile = "";
        for (int i = 0; i < files.Length; i++)
        {
            saveFile = files[i].FullName.Replace(srcDirectory.FullName, destDirectory.FullName);
            CopyFile(files[i].FullName, saveFile);
        }

        DirectoryInfo[] dirs = srcDirectory.GetDirectories();

        for (int j = 0; j < dirs.Length; j++)
        {
            CopyDirectory(dirs[j].FullName, destDirectory.FullName + @"\" + dirs[j].Name, searchPattern, searchOption);
        }
    }

    public static string CreateDirectory(string path)
    {
        string directoryName = Path.GetDirectoryName(path);
        if (directoryName.Length > 0 && !Directory.Exists(Path.GetFullPath(directoryName)))
        {
            Directory.CreateDirectory(directoryName);
        }

        return directoryName;
    }

    public static void CopyFile(string srcFile, string destFile)
    {
        if (!File.Exists(srcFile))
        {
            return;
        }

        CreateDirectory(destFile);
// #if UNITY_EDITOR
//         Debug.Log(srcFile + "___copy to__" + destFile);
// #endif
        File.Copy(srcFile, destFile, true);
    }

    #endregion
}