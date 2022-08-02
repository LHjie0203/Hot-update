using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AssetBundleEditor : Editor
{
    private static string outPath = Application.streamingAssetsPath + "/ABPacks";
    private static string newOutPath = Application.streamingAssetsPath + "/" + Application.version ;
    [MenuItem("AB包/一键打包")]
    public static void Build()
    {
        //检测这个文件路径是否存在
        IsHasPath(newOutPath);
        //资源筛选
        ABPackBuild();
        //打包
        BuildPipeline.BuildAssetBundles(newOutPath, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows64);
        //弹出打包后的输出文件夹
        Process.Start(newOutPath);
    }
    static void IsHasPath(string path)
    {
        //检测这个文件路径是否存在
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    
    }
    public static void ABPackBuild()
    {
        IsHasPath(Application.streamingAssetsPath);
        //需要打包的资源路径
        string packPath = Application.dataPath + "/Resources";
        //筛选出要打包的资源文件
        string[] allFiles = Util.GetPathAllFiles(packPath, new string[] { ".meta"});
        StringBuilder sBuilder = new StringBuilder();
        foreach (var file in allFiles)
        {
            //转换路径下的斜线
            string f = file.Replace(@"\", "/");
            //替换成Assets路径下
            string f1 = f.Replace(Application.dataPath, "Assets");
            //拆分资源名称
            string abName = f1.Substring(f1.LastIndexOf('/') + 1).Split('.')[0];
            //使用打包API，打成AB包，并设置AB包名与扩展名
            AssetImporter importer = AssetImporter.GetAtPath(f1);
            importer.assetBundleName = abName;
            importer.assetBundleVariant = "u3d";
            sBuilder.Append(abName +"."+ importer.assetBundleVariant+ "/" + GetMD5(file) + "\r\n");
        }
        SaveResABList(sBuilder.ToString());
        SaveGameVersion();
    }
    public static void  SelectedFilesBuild()
    { 

    }
    [MenuItem("AB包/打开DataPath路径")]
    public static void OpenDataPath()
    {
        Process.Start(Application.dataPath);
    }
    [MenuItem("AB包/打开S目录路径")]
    public static void OpenSPath()
    {
        Process.Start(Application.streamingAssetsPath);
    }
    [MenuItem("AB包/打开P目录路径")]
    public static void OpenPPath()
    {
        Process.Start(Application.persistentDataPath);
    }
    /// <summary>
    /// 保存AB包的资源清单列表
    /// </summary>
    /// <param name="content"></param>
    static void SaveResABList(string content)
    {
        File.WriteAllText(Application.streamingAssetsPath + "/ResABList.txt", content);
    }
    /// <summary>
    /// 保存资源版本文件
    /// </summary>
    /// <param name="content"></param>
    static void SaveGameVersion()
    {
        File.WriteAllText(Application.streamingAssetsPath + "/GameVersion.txt", Application.version);
    }
    /// <summary>
    /// 转化一个资源的MD5码
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static string GetMD5(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] mData = md5.ComputeHash(data);
        return BitConverter.ToString(mData).Replace("-", "");
    }
}