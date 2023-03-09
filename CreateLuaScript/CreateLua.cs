using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;


public class CreateLua : Editor
{
    [MenuItem("Assets/Create/Lua Script", false, 31)]
    public static void CreateNewLua()
    {
        //根据提供的LuaComponent.lua模板文件来在选中的文件夹路径下创建新的lua文件，并命名；CreateScriptAssetAction是创建时执行的方法；
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAssetAction>(), Path.Combine(GetSelectedDirectoryPath(), "New Lua.lua"), null, "Assets/Editor/Template/LuaComponent.lua");
    }

    //返回当前文件夹的路径
    public static string GetSelectedDirectoryPath()
    {
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

}


//点击Enter或者点击其他空白处时，结束编辑文件名的回调方法
class CreateScriptAssetAction : EndNameEditAction
{
    //这里的三个参数均来自于StartNameEditingIfProjectWindowExists函数中的参数
    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        //创建资源
        UnityEngine.Object obj = CreateAssetFromTemplate(pathName, resourceFile);
        //选中资源
        ProjectWindowUtil.ShowCreatedAsset(obj);
    }

    internal static UnityEngine.Object CreateAssetFromTemplate(string pathName, string resourceFile)
    {
        //获取要创建的资源绝对路径
        string fullName = Path.GetFullPath(pathName);
        //读取本地模板文件
        StreamReader sr = new StreamReader(resourceFile);
        string content = sr.ReadToEnd();
        sr.Close();

        //写入新文件,参数分别表示要写入的完整文件路径、覆盖数据、不省略字节流标记的编码格式（为true相当于System.Text.Encoding.UTF8）
        StreamWriter sw = new StreamWriter(fullName,false,new System.Text.UTF8Encoding(false));

        sw.Write(content);
        sw.Close();

        //导入并刷新刷新本地资源
        //AssetDatabase.ImportAsset(pathName);
        AssetDatabase.Refresh();
        return AssetDatabase.LoadAssetAtPath(pathName,typeof(UnityEngine.Object));  
    }
}