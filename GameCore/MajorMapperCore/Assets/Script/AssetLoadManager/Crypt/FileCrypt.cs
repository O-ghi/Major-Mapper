using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

public class FileCrypt
{
	private const int BUFFER_SIZE = 1024 * 512;

	private readonly byte[] SecretKey;
	private readonly Thread mThread;
	private readonly Stream mSrcStream;
	private readonly Stream mDstStream;

	public readonly uint TotalSize;

	private enum CryptStatus
	{
		None,
		Success,
		Failed,
	}

	private CryptStatus Status = CryptStatus.None;

	public bool IsDone => Status != CryptStatus.None;

	public bool IsSuccess => Status == CryptStatus.Success;

	public bool IsFailed => Status == CryptStatus.Failed;

	private uint nReadSize = 0;

	public uint ReadSize { get => nReadSize; }

	public float Progress { get => (float)nReadSize / (float)TotalSize; }

	private FileCrypt(byte[] key, Stream dstStream, Stream srcStream, int srcPos, uint totalSize)
	{
		SecretKey = key;
		mThread = new Thread(Run);
		mSrcStream = srcStream;
		mDstStream = dstStream;
		nReadSize = (uint)srcPos;

		TotalSize = totalSize;
	}

	public static FileCrypt Create(string key, string srcPath, string destPath)
	{
		byte[] keys = Encoding.UTF8.GetBytes(key);
		return Create(keys, srcPath, destPath);
	}

	public static FileCrypt Create(byte[] key, string srcPath, string destPath)
	{
		try
		{
			Stream srcStream = File.OpenRead(srcPath);
			Stream dstStream = File.OpenWrite(destPath);

			FileCrypt crypt = new FileCrypt(key, dstStream, srcStream, 0, (uint)srcStream.Length);
			return crypt;
		}
		catch (System.Exception ex)
		{
			Debug.LogException(ex);
			return null;
		}
	}

	public static bool FileCopyDirectly(byte[] keys, string srcPath, string destPath)
	{
		try
		{
			if (keys == null || keys.Length <= 0)
			{
				File.Copy(srcPath, destPath);
				return true;
			}

			byte[] buffer = new byte[BUFFER_SIZE];
			uint keyLength = (uint)keys.Length;

			using (Stream srcStream = File.OpenRead(srcPath))
			{
				long fileSize = srcStream.Length;
				uint readSize = 0;

				using (Stream dstStream = File.OpenWrite(destPath))
				{
					int read = 0;

					while ((read = srcStream.Read(buffer, 0, BUFFER_SIZE)) > 0)
					{
						Make(keys, readSize, buffer, read, keyLength);
						dstStream.Write(buffer, 0, read);

						readSize += (uint)read;
					}
				}

				return fileSize == readSize;
			}
		}
		catch (System.Exception ex)
		{
			Debug.LogException(ex);
		}

		return false;
	}

	public bool Start()
	{
		if (IsDone)
			return false;

		mThread.Start();
		return true;
	}

	private void Run()
	{
		byte[] buffer = new byte[BUFFER_SIZE];
		uint keyLength = (uint)SecretKey.Length;

		int read = 0;

		try
		{
			while ((read = mSrcStream.Read(buffer, 0, BUFFER_SIZE)) > 0)
			{
				Make(SecretKey, nReadSize, buffer, read, keyLength);
				mDstStream.Write(buffer, 0, read);

				nReadSize += (uint)read;
			}

			mSrcStream.Close();
			mDstStream.Close();
			Status = CryptStatus.Success;
		}
		catch (Exception ex)
		{
			Status = CryptStatus.Failed;
			Debug.LogException(ex);
		}
	}

	private static void Make(byte[] secretKey, uint iPosition, byte[] buffer, int bufferCount, uint keyLength)
	{
		for (uint i = 0; i < bufferCount; i++)
		{
			uint pos = (iPosition + i) % keyLength;
			byte key = secretKey[pos];
			byte b = buffer[i];
			buffer[i] = (byte)(b ^ key);
		}
	}
}
