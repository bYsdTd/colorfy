using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Checksums;

public static class GZIPWrapper
{
	public static void ZipFile(string filePathToZip, string zipedFilePath, int compressionLevel)
	{
		CheckPath(filePathToZip, zipedFilePath);

		System.IO.FileStream inputFileStream = new System.IO.FileStream(filePathToZip, System.IO.FileMode.Open);
		System.IO.FileStream outputFileStream = System.IO.File.Create(zipedFilePath);

		ZipOutputStream zipOutputStream = new ZipOutputStream(outputFileStream);
		ZipEntry zipEntry = new ZipEntry(Path.GetFileName(filePathToZip));

		zipOutputStream.PutNextEntry(zipEntry);
		zipOutputStream.SetLevel(compressionLevel);

		byte[] buffer = new byte[2048];
		int size = 0;

		try
		{
			do
			{
				var readSize = inputFileStream.Read(buffer, 0, buffer.Length);
				zipOutputStream.Write(buffer, 0, readSize);
				size += readSize;
			}
			while (size < inputFileStream.Length);
		}
		catch (Exception e)
		{
			throw;
		}
		finally
		{
			zipOutputStream.Finish();
			zipOutputStream.Close();
			inputFileStream.Close();
		}
	}

	public static void UnzipFile(string filePathToUnzip, string unzipedFilePath)
	{
		CheckPath(filePathToUnzip, unzipedFilePath);

		using (ZipInputStream s = new ZipInputStream(File.OpenRead(filePathToUnzip)))
		{
			ZipEntry entry = s.GetNextEntry();
			UtilsFunc.Assert (entry != null, "entry is null");

			using (FileStream outputStream = File.Create(unzipedFilePath))
			{
				int size = 0;  
				byte[] data = new byte[2048];  
				while (true)  
				{
					size = s.Read(data, 0, data.Length);  
					if (size > 0)  
					{  
						outputStream.Write(data, 0, size);  
					}  
					else
					{  
						break;  
					}  
				}
			}
			// No Dir supported in this function.
			UtilsFunc.Assert(null == s.GetNextEntry());
		}
	}

	public static byte[] UnzipBytes(byte[] input)
	{
		Inflater decompressor = new Inflater();
		decompressor.SetInput(input);
		byte[] ret = null;

		using (MemoryStream bos = new MemoryStream(input.Length))
		{
			byte[] buf = new byte[1024];
			while (!decompressor.IsFinished) {
				int count = decompressor.Inflate(buf);
				bos.Write(buf, 0, count);
			}
			ret = bos.ToArray();
		}

		return ret;
	}
	public static byte[] UnzipBytesWithoutHeader(byte[] input)
	{
		Inflater decompressor = new Inflater(true);
		decompressor.SetInput(input);
		byte[] ret = null;

		using (MemoryStream bos = new MemoryStream(input.Length))
		{
			byte[] buf = new byte[1024];
			while (!decompressor.IsFinished) {
				int count = decompressor.Inflate(buf);
				bos.Write(buf, 0, count);
			}
			ret = bos.ToArray();
		}

		return ret;
	}
	public static byte[] ZipBytes(byte[] input, int compressLevel)
	{
		UtilsFunc.Assert(input != null);
		Deflater compressor = new Deflater();
		compressor.SetLevel(compressLevel);

		compressor.SetInput(input);
		compressor.Finish();
		byte[] ret = null;

		using (MemoryStream bos = new MemoryStream(input.Length))
		{
			byte[] buf = new byte[1024];
			while (!compressor.IsFinished) {
				int count = compressor.Deflate(buf);
				bos.Write(buf, 0, count);
			}
			ret = bos.ToArray();
		}

		return ret;
	}

	private static void CheckPath(string inputFilePath, string outputFilePath)
	{
		UtilsFunc.Assert(!String.IsNullOrEmpty(inputFilePath));
		UtilsFunc.Assert(!String.IsNullOrEmpty(outputFilePath));
		UtilsFunc.Assert(File.Exists(inputFilePath));
		UtilsFunc.Assert(!File.Exists(outputFilePath));

		UtilsFunc.Assert(Directory.Exists(Path.GetDirectoryName(outputFilePath)));
	}
}