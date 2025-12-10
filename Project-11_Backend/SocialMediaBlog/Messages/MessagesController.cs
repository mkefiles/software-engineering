using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SocialMediaBlog.CommonInterfaces;
using SocialMediaBlog.DatabaseContext;

namespace SocialMediaBlog.Messages;

// DESC: Enable API-specific behavior
// NOTE: Attribute provides for behavior such as Parameter Source Inference,
// ... Attribute Routing (as a requirement) and Model Validation (error handling)
[ApiController]

// DESC: Set the base-point, for all Messages end-points, to '/messages'
// NOTE: ASP.NET takes "MessagesController" and, given the attribute of "[controller]",
// ... replaces "Controller", at run-time, with "Messages" (i.e., all end-points
// ... will start with "Messages"
[Route("[controller]")]
public class MessagesController : ControllerBase {
    
    private readonly SocialMediaBlogDbCtx _dbContext;
    private readonly IMessagesService _messagesService;

    public MessagesController(
        SocialMediaBlogDbCtx dbContext, IMessagesService messagesService
    ) {
        _dbContext = dbContext;
        _messagesService = messagesService;
    }

    /**
     * End-Point: POST - localhost:5099/messages <br /><br />
     * Requirements:<br/>
     * - Message Text CANNOT be blank <br />
     * - Message Text CANNOT exceed 255 characters <br />
     * - Posted By MUST match an existing Account/User <br />
     * NOTE: ASP.NET Core handles validation BEFORE running the method. Any
     * failed validation messages, specified in the DTO, will be included
     * in a 400 Response. <br /><br />
     * Success: 200 (Ok) with body containing Message as JSON (incl. ID) <br />
     * Fail: 400 (Client Error) for Misc. Reasons
     */
    [HttpPost]
    public IActionResult CreateNewMessage(
        [FromKeyedServices("Msgs. DI w/ Interface")] ICommonContract commonContract,
        MessagesDto newMessageDtoPayload
    ) {
        try {
            // DESC: Convert to Entity
            // NOTE: This allows communicating with the DbContext
            MessagesEntity newMessagesEntity = new MessagesEntity(
                newMessageDtoPayload.PostedBy,
                newMessageDtoPayload.MessageText,
                DateTime.Now
            );

            Dictionary<string, object> serviceLayerResponse = commonContract
                .CreateNewEntity(newMessagesEntity, _dbContext);
            
            // DESC: Check if "DTO" is present in Dictionary
            bool isDtoPresent = serviceLayerResponse.ContainsKey("DTO");
            
            // DESC: Handle if "DTO" is/not present
            switch (isDtoPresent) {
                case true:
                    return Ok(serviceLayerResponse["DTO"]);
                default:
                    return BadRequest();
            }
        } catch (Exception e) {
            Console.WriteLine("Unexpected Error: " +  e.Message);
            return StatusCode(400, "An unexpected error occured");
        }
    }

    /**
     * End-Point: GET - localhost:5099/messages <br /><br />
     * Success: 200 (Ok) with body containing Message as JSON (incl. ID) <br />
     * Fail: 200 (Ok) with body containing an empty list
     */
    [HttpGet]
    public IActionResult ReadAllMessages() {
        try {
            List<MessagesDto> allMessages = _messagesService.ReadAllMessages(_dbContext);
            return Ok(allMessages);
        } catch (Exception e) {
            Console.WriteLine("Unexpected Error: " +  e.Message);
            return StatusCode(400, "An unexpected error occured");
        }
    }

    /**
     * End-Point: GET - localhost:5099/messages/{MessageId} <br /><br />
     * Success: 200 (Ok) with body containing Message as JSON (incl. ID) <br />
     * Fail: 200 (Ok)
     */
    [HttpGet("{messageId}")]
    public IActionResult ReadMessageById(int messageId) {
        try {
            MessagesDto messageWithId = _messagesService.ReadMessageById(messageId, _dbContext);

            if (messageWithId == null!) {
                return Ok();
            }
            
            return Ok(messageWithId);
        } catch (Exception e) {
            Console.WriteLine("Unexpected Error: " +  e.Message);
            return StatusCode(400, "An unexpected error occured");
        }
    }

    /**
     * End-Point: DELETE - localhost:5099/messages/{MessageId} <br /><br />
     * Success: 200 (Ok) with body containing number of rows updated. Note
     * that this would always return `1` when the message exists because
     * there should only ever be one with the ID. <br />
     * Fail: 200 (Ok)
     */
    [HttpDelete("{messageId}")]
    public IActionResult DeleteMessageById(int messageId) {
        try {
            int numberOfDeletedMessages = _messagesService.DeleteMessageById(messageId, _dbContext);

            if (numberOfDeletedMessages == 0) {
                return Ok();
            }
            
            return Ok(1);

        } catch (Exception e) {
            Console.WriteLine("Unexpected Error: " +  e.Message);
            return StatusCode(400, "An unexpected error occured");
        }
    }
    
    /**
     * End-Point: PATCH - localhost:5099/messages/{MessageId} <br /><br />
     * Requirements:<br/>
     * - Message ID should already exist <br />
     * - Message Text CANNOT be blank <br />
     * - Message Text CANNOT exceed 255 characters <br />
     * NOTE: ASP.NET Core handles validation BEFORE running the method. Any
     * failed validation messages, specified in the DTO, will be included
     * in a 400 Response. Also, ASP.NET does not appear to throw the Validation
     * warnings if a piece of data is missing altogether. Valuable information
     * for any future ASP.NET Core projects.<br /><br />
     * Success: 200 (Ok) with body containing number of rows updated. Note
     * that this would always return `1` when the message exists because
     * there should only ever be one with the ID. <br />
     * Fail: 400 (Client Error)
     */
    [HttpPatch("{messageId}")]
    public IActionResult UpdateMessageById(
        int messageId, MessagesDto updatedMessageDtoPayload
    ) {
        try {
            MessagesEntity updatedMessageEntity = new MessagesEntity();
            updatedMessageEntity.MessageId = messageId;
            updatedMessageEntity.MessageText = updatedMessageDtoPayload.MessageText;
            
            int numberOfUpdatedMessages = _messagesService.UpdateMessageById(
                updatedMessageEntity, _dbContext);
            
            if (numberOfUpdatedMessages == 0) {
                return BadRequest();
            }
            
            return Ok(1);
        } catch (Exception e) {
            Console.WriteLine("Unexpected Error: " +  e.Message);
            return StatusCode(400, "An unexpected error occured");
        }
    }

}
