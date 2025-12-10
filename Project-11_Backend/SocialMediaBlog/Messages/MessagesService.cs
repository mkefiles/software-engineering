using System.Collections.Generic;
using System.Linq;
using SocialMediaBlog.CommonInterfaces;
using SocialMediaBlog.DatabaseContext;

namespace SocialMediaBlog.Messages;

public class MessagesService : ICommonContract, IMessagesService {
    
    public Dictionary<string, object> CreateNewEntity<TNe>(
        TNe newMessageEntityAsGeneric, SocialMediaBlogDbCtx dbCtx
    ) {
        // DESC: Cast method entity-parameter to intended Entity
        MessagesEntity? newMessageEntity = newMessageEntityAsGeneric as MessagesEntity;
        
        // DESC: Check if Database contains PostedBy ID
        bool postedByIdExists = dbCtx.Accounts.Any((acctEntityInDb) =>
            acctEntityInDb.AccountId == newMessageEntity!.PostedBy
        );
        
        // DESC: Initialize Dictionary for return
        Dictionary<string, object> returnedDictionary = new();
        
        // DESC: Handle if PostedBy ID exists/does not exist
        if (postedByIdExists == true) {
            // DESC: Add Message to Database
            dbCtx.Messages.Add(newMessageEntity!);
            dbCtx.SaveChanges();
            
            // DESC: Map Entity to DTO (for return)
            MessagesDto newMessageDto = new MessagesDto(
                newMessageEntity.MessageId,
                newMessageEntity.PostedBy,
                newMessageEntity.MessageText,
                newMessageEntity.TimePostedEpoch
            );
        
            // DESC: Update Dictionary to contain DTO
            returnedDictionary.Add("DTO", newMessageDto);
        }
        
        return returnedDictionary;
    }

    public List<MessagesDto> ReadAllMessages(SocialMediaBlogDbCtx dbCtx) {
        // DESC: Get all messages from Database
        List<MessagesEntity> listOfMessageEntities = dbCtx.Messages.ToList();
        
        // DESC: Map each Entity to DTO
        List<MessagesDto> listOfMessageDtos = new List<MessagesDto>();
        foreach (MessagesEntity messageEntity in listOfMessageEntities) {
            listOfMessageDtos.Add(new MessagesDto(
                messageEntity.MessageId, messageEntity.PostedBy,
                messageEntity.MessageText, messageEntity.TimePostedEpoch
            ));
        }
        
        return listOfMessageDtos;
    }

    public MessagesDto ReadMessageById(int id, SocialMediaBlogDbCtx dbCtx) {
        // DESC: Get specific message from Database
        MessagesEntity? messageEntity = dbCtx.Messages.Find(id);
        
        // DESC: Return empty DTO if null, otherwise returned a DTO
        // ... mapped from the Entity
        if (messageEntity == null) {
            return null!;
        }
        
        return new MessagesDto(
            messageEntity.MessageId, messageEntity.PostedBy,
            messageEntity.MessageText, messageEntity.TimePostedEpoch
        );
    }

    public int DeleteMessageById(int id, SocialMediaBlogDbCtx dbCtx) {
        // DESC: Locate MessagesEntity by ID
        MessagesEntity? messageEntity = dbCtx.Messages.Find(id);
        
        // DESC: Return `0` if MessageEntity does not exist
        if (messageEntity == null) {
            return 0;
        }
        
        // DESC: Otherwise, remove it from DB and return `1`
        dbCtx.Messages.Remove(messageEntity);
        dbCtx.SaveChanges();
        
        return 1;
    }

    public int UpdateMessageById(
        MessagesEntity messageToUpdate, SocialMediaBlogDbCtx dbCtx
    ) {
        // DESC: Locate MessagesEntity by ID
        MessagesEntity? messageEntity = dbCtx.Messages.Find(messageToUpdate.MessageId);
        
        // DESC: Return `0` if MessageEntity does not exist
        if (messageEntity == null) {
            return 0;
        }
        
        // DESC: Otherwise, update it in DB and return `1`
        messageEntity.MessageText = messageToUpdate.MessageText;
        dbCtx.Messages.Update(messageEntity);
        dbCtx.SaveChanges();

        return 1;
    }
}