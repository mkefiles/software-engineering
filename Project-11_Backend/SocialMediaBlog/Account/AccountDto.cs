using System.ComponentModel.DataAnnotations;

namespace SocialMediaBlog.Account;

public class AccountDto {
    private int _accountId;
    private string _username;
    private string _password;

    public AccountDto() { }
    
    public AccountDto(string username, string password) {
        _username = username;
        _password = password;
    }

    public AccountDto(int accountId, string username, string password) {
        _accountId = accountId;
        _username = username;
        _password = password;
    }

    public int AccountId {
        get => _accountId;
        set => _accountId = value;
    }

    [Required(ErrorMessage =  "Username is required")]
    public string Username {
        get => _username;
        set => _username = value;
    }

    [MinLength(4,  ErrorMessage = "Password must be at least 4 characters long")]
    public string Password {
        get => _password;
        set => _password = value;
    }
}