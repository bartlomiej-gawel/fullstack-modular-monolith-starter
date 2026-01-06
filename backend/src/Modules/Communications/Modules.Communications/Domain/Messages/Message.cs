using Common.Abstractions.Domain;
using ErrorOr;
using Modules.Communications.Domain.Recipients;

namespace Modules.Communications.Domain.Messages;

public sealed class Message : AggregateRoot<MessageId>
{
    private const int MinSubjectLength = 2;
    private const int MaxSubjectLength = 200;
    private const int MinFailureReasonLength = 2;
    private const int MaxFailureReasonLength = 500;

    private Message()
    {
    }

    private Message(
        MessageId id,
        RecipientId recipientId,
        string subject,
        string content,
        MessageChannel channel) : base(id)
    {
        RecipientId = recipientId;
        Subject = subject;
        Content = content;
        Channel = channel;
        Status = MessageStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        SentAt = null;
        FailedAt = null;
        FailureReason = null;
        IsRead = false;
        ReadAt = null;
        IsDeleted = false;
        DeletedAt = null;
    }

    public RecipientId RecipientId { get; private set; } = null!;
    public string Subject { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public MessageChannel Channel { get; private set; }
    public MessageStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? FailedAt { get; private set; }
    public string? FailureReason { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public static ErrorOr<Message> Create(
        RecipientId recipientId,
        string subject,
        string content,
        MessageChannel channel)
    {
        if (string.IsNullOrWhiteSpace(subject))
            return Errors.InvalidSubject;

        if (subject.Length is < MinSubjectLength or > MaxSubjectLength)
            return Errors.InvalidSubjectLength;

        if (string.IsNullOrWhiteSpace(content))
            return Errors.InvalidContent;

        return new Message(
            MessageId.Create(),
            recipientId,
            subject,
            content,
            channel);
    }

    public ErrorOr<Success> MarkAsSent()
    {
        if (Status is MessageStatus.Sent)
            return Errors.AlreadySent;

        Status = MessageStatus.Sent;
        SentAt = DateTime.UtcNow;

        return Result.Success;
    }

    public ErrorOr<Success> MarkAsFailed(string reason)
    {
        if (Status is MessageStatus.Failed)
            return Errors.AlreadyFailed;

        if (reason.Length is < MinFailureReasonLength or > MaxFailureReasonLength)
            return Errors.InvalidFailureReasonLength;

        Status = MessageStatus.Failed;
        FailedAt = DateTime.UtcNow;
        FailureReason = reason;

        return Result.Success;
    }

    public ErrorOr<Success> MarkAsRead()
    {
        if (Channel is not MessageChannel.InApp)
            return Errors.CannotMarkAsRead;

        if (IsRead)
            return Errors.AlreadyRead;

        IsRead = true;
        ReadAt = DateTime.UtcNow;

        return Result.Success;
    }

    public ErrorOr<Success> MarkAsDeleted()
    {
        if (Channel is not MessageChannel.InApp)
            return Errors.CannotMarkAsDeleted;

        if (IsDeleted)
            return Errors.AlreadyDeleted;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;

        return Result.Success;
    }

    private static class Errors
    {
        public static readonly Error InvalidSubject = Error.Validation(
            "Communications.Message.InvalidSubject",
            "Message subject cannot be null or empty.");

        public static readonly Error InvalidSubjectLength = Error.Validation(
            "Communications.Message.InvalidSubjectLength",
            "Message subject must be between 2 and 200 characters.");

        public static readonly Error InvalidContent = Error.Validation(
            "Communications.Message.InvalidContent",
            "Message content cannot be null or empty.");

        public static readonly Error InvalidFailureReasonLength = Error.Validation(
            "Communications.Message.InvalidFailureReasonLength",
            "Message failure reason must be between 2 and 500 characters.");

        public static readonly Error AlreadySent = Error.Conflict(
            "Communications.Message.AlreadySent",
            "Message has already been sent.");

        public static readonly Error AlreadyFailed = Error.Conflict(
            "Communications.Message.AlreadyFailed",
            "Message has already been marked as failed.");

        public static readonly Error CannotMarkAsRead = Error.Conflict(
            "Communications.Message.CannotMarkAsRead",
            "Only InApp messages can be marked as read.");

        public static readonly Error AlreadyRead = Error.Conflict(
            "Communications.Message.AlreadyRead",
            "Message has already been read.");

        public static readonly Error CannotMarkAsDeleted = Error.Conflict(
            "Communications.Message.CannotMarkAsDeleted",
            "Only InApp messages can be deleted.");

        public static readonly Error AlreadyDeleted = Error.Conflict(
            "Communications.Message.AlreadyDeleted",
            "Message has already been deleted.");
    }
}