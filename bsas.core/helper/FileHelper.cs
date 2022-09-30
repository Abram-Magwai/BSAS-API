namespace bsas.core.helper
{
    public static class FileHelper
    {
        public static byte[] streamToByteArray(Stream input)
        {
            MemoryStream ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }
        public static bool IsPdf(string fileName)
        {
            string[] fileNameParts = fileName.Split(".");
            if (fileNameParts[fileNameParts.Length - 1] != "pdf")
                return false;
            return true;
        }
    }
}