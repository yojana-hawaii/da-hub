using SkiaSharp;

namespace mvc.Utilities;

public static class ResizeImage
{
    /// <summary>
    /// David Stovell - Youtube F24 PROG1322 07.5 Step 13 Graphics and Image
    /// maintains aspect ratio
    /// example: tall and narrow image > shink until height matches max_height but width will be small than max_width
    /// If image smaller, it will not be enlarged
    /// For WebP image at full Quality without extra parameters
    /// WebP not as universally supported like png, jpg etc
    /// </summary>
    /// <param name="originalImage">Byte Array from uploaded file</param>
    /// <param name="max_height">Default 75</param>
    /// <param name="max_width">Default 90</param>
    /// <returns>Byte Array of resized image > MIME type "image/webp</returns>
    public static Byte[] ShrinkImageWebp(Byte[] originalImage, int max_height = 75, int max_width = 90)
    {

        SKSurface surface = ShrinkImage(originalImage, max_height, max_width);
        using SKImage newImage = surface.Snapshot();
        using SKData newImageData = newImage.Encode(SKEncodedImageFormat.Webp, 100);

        return newImageData.ToArray();
    }

    


    //==========================================================================================================



    /// <summary>
    /// View Model used as return type for the shrinkImage() method
    /// that allows you to specify image format (Mime Type) and Quality rather than default WebP above
    /// </summary>
    public class ImageVM
    {
        public Byte[]? Content { get; set; }
        public string? MimeType { get; set; }
    }

    /// <summary>
    /// same as above in maintaining aspect ratio and smaller image will not be enlarged
    /// change image format and mime type or lower quality
    /// </summary>
    /// <param name="originalImage">Byte Array from the uploaded file</param>
    /// <param name="max_height">Default 100</param>
    /// <param name="max_width">Default 120</param>
    /// <param name="selectMimeType">Default format set to WebP but can be changed</param>
    /// MIME type need to match: image/webp is the default
    /// <param name="quality">value from 1 to 100. Defaault 100 for best quality</param>
    /// <returns></returns>
    public static ImageVM ShrinkImageAnyFormatAndQuality(Byte[] originalImage, int max_height = 100, int max_width = 120, 
        SKEncodedImageFormat selectMimeType = SKEncodedImageFormat.Webp, int quality = 100)
    {
        SKSurface surface = ShrinkImage(originalImage, max_height, max_width);
        using SKImage newImage = surface.Snapshot();
        using SKData newImageData = newImage.Encode(selectMimeType, quality);

        ImageVM imageVM = new ImageVM
        {
            Content = newImageData.ToArray(),
            MimeType = selectMimeType switch
            {
                SKEncodedImageFormat.Bmp => "image/bmp",
                SKEncodedImageFormat.Gif => "image/gif",
                SKEncodedImageFormat.Ico => "image/vnd.microsoft.icon",
                SKEncodedImageFormat.Jpeg => "image/jpeg",
                SKEncodedImageFormat.Png => "image/png",
                SKEncodedImageFormat.Wbmp => "image/wbmp",
                SKEncodedImageFormat.Webp => "image/webp",
                SKEncodedImageFormat.Pkm => "image/octet-stream",
                SKEncodedImageFormat.Ktx => "image/ktx",
                SKEncodedImageFormat.Astc => "image/png",
                SKEncodedImageFormat.Dng => "image/DNG",
                SKEncodedImageFormat.Heif => "image/heif",
                SKEncodedImageFormat.Avif => "image/",
                _ => "image/jpeg"
            }
        };

        return imageVM;
    }


    private static SKSurface ShrinkImage(byte[] originalImage, int max_height, int max_width)
    {
        using SKMemoryStream sourceStream = new SKMemoryStream(originalImage);
        using SKCodec codec = SKCodec.Create(sourceStream);
        sourceStream.Seek(0);

        using SKImage image = SKImage.FromEncodedData(SKData.Create(sourceStream));
        int newHeight = image.Height;
        int newWidth = image.Width;

        if (max_height > 0 && newHeight > max_height)
        {
            double scale = (double)max_height / newHeight;
            newHeight = max_height;
            newWidth = (int)Math.Floor(newWidth * scale);
        }
        if (max_width > 0 && newWidth > max_width)
        {
            double scale = (double)max_width / newWidth;
            newWidth = max_width;
            newHeight = (int)Math.Floor(newHeight * scale);
        }

        var info = codec.Info.ColorSpace.IsSrgb ? new SKImageInfo(newWidth, newHeight) : new SKImageInfo(newWidth, newHeight);
        SKSurface surface = SKSurface.Create(info);
        SKPaint paint = new SKPaint();

        //High quality without antialiasing
        paint.IsAntialias = true;

        //paint.FilterQuality = SKFilterQuality.High; > FilterQuality using in youtube is obsolete in new version. Adding SKSampling option instead
        SKSamplingOptions sampling = new SKSamplingOptions(SKCubicResampler.Mitchell);

        surface.Canvas.Clear(SKColors.White);
        var rect = new SKRect(0, 0, newWidth, newHeight);
        surface.Canvas.DrawImage(image, rect, sampling, paint);
        surface.Canvas.Flush();
        
        return surface;
    }
}
