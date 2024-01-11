using UnityEngine;

using System.IO;
using System;
using System.Text;
using System.Threading;
using Unity.SharpZipLib.Zip;
using Unity.SharpZipLib.Checksum;

public delegate void ZipProgressHandler(string filename, long loadedsize, long toltalsize);
public delegate void ZipCompleteHandler();
public delegate void ZipErrorHandler(string error);

public class ZipTools
{

    public static bool isNeedSleep = false;

    /// <summary>
    /// ѹ�����Ŀ¼
    /// </summary>
    /// <param name="strDirectory">The directory.</param>
    /// <param name="zipedFile">The ziped file.</param>
    public static void ZipFileDirectory(string strDirectory, string zipedFile, ZipProgressHandler progressHandler = null, ZipCompleteHandler completeHandler = null)
    {
        ZipConstants.DefaultCodePage = Encoding.UTF8.CodePage;

        using (System.IO.FileStream ZipFile = System.IO.File.Create(zipedFile))
		{
            using (ZipOutputStream s = new ZipOutputStream(ZipFile))
            {
                ZipSetp(strDirectory, s, "", progressHandler, completeHandler);
                if (completeHandler != null)
                {
                    completeHandler();
                }
            }
        }
    }

    /// <summary>
    /// �ݹ����Ŀ¼
    /// </summary>
    /// <param name="strDirectory">The directory.</param>
    /// <param name="s">The ZipOutputStream Object.</param>
    /// <param name="parentPath">The parent path.</param>
    private static void ZipSetp(string strDirectory, ZipOutputStream s, string parentPath, ZipProgressHandler progressHandler = null, ZipCompleteHandler completeHandler = null)
    {
        ZipConstants.DefaultCodePage = Encoding.UTF8.CodePage;

        if (strDirectory[strDirectory.Length - 1] != Path.DirectorySeparatorChar)
		{
            strDirectory += Path.DirectorySeparatorChar;
        }

        string[] filenames = Directory.GetFileSystemEntries(strDirectory);
        int i = 0;
        int count = filenames.Length;
        foreach (string file in filenames)// �������е��ļ���Ŀ¼
        {

			string value = "\\";
# if UNITY_IPHONE || UNITY_IOS
			value = "/";
#endif

			if (Directory.Exists(file))// �ȵ���Ŀ¼��������������Ŀ¼�͵ݹ�Copy��Ŀ¼������ļ�
            {
                string pPath = parentPath;
                pPath += file.Substring(file.LastIndexOf(value) + 1);
                pPath += value;
                ZipSetp(file, s, pPath, progressHandler, completeHandler);
            }

            else // ����ֱ��ѹ���ļ�
            {
                //��ѹ���ļ�
                using (FileStream fs = File.OpenRead(file))
                {

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);

                    string fileName = parentPath + file.Substring(file.LastIndexOf(value) + 1);
                    ZipEntry entry = new ZipEntry(fileName);

                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;

                    fs.Close();
                    i++;

                    if (progressHandler != null)
                    {
                        progressHandler(fileName, i, count);
                    }
                    s.PutNextEntry(entry);

                    s.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }

    public static long GetFileCRC(string filename, byte[] buffer)
    {
        using (FileStream fs = File.OpenRead(filename))
        {
			Crc32 crc = new Crc32();

			int bufferLenght = buffer.Length;
			int nread = 0;

			while ((nread = fs.Read(buffer, 0, bufferLenght)) > 0)
			{
				crc.Update(buffer);
			}
			return crc.Value;
		}
    }

	#region ��ѹ�ļ���ѹ����
	/// <summary>
	/// ��ѹ���ļ�(ѹ���ļ��к�����Ŀ¼)
	/// </summary>
	/// <param name="zipfilepath">����ѹ�����ļ�·��</param>
	/// <param name="unzippath">��ѹ����ָ��Ŀ¼</param>
	/// <returns>��ѹ����ļ��б�</returns>
	public static void UnZipDir(string zipfilepath, string unzippath, ZipProgressHandler progressHandler = null, ZipCompleteHandler completeHandler = null, ZipErrorHandler errorHandler = null, bool replace = true)
    {
        try
        {
            ZipConstants.DefaultCodePage = Encoding.UTF8.CodePage;

            using (FileStream filestream = File.OpenRead(zipfilepath))
			{
                string zipfilename = Path.GetFileNameWithoutExtension(zipfilepath);

                //������Ŀ¼�Ƿ��ԡ�\\����β
                if (unzippath.EndsWithNonAlloc("/") == false || unzippath.EndsWithNonAlloc(":/") == false)
                {
                    unzippath += "/";
                }
                string directoryName = Path.GetDirectoryName(unzippath);
                //���ɽ�ѹĿ¼���û���ѹ��Ӳ�̸�Ŀ¼ʱ������Ҫ������
                if (!string.IsNullOrEmpty(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                
                ZipInputStream s = new ZipInputStream(filestream);

                StringBuilder filepathsb = new StringBuilder();
                string filePath;
                string fileName;
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    fileName = Path.GetFileName(theEntry.Name);
                    long read_size = 0;

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        //����ļ���ѹ�����СΪ0��ô˵������ļ��ǿյ�,��˲���Ҫ���ж���д��
                        if (theEntry.CompressedSize == 0)
                            break;

                        filepathsb.Clear();
                        filepathsb.Append(unzippath);
                        filepathsb.Append(theEntry.Name);
                        filepathsb.Replace("\\", "/");
                        filePath = filepathsb.ToString();

                        if (replace == false && File.Exists(filePath))
                        {
                            continue;
                        }

                        //���������Ŀ¼����Ŀ¼
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        FileStream streamWriter = File.Create(filePath);
                        
                        byte[] data = new byte[256 * 1024];
                        while (true)
                        {
                            int size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                read_size += size;
                                streamWriter.Write(data, 0, size);
                                if (progressHandler != null)
                                {
                                    string message = string.Format("[{0}] {1}", zipfilename, Path.GetFileNameWithoutExtension(theEntry.Name));
                                    progressHandler(message, read_size, s.Length);
                                }
                            }
                            else
                            {
                                break;
                            }

                            if(isNeedSleep)
                            {
                                Thread.Sleep(35);
                            }
                        }
                        streamWriter.Close();
                    }
                }
                s.Dispose();
                s.Close();
                //GC.Collect();
                if (completeHandler != null)
                    completeHandler();
            }
        }
        catch (Exception e)
        {
            if (errorHandler != null)
                errorHandler(e.Message);
            Debug.Log(e);

			File.Delete(zipfilepath);
        }

    }
#endregion
}
