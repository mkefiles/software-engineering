using System.Collections.Generic;
using SocialMediaBlog.DatabaseContext;

namespace SocialMediaBlog.Messages;

public interface IMessagesService {
    List<MessagesDto> ReadAllMessages(SocialMediaBlogDbCtx dbCtx);
    
    MessagesDto ReadMessageById(int id, SocialMediaBlogDbCtx dbCtx);
    
    int DeleteMessageById(int id, SocialMediaBlogDbCtx dbCtx);
    
    int UpdateMessageById(
        MessagesEntity messageToUpdate, SocialMediaBlogDbCtx dbCtx
    );
}