using System.Collections.Generic;
using SocialMediaBlog.DatabaseContext;
using SocialMediaBlog.Messages;

namespace SocialMediaBlog.Account;

// NOTE: PT01A - D.I. - Service Interface
// https://medium.com/@ravipatel.it/dependency-injection-and-services-in-asp-net-core-a-comprehensive-guide-dd69858c1eab
public interface IAccountService {
    Dictionary<string, object> CheckForExistingUser(
        AccountEntity existingAccount, SocialMediaBlogDbCtx dbCtx
    );

    List<MessagesDto> RetrieveMessagesByAccountId(int accountId, SocialMediaBlogDbCtx dbCtx);
}

