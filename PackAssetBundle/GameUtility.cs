using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XLua;

/// <summary>
/// 功能：通用静态方法
/// </summary>

[Hotfix]
public class GameUtility
{
    /// <summary>
    /// Assets文件夹名
    /// </summary>
    public const string AssetsFolderName = "Assets";

    /// <summary>
    /// 表示脚本的文件后缀
    /// </summary>
    public static string[] ScriptExtensions = { ".lua", ".Lua", ".txt",".cs"};

    /// <summary>
    /// 表示系统生成的无用文件后缀
    /// </summary>
    public static string[] ValidExtensions = { ".meta", ".bytes" };

    /// <summary>
    /// 表示Unity字节文件的后缀
    /// </summary>
    public static string[] BytesExtensions = { ".bytes"};

    /// <summary>
    /// 将路径格式转换为Unity文件路径 “\” ==》“/”
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string FormatToUnityPath(string path)
    {
        return path.Replace(@"\", "/");
    }

    /// <summary>
    /// 将路径格式转换为Window文件路径 “/” ==》“\”
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string FormatToSysFilePath(string path)
    {
        return path.Replace("/", @"\");
    }

    /// <summary>
    /// 绝对路径转化为Unity资源路径
    /// </summary>
    /// <param name="full_path"></param>
    /// <returns></returns>
    public static string FullPathToAssetPath(string full_path)
    {
        full_path = FormatToUnityPath(full_path);
        if (!full_path.StartsWith(Application.dataPath))
        {
            return full_path;
        }
        string ret_path = full_path.Replace(Application.dataPath, "");
        return AssetsFolderName + ret_path;
    }

    /// <summary>
    /// 将lua脚本从源文件夹复制到bytes文件夹中
    /// </summary>
    /// <param name="path">Lua脚本在文件夹中地路径</param>
    /// <returns></returns>
    public static string LuaScriptPathToBytesPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }
        path = FormatToUnityPath(path);
        path = path.Replace("LuaScripts", "LuaBytes");
        path = path + ".bytes";
        return path;
    }

    /// <summary>
    /// 返回文件路径的后缀
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileExtension(string path)
    {
        return Path.GetExtension(path).ToLower();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">要搜索的目录的相对或绝对路径</param>
    /// <param name="extensions">后缀名数组</param>
    /// <param name="exclude">筛选模式</param>
    /// <returns></returns>
    public static string[] GetSpecifyFilesInFolder(string path, string[] extensions = null, bool exclude = false)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        if (extensions == null)
        {
            return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        }
        else if (exclude)
        {
            return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(f => !extensions.Contains(GetFileExtension(f))).ToArray();
        }
        else
        {
            return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(f => extensions.Contains(GetFileExtension(f))).ToArray();
        }
    }
    
