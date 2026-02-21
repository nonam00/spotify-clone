namespace Infrastructure.Email;

public static class EmailServiceContract
{
    public const string EmailExchange = "email-service-exchange";
    
    public const string SendConfirmEmailRoutingKey = "email-service.send-confirm-email";
    public const string SendConfirmEmailQueue = "email-service.send-confirm-email";
    
    public const string SendRestoreEmailRoutingKey = "email-service.send-restore-email";
    public const string SendRestoreEmailQueue = "email-service.send-restore-email";
}

internal record SendEmailMessage(string Email, string Code);
