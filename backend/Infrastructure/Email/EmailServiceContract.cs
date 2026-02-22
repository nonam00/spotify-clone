namespace Infrastructure.Email;

internal static class EmailServiceContract
{
    public const string EmailExchange = "email-service-exchange";
    public const string SendEmailRoutingKey = "email-service.send-email";
    public const string SendEmailQueue = "email-service.send-email";
}

internal enum EmailTopicType
{
    Confirm = 1,
    Restore = 2
}

internal record SendEmailMessage(string Email, string Code, EmailTopicType Topic);
