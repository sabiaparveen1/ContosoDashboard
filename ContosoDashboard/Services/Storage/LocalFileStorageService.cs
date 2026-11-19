namespace ContosoDashboard.Services.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly ILogger<LocalFileStorageService> _logger;
    private readonly string _storagePath;

    public LocalFileStorageService(ILogger<LocalFileStorageService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _storagePath = configuration["FileStorage:LocalPath"] ?? Path.Combine(AppContext.BaseDirectory, "uploads");
        
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
            _logger.LogInformation("Created storage directory: {StoragePath}", _storagePath);
        }
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string fileExtension, CancellationToken cancellationToken = default)
    {
        if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("File name cannot be empty", nameof(fileName));

        var uniqueFileName = GenerateUniqueFileName(fileName, fileExtension);
        var filePath = Path.Combine(_storagePath, uniqueFileName);

        try
        {
            using (var fileStreamWriter = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await fileStream.CopyToAsync(fileStreamWriter, 81920, cancellationToken);
            }

            _logger.LogInformation("File uploaded successfully: {FileName} -> {UniqueFileName}", fileName, uniqueFileName);
            return uniqueFileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            throw;
        }
    }

    public async Task<Stream> DownloadAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storagePath)) throw new ArgumentException("Storage path cannot be empty", nameof(storagePath));

        var filePath = Path.Combine(_storagePath, storagePath);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("File not found: {StoragePath}", storagePath);
            throw new FileNotFoundException($"File not found: {storagePath}");
        }

        try
        {
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _logger.LogInformation("File downloaded: {StoragePath}", storagePath);
            return fileStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {StoragePath}", storagePath);
            throw;
        }
    }

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storagePath)) throw new ArgumentException("Storage path cannot be empty", nameof(storagePath));

        var filePath = Path.Combine(_storagePath, storagePath);

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File deleted: {StoragePath}", storagePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {StoragePath}", storagePath);
            throw;
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storagePath)) throw new ArgumentException("Storage path cannot be empty", nameof(storagePath));

        var filePath = Path.Combine(_storagePath, storagePath);
        return Task.FromResult(File.Exists(filePath));
    }

    public Task<long> GetFileSizeAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storagePath)) throw new ArgumentException("Storage path cannot be empty", nameof(storagePath));

        var filePath = Path.Combine(_storagePath, storagePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {storagePath}");
        }

        var fileInfo = new FileInfo(filePath);
        return Task.FromResult(fileInfo.Length);
    }

    private string GenerateUniqueFileName(string originalFileName, string fileExtension)
    {
        var timestamp = DateTime.UtcNow.Ticks;
        var randomPart = Guid.NewGuid().ToString("N").Substring(0, 8);
        var extension = string.IsNullOrWhiteSpace(fileExtension) ? Path.GetExtension(originalFileName) : fileExtension;
        
        return $"{timestamp}_{randomPart}{extension}";
    }
}
