using System.Collections.Generic;
using System.IO;
using System.Text;

public static class StringCrypt
{
	private const string ValidChars = @"!#$%&'(),. +-_[]`^;{}~";

	private const char SeparatorChar = '=';
	private const char MinChar = ' ';
	private const char MaxChar = '~';
	private const int CharSize = MaxChar - MinChar + 1;

	private static char[] EncryptChars = new char[CharSize];        // 加密字符表
	private static char[] DecryptChars = new char[CharSize];        // 解密字符表

	public static int RandomSeed = 0;

	private static StringBuilder Builder = new StringBuilder();

	public static void Init(int seed)
	{
		RandomSeed = seed;

		if (0 == seed)
			return;

		EncryptChars['/' - MinChar] = EncryptChars['\\' - MinChar] = SeparatorChar;
		DecryptChars[SeparatorChar - MinChar] = Path.DirectorySeparatorChar;

		List<char> clist = new List<char>();
		for (char c = '0'; c <= '9'; c++)
		{
			clist.Add(c);
		}

		for (char c = 'a'; c <= 'z'; c++)
		{
			clist.Add(c);
		}

		for (char c = 'A'; c <= 'Z'; c++)
		{
			clist.Add(c);
		}

		foreach (char c in ValidChars)
		{
			clist.Add(c);
		}

		List<char> elist = new List<char>(clist);

		int len = 0;
		while ((len = clist.Count - 1) >= 0)
		{
			RandomSeed = RandomSeed * 214013 + 2531011;
			int r = (RandomSeed >> 16) & len;

			char c = clist[len];
			char e = elist[r];

			clist.RemoveAt(len);
			elist.RemoveAt(r);

			EncryptChars[c - MinChar] = e;
			DecryptChars[e - MinChar] = c;
		}
	}

	public static string Encrypt(string str)
	{
		if (0 == RandomSeed)
			return str;

		Builder.Length = 0;

		for (int i = 0; i < str.Length; ++i)
		{
			char c = str[i];
			char d = EncryptChars[c - MinChar];
			Builder.Append(d);
		}

		return Builder.ToString();
	}

	public static string Decrypt(string str)
	{
		if (0 == RandomSeed)
			return str;

		Builder.Length = 0;

		for (int i = 0; i < str.Length; ++i)
		{
			char c = str[i];
			char d = DecryptChars[c - MinChar];
			Builder.Append(d);
		}

		return Builder.ToString();
	}
}
