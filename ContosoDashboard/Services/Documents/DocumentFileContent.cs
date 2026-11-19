using System.IO.Compression;
using System.Text;

namespace ContosoDashboard.Services.Documents;

internal static class DocumentFileContent
{
    public static async Task<bool> MatchesExtensionAsync(MemoryStream file, string extension, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var header = new byte[8];
        var length = file.Read(header, 0, header.Length);
        file.Position = 0;

        return extension switch
        {
            ".pdf" => StartsWith(header, length, "%PDF-"u8),
            ".png" => StartsWith(header, length, [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]),
            ".jpg" or ".jpeg" => StartsWith(header, length, [0xFF, 0xD8, 0xFF]),
            ".gif" => StartsWith(header, length, "GIF87a"u8) || StartsWith(header, length, "GIF89a"u8),
            ".bmp" => StartsWith(header, length, "BM"u8),
            // Legacy Office formats share the Compound File Binary signature.
            ".doc" or ".xls" or ".ppt" => StartsWith(header, length, [0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1]),
            ".docx" => StartsWith(header, length, [0x50, 0x4B, 0x03, 0x04]) && HasOfficePart(file, "word/document.xml"),
            ".xlsx" => StartsWith(header, length, [0x50, 0x4B, 0x03, 0x04]) && HasOfficePart(file, "xl/workbook.xml"),
            ".pptx" => StartsWith(header, length, [0x50, 0x4B, 0x03, 0x04]) && HasOfficePart(file, "ppt/presentation.xml"),
            ".txt" => await IsPlainTextAsync(file, cancellationToken),
            _ => false
        };
    }

    private static bool StartsWith(byte[] header, int length, ReadOnlySpan<byte> signature) =>
        header.AsSpan(0, length).StartsWith(signature);

    private static bool HasOfficePart(Stream file, string partName)
    {
        try
        {
            using var archive = new ZipArchive(file, ZipArchiveMode.Read, leaveOpen: true);
            return archive.GetEntry("[Content_Types].xml") != null && archive.GetEntry(partName) != null;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    // Plain text has no magic bytes; accept UTF-8 text without binary control characters.
    private static async Task<bool> IsPlainTextAsync(Stream file, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(file, new UTF8Encoding(false, throwOnInvalidBytes: true),
            detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var chars = new char[4096];
        try
        {
            int count;
            while ((count = await reader.ReadAsync(chars.AsMemory(), cancellationToken)) > 0)
                for (var i = 0; i < count; i++)
                    if (char.IsControl(chars[i]) && chars[i] is not ('\t' or '\r' or '\n' or '\f'))
                        return false;
            return true;
        }
        catch (DecoderFallbackException)
        {
            return false;
        }
    }
}
