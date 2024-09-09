using Microsoft.ApplicationInsights;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Backend.BankingTranxSystem.SharedServices.Helper;

public class ImageResizer(int allowedSize,
                    string sourcePath,
                    string destinationPath,
                    TelemetryClient telemetryClient)
{
    private int allowedFileSizeInByte = allowedSize;
    private string sourcePath = sourcePath;
    private string destinationPath = destinationPath;
    private readonly TelemetryClient _telemetryClient = telemetryClient;

    public bool ScaleImage()
    {
        try
        {
            using MemoryStream ms = new();
            using FileStream fs = new(sourcePath, FileMode.Open);
            Bitmap bmp = (Bitmap)Image.FromStream(fs);
            SaveTemporary(bmp, ms, 100);

            while (ms.Length < 0.9 * allowedFileSizeInByte || ms.Length > allowedFileSizeInByte)
            {
                double scale = Math.Sqrt((double)allowedFileSizeInByte / (double)ms.Length);
                ms.SetLength(0);
                bmp = ScaleImage(bmp, scale);
                SaveTemporary(bmp, ms, 100);
            }

            if (bmp != null)
                bmp.Dispose();
            SaveImageToFile(ms);
            return true;
        }
        catch (Exception ex)
        {
            _telemetryClient.TrackEvent("Scale Image threw an exception");
            _telemetryClient.TrackException(ex);
            return false;
        }
    }

    private void SaveImageToFile(MemoryStream ms)
    {
        byte[] data = ms.ToArray();

        using FileStream fs = new FileStream(destinationPath, FileMode.Create);
        fs.Write(data, 0, data.Length);
    }

    private void SaveTemporary(Bitmap bmp, MemoryStream ms, int quality)
    {
        EncoderParameter qualityParam = new(Encoder.Quality, quality);
        var codec = GetImageCodecInfo();
        var encoderParams = new EncoderParameters(1);
        encoderParams.Param[0] = qualityParam;

        if (codec != null)
            bmp.Save(ms, codec, encoderParams);
        else
            bmp.Save(ms, GetImageFormat());
    }

    public Bitmap ScaleImage(Bitmap image, double scale)
    {
        int newWidth = (int)(image.Width * scale);
        int newHeight = (int)(image.Height * scale);

        Bitmap result = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);
        result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        using (Graphics g = Graphics.FromImage(result))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g.DrawImage(image, 0, 0, result.Width, result.Height);
        }
        return result;
    }

    private ImageCodecInfo GetImageCodecInfo()
    {
        FileInfo fi = new FileInfo(sourcePath);

        switch (fi.Extension)
        {
            case ".bmp": return ImageCodecInfo.GetImageEncoders()[0];
            case ".jpg":
            case ".jpeg": return ImageCodecInfo.GetImageEncoders()[1];
            case ".gif": return ImageCodecInfo.GetImageEncoders()[2];
            case ".tiff": return ImageCodecInfo.GetImageEncoders()[3];
            case ".png": return ImageCodecInfo.GetImageEncoders()[4];
            default: return null;
        }
    }

    private ImageFormat GetImageFormat()
    {
        FileInfo fi = new FileInfo(sourcePath);

        switch (fi.Extension)
        {
            case ".jpg": return ImageFormat.Jpeg;
            case ".bmp": return ImageFormat.Bmp;
            case ".gif": return ImageFormat.Gif;
            case ".png": return ImageFormat.Png;
            case ".tiff": return ImageFormat.Tiff;
            default: return ImageFormat.Png;
        }
    }
}