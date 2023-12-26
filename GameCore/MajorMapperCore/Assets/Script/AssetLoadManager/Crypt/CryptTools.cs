using System;
using System.IO;
using System.Text;
using UnityEngine;


public delegate void CryptProgressHandler(string filename, long loadedsize, long toltalsize);

public static class CryptTools
{
	private static string SecretKey;
	private static byte[] SecretKeyBytes;
	private const string InnerKey = "InnerKey";

	public static void Init(string key)
	{
		SecretKey = key;

		if (string.IsNullOrEmpty(key))
		{
			StringCrypt.Init(0);
		}
		else
		{
			SecretKeyBytes = Encoding.UTF8.GetBytes(key);

			byte[] innerKey = Encoding.UTF8.GetBytes(InnerKey);
			int innerLength = innerKey.Length;

			for (int i = 0; i < SecretKeyBytes.Length; ++i)
			{
				byte ckey = innerKey[i % innerLength];
				SecretKeyBytes[i] = (byte)(ckey ^ SecretKeyBytes[i]);
			}

			int seed = MakeSeed(SecretKeyBytes);
			StringCrypt.Init(seed);
		}
	}

	public static void Init()
	{
		string key = GameInfoManager.GetAttibute("secret");
		Init(key);
	}

	private static int MakeSeed(byte[] keys)
	{
		int seed = 0;

		int count = keys.Length / 4;
		int secretLen = SecretKeyBytes.Length;

		for (int i = 0; i < count; i++)
		{
			int index = i * count;

			int b0 = index + 0 >= secretLen ? 0 : SecretKeyBytes[index + 0];
			int b1 = index + 1 >= secretLen ? 0 : SecretKeyBytes[index + 1];
			int b2 = index + 2 >= secretLen ? 0 : SecretKeyBytes[index + 2];
			int b3 = index + 3 >= secretLen ? 0 : SecretKeyBytes[index + 3];

			int temp = (b3 << 24) ^ (b2 << 16) ^ (b1 << 8) ^ b0;
			seed ^= temp;
		}

		return seed;
	}

	private const string EXTENSION = ".bundle";
	private const string SEARCH_PATTERN = "*" + EXTENSION;
	private static readonly int EXTENSION_LENGTH = EXTENSION.Length;
	private const int MAX_PATH = 260 - 10;

	public static void DirectoryCopyEncrypt(string srcPath, string dstPath, CryptProgressHandler handler = null)
	{
		DirectoryInfo dir = new DirectoryInfo(srcPath);
		int subIndex = dir.FullName.Length + 1;

		FileInfo[] files = dir.GetFiles(SEARCH_PATTERN, SearchOption.AllDirectories);

		int max = 0;
		int error = 0;
		int fileCount = files.Length;
		int iFile = 0;

		bool needEncrypt = !string.IsNullOrEmpty(SecretKey);

		Debug.LogFormat("srcPath:{0} FullName:{1} needEncrypt:{2}", srcPath, dir.FullName, needEncrypt);

		foreach (FileInfo file in files)
		{
			++iFile;

			string fullName = file.FullName;

			string destPath, subPath;

			if (needEncrypt)
			{
				subPath = fullName.Substring(subIndex, fullName.Length - subIndex - EXTENSION_LENGTH);

				string encryptString = StringCrypt.Encrypt(subPath);
				string decryptString = StringCrypt.Decrypt(encryptString);

				if (subPath != decryptString)
				{
					Debug.LogErrorFormat("Encrypt error! subPath:{0} encrypt:{1} decrypt:{2} fullName:{3}", subPath, encryptString, decryptString, fullName);
					++error;
				}

				if (subPath.Length > max)
					max = subPath.Length;

				if (subPath.Length > MAX_PATH)
				{
					Debug.LogErrorFormat("subPath.Length error! subPath:{0} length:{1}", subPath, subPath.Length);
					++error;
				}

				destPath = Path.Combine(dstPath, encryptString);
			}
			else
			{
				subPath = fullName.Substring(subIndex, fullName.Length - subIndex);
				destPath = Path.Combine(dstPath, subPath);
				string parentPath = Path.GetDirectoryName(destPath);
				CreateDir(parentPath);
			}

			if (handler != null)
				handler(subPath, iFile, fileCount);

			FileCrypt.FileCopyDirectly(SecretKeyBytes, fullName, destPath);
		}

		if (handler != null)
			handler("", 1, 0);
	}

	public static void FileCopy(string srcDir, string dstDir, string name)
	{
		try
		{
			string srcFilePath = Path.Combine(srcDir, name);
			string dstFilePath = Path.Combine(dstDir, name);

			string dstParent = Path.GetDirectoryName(dstFilePath);
			CreateDir(dstParent);

			Debug.LogFormat("FileCopy srcFilePath:{0} dstFilePath:{1}", srcFilePath, dstFilePath);

			FileInfo srcFileInfo = new FileInfo(srcFilePath);
			srcFileInfo.CopyTo(dstFilePath);
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	public static void FileCopyEncrypt(string srcDir, string dstDir, string subPath)
	{
		if (!subPath.EndsWith(EXTENSION))
		{
			FileCopy(srcDir, dstDir, subPath);
		}
		else
		{
			string srcPath;
			if (0 == StringCrypt.RandomSeed)
			{
				srcPath = Path.Combine(srcDir, subPath);
			}
			else
			{
				string subName = subPath.Substring(0, subPath.Length - EXTENSION_LENGTH);
				string encryptString = StringCrypt.Encrypt(subName);
				srcPath = Path.Combine(srcDir, encryptString);
			}

			string dstPath = Path.Combine(dstDir, subPath);
			string dstParent = Path.GetDirectoryName(dstPath);
			CreateDir(dstParent);

			FileCrypt.FileCopyDirectly(SecretKeyBytes, srcPath, dstPath);
		}
	}

	public static void CreateDir(string dirPath, bool clear = false)
	{
		if (!Directory.Exists(dirPath))
		{
			string parentPath = Path.GetDirectoryName(dirPath);
			_CreateDir(parentPath);

			Directory.CreateDirectory(dirPath);
			return;
		}

		if (!clear)
			return;

		foreach (string file in Directory.EnumerateDirectories(dirPath))
		{
			Directory.Delete(file, true);
		}

		foreach (string file in Directory.EnumerateFiles(dirPath))
		{
			File.Delete(file);
		}
	}

	private static void _CreateDir(string dirPath)
	{
		if (Directory.Exists(dirPath))
			return;

		string parentPath = Path.GetDirectoryName(dirPath);
		_CreateDir(parentPath);
		Directory.CreateDirectory(dirPath);
	}
}
