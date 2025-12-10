using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SocialMediaBlog.CommonInterfaces;
using SocialMediaBlog.DatabaseContext;
using SocialMediaBlog.Messages;

namespace SocialMediaBlog.Account;

// DESC: Enable API-specific behavior
// NOTE: Attribute provides for behavior such as Parameter Source Inference,
// ... Attribute Routing (as a requirement) and Model Validation (error handling)
[ApiController]

// DESC: Set the base-point, for all Account end-points, to '/'
// NOTE: Given that the spec requires each to have their own endpoint, it
// ... makes the most sense to start with "/" and proceed from there
[Route("/")]
public class AccountController : ControllerBase {

    // NOTE: PT03 - D.I. - Inject Service(s) into Controller
    
    /*
     * CALL OUT: Constructor-based D.I. is NOT used for the Service-method
     * ... that impl's the CommonContract Interface due to the new "Keyed
     * ... Services" functionality -- that appears to use Method-based D.I.
     */
    
    private readonly SocialMediaBlogDbCtx _dbContext;
    private readonly IAccountService _accountService;

    public AccountController(
        SocialMediaBlogDbCtx dbContext,
        IAccountService accountService
    ) {
        _dbContext = dbContext;
        _accountService = accountService;
    }


    /**
     * End-Point: POST - localhost:5099/register <br /><br />
     * Requirements:<br/>
     * - Username CANNOT be blank <br />
     * - User MUST NOT exist in Database <br />
     * - Password MUST BE greater-than or equal to four characters <br />
     * NOTE: ASP.NET Core handles validation BEFORE running the method. Any
     * failed validation messages, specified in the DTO, will be included
     * in a 400 Response. <br /><br />
     * Success: 200 (Ok) with body containing Account as JSON (incl. ID) <br />
     * Fail: 409 (Conflict) for dup. username, otherwise 400 (Client Error)
     */
    [HttpPost("/register")]
    public IActionResult CreateNewUser(
        [FromKeyedServices("Acct. DI w/ Interface")] ICommonContract commonContract,
        AccountDto newAccountDtoPayload
    ) {
        try {
            // DESC: Map JSON Payload to Entity
            // NOTE: It is BEST PRACTICE to map to DTO ASAP, which is
            // ... why the payload is received as a DTO
            // NOTE: The `AccountId` will NOT be known at this point
            AccountEntity newAccountEntity = new AccountEntity(
                newAccountDtoPayload.Username,
                newAccountDtoPayload.Password
            );
            
            // DESC: Call Service-method (w/ Validated Data)
            Dictionary<string, object> serviceLayerResponse = commonContract
                .CreateNewEntity(newAccountEntity, _dbContext);
            
            

            // DESC: Extract the Status Code from the Dictionary
            int? statusCode = (serviceLayerResponse.ContainsKey("statusCode"))
                ? Convert.ToInt32(serviceLayerResponse["statusCode"])
                : null;
            
            // DESC: Act accordingly (based on the Status Code)
            switch (statusCode) {
                case 200:
                    return Ok(serviceLayerResponse["DTO"]);
                case 409:
                    return Conflict();
                default:
                    return BadRequest();
            }
            
        } catch (Exception e) {
            // NOTE: This is in place as a catch-all for any other errors
            Console.WriteLine("Unexpected Error: " +  e.Message);
            return StatusCode(400, "An unexpected error occured");
        }
    }
    
    
    
    /**
     * End-Point: POST - localhost:5099/login <br /><br />
     * Requirements:<br/>
     * - Username AND Password MUST match existing acct. <br />
     * NOTE: ASP.NET Core handles validation BEFORE running the method. Any
     * failed validation messages, specified in the DTO, will be included
     * in a 400 Response. <br /><br />
     * Success: 200 (Ok) with body containing Account as JSON (incl. ID) <br />
     * Fail: 401 (Unauthorized) for bad credentials
     */
    [HttpPost("/login")]
    public IActionResult ReadExistingUser(AccountDto existingAccountDtoPayload) {
        try {
            // DESC: Convert to Entity
            // NOTE: This allows communicating with the DbContext
            AccountEntity existingAccountEntity = new AccountEntity(
                existingAccountDtoPayload.Username,
                existingAccountDtoPayload.Password
            );
            
            // DESC: Call Service-method (w/ Validated Data)
            Dictionary<string, object> serviceLayerResponse = _accountService
                .CheckForExistingUser(existingAccountEntity, _dbContext);

            // DESC: Check if "isValidLoginAttempt" exists in Service
            // ... layer response
            // NOTE: It is NOT added to the Dictionary if the Username AND
            // ... Password combination is not found
            bool isValidLoginAttempt = serviceLayerResponse.ContainsKey("isValidLoginAttempt");

            switch (isValidLoginAttempt) {
                case true:
                    return Ok(serviceLayerResponse["DTO"]);
                default:
                    return StatusCode(401, "Username and/or password are invalid");
            }
        } catch (Exception e) {
            // NOTE: This is in place as a catch-all for any other errors
            Console.WriteLine("Unexpected Error: " +  e.Message);
            return StatusCode(401, "An unexpected error occured");
        }
    }
    
    /**
     * End-Point: GET - localhost:5099/accounts/{accountId}/messages <br /><br />
     * Success: 200 (Ok) with body containing Messages as JSON (incl. ID) <br />
     * Fail: 200 (Ok) with empty list
     */
    [HttpGet("/accounts/{accountId}/messages")]
    public IActionResult ReadAllMessagesByAccountId(int accountId) {
        try {
            List<MessagesDto> allMessagesByAccount = _accountService
                .RetrieveMessagesByAccountId(accountId, _dbContext);
            return Ok(allMessagesByAccount);
        } catch (Exception e) {
            Console.WriteLine("Unexpected Error: " +  e.Message);
            return StatusCode(400, "An unexpected error occured");
        }
    }

}
