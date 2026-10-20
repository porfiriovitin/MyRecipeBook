using System.Text;

namespace CommomTestsUtilities.Files;

public static class ImageStreamBuilder
{
    public static MemoryStream Build(string format)
    {
        var content = format switch
        {
            "png" => Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+jRZkAAAAASUVORK5CYII="),
            "jpeg" => Convert.FromBase64String(
                "/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAP//////////////////////////////////////////////////////////////////////////////////////2wBDAf//////////////////////////////////////////////////////////////////////////////////////wAARCAABAAEDASIAAhEBAxEB/8QAFQABAQAAAAAAAAAAAAAAAAAAAAX/xAAUEAEAAAAAAAAAAAAAAAAAAAAA/8QAFAEBAAAAAAAAAAAAAAAAAAAAAP/EABQRAQAAAAAAAAAAAAAAAAAAAAD/2gAMAwEAAhEDEQA/AKAA/9k="),
            "gif" => Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7"),
            "txt" => Encoding.UTF8.GetBytes("This is a plain text document, not an image. It must be rejected during image validation."),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unsupported test file format.")
        };

        return new MemoryStream(content);
    }
}
