using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ManicDigger.Common
{
	/// <summary>
	/// Provides functions for loading and reading asset files from given folders
	/// </summary>
	public class AssetLoader
	{
		public AssetLoader(string[] datapaths_)
		{
			this.datapaths = datapaths_;
		}
		string[] datapaths;
		public void LoadAssetsAsync(AssetList list, FloatRef progress)
		{
			List<Asset> assets = new List<Asset>();
			foreach (string path in datapaths)
			{
				try
				{
					if (!Directory.Exists(path))
					{
						continue;
					}
					foreach (string s in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
					{
						try
						{
							FileInfo f = new FileInfo(s);
							if (f.Name.Equals("thumbs.db", StringComparison.InvariantCultureIgnoreCase))
							{
								continue;
							}
							
							byte[] data = File.ReadAllBytes(s);
							int dataLength = data.Length;
							string md5Hash = Md5(data);
							
							// Calculate relative path from base data directory
							string relativePath = s;
							if (relativePath.StartsWith(path))
							{
								relativePath = relativePath.Substring(path.Length);
								if (relativePath.StartsWith(Path.DirectorySeparatorChar.ToString()) || 
								    relativePath.StartsWith(Path.AltDirectorySeparatorChar.ToString()))
								{
									relativePath = relativePath.Substring(1);
								}
							}
							else
							{
								relativePath = f.Name;
							}
							
							// Normalize path separators to forward slashes and convert to lowercase
							string normalizedPath = relativePath.Replace(Path.DirectorySeparatorChar, '/').Replace(Path.AltDirectorySeparatorChar, '/').ToLowerInvariant();
							
							// Add asset with full relative path (e.g., "gui/wow/actionbar_bg.png")
							Asset a = new Asset();
							a.data = data;
							a.dataLength = dataLength;
							a.name = normalizedPath;
							a.md5 = md5Hash;
							assets.Add(a);
							
							// Also add asset with just filename for backward compatibility (e.g., "actionbar_bg.png")
							// This ensures old code that references just filenames continues to work
							// Note: This creates duplicate Asset objects, but they share the same byte array reference
							// (C# arrays are reference types, so both assets point to the same data in memory)
							// Future optimization: Use dictionary to map multiple names to single Asset instance
							if (normalizedPath != f.Name.ToLowerInvariant())
							{
								Asset aCompat = new Asset();
								aCompat.data = data;
								aCompat.dataLength = dataLength;
								aCompat.name = f.Name.ToLowerInvariant();
								aCompat.md5 = md5Hash;
								assets.Add(aCompat);
							}
						}
						catch
						{
						}
					}
				}
				catch
				{
				}
			}
			progress.value = 1;
			list.count = assets.Count;
			// Ensure array is large enough to hold all assets
			int arraySize = Math.Max(2048, assets.Count);
			list.items = new Asset[arraySize];
			for (int i = 0; i < assets.Count; i++)
			{
				list.items[i] = assets[i];
			}
		}

		MD5CryptoServiceProvider sha1 = new MD5CryptoServiceProvider();
		string Md5(byte[] data)
		{
			string hash = ToHex(sha1.ComputeHash(data), false);
			return hash;
		}

		public static string ToHex(byte[] bytes, bool upperCase)
		{
			StringBuilder result = new StringBuilder(bytes.Length * 2);

			for (int i = 0; i < bytes.Length; i++)
			{
				result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
			}

			return result.ToString();
		}
	}
}
