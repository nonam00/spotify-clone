namespace Infrastructure.Files;

internal static class FileServiceMessaging
{
    public const string FileExchange = "file-service-exchange";
    public const string DeleteRoutingKey = "file-service.delete-file";
    public const string DeleteFileQueue = "file-service.delete-file";
}

internal record DeleteFileMessage(string FileType, string FileId);