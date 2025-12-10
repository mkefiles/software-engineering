using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SocialMediaBlog.Account;

namespace SocialMediaBlog.Messages;

// DESC: Provide Custom Table Name
[Table("message")]
public class MessagesEntity {
    /* SECTION: Instance-Variables */
    
    private int _messageId;
    private int _postedBy;
    private string _messageText;
    private DateTime _timePostedEpoch;
    
    // NOTE: PT02 / 03 - This is added to enforce a one-to-many
    // ... Foreign Key relationship between `accountId` and `_postedBy`
    /* learn.microsoft.com/en-us/ef/core/modeling/relationships/foreign-and-principal-keys */
    // NOTE: `init;` basically ensures that once the Object is set it
    // ... becomes immutable (i.e., no future setting)
    public AccountEntity Account { get; set; } = null!;
    
    /* SECTION: No-Args Constructor */

    public MessagesEntity() { }

    public MessagesEntity(int postedBy, string messageText, DateTime timePostedEpoch) {
        _postedBy = postedBy;
        _messageText = messageText;
        _timePostedEpoch = timePostedEpoch;
    }

    public MessagesEntity(int messageId, int postedBy, string messageText, DateTime timePostedEpoch) {
        _messageId = messageId;
        _postedBy = postedBy;
        _messageText = messageText;
        _timePostedEpoch = timePostedEpoch;
    }

    /* SECTION: Getter/Setter Methods */

    // NOTE: `[Key]` attribute - marks property as Primary Key
    // NOTE: `[DatabaseGen.] attr. - tells DB to auto-generate the
    // ... key -- `Identity` specifies for it to generate it when
    // ... the row is inserted
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MessageId {
        get => _messageId;
        set => _messageId = value;
    }
    
    public int PostedBy {
        get => _postedBy;
        set => _postedBy = value;
    }
    
    public string MessageText {
        get => _messageText;
        set => _messageText = value;
    }

    public DateTime TimePostedEpoch {
        get => _timePostedEpoch;
        set => _timePostedEpoch = value;
    }
}