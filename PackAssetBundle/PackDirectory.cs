using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Codice.Utils;
using System.Linq;

public class PackDirectory : Editor
{
    ////打包所有Lua文件为AB包
    //[MenuItem("Tools/AssetBundles/Pack Lua Scripts")]
    //public static void PackLuaScripts()
    //{
    //    string LuaPackagePath = "/LuaScripts";
    //    PackCustomAssetBundle("luascripts", LuaPackagePath);
    //}

    //显示在项目面板上的按键
    [InitializeOnLoadMethod]
    static void Init()
    {
        EditorApplication.projectWindowItemOnGUI = delegate (string guid, Rect selectionRect)
        {
            if (Selection.activeObject && guid == AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Selection.activeObject)))
            {
                selectionRect.x += (selectionRect.width - 50f);
                selectionRect.width = 50f;
                if (GUI.Button(selectionRect, "Pack"))
                {
                    PacKSelectedFolder();
                    
                }
            }
        };
    }

    [MenuItem("Assets/Pack Folder",false,1999)]
    /// <summary>
    /// 打包选中的文件夹为AssetBundle
    /// </summary>
    public static void PacKSelectedFolder()
    {
        string path = string.Empty;
        string bundleName = string.Empty;
        //获取文件夹的路径
        foreach (UnityEngine.Object item in Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets))
        {
            bundleName = item.name;
            path = AssetDatabase.GetAssetPath(item);
        }
        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
        {
            Debug.Log("Path is Empty or not a Directory!");
            return;
        }
        PackCustomAssetBundle(bundleName, path);
    }


    /// <summary>
    /// 打包指定完整/相对路径的文件夹为AssetBundle。
    /// </summary>
    /// <param name="bundleName">AssetBundle名</param>
    /// <param name="dirPath">文件夹的完整/相对路径</param>
    public static void PackCustomAssetBundle(string bundleName, string dirPath)
    {       
        //只生成一个AB包即可
        AssetBundleBuild[] bundleBuild = new AssetBundleBuild[1];
        bundleBuild[0].assetBundleName = bundleName;

        //获取要打包的文件夹下的所有文件的路径
        string[] filePaths = GameUtility.GetSpecifyFilesInFolder(dirPath, GameUtility.ValidExtensions, true);

        //检查如果有脚本文件则复制并添加.bytes后缀
        for (int i = 0; i < filePaths.Length; i++)
        {
            if (GameUtility.ScriptExtensions.Contains(Path.GetExtension(filePaths[i])))
            {
                File.Copy(filePaths[i], filePaths[i] + ".bytes", true);
                filePaths[i] = filePaths[i] + ".bytes";
            }
            filePaths[i] = GameUtility.FullPathToAssetPath(filePaths[i]);
            Debug.Log(filePaths[i]);
        }

        bundleBuild[0].assetNames = filePaths;
        Debug.Log(bundleBuild[0].assetNames.Length);

        AssetDatabase.Refresh();

#if UNITY_STANDALONE_WIN
        //打包
        if (Application.platform == RuntimePlatform.Android)
        {

        }
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", bundleBuild, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
#elif UNITY_ANDROID


#endif

        //删除复制出来的.bytes文件
        string[] bytesFilePaths = GameUtility.GetSpecifyFilesInFolder(dirPath, GameUtility.BytesExtensions);
        foreach (var item in bytesFilePaths)
        {
            if (File.Exists(item))
            {
                File.Delete(item);
                File.Delete(item + ".meta");
            }
        }

        AssetDatabase.Refresh();
    }

}
