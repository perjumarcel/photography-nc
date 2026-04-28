# Email delivery

Transactional emails (currently: contact-form notifications) are sent through the `IEmailSender` abstraction defined in `Photography.Application/Common/Email/`. Two implementations live in `Photography.Infrastructure/Email/`:

| Provider | When used | Behaviour |
|----------|-----------|-----------|
| `NoOp`   | Default; missing or unrecognised `Email:Provider` | Structured-logs the outgoing message at `Information`. Lets the contact endpoint work in dev/CI without any SMTP credentials. |
| `Smtp`   | `Email:Provider = "Smtp"` | Uses [MailKit](https://github.com/jstedfast/MailKit) (`SmtpClient` + `MimeMessage`). One client per send (registered as `Transient`) because `SmtpClient` is not thread-safe. |

The provider is selected in `Photography.Infrastructure/DependencyInjection.cs` using the same pattern as the storage provider switch.

## Configuration

```jsonc
// appsettings.json (defaults are no-op safe)
"Email": {
  "Provider": "NoOp",            // or "Smtp"
  "FromAddress": "",             // required for Smtp
  "FromDisplayName": "",
  "AdminRecipient": "",          // generic admin inbox; not used directly today
  "Smtp": {
    "Host": "",                  // required for Smtp
    "Port": 587,                 // 587 STARTTLS, 465 implicit TLS
    "Username": "",
    "Password": "",
    "UseSsl": false,             // true → SslOnConnect (port 465)
    "TimeoutSeconds": 30
  }
},
"Contact": {
  "NotificationRecipient": ""    // empty → handler logs only, no email is sent
}
```

Secrets (`Smtp:Username`, `Smtp:Password`) should be supplied through environment variables or a secret store rather than committed to `appsettings.json`. The standard ASP.NET Core configuration providers apply — e.g.

```
Email__Smtp__Username=apikey
Email__Smtp__Password=...
```

## Behaviour in `SendContactMessageHandler`

1. Validate name / email / message length.
2. Structured-log the submission (visible regardless of provider).
3. If `Contact:NotificationRecipient` is set: build a plain-text email (`ReplyTo` = submitter), call `IEmailSender.SendAsync`.
4. Email send failures are caught and logged at `Error` — the public endpoint always returns `200` so anonymous visitors never see infrastructure errors.

## Local development

The default `NoOp` provider means you can run `docker compose up` with no extra config and the contact form will work end-to-end (the message lands in the API logs). Wire real SMTP only when you want notifications to leave the box.

## Verification

```bash
# Trigger the endpoint locally:
curl -X POST http://localhost:5080/api/public/contact \
  -H "Content-Type: application/json" \
  -d '{"name":"Tester","email":"tester@example.com","message":"Hello"}'

# With Provider=NoOp you should see:
#   [Email NO-OP] To=... Subject=Contact form: Tester ReplyTo=tester@example.com BodyChars=...
```
