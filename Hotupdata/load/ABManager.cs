using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;

public class ABManager:MonoBehaviour
{
    /// <summary>
    /// AB资源包的缓存
    /// </summary>
    Dictionary<string, MyAssetBundle> ABCache = new Dictionary<string, MyAssetBundle>();
    /// <summary>
    /// AB资源包的依赖项关系
    /// </summary>
    Dictionary<string, string[]> dependDict = new Dictionary<string, string[]>();

    string path;
    // Start is called before the first frame update
    void Start()
    {
        path = Application.streamingAssetsPath + "/AB";
        InitDepend();
    }
    
    public void InitDepend()
    {
        //AB.manifest 清单的路径
        string mfPath = path + "/AB";
        //加载AB包的清单
         assetBundle = AssetBundle.LoadFromFile(mfPath);
        
        //清单的内容
        AssetBundleManifest assetBundleManifest =  assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        //获取清单的所有AB包资源
        var abList = assetBundleManifest.GetAllAssetBundles();

        foreach (var item in abList)

        {
            //资源包对应的依赖项关联关系(首先是能加载物体的时候能把他的依赖关系加载出来)
            var dependList = assetBundleManifest.GetAllDependencies(item);
            dependDict.Add(item, dependList);
        }
    }
    /// <summary>
    /// 加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <returns></returns>
    public T LoadAsset<T>(string abName) where T:UnityEngine.Object
    {
        string bName = abName.ToLower() + ".game";
        //判断是否有依赖项
        if (dependDict.ContainsKey(bName))
        {
            //有的话逐个加载(逻辑来分析，每个加载的模型，要先加载依赖项(比如装备、武器、衣服等等))
            var dependList = dependDict[bName];
            foreach (var depend in dependList)
            {
                //加载一个依赖项
                LoadAssetBundle(depend);
            }
        }
        //加载资源
        var ab = LoadAssetBundle(bName);
        return ab.aBundle.LoadAllAssets<T>()[0];
    }
    /// <summary>
    /// 加载AB包资源(不管是依赖项的资源还是正常的资源，都会走这，因为都是AB包)
    /// </summary>
    /// <param name="abName"></param>
    MyAssetBundle LoadAssetBundle(string abName)
    {
        string bName = this.path + "/" + abName;
        if (ABCache.ContainsKey(abName))
        {
            ABCache[abName].Count++;
            return ABCache[abName];
        }
        else
        {
            AssetBundle ab = AssetBundle.LoadFromFile(bName);
            MyAssetBundle myAsset = new MyAssetBundle(ab);
            myAsset.Count++;
            ABCache.Add(abName, myAsset);
            return myAsset;
        }
    }

    public void UnLoad(string abName)
    {
        if (dependDict.ContainsKey(abName))
        {
            var dependList = dependDict[abName];
            foreach (var depend in dependList)
            {
                UnLoadAssetBundle(depend);
            }
        }
        UnLoadAssetBundle(abName);
    }
    /// <summary>
    /// 删除AB包资源
    /// </summary>
    /// <param name="abName"></param>
    void UnLoadAssetBundle(string abName)
    {
        if (ABCache.ContainsKey(abName))
        {
            ABCache[abName].Count--;
            if (ABCache[abName].Count <= 0)
            {
                ABCache[abName].aBundle.Unload(false);
                ABCache.Remove(abName);
            }
        }
    }
}

public class MyAssetBundle
{
    public int Count;
    public AssetBundle aBundle;
    public MyAssetBundle(AssetBundle ab)
    {
        this.aBundle = ab;
    }
}
