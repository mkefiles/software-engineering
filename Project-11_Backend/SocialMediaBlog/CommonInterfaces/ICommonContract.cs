using System.Collections.Generic;
using SocialMediaBlog.DatabaseContext;

namespace SocialMediaBlog.CommonInterfaces;

public interface ICommonContract {
    /**
     * CreateNewEntity returns a Dictionary where the Key is
     * a String (e.g., "DTO" / "statusCode") and the
     * Value is the appropriate Object that matches the String.
     * This provides the ability to extract both the DTO and the
     * Status Code from one data-structure
     * <br /><br />
     * The parameter is a generic, which allows for passing of
     * either an AccountEntity or a MessagesEntity.
     */
    Dictionary<string, object> CreateNewEntity<TNe>(TNe newEntity, SocialMediaBlogDbCtx dbCtx);
}