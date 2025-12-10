using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SocialMediaBlog.CommonInterfaces;
using SocialMediaBlog.DatabaseContext;
using SocialMediaBlog.Messages;

namespace SocialMediaBlog.Account;

// NOTE: PT01B - D.I. - Service Interface Implementation
public class AccountService : ICommonContract, IAccountService {
    
    
    public Dictionary<string, object> CreateNewEntity<TNe>(
        TNe newAccountEntityAsGeneric, SocialMediaBlogDbCtx dbCtx
    ) {
        // DESC: Cast method entity-parameter to intended Entity
        // NOTE: This is only necessary because I am trying to use a 
        // ... common interface across multiple entity-types (more for 
        // ... educational purposes)
        AccountEntity? newAccountEntity = newAccountEntityAsGeneric as AccountEntity;
        
        // DESC: Check if Account Exists (based on Username)
        // NOTE: The parameter is a LINQ Express Tree
        bool accountExists = dbCtx.Accounts.Any((acctEntityInDb) => 
            acctEntityInDb.Username == newAccountEntity!.Username
        );

        // DESC: Initialize Dictionary for return
        Dictionary<string, object> returnedDictionary = new();
        
        // DESC: Check if the Account exists (by Username)
        if (accountExists == true) {
            // NOTE: Account exists, so this returns the 409 code
            returnedDictionary.Add("statusCode", 409);
        } else {
            // NOTE: Account does not exist, so this handles
            // ... persisting to the DB then returning the necessary
            // ... DTO
            // NOTE: `Add()` does NOT persist the Entity, it "adds" the
            // ... Entity to the DbContext, which allows for tracking of
            // ... the state of the entity
            EntityEntry ee = dbCtx.Accounts.Add(newAccountEntity!);
            dbCtx.SaveChanges();

            // DESC: Assign Entity-values to DTO
            // NOTE: ASP.NET Core E.F.C. will auto-assign the Account
            // ... Entity a unique ID, from the database, when SaveChanges()
            // ... is called. This is, in part, handled by the DbContext
            AccountDto newAccountDto = new AccountDto(
                newAccountEntity.AccountId,
                newAccountEntity.Username,
                newAccountEntity.Password
            );
            
            // DESC: Add values to the Dictionary
            // NOTE: This is for Controller-extraction
            returnedDictionary.Add("statusCode", 200);
            returnedDictionary.Add("DTO", newAccountDto);
        }
        
        return returnedDictionary;
    }

    public Dictionary<string, object> CheckForExistingUser(
        AccountEntity existingAccount, SocialMediaBlogDbCtx dbCtx
    ) {
        // DESC: Get the AccountEntity where BOTH the Username
        // ... AND Password match what was provided
        // NOTE: SingleOrDefault() returns the 'single' match (or
        // ... null), but throws an exception if multiple matches exist
        AccountEntity? existingAccountEntity = dbCtx.Accounts.SingleOrDefault(
            acctEntityInDb => acctEntityInDb.Username == existingAccount.Username &&
                              acctEntityInDb.Password == existingAccount.Password
        );
        
        // DESC: Initialize Dictionary for return
        Dictionary<string, object> returnedDictionary = new();        

        // DESC: Map Entity to DTO and update returned Dictionary
        // ... IF Username/Password combo. exists in the Database
        if (existingAccountEntity != null) {
            AccountDto existingAccountDto = new AccountDto(
                existingAccountEntity.AccountId,
                existingAccountEntity.Username,
                existingAccountEntity.Password
            );
            
            returnedDictionary.Add("isValidLoginAttempt", true);
            returnedDictionary.Add("DTO", existingAccountDto);
        }
        
        return returnedDictionary;
    }
    
    public List<MessagesDto> RetrieveMessagesByAccountId(int accountId, SocialMediaBlogDbCtx dbCtx) {
        // DESC: Get all messages, by Account, from Database
        List<MessagesEntity> listOfMessageEntities = dbCtx.Messages
            .Where(msgEntityInDb => msgEntityInDb.PostedBy == accountId)
            .ToList();
        
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
}