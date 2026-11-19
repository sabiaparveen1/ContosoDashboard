namespace ContosoDashboard.Services.Storage;

public interface IFileStorageService
{
    /// <summary>Uploads a file to storage and returns the storage path</summary>
    Task<string> UploadAsync(Stream fileStream, string fileName, string fileExtension, CancellationToken cancellationToken = default);

    /// <summary>Downloads a file from storage</summary>
    Task<Stream> DownloadAsync(string storagePath, CancellationToken cancellationToken = default);

    /// <summary>Deletes a file from storage</summary>
    Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default);

    /// <summary>Checks if a file exists in storage</summary>
    Task<bool> ExistsAsync(string storagePath, CancellationToken cancellationToken = default);

    /// <summary>Gets the file size in bytes</summary>
    Task<long> GetFileSizeAsync(string storagePath, CancellationToken cancellationToken = default);
}
