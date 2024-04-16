namespace MoonCafe.Utils;

public static class ImageUploader
{
	public static async Task<string> UploadImageAsync(IWebHostEnvironment hostEnvironment, IFormFile img)
	{
		string wwwRootPath = hostEnvironment.WebRootPath;
		string fileName = FileNameControl(Path
			.GetFileNameWithoutExtension(img.FileName));
		string extension = Path.GetExtension(img.FileName);
		fileName = fileName + DateTime.Now.ToString("yyyyMMddhhmmss") + extension;

		string path = Path.Combine(wwwRootPath + "/ImageUpload/", fileName);

		using (var fileStream = new FileStream(path, FileMode.Create))
		{
			await img.CopyToAsync(fileStream);
		}

		return "/ImageUpload/" + fileName;
	}

	public static async Task<bool> DeleteImageAsync(IWebHostEnvironment hostEnvironment, string? path)
	{
		try
		{
			if (!string.IsNullOrEmpty(path))
			{
				string wwwRootPath = hostEnvironment.WebRootPath;
				string filePath = Path.Combine(wwwRootPath + path);
				FileInfo fileInfo = new FileInfo(filePath);
				if (fileInfo.Exists)
				{
					fileInfo.Delete();
					return true;
				}
			}

			return false;
		}
		catch (Exception ex)
		{
			return false;
		}
	}

	private static string FileNameControl(string filename)
	{
		filename.Replace("ş", "s");
		filename.Replace("ç", "c");
		filename.Replace("ü", "u");
		filename.Replace("ğ", "g");
		filename.Replace("ı", "i");
		filename.Replace("ö", "o");
		filename.Replace("Ş", "S");
		filename.Replace("Ç", "C");
		filename.Replace("Ü", "U");
		filename.Replace("Ğ", "G");
		filename.Replace("İ", "I");
		filename.Replace("Ö", "O");
		filename.Replace("-", "");
		filename.Replace("_", "");
		filename.Replace("?", "");
		filename.Replace("!", "");
		filename.Replace("#", "");
		filename.Replace("*", "");
		filename.Replace("/", "");
		filename.Replace("&", "");
		filename.Replace("%", "");
		filename.Replace("$", "");
		filename.Replace("^", "");
		filename.Replace("{", "");
		filename.Replace("}", "");
		filename.Replace("[", "");
		filename.Replace("]", "");
		filename.Replace(")", "");
		filename.Replace("(", "");
		filename.Replace(":", "");

		return filename;
	}

	internal static async Task DeleteImageAsync(object hostEnvironment, string? artistImageUrl)
	{
		throw new NotImplementedException();
	}
}