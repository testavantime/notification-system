# Notification Service

A .NET Web API service for sending SMS and Email notifications based on templates with configurable settings.

## Features

- **Email Notifications**: Send HTML emails using SMTP (MailKit)
- **SMS Notifications**: Send SMS messages using Twilio
- **Template System**: Support for customizable email and SMS templates
- **Configuration**: Enable/disable SMS and Email notifications independently
- **Parameter Substitution**: Dynamic content replacement in templates
- **Multiple Recipients**: Send to multiple email addresses or phone numbers
- **Error Handling**: Comprehensive error handling and logging

## Prerequisites

- .NET 9.0 SDK
- SMTP server credentials (for email)
- Twilio account credentials (for SMS)

## Configuration

Update the `appsettings.json` file with your configuration:

```json
{
  "NotificationConfig": {
    "SmsEnabled": true,
    "EmailEnabled": true,
    "Email": {
      "SmtpServer": "smtp.gmail.com",
      "SmtpPort": 587,
      "Username": "your-email@gmail.com",
      "Password": "your-app-password",
      "UseSsl": true,
      "FromEmail": "your-email@gmail.com",
      "FromName": "Your Service Name"
    },
    "Sms": {
      "AccountSid": "your-twilio-account-sid",
      "AuthToken": "your-twilio-auth-token",
      "FromNumber": "+1234567890"
    }
  }
}
```

### Email Configuration

- **SmtpServer**: Your SMTP server (e.g., smtp.gmail.com)
- **SmtpPort**: SMTP port (usually 587 for TLS)
- **Username**: Your email username
- **Password**: Your email password or app password
- **UseSsl**: Enable SSL/TLS
- **FromEmail**: Sender email address
- **FromName**: Sender display name

### SMS Configuration

- **AccountSid**: Your Twilio Account SID
- **AuthToken**: Your Twilio Auth Token
- **FromNumber**: Your Twilio phone number

## Available Templates

### Email Templates

1. **welcome** - Welcome email for new users
   - Parameters: `Name`, `Email`

2. **password-reset** - Password reset email
   - Parameters: `Name`, `ResetLink`

3. **order-confirmation** - Order confirmation email
   - Parameters: `Name`, `OrderId`, `Amount`, `DeliveryDate`

### SMS Templates

1. **welcome** - Welcome SMS
   - Parameters: `Name`

2. **password-reset** - Password reset SMS
   - Parameters: `ResetCode`

3. **order-confirmation** - Order confirmation SMS
   - Parameters: `OrderId`, `Amount`, `DeliveryDate`

4. **verification** - Verification code SMS
   - Parameters: `VerificationCode`

## API Endpoints

### Send Combined Notification

```http
POST /api/notification/send
Content-Type: application/json

{
  "templateName": "welcome",
  "parameters": {
    "Name": "John Doe",
    "Email": "john@example.com"
  },
  "toEmails": ["john@example.com"],
  "toPhoneNumbers": ["+1234567890"],
  "type": "Both"
}
```

### Send Email Only

```http
POST /api/notification/email
Content-Type: application/json

{
  "templateName": "welcome",
  "parameters": {
    "Name": "John Doe",
    "Email": "john@example.com"
  },
  "toEmails": ["john@example.com"]
}
```

### Send SMS Only

```http
POST /api/notification/sms
Content-Type: application/json

{
  "templateName": "verification",
  "parameters": {
    "VerificationCode": "123456"
  },
  "toPhoneNumbers": ["+1234567890"]
}
```

### Health Check

```http
GET /api/notification/health
```

## Running the Service

1. **Build the project**:
   ```bash
   dotnet build
   ```

2. **Run the service**:
   ```bash
   dotnet run
   ```

3. **Access the API**:
   - Swagger UI: `https://localhost:7001/swagger`
   - Health Check: `https://localhost:7001/api/notification/health`

## Example Usage

### C# Client Example

```csharp
using var client = new HttpClient();
client.BaseAddress = new Uri("https://localhost:7001/");

var request = new
{
    templateName = "welcome",
    parameters = new Dictionary<string, string>
    {
        ["Name"] = "John Doe",
        ["Email"] = "john@example.com"
    },
    toEmails = new List<string> { "john@example.com" },
    toPhoneNumbers = new List<string> { "+1234567890" },
    type = "Both"
};

var response = await client.PostAsJsonAsync("api/notification/send", request);
var result = await response.Content.ReadFromJsonAsync<NotificationResponse>();
```

### cURL Example

```bash
curl -X POST "https://localhost:7001/api/notification/send" \
  -H "Content-Type: application/json" \
  -d '{
    "templateName": "welcome",
    "parameters": {
      "Name": "John Doe",
      "Email": "john@example.com"
    },
    "toEmails": ["john@example.com"],
    "toPhoneNumbers": ["+1234567890"],
    "type": "Both"
  }'
```

## Adding New Templates

To add new templates, modify the `TemplateService.cs` file:

```csharp
_emailTemplates["new-template"] = @"
    <html>
    <body>
        <h2>New Template</h2>
        <p>Hello {{Name}},</p>
        <p>{{CustomMessage}}</p>
    </body>
    </html>";

_smsTemplates["new-template"] = "Hello {{Name}}! {{CustomMessage}}";
```

## Error Handling

The service provides detailed error responses:

```json
{
  "success": false,
  "message": "Failed to send notification",
  "errors": [
    "Email configuration is incomplete",
    "SMS configuration is incomplete"
  ],
  "emailMessageId": null,
  "smsMessageId": null
}
```

## Logging

The service uses structured logging with the following log levels:
- **Information**: Successful operations
- **Warning**: Partial failures
- **Error**: Complete failures

## Security Considerations

1. Store sensitive configuration in environment variables or secure configuration providers
2. Use app passwords for email services
3. Keep Twilio credentials secure
4. Validate input parameters
5. Implement rate limiting for production use

## Troubleshooting

### Email Issues
- Verify SMTP server settings
- Check firewall settings
- Ensure app passwords are used for Gmail
- Verify SSL/TLS settings

### SMS Issues
- Verify Twilio credentials
- Check phone number format (E.164)
- Ensure sufficient Twilio credits
- Verify from number is verified in Twilio

## License

This project is licensed under the MIT License. 