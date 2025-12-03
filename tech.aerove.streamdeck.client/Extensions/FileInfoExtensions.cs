namespace Tech.Aerove.StreamDeck.Client
{
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Gets the file as a data url. Only supports images
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Data url or null</returns>
        public static string? GetFileAsDataURL(this FileInfo file)
        {
            try
            {
                if (!file.Exists) { return null; }
                var bytes = File.ReadAllBytes(file.FullName);
                var base64String = Convert.ToBase64String(bytes);
                var mediaType = file.Extension.Replace(".", "").Replace("svg", "svg+xml");
                var dataURL = $"data:image/{mediaType};base64,{base64String}";
                return dataURL;
            }
            catch
            {
                return null;
            }

        }
    }
}
