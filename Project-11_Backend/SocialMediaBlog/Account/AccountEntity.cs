using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SocialMediaBlog.Messages;

namespace SocialMediaBlog.Account;

// DESC: Provide Custom Table Name
[Table("account")]
[Index(nameof(Username), IsUnique = true)]
public class AccountEntity {
    /* SECTION: Instance-Variables */
    
    private int _accountId;
    private string _username;
    private string _password;

    // NOTE: PT01 / 03 - This is added to enforce a one-to-many
    // ... Foreign Key relationship between `_accountId` and `postedBy`
    /* learn.microsoft.com/en-us/ef/core/modeling/relationships/foreign-and-principal-keys */
    public List<MessagesEntity> MessageEntities { get; } = new();

    public AccountEntity() { }

    public AccountEntity(string username, string password) {
        _username = username;
        _password = password;
    }

    public AccountEntity(int accountId, string username, string password) {
        _accountId = accountId;
        _username = username;
        _password = password;
    }

    /* SECTION: Getter/Setter Methods */

    // NOTE: `[Key]` attribute - marks property as Primary Key
    // NOTE: `[DatabaseGen.] attr. - tells DB to auto-generate the
    // ... key -- `Identity` specifies for it to generate it when
    // ... the row is inserted
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AccountId {
        get => _accountId;
        set => _accountId = value;
    }
    
    public string Username {
        get => _username;
        set => _username = value;
    }
    
    public string Password {
        get => _password;
        set => _password = value;
    }
}