    /// <summary>
    /// 返回文件夹内所有符合范例的文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static string[] GetSpecifyFilesInFolder(string path, string pattern)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        return Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
    }

    /// <summary>
    /// 获取文件夹内的所有文件，包括子文件夹。
    /// </summary>
    /// <param name="path">要搜索的目录的相对或绝对路径</param>
    /// <returns></returns>
    public static string[] GetAllFilesInFolder(string path)
    {
        return GetSpecifyFilesInFolder(path);
    }

    /// <summary>
    /// 获取文件夹内所有子文件夹
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string[] GetAllDirsInFolder(string path)
    {
        return Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
    }

    /// <summary>
    /// 检查文件的目录路径是否存在，如果不存在则创建一个
    /// </summary>
    /// <param name="filePath"></param>
    public static void CheckFileAndCreateDirWhenNeeded(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        string fullFilePath = Path.GetFullPath(filePath);
        string dirPath = Path.GetDirectoryName(fullFilePath);
        if(!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        //FileInfo file_info = new FileInfo(filePath);
        //DirectoryInfo dir_info = file_info.Directory;
        //if (!dir_info.Exists)
        //{
        //    Directory.CreateDirectory(dir_info.FullName);
        //}
    }

    /// <summary>
    /// 检查目录路径是否存在，如果不存在则创建一个
    /// </summary>
    /// <param name="folderPath"></param>
    public static void CheckDirAndCreateWhenNeeded(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath))
        {
            return;
        }

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }

    /// <summary>
    /// 安全地将字符串写入到字节数组中
    /// </summary>
    /// <param name="outFile"></param>
    /// <param name="outBytes"></param>
    /// <returns></returns>
    public static bool SafeWriteAllBytes(string outFile, byte[] outBytes)
    {
        try
        {
            if (string.IsNullOrEmpty(outFile))
            {
                return false;
            }

            CheckFileAndCreateDirWhenNeeded(outFile);
            if (File.Exists(outFile))
            {
                File.SetAttributes(outFile, FileAttributes.Normal);
            }
            File.WriteAllBytes(outFile, outBytes);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("SafeWriteAllBytes failed! path = {0} with err = {1}", outFile, ex.Message));
            return false;
        }
    }

    /// <summary>
    /// 安全地将字符串写入到字符串数组中
    /// </summary>
    /// <param name="outFile"></param>
    /// <param name="outLines"></param>
    /// <returns></returns>
    public static bool SafeWriteAllLines(string outFile, string[] outLines)
    {
        try
        {
            if (string.IsNullOrEmpty(outFile))
            {
                return false;
            }

            CheckFileAndCreateDirWhenNeeded(outFile);
            if (File.Exists(outFile))
            {
                File.SetAttributes(outFile, FileAttributes.Normal);
            }
            File.WriteAllLines(outFile, outLines);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("SafeWriteAllLines failed! path = {0} with err = {1}", outFile, ex.Message));
            return false;
        }
    }

    /// <summary>
    /// 安全地将字符串写入到字符串文档中
    /// </summary>
    /// <param name="outFile"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool SafeWriteAllText(string outFile, string text)
    {
        try
        {
            if (string.IsNullOrEmpty(outFile))
            {
                return false;
            }

            CheckFileAndCreateDirWhenNeeded(outFile);
            if (File.Exists(outFile))
            {
                File.SetAttributes(outFile, FileAttributes.Normal);
            }
            File.WriteAllText(outFile, text);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("SafeWriteAllText failed! path = {0} with err = {1}", outFile, ex.Message));
            return false;
        }
    }


    /// <summary>
    /// 安全地读取字符串文档为字节数组
    /// </summary>
    /// <param name="inFile"></param>
    /// <returns></returns>
    public static byte[] SafeReadAllBytes(string inFile)
    {
        try
        {
            if (string.IsNullOrEmpty(inFile))
            {
                return null;
            }

            if (!File.Exists(inFile))
            {
                return null;
            }

            File.SetAttributes(inFile, FileAttributes.Normal);
            return File.ReadAllBytes(inFile);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("SafeReadAllBytes failed! path = {0} with err = {1}", inFile, ex.Message));
            return null;
        }
    }

    /// <summary>
    /// 安全地读取字符串文档为字符串数组
    /// </summary>
    /// <param name="inFile"></param>
    /// <returns></returns>
    public static string[] SafeReadAllLines(string inFile)
    {
        try
        {
            if (string.IsNullOrEmpty(inFile))
            {
                return null;
            }

            if (!File.Exists(inFile))
            {
                return null;
            }

            File.SetAttributes(inFile, FileAttributes.Normal);
            return File.ReadAllLines(inFile);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("SafeReadAllLines failed! path = {0} with err = {1}", inFile, ex.Message));
            return null;
        }
    }

    /// <summary>
    /// 安全地读取字符串文档为字符串
    /// </summary>
    /// <param name="inFile"></param>
    /// <returns></returns>
    public static string SafeReadAllText(string inFile)
    {
        try
        {
            if (string.IsNullOrEmpty(inFile))
            {
                return null;
            }

            if (!File.Exists(inFile))
            {
                return null;
            }

            File.SetAttributes(inFile, FileAttributes.Normal);
            return File.ReadAllText(inFile);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("SafeReadAllText failed! path = {0} with err = {1}", inFile, ex.Message));
            return null;
        }
    }

    /// <summary>
    /// 清空文件夹内容
    /// </summary>
    /// <param name="dirPath">文件夹地绝对路径或相对路径</param>
    public static void DeleteDirectory(string dirPath)
    {
        string[] files = Directory.GetFiles(dirPath);
        string[] dirs = Directory.GetDirectories(dirPath);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(dirPath, false);
    }

    /// <summary>
    /// 安全清空文件夹
    /// </summary>
    /// <param name="folderPath">绝对路径</param>
    /// <returns></returns>
    public static bool SafeClearDir(string folderPath)
    {
        try
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return true;
            }

            if (Directory.Exists(folderPath))
            {
                DeleteDirectory(folderPath);
            }
            Directory.CreateDirectory(folderPath);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("SafeClearDir failed! path = {0} with err = {1}", folderPath, ex.Message));
            return false;
        }
    }

    /// </summary>
    /// 安全删除Unity文件夹下的所有文件（不删除文件夹）
    /// </summary>
    /// <param name="DestDir">要清空的Unity文件夹名</param>
    public static bool SafeClearUnityDir(string destDirName)
    {
        try
        {
            if (string.IsNullOrEmpty(destDirName))
            {
                return false;
            }
            string DestDirPath = Path.Combine(Application.dataPath, destDirName);
            if (Directory.Exists(DestDirPath))
            {
                DeleteDirectory(DestDirPath);
            }
            Directory.CreateDirectory(DestDirPath);
            return true;

        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("SafeClearDir failed! Name = {0} with err = {1}", destDirName, ex.Message));
            return false;
        }
        
    }

    public static bool SafeDeleteDir(string folderPath)
    {
        try
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return true;
            }

            if (Directory.Exists(folderPath))
            {
                DeleteDirectory(folderPath);
            }
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("SafeDeleteDir failed! path = {0} with err: {1}", folderPath, ex.Message));
            return false;
        }
    }

    /// <summary>
    /// 安全地删除指定路径的文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool SafeDeleteFile(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return true;
            }

            if (!File.Exists(filePath))
            {
                return true;
            }
            File.SetAttributes(filePath, FileAttributes.Normal);
            File.Delete(filePath);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("SafeDeleteFile failed! path = {0} with err: {1}", filePath, ex.Message));
            return false;
        }
    }

    /// <summary>
    /// 重命名文件
    /// </summary>
    /// <param name="sourceFileName"></param>
    /// <param name="destFileName"></param>
    /// <returns></returns>
    public static bool SafeRenameFile(string sourceFileName, string destFileName)
    {
        try
        {
            if (string.IsNullOrEmpty(sourceFileName))
            {
                return false;
            }

            if (!File.Exists(sourceFileName))
            {
                return true;
            }
            File.SetAttributes(sourceFileName, FileAttributes.Normal);
            File.Move(sourceFileName, destFileName);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("SafeRenameFile failed! path = {0} with err: {1}", sourceFileName, ex.Message));
            return false;
        }
    }

    /// <summary>
    /// 安全地复制文件到指定路径
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <param name="destPath"></param>
    /// <returns></returns>
    public static bool SafeCopyFile(string sourcePath, string destPath)
    {
        try
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                return false;
            }

            if (!File.Exists(sourcePath))
            {
                return false;
            }
            CheckFileAndCreateDirWhenNeeded(destPath);
            if (File.Exists(destPath))
            {
                File.SetAttributes(destPath, FileAttributes.Normal);
            }
            File.Copy(sourcePath, destPath, true);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("SafeCopyFile failed! formFile = {0}, toFile = {1}, with err = {2}",
                sourcePath, destPath, ex.Message));
            return false;
        }
    }

    /// <summary>
    /// 复制文件到指定文件夹并添加后缀
    /// </summary>
    /// <param name="sourceDirName">源文件夹名字</param>
    /// <param name="destDirName">目标文件夹名字</param>
    /// <param name="extension">要添加地后缀名</param>
    public static void CopyFileAndAddExtensionOld(string sourceDirName, string destDirName, string extension = "")
    {
        string sourceDirPath = Path.Combine(Application.dataPath, sourceDirName);
        string destDirPath = Path.Combine(Application.dataPath, destDirName);
        if (!Directory.Exists(destDirPath))
        {
            Directory.CreateDirectory(destDirPath);
        }
        //获取源文件夹内所有文件夹和文件
        IEnumerable<string> files = Directory.EnumerateFileSystemEntries(sourceDirPath);
        if (files != null && files.Count() > 0)
        {
            foreach (string file in files)
            {
                if (Path.GetExtension(file).ToString().Equals(".meta"))
                {
                    continue;
                }
                string destPath = Path.Combine(destDirPath, Path.GetFileName(file));

                if (File.Exists(file))
                {
                    File.Copy(file, AddBytesExtension(destPath, extension), true);
                    continue;
                }
                CopyFileAndAddExtensionOld(file, destPath);
            }
        }
    }

    /// <summary>
    /// 复制文件到指定的路径的文件夹中
    /// </summary>
    /// <param name="sourcePath">文件原文件的绝对/相对路径</param>
    /// <param name="destPath">文件目标文件夹的绝对/相对路径</param>
    /// <param name="extension">复制后要增加的后缀</param>
    public static void CopyFile(string sourcePath, string destDirPath, string extension = "")
    {
        
    }



    /// <summary>
    /// 对目标文件夹内的所有文件添加.bytes后缀
    /// </summary>
    /// <param name="destDir"></param>
    public static string AddBytesExtension(string path, string extension)
    {
        if (string.IsNullOrEmpty(extension))
        {
            return path;
        }
        return Path.ChangeExtension(path, extension);
    }




}

#if UNITY_EDITOR
public static class GameUtilityExporter
{
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>(){
            typeof(GameUtility),
        };
}
#endif
