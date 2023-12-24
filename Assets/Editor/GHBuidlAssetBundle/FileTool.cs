using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace Core
{
    /// <summary>
    /// 文件操作相关
    /// </summary>
    public class FileTool
    {
        public static readonly Encoding utf8 = new UTF8Encoding(false);
        /// 文件操作相关
        #region
        /// <summary> 
        /// 检测指定目录是否存在 
        /// </summary> 
        /// <param name="directoryPath">目录的绝对路径</param>         
        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
        /// <summary> 
        /// 检测指定文件是否存在,如果存在则返回true。 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param>         
        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }
        /// <summary> 
        /// 删除指定文件 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param> 
        public static void DeleteFile(string filePath)
        {
            if (IsExistFile(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        /// <summary> 
        /// DeleteDirectory
        /// </summary> 
        /// <param name="directoryPath">directoryPath</param>
        /// <param name="recursive">recursive folder</param> 
        public static bool DeleteDirectory(string directoryPath, bool recursive = true)
        {
            if (IsExistDirectory(directoryPath))
            {
                try
                {
                    foreach (string file in Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories))
                    {
                        File.Delete(file);
                    }
                    Directory.Delete(directoryPath, recursive);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            return false;
        }
        /// <summary>
        /// 是否绝对路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsAbsolutePath(string filePath)
        {
            return filePath.Contains("://");
        }

        ///返回指定目录中与指定的搜索模式匹配的文件的名称（包含它们的路径），并使用一个值以确定是否搜索子目录。
        ///dirPaths目录数组,searchPattern, 多个以|分隔,如[*.png|*.jpg]
        public static List<string> GetAllFilePaths(string[] dirPaths, string searchPattern = "*",
                                               SearchOption searchOption = SearchOption.AllDirectories)
        {
            List<string> ls = new List<string>();
            for (int i = 0, len = dirPaths.Length; i < len; i++)
            {
                string[] ps = GetAllFilePaths(dirPaths[i], searchPattern, searchOption);
                ls.AddRange(ps);
            }
            return ls;
        }
        ///返回指定目录中与指定的搜索模式匹配的文件的名称（包含它们的路径），并使用一个值以确定是否搜索子目录。
        ///dirPaths目录,searchPattern, 多个以|分隔,如[*.png|*.jpg]
        public static string[] GetAllFilePaths(string dirPaths, string searchPattern = "*",
                                               SearchOption searchOption = SearchOption.AllDirectories)
        {
            try
            {
                // dirPaths = dirPaths.Replace("file://", "");// dirPaths.Replace('/', '\\');
                //Debug.Log(IsExistFile(dirPaths)+":"+IsExistDirectory(dirPaths)+":"+dirPaths);
                if (IsExistFile(dirPaths))
                {
                    return (searchPattern == "*" || dirPaths.Contains(searchPattern)) ?
                    new string[] { dirPaths } : new string[0];
                }
                if (IsExistDirectory(dirPaths))
                {
                    if (!searchPattern.Contains("|"))
                    {
                        string[] lst = Directory.GetFiles(dirPaths, searchPattern, searchOption);
                        
                        for(int i =0;i< lst.Length;i++)
                        {
                            lst[i] = lst[i].Replace("\\", "/");
                        }
                        return lst;
                    }
                    else
                    {
                        string[] ss = searchPattern.Split('|');
                        List<string> ls = new List<string>();
                        string[] arr;
                        for (int i = 0, len = ss.Length; i < len; i++)
                        {
                            arr = Directory.GetFiles(dirPaths, ss[i], searchOption);
                            for (int j = 0; j < arr.Length; j++)
                            {
                                arr[j] = arr[j].Replace("\\", "/");
                            }
                            ls.AddRange(arr);
                        }
                        return ls.ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return new string[0];
        }

        ///获取当前文件的附属信息
        public static FileInfo GetFileInfo(string path)
        {
            try
            {
                return new FileInfo(path);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return null;
        }
        ///获取当前文件的大小（字节）。
        public static long GetFileSize(string path)
        {
            try
            {
                FileInfo f = GetFileInfo(path);
                if (f.Exists) return f.Length;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return 0;
        }

        /// <summary>
        /// 二进制写入文件
        /// </summary>
        /// <param name="Path">路径</param>
        /// <param name="data">数据</param>
        /// <param name="append">追加</param>
        /// <returns>成功或失败</returns>
        static public bool WriteBytesToFile(string path, byte[] data, bool append = true)
        {
            bool res = false;
            if (data == null || path == null || path.Length < 1 )
            {
                Debug.LogError("FileTool.writeBytesToFile(),args Error");
                return res;
            }

            string dir = Path.GetDirectoryName(path);
            if (!IsExistDirectory(dir)) Directory.CreateDirectory(dir);
            //Debug.Debug(Path+"---"+Path.GetDirectoryName(Path)+"--"+IsExistFile(Path));

            if (!append)
            {
                File.WriteAllBytes(path, data);
            }
            else
            {
                using (FileStream fs = new FileStream(path, FileMode.Append))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            res = true;

            return res;
        }

        ///创建目录
        static public DirectoryInfo CreateDirectory(string dir)
        {
            if (!string.IsNullOrEmpty(dir) && !IsExistDirectory(dir)) return Directory.CreateDirectory(dir);
            return null;
        }
        /// <summary>
        /// 二进制流数据文件读取
        /// </summary>
        /// <param name="Path">路径</param>
        /// <returns>数据</returns>
        static public byte[] ReadBytesFromFile(string path)
        {
            if (path == null || path.Length < 1)
            {
                Debug.LogError("FileTool.readBytesFromFile(),args Error");
            }
            byte[] data = null;
            try
            {
                if (IsExistFile(path))
                    data = File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return data;
        }


        ///写文本到文件,path路径,contents内容,encoding编码,append追加
        static public bool WriteAllText(string path, string contents, Encoding encoding = null, bool append = false)
        {
            if (path == null || path.Length < 1)
            {
                Debug.LogError("FileTool.WriteAllText:path Error");
            }
            if (contents == null || contents.Length < 1)
            {
                Debug.Log("FileTool.WriteAllText:contents Empty");
            }
            if (encoding == null)
            {
                encoding = utf8;
            }
            try
            {
                string dir = Path.GetDirectoryName(path);
                if (!IsExistDirectory(dir)) Directory.CreateDirectory(dir);

                if (append) File.AppendAllText(path, contents, encoding);
                else File.WriteAllText(path, contents, encoding);
            }
            catch (Exception)
            {
                //Debug.LogError(e);
                return false;
            }
            return true;
        }
        //读取文本从文件
        static public string ReadAllText(string path, Encoding encoding = null)
        {
            if (path == null || path.Length < 1)
            {
                Debug.LogError("FileTool.readAllText(),args Error");
            }
            if (encoding == null)
            {
                encoding = utf8;
            }
            try
            {
                if (IsExistFile(path))
                    return File.ReadAllText(path, encoding);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return null;
        }

        static public bool CopyFullPath(string soucrePath, string destPath, bool overwrite = true)
        {
            try
            {
                destPath = Path.GetFullPath(destPath);
                string dir = Path.GetDirectoryName(destPath);
                if (!IsExistDirectory(dir)) Directory.CreateDirectory(dir);
                soucrePath = Path.GetFullPath(soucrePath);
                if (IsExistFile(soucrePath))
                    System.IO.File.Copy(soucrePath, destPath, overwrite);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="soucrePath"></param>
        /// <param name="destPath"></param>
        /// <param name="overwrite"></param>
        static public bool Copy(string soucrePath, string destPath, bool overwrite = true)
        {
            try
            {
                string dir = Path.GetDirectoryName(destPath);
                if (!IsExistDirectory(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                if (IsExistFile(soucrePath))
                {
                    File.Copy(soucrePath, destPath, overwrite);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
            return true;
        }
		/// <summary>
		/// 移动文件
		/// </summary>
		/// <param name="soucrePath"></param>
		/// <param name="destPath"></param>
		static public bool MoveFile(string soucrePath, string destPath)
		{
            try
			{
				if(IsExistDirectory(soucrePath))Directory.Move(soucrePath,destPath);
				else if(IsExistFile(soucrePath))File.Move(soucrePath, destPath);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				return false;
			}
			return true;
		}

        /// <summary>
        /// change extentions all file if folder
        /// </summary>
        /// <param name="root"></param>
        /// <param name="old"></param>
        /// <param name="news"></param>
        public static void ReplaceExt(string root, string old, string news)
        {
            string[] filePaths = Directory.GetFiles(root, "*." + old, SearchOption.AllDirectories);
            float count = 0;
            int maxLength = filePaths.Length;
            foreach (string f in filePaths)
            {

#if UNITY_EDITOR
                //progress change
                count++;
                EditorUtility.DisplayProgressBar(string.Format("Change extentions from \"{0}\" to \"{1}\"", old, news), string.Format("Changing {0}/{1}", count, maxLength), (count / maxLength));
#endif
                //
                string cn = Path.ChangeExtension(f, news);
                if (File.Exists(cn)) File.Delete(cn);
                File.Move(f, cn);
            }
#if UNITY_EDITOR

            EditorUtility.ClearProgressBar();
#endif
        }
#if UNITY_EDITOR
        /// <summary>
        /// move unity asset
        /// </summary>
        /// <param name="soucrePath"></param>
        /// <param name="destPath"></param>
        /// <returns></returns>
        static public bool MoveAsset(string soucrePath, string destPath)
        {
            string dd = Application.dataPath;
            string ad = "Assets";
            soucrePath = soucrePath.Replace("\\", "/").Replace(dd, ad);
            destPath = destPath.Replace("\\", "/").Replace(dd, ad);
            if (!IsExistDirectory(Path.GetDirectoryName(destPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                AssetDatabase.Refresh();
            }
            if (IsExistFile(destPath))
            {
                Debug.LogError("Destination file is exist:" + destPath);
                return false;
            }
            string res = AssetDatabase.MoveAsset(soucrePath, destPath);
            Debug.Log(res);
            AssetDatabase.Refresh();
            return true;
        }
        /// <summary>
        /// Move all Unity Asset in a Folder to another
        /// </summary>
        /// <param name="directorySource">directory Source</param>
        /// <param name="directoryTarget">directory Target</param>
        static public void MoveFolder(string directorySource, string directoryTarget)
		{
            if (!FileTool.IsExistDirectory(directorySource)) return;
  
            if (!Directory.Exists(directoryTarget))
			{
				Directory.CreateDirectory(directoryTarget);
				AssetDatabase.Refresh();
			}

			DirectoryInfo directoryInfo = new DirectoryInfo(directorySource);
			FileInfo[] files = directoryInfo.GetFiles();
			string dd = Application.dataPath;
			string ad = "Assets";
			string dt = directoryTarget.Replace(dd,ad)+"/";
            string old;

            foreach (FileInfo file in files)
			{
				old = file.FullName.Replace("\\","/").Replace(dd,ad);
                AssetDatabase.MoveAsset(old,dt+file.Name);
			}

			DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
			foreach (DirectoryInfo dir in directoryInfoArray)
			{
				MoveFolder(Path.Combine(directorySource, dir.Name), Path.Combine(directoryTarget, dir.Name));
			}
		}
        /// <summary>
        /// CopyFolderTo
        /// </summary>
        /// <param name="directorySource">directory Source</param>
        /// <param name="directoryTarget">directory Target</param>
        static public void CopyFolderTo(string directorySource, string directoryTarget)
		{
			if (!Directory.Exists(directoryTarget))
			{
				Directory.CreateDirectory(directoryTarget);
			}
 
			DirectoryInfo directoryInfo = new DirectoryInfo(directorySource);
			FileInfo[] files = directoryInfo.GetFiles();

			foreach (FileInfo file in files)
			{
				file.CopyTo(Path.Combine(directoryTarget, file.Name));
			}

			DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
			foreach (DirectoryInfo dir in directoryInfoArray)
			{
				CopyFolderTo(Path.Combine(directorySource, dir.Name), Path.Combine(directoryTarget, dir.Name));
			}
		}
#else
		/// <summary>
		/// 从一个目录将其内容移动到另一目录  
		/// </summary>
		/// <param name="directorySource">源目录</param>
		/// <param name="directoryTarget">目标目录</param>
		static public void MoveFolderTo(string directorySource, string directoryTarget)
		{
			//检查是否存在目的目录  
			if (!Directory.Exists(directoryTarget))
			{
				Directory.CreateDirectory(directoryTarget);
			}
			//先来移动文件  
			DirectoryInfo directoryInfo = new DirectoryInfo(directorySource);
			FileInfo[] files = directoryInfo.GetFiles();
			//移动所有文件  
			foreach (FileInfo file in files)
			{
				//如果自身文件在运行，不能直接覆盖，需要重命名之后再移动  
				if (File.Exists(Path.Combine(directoryTarget, file.Name)))
				{
					if (File.Exists(Path.Combine(directoryTarget, file.Name + ".bak")))
					{
						File.Delete(Path.Combine(directoryTarget, file.Name + ".bak"));
					}
					File.Move(Path.Combine(directoryTarget, file.Name), Path.Combine(directoryTarget, file.Name + ".bak"));
					
				}
				file.MoveTo(Path.Combine(directoryTarget, file.Name));
				
			}
			//最后移动目录  
			DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
			foreach (DirectoryInfo dir in directoryInfoArray)
			{
				MoveFolderTo(Path.Combine(directorySource, dir.Name), Path.Combine(directoryTarget, dir.Name));
			}
			if (Directory.Exists(directorySource))Directory.Delete (directorySource);
		}
		/// <summary>
		/// 从一个目录将其内容复制到另一目录
		/// </summary>
		/// <param name="directorySource">源目录</param>
		/// <param name="directoryTarget">目标目录</param>
		static public void CopyFolderTo(string directorySource, string directoryTarget)
		{
			//检查是否存在目的目录  
			if (!Directory.Exists(directoryTarget))
			{
				Directory.CreateDirectory(directoryTarget);
			}
			//先来复制文件  
			DirectoryInfo directoryInfo = new DirectoryInfo(directorySource);
			FileInfo[] files = directoryInfo.GetFiles();
			//复制所有文件  
			foreach (FileInfo file in files)
			{
				file.CopyTo(Path.Combine(directoryTarget, file.Name));
			}
			//最后复制目录  
			DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
			foreach (DirectoryInfo dir in directoryInfoArray)
			{
				CopyFolderTo(Path.Combine(directorySource, dir.Name), Path.Combine(directoryTarget, dir.Name));
			}
		}
#endif
#endregion


    }
}