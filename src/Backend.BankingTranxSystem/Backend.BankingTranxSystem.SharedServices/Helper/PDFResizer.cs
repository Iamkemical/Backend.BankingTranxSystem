using BitMiracle.Docotic.Pdf;

namespace Backend.BankingTranxSystem.SharedServices.Helper;

/// <summary>
/// Resize PDF document based on given parameters.
/// </summary>
/// <param name="allowedSize"></param>
/// <param name="sourcePath"></param>
/// <param name="destinationPath"></param>
public class PDFResizer(int allowedSize, string sourcePath, string destinationPath)
{
    private int _allowedFileSizeInByte = allowedSize;
    private readonly string _sourcePath = sourcePath;
    private readonly string _destinationPath = destinationPath;

    public void ScalePdf()
    {
        using var pdf = new PdfDocument(_sourcePath);
        pdf.ReplaceDuplicateObjects();
        pdf.RemoveUnusedFontGlyphs();
        pdf.RemoveStructureInformation();
        pdf.RemoveUnusedResources();
        pdf.RemovePieceInfo();
        pdf.FlattenControls();


        XmpMetadata xmp = pdf.Metadata;
        xmp.Unembed();
        pdf.Info.Clear(false);

        var saveOptions = new PdfSaveOptions();
        pdf.Save(_destinationPath, saveOptions);
    }
}