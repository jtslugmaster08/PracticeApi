using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using PracticeModel.Dto;
using PracticeModel.Entities;
using PracticeModel.Interface.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;

namespace PracticeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<BaseUser> _signInManager;
        private readonly UserManager<BaseUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IErrorService _errorService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        public AccountController(SignInManager<BaseUser> signInManager, UserManager<BaseUser> UserManager, IConfiguration configuration, IEmailService EmailService, IErrorService errorService, IMapper Mapper, IUserService UserService)
        {
            _signInManager = signInManager;
            _userManager = UserManager;
            _configuration = configuration;
            _errorService = errorService;
            _emailService = EmailService;
            _userService = UserService;
            _mapper = Mapper;
        }

        [HttpPost]
        [Route("ChangePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword(PasswordResetDto dto)
        {
            ResponseDto<PasswordResetDto> responseDto = new ResponseDto<PasswordResetDto>();
            var theUser = _userManager.FindByNameAsync(dto.UserName).Result;
            if (theUser != null)
            {
                var result = _userManager.ChangePasswordAsync(theUser, dto.CurrentPassword, dto.NewPassword).Result;
                if (result != null && result.Succeeded)
                {
                    return Ok(responseDto);
                }
                else
                {
                    foreach (var errors in result.Errors)
                    {
                        responseDto.Errors.Add(new ErrorDto { ErrorCode = errors.Description });
                    }
                }
            }

            return Ok(responseDto);

        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user != null)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    var baseappUrl = _configuration.GetSection("AppUrl").Value;
                    string returnUrl = Url.Content(baseappUrl ?? "");
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var uriBuilder = new UriBuilder(baseappUrl + "account/forgotPassword");
                    var parameters = HttpUtility.ParseQueryString(string.Empty);
                    parameters["userId"] = userId;
                    parameters["code"] = code;
                    uriBuilder.Query = parameters.ToString();

                    Uri finalUrl = uriBuilder.Uri;

                    var res = _emailService.SendEmail(user.Email, "ForgotPassword", $"Rest your password by <a href='{HtmlEncoder.Default.Encode(finalUrl.AbsoluteUri)}'>clicking here </a>.");
                }
            }

            return Ok(true);
        }

        [HttpPost]
        [Route("ConfirmForgotPassword")]
        public async Task<IActionResult> ConfirmForgotPassword(ConfirmForgotPasswordDto confirmForgotPassword)
        {
            ResponseDto<IdentityResult> response = new ResponseDto<IdentityResult>();
            var retVal = confirmForgotPassword;
            if (retVal != null)
            {
                var code3 = WebEncoders.Base64UrlDecode(retVal.Code);
                var token = Encoding.UTF8.GetString(code3);

                var user = await _userManager.FindByIdAsync(retVal.UserId);
                if (user != null)
                {
                    var randomPassword = _userService.GeneratePassword(16);
                    var success = await _userManager.ResetPasswordAsync(user, token, randomPassword);
                    response.Data = success;
                    if (success != null)
                    {
                        if (success.Succeeded)
                        {
                            var res = _emailService.SendEmail(user.Email, "New Password", $"Please login with the new password. You will be prompted to change your password. Please do so. <b>{randomPassword}</b>");
                            return Ok(true);
                        }
                        else if (success.Errors.FirstOrDefault()?.Description == "Invalid token.")
                        {
                            foreach (var error in success.Errors)
                            {

                                response.Errors.Add(new ErrorDto { ErrorCode = error.Description });
                            }
                            return Ok(response);
                        }
                        else
                        {
                            foreach (var error in success.Errors)
                            {
                                response.Errors.Add(new ErrorDto { ErrorMessage = error.Description });
                            }
                            return Ok(response);
                        }
                    }
                    else
                    {
                        return Ok(false);
                    }

                }
                else
                {
                    return Ok(false);
                }
            }
            else
            {
                return Ok(false);
            }
        }
        ///confirmemail
        ///
        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
        {
            ResponseDto<IdentityResult> response = new ResponseDto<IdentityResult>();
            var retVal = confirmEmailDto;
            if (retVal != null)
            {
                var code3 = WebEncoders.Base64UrlDecode(retVal.Code);
                var token = Encoding.UTF8.GetString(code3);
                var user = await _userManager.FindByNameAsync(retVal.UserId);
                if (user != null)
                {

                    var success = await _userManager.ConfirmEmailAsync(user, token);
                    response.Data = success;
                    if (success != null)
                    {
                        if (success.Succeeded)
                        {
                            return Ok(response);
                        }
                        else if (success.Errors.FirstOrDefault()?.Description == "Invalid token.")
                        {
                            foreach (var error in success.Errors)
                            {
                                response.Errors.Add(new ErrorDto { ErrorCode = error.Description });

                            }
                            var baseAppurl = _configuration.GetSection("AppUrl").Value;
                            string returnUrl = Url.Content(baseAppurl ?? "");
                            var userId = await _userManager.GetUserIdAsync(user);
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                            var uriBuilder = new UriBuilder(baseAppurl + "account/confirmEmail");
                            var parameters = HttpUtility.ParseQueryString(string.Empty);
                            parameters["userId"] = userId;
                            parameters["code"] = code;
                            uriBuilder.Query = parameters.ToString();

                            Uri finalUrl = uriBuilder.Uri;

                            var res = _emailService.SendEmail(user.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(finalUrl.AbsoluteUri)}'>clicking here</a>.");

                            return Ok(response);
                        }
                        else
                        {
                            foreach (var error in success.Errors)
                            {
                                response.Errors.Add(new ErrorDto { ErrorCode = error.Description });

                            }
                            return Ok(response);

                        }

                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    return Ok(false);
                }
            }
            else
            {
                return Ok(false);
            }
        }


        [HttpPost]
        [Route("UpdateEmail")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateEmail(UpdateEmailDto updateEmailDto)
        {
            ResponseDto<UpdateEmailDto> responseDto = new ResponseDto<UpdateEmailDto> { Data = updateEmailDto };
            var tempUser = _userManager.FindByIdAsync(this.User.Identity.Name).Result;
            if (tempUser != null)
            {
                var baseAppurl = _configuration.GetSection("AppUrl").Value;
                var userId = await _userManager.GetUserIdAsync(tempUser);
                var code = await _userManager.GenerateChangeEmailTokenAsync(tempUser, updateEmailDto.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var newemail = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(updateEmailDto.NewEmail));

                var uriBuilder = new UriBuilder(baseAppurl + "validateNewEmail");
                var parameters = HttpUtility.ParseQueryString(string.Empty);
                parameters["userId"] = userId;
                parameters["code"] = code;
                parameters["e"] = newemail;
                uriBuilder.Query = parameters.ToString();

                Uri finalUrl = uriBuilder.Uri;

                var res = _emailService.SendEmail(updateEmailDto.NewEmail, "Confirm New Email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(finalUrl.AbsoluteUri)}'>clicking here</a>.");
            }
            else
            {

            }
            return Ok(responseDto);
        }

        [HttpPost]
        [Route("ConfirmUpdateEmail")]
        public async Task<IActionResult> ConfirmUpdateEmail(ConfirmUpdateEmailDto confirmUpdateEmailDto)
        {
            ResponseDto<IdentityResult> response = new ResponseDto<IdentityResult>();
            var retVal = confirmUpdateEmailDto;
            if (retVal != null)
            {
                var code3 = WebEncoders.Base64UrlDecode(retVal.Code);
                var token = Encoding.UTF8.GetString(code3);
                var em = WebEncoders.Base64UrlDecode(retVal.NewEmail);
                var email = Encoding.UTF8.GetString(em);

                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var success = await _userManager.ChangeEmailAsync(user, email, token);
                    response.Data = success;
                    if (success != null)
                    {
                        if (success.Succeeded)
                        {
                            return Ok(response);
                        }
                        else
                        {
                            foreach (var er in success.Errors)
                            {
                                response.Errors.Add(new ErrorDto { ErrorCode = er.Code });
                            }
                            return Ok(response);
                        }
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    return Ok(false);
                }

            }
            else
            {
                return Ok(false);
            }
        }

        [HttpPost]
        [Route("SignIn")]
        public async Task<IActionResult> Login(SigninDto signin)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<SigninDto, BaseUser>(signin);
                var result = await _signInManager.PasswordSignInAsync(user, signin.Password, false, false);
                if (result.Succeeded)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }

        //create new user

        [HttpPost]
        [Route("CreateNewUser")]
        public async Task<IActionResult> CreateUser(MembershipDto model)
        {
            ResponseDto<MembershipDto> response = new ResponseDto<MembershipDto>();
            response.RequestData = JsonSerializer.Serialize(model);
            try
            {
                if (ModelState.IsValid)
                {

                    BaseUser baseUser = _mapper.Map<MembershipDto, BaseUser>(model);

                    var result = await _userManager.CreateAsync(baseUser);
                    if (result.Succeeded)
                    {
                        var baseAppurl = _configuration.GetSection("AppUrl").Value;
                        string returnUrl = Url.Content(baseAppurl ?? "");
                        var userId = await _userManager.GetUserIdAsync(baseUser);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(baseUser);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        var uriBuilder = new UriBuilder(baseAppurl + "account/confirmemail");
                        var parameters = HttpUtility.ParseQueryString(string.Empty);
                        parameters["userId"] = userId;
                        parameters["code"] = code;
                        uriBuilder.Query = parameters.ToString();
                        Uri finalUrl = uriBuilder.Uri;

                        var res = _emailService.SendEmail(baseUser.Email, "Confirm you Email",
                            $"Please confirm you account by <a href='{HtmlEncoder.Default.Encode(finalUrl.AbsoluteUri)}'>clicking here </a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = baseUser.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(baseUser, isPersistent: false);
                            return Ok(response);
                        }
                    }
                    else
                    {
                        foreach(var item in result.Errors)
                        {
                            response.Errors.Add(new ErrorDto { ErrorCode = item.Description });
                        }
                    }
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _errorService.CreateError(ex);
                response.Errors.Add(new ErrorDto { ErrorCode= ex.Message });
            }
            return Ok(response);
        }

        //Create Token
        [HttpPost]
        [Route("Token")]
        public async Task<IActionResult> CreateToken(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.UserName);
                if (user != null)
                {
                    var result = _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                    if (result.Result.Succeeded)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        var claim = new List<Claim>()
                        {
                            new Claim(JwtRegisteredClaimNames.Sub,model.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.UniqueName,model.UserName),
                        };
                        foreach (var userRole in userRoles)
                        {
                            claim.Add(new Claim(ClaimTypes.Role, userRole));
                        }

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(_configuration["Tokens:Issuer"], _configuration["tokens:Audience"], claim, signingCredentials: credential, expires: DateTime.Now.AddMinutes(10));

                        return Created("", new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo,
                        });
                    }
                }

            }
            return BadRequest();
        }
    }


}
