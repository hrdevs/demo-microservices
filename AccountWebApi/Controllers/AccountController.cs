using AccountWebApi.Models;
using JwtAuthenticationManager;
using JwtAuthenticationManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccountWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountDbContext _accountDbContext;
        private readonly JwtTokenHandler _jwtTokenHandler;

        public AccountController(AccountDbContext accountDbContext, JwtTokenHandler jwtTokenHandler)
        {
            _accountDbContext = accountDbContext;
            _jwtTokenHandler = jwtTokenHandler;
        }

        [HttpPost("authenticate")]
        public ActionResult<AuthenticationResponse> Authenticate([FromBody] AuthenticationRequest authenticationRequest)
        {
            var account = _accountDbContext.Accounts.FirstOrDefault(x => x.UserName == authenticationRequest.UserName && x.Password == authenticationRequest.Password);
            if(account == null) return Unauthorized("User not found");

            var authenticationResponse = _jwtTokenHandler.GenerateJwtToken(account.UserName, account.Role);
            if(authenticationResponse == null) return Unauthorized();

            return authenticationResponse;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(Account account)
        {
            await _accountDbContext.Accounts.AddAsync(account);
            await _accountDbContext.SaveChangesAsync();

            return Ok("Account registered!");
        }

        [HttpGet("/api/accounts")]
        public ActionResult<IEnumerable<Account>> GetAccounts() 
        {
            return _accountDbContext.Accounts;
        }

        [HttpGet("{accountId:int}")]
        public async Task<ActionResult<Account>> GetById(int accountId)
        {
            var account = await _accountDbContext.Accounts.FindAsync(accountId);

            return account;
        }

        [HttpPut]
        public async Task<ActionResult> Update(Account account)
        {
            _accountDbContext.Accounts.Update(account);
            await _accountDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{accountId:int}")]
        public async Task<ActionResult> Delete(int accountId)
        {
            var account = await _accountDbContext.Accounts.FindAsync(accountId);
            _accountDbContext.Accounts.Remove(account);
            await _accountDbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
