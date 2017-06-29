using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System;

public class Packager : MonoBehaviour {

    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();

    [MenuItem("AssetBundle/Get AssetBundle Names")]
    static void GetAllAssetbundleNames()
    {
        var names = AssetDatabase.GetAllAssetBundleNames();
        foreach (var name in names)
        {
            Debug.Log("AssetBundle:"+name);
        }
    }

    [MenuItem("AssetBundle/Create AssetBundle Main")]
    static void CreateAssetBundleMain()
    {
        string assetBundlePath = Application.streamingAssetsPath + "/ABs";
        if (!Directory.Exists(assetBundlePath))
        {
            Directory.CreateDirectory(assetBundlePath);           
        }
        BuildPipeline.BuildAssetBundles(assetBundlePath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();       
    }

    [MenuItem("AssetBundle/Set FileBundle Name")]
    static void SetAssetBundleName()
    {

        #region 设置资源的AssetBundle的名称和文件扩展名
        UnityEngine.Object[] selects = Selection.objects;
        foreach (UnityEngine.Object selected in selects)
        {
            string path = AssetDatabase.GetAssetPath(selected);
            AssetImporter asset = AssetImporter.GetAtPath(path);
            asset.assetBundleName = selected.name;      //设置Bundle文件的名称
            //asset.assetBundleVariant = "unity3d";       //设置Bundle文件的扩展名
            asset.SaveAndReimport();
        }
        AssetDatabase.Refresh();
        #endregion
    }

    [MenuItem("AssetBundle/Build Mobile Resource", false, 12)]
    public static void BuildAndroidResource()
    {
        BuildAssetResource(true);
    }

    /// <summary>
    /// 生成绑定素材
    /// </summary>
    public static void BuildAssetResource(bool isWin)
    {
        //string dataPath = Application.persistentDataPath + "/Game/";
        //if (Directory.Exists(dataPath))
        //{
        //    Directory.Delete(dataPath, true);
        //}
        //string assetfile = string.Empty;  //素材文件名
       

        //----------打包Assetbundle文件-------------
        string assetBundlePath = Application.streamingAssetsPath + "/ABs/";
        BuildPipeline.BuildAssetBundles(assetBundlePath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);//路径，打包选择的压缩方式，选择打包的指定平台

        string resPath = AppDataPath + "/StreamingAssets/";
        if (!Directory.Exists(resPath)) Directory.CreateDirectory(resPath);
        string luaPath = resPath + "/lua/";

        //----------复制Lua文件----------------
        if (Directory.Exists(luaPath))
        {
            Directory.Delete(luaPath, true);
        }
        Directory.CreateDirectory(luaPath);

        paths.Clear(); files.Clear();
        string luaDataPath = Application.dataPath + "/uLua/lua/".ToLower();
        Recursive(luaDataPath);
        //int n = 0;
        foreach (string f in files)
        {
            if (f.EndsWith(".meta")) continue;
            string newfile = f.Replace(luaDataPath, "");
            string newpath = luaPath + newfile;
            string path = Path.GetDirectoryName(newpath);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            if (File.Exists(newpath))
            {
                File.Delete(newpath);
            }
            File.Copy(f, newpath, true);

        }
        EditorUtility.ClearProgressBar();

        ///----------------------创建文件列表-----------------------
        string newFilePath = resPath + "/files.txt";
        if (File.Exists(newFilePath)) File.Delete(newFilePath);

        paths.Clear(); files.Clear();
        Recursive(resPath);

        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        for (int i = 0; i < files.Count; i++)
        {
            string file = files[i];
            string ext = Path.GetExtension(file);
            if (file.EndsWith(".meta") || file.Contains(".DS_Store")) continue;

            string md5 = md5file(file);
            string value = file.Replace(resPath, string.Empty);
            sw.WriteLine(value + "|" + md5);
        }
        sw.Close(); fs.Close();
        AssetDatabase.Refresh();
    }
    static string AppDataPath
    {
        get { return Application.dataPath.ToLower(); }
    }
    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
            files.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }
    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    public static string md5file(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }
}
