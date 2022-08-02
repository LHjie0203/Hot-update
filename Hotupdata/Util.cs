using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Util : MonoBehaviour
{
    /// <summary>
    /// 通过所给的文件路径得到所有的文件资源路径
    /// </summary>
    /// <param name="path">查找资源的路径</param>
    /// <param name="screenList">筛选的文件类型</param>
    /// <param name="include">是否包括</param>
    /// <returns></returns>
    public static string[] GetPathAllFiles(string path, string[] screenList, bool include = false)
    {
        //路径是否合法
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
        //找到路径下的所有文件夹下的资源(包括子文件夹)
        string[] allFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        //包括所筛选的类型
        if (include)
        {
            return allFiles.Where((f) => screenList.Contains(Path.GetExtension(f))).ToArray();
        }
        //不包括筛选的类型
        else
        {
            return allFiles.Where((f) => !screenList.Contains(Path.GetExtension(f))).ToArray();
        }
    }
}
