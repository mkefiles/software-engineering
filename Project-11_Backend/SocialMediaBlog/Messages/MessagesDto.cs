using System;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaBlog.Messages;

public class MessagesDto {
    private int _messageId;
    private int _postedBy;
    private string _messageText;
    private DateTime _timePostedEpoch;

    public MessagesDto() { }

    public MessagesDto(int messageId, int postedBy, string messageText, DateTime timePostedEpoch) {
        _messageId = messageId;
        _postedBy = postedBy;
        _messageText = messageText;
        _timePostedEpoch = timePostedEpoch;
    }

    public int MessageId {
        get => _messageId;
        set => _messageId = value;
    }

    [Required(ErrorMessage = "ID associated with Username is required")]
    public int PostedBy {
        get => _postedBy;
        set => _postedBy = value;
    }

    [StringLength(255, MinimumLength = 3, ErrorMessage = "Message must be between 3 and 255 characters long")]
    public string MessageText {
        get => _messageText;
        set => _messageText = value ?? throw new ArgumentNullException(nameof(value));
    }

    public DateTime TimePostedEpoch {
        get => _timePostedEpoch;
        set => _timePostedEpoch = value;
    }
}