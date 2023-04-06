using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models.Notifications;
using Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Customers;
using Services.Messages;
using Services.Notifications;
using Services.Posts;
using WebApi.Extensions;
using WebApi.Models;
using WebApi.Models.Auth;
using WebApi.Models.Clients;
using WebApi.Models.Messages;
using WebApi.Utils;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Drawing.Imaging;
using System.Net;
using Newtonsoft.Json.Linq;

namespace WebApi.Controllers
{

  /// <summary>
  /// Handle Client interactions  
  /// </summary>
  [Route("/api/v1/clients")]

  public class ClientsController : BaseControler
  {

    private readonly ICustomerService _customerService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly IPostService _postService;
    private readonly IMessageService _messageService;
    private readonly IStringLocalizer<ClientsController> _localizer;
    //private INotificationTransport _emailTransport;

        public ClientsController(ICustomerService customerService,
                                IConfiguration config,
                                IMapper mapper,
                                IPostService postService,
                                INotificationService notificationService,
                                IMessageService messageService,
                                IStringLocalizer<ClientsController> localizer
                                /*INotificationTransport emailTransport*/)
        {
            _customerService = customerService;
            _configuration = config;
            _postService = postService;
            _notificationService = notificationService;
            _mapper = mapper;
            _messageService = messageService;
            _localizer = localizer;
            //_emailTransport = emailTransport;
        }
        /// <summary>
        /// Create Account.
        /// Input: CreateAccount Object; Output: token
        /// </summary>
        /// <typeparam name = "createModel"> CreateAccount </typeparam>
        /// <returns> token </returns>
        /// <exception cref = "CantUseEmail"> Cant Use Email </exception>
        /// <exception cref = "NickNameTaken"> NickName Taken </exception>
        /// <exception cref = "ModelStateNotValid"> Access=false, CreateAccount Object </exception>
        [HttpPost] // <-for swagger
        public async Task<object> PostAsync([FromBody] CreateAccount createModel)
        {
          //if (_customerService.FindByEmail(createModel.Email) != null)
          //  ModelState.AddModelError("Email", "Email already exists");

          if (ModelState.IsValid)
          {
            var user = _mapper.Map<CreateAccount, User>(createModel);
            if (_customerService.FindByNickName(user.NickName) != null)
            {
                createModel.Password = null;
                //createModel.ConfirmPassword = null;
                return Ok(CreateResponse(createModel, false,
                    new Error[1] { new Error("NickNameTaken", _localizer["NickNameTaken"] ) }));
            }
            else if (_customerService.FindByEmail(user.Email) != null)
            {
              return Ok(CreateResponse(createModel, false,
                new Error[1] { new Error("CantUseEmail", _localizer["CantUseEmail"]) }));
            }

            var customer = _customerService.RegisterCustomer(user);

            //sends confirm email.
            int application = createModel.Application ?? (int)ApplicationType.Web;
            await SendEmailAsync(customer, application, "RegistrationApproval");

            return Ok(CreateResponse(new AuthToken(createModel.Email,
                _configuration["JwtIssuer"], _configuration["JwtKey"], user.ID).ToString()));
          }
          else
            return Ok(CreateResponse(createModel, false, ModelState.GetErrors()));
        }

        /// <summary>
        /// Login.
        /// Input: LoginModel Object; Output: token
        /// </summary>
        /// <typeparam name = "loginModel"> LoginModel </typeparam>
        /// <returns> token </returns>
        /// <exception cref = "EmailUnconfirmed"> Unconfirmed Email </exception>
        /// <exception cref = "PasswordDoesntMatch"> Password Doesnt Match </exception>
        [Route("Login")]
        [HttpPost]
        public object Login([FromBody]LoginModel loginModel)
        {
          var customerExists = _customerService.UserExists(loginModel.Email, loginModel.Password);
          if (!customerExists)
            ModelState.AddModelError("PasswordDoesntMatch", _localizer["PasswordDoesntMatch"]);

          if (ModelState.IsValid)
          {

            var user = _customerService.FindByEmail(loginModel.Email);
            if (!user.EmailConfirmed.GetValueOrDefault())
            {
              return Ok(CreateResponse(null, false, new List<Error>
                {
                new Error("EmailUnconfirmed", _localizer["EmailUnconfirmed"])
                }));
            }

            if (_customerService.IsBlockedByAdmin(user.ID))
            {
                return Ok(CreateResponse(null, false, new List<Error>
                    {
                    new Error("90DaysBlocked", _localizer["90DaysBlocked"])
                    }));
            }

            user.Deactivated = false;
            _customerService.UpdateUser(user);

            var token = new AuthToken(loginModel.Email, _configuration["JwtIssuer"], _configuration["JwtKey"], user.ID);



            return Ok(CreateResponse(token.ToString()));

          }
          else
            return Ok(CreateResponse(loginModel, false, ModelState.GetErrors()));
        }

        /// <summary>
        /// Recovery Password.
        /// Input: LoginModel Object; Output: No parameters
        /// </summary>
        /// <typeparam name = "LoginModel"> LoginModel </typeparam>
        /// <returns> No parameters </returns>
        /// <exception cref = "EmailNotFound"> Email Not Found </exception>
        [Route("Recovery")]
        [HttpPost]
        public async Task<object> Recovery([FromBody]RecoveryModel model)
        {
            //Microsoft.Extensions.Primitives.StringValues code = String.Empty;
            //if (!Request.Headers.TryGetValue("CaptchaCode", out code) || model.Captcha != _customerService.DecryptString(code, _configuration["CryptKey"]))
            //    return CreateResponse(null, false, new List<Error> { new Error("CaptchaIncorrect", _localizer["CaptchaIncorrect"]) });
            //Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //var captchaResponse = Request.Form["g-recaptcha-response"];

            try
            {
                var user = _customerService.FindByEmail(model.Email);
                if (user == null)
                    return CreateResponse(null, false, new List<Error> { new Error("EmailNotFound", _localizer["EmailNotFound"]) });

                if (_customerService.IsBlockedByAdmin(user.ID))
                {
                    return Ok(CreateResponse(null, false, new List<Error>
                    {
                        new Error("90DaysBlocked", _localizer["90DaysBlocked"])
                    }));
                }

                if (model.IsWeb) // IsWeb=false for Mobile version (without Captcha)
                {
                    var result = false;
                    var captchaResponse = model.Captcha;
                    var secretKey = _configuration["SecretKey"];
                    var apiUrl = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";
                    var requestUri = string.Format(apiUrl, secretKey, captchaResponse);
                    var request = (HttpWebRequest)WebRequest.Create(requestUri);

                    using (WebResponse response = request.GetResponse())
                    {
                        using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                        {
                            JObject jResponse = JObject.Parse(stream.ReadToEnd());
                            var isSuccess = jResponse.Value<bool>("success");
                            result = isSuccess;
                        }
                    }

                    if (!result)
                        return CreateResponse(null, false, new List<Error> { new Error("CaptchaIncorrect", _localizer["CaptchaIncorrect"]) });
                }

                var email = model.Email;
                if (string.IsNullOrEmpty(email) || user == null)
                    return CreateResponse(null, false, new List<Error> { new Error("EmailNotFound", _localizer["EmailNotFound"]) });

                //sends confirm email.
                var success = await SendEmailAsync(user, (int)ApplicationType.Web, "PasswordReminder");

                if (success)
                    return Ok(CreateResponse(success: success));
                else
                    return CreateResponse(null, false, new List<Error> { new Error("EmailNotSent", _localizer["EmailNotSent"]) });
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Approve Email API. 1) WEB: Api runs from the email link sent user after registration. Approves user email by given token and redirect to BlaBlogLucy.com/Signup. 2) Mobile: Registration -> Emailed link to user for approving -> Redirection to page from user email link -> Call this API which is approves user email with given token (new scenario)
        /// id = user id
        /// token= user approval token
        /// application = Web or Mobile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/EmailApprove")]
        //[HttpPost]   //Note: allow HttpGet because email hotlink.
        [HttpGet]  // <-for swagger
        public object EmailApprove([FromRoute]int id, [FromQuery]string token, [FromQuery]int application)
        {
            try
            {
                bool isOk = _customerService.ApproveEmail(id, token);
                if (isOk)
                {
                    if (application == (int)ApplicationType.Web)
                        return Redirect(_configuration["ApiKeys:Host"] + $@"signup");// + $@"home?emailApproved={isOk}");
                    else
                        return Ok(CreateResponse(_localizer["EmailApproved"].Value, success: true));
                }
                else
                    return Ok(CreateResponse(null, false, new Error[1] { new Error("EmailNotApproved", _localizer["EmailNotApproved"]) }));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Get User Info 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("info")]
        [Authorize]
        public object Info()
        {
            try
            { 
                var user = _customerService.GetUserById(UserId);
                var model = new UserInfo(user, _postService, _customerService, _messageService);
                return CreateResponse(model);
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        #region notificaton
        /// <summary>
        /// New Stories Info
        /// Input: page, limit; Output: { new_commented_stories_no, new_commented_stories, new_liked_stories_no, new_liked_stories, new_notifications_no, new_notifications_on_current_page }
        /// </summary>
        /// <typeparam name = "page"> int </typeparam>
        /// <typeparam name = "limit"> int </typeparam>
        /// <returns> { new_commented_stories_no, new_commented_stories, new_liked_stories_no, new_liked_stories, new_notifications_no, new_notifications_on_current_page
        [HttpGet]
        [Route("newStories")]
        [Authorize] 
        public object NewStories (int page = 1, int limit = 3)
        {
            try
            { 
                var commenteds = _customerService.GetNewCommentedStories(UserId).Distinct();
                var likeds = _customerService.GetNewLikedStories(UserId).Distinct();
                var notifications = _postService.GetNewNotifications(UserId, false).Distinct();
                var notificationsOnPage = notifications.Skip((page - 1) * limit).Take(limit).ToList();

                return CreateResponse(new
                {
                    new_commented_stories_no = commenteds.Count(),
                    new_commented_stories = commenteds,
                    new_liked_stories_no = likeds.Count(),
                    new_liked_stories = likeds,
                    new_notifications_no = notifications.Count(),
                    new_notifications_on_current_page = notificationsOnPage
                });
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Reset counter of new stories from users whom Im following to "0" for logged user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("ClearBlaBlogNotif")]
        [Authorize]
        public object ClearBlaBlogNotif()
        {
          try
          {
                _postService.ClearBlaBlogNotif(UserId);
                return Ok(CreateResponse(_localizer["HasNoResponse"].Value, success: true));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        #endregion

        #region BlockUnblock  
        /// <summary>
        /// Tail (follow/subscribe) to the selected user.
        /// Input: id (User ID) ; Output: success = true/false
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <returns> success = true/false </returns>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        [Authorize]
        [HttpPost]
        [Route("{id}/tail")]
        public object Tail(int id)
        {
            try 
            { 
                if (UserId == id)
                    return Ok(CreateResponse(null, false, new Error[1] { new Error("UserCantTailHimself", _localizer["UserCantTailHimself"]) }));
                _customerService.Tail(UserId, id);
                return Ok(CreateResponse(_localizer["HasNoResponse"].Value, success: true));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// UnTail (unfollow/unsubscribe) from the selected user.
        /// Input: id (User ID) ; Output: success = true/false
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <returns> success = true/false </returns>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        [Authorize]
        [HttpDelete]
        [Route("{id}/tail")]
        public object TailDelete(int id)
        {
            try
            { 
                _customerService.UnTail(UserId, id);
                return Ok(CreateResponse(_localizer["HasNoResponse"].Value, success: true));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// UnTail (unfollow/unsubscribe) from all of users.
        /// Input: id (User ID) ; Output: success = true/false
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <returns> success = true/false </returns>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        [Authorize]
        [HttpDelete]
        [Route("UntailAll")]
        public object UntailAll()
        {
            try
            {
                _customerService.UnTailAll(UserId);
                return Ok(CreateResponse(_localizer["HasNoResponse"].Value, success: true));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Hide user.
        /// Input: id (User ID) ; Output: success = true/false
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <returns> success = true/false </returns>
        /// <exception cref = "DoesntExistInDB"> Doesnt Exist In DB </exception>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        [Authorize]
        [HttpPost]
        [Route("{id}/hide")]
        public object Hide(int id)
        {
            try
            {
                var userExist = _customerService.GetUserById(id);
                if (userExist != null)
                    _customerService.Hide(UserId, id);
                return Ok(CreateResponse(_localizer["HasNoResponse"].Value, success: true));
            }
            catch (Exception ex)
            {
                return BadRequest("User ID = " + id.ToString() + " does not exist in the database");
            }
        }

        /// <summary>
        /// UnHide user.
        /// Input: id (User ID); Output: success = true/false
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <returns> success = true/false </returns>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        [Authorize]
        [HttpDelete]
        [Route("{id}/hide")]
        public object HideDelete(int id)
        {
            try
            {
            _customerService.UnHide(UserId, id);
                return Ok(CreateResponse(_localizer["HasNoResponse"].Value, success: true));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }


        /// <summary>
        /// Block user.
        /// Input: ReportModel Object ; Output: success = true/false
        /// </summary>
        /// <typeparam name = "model"> ReportModel </typeparam>
        /// <returns> success = true/false </returns>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        [Authorize]
        [HttpPost]
        [Route("block")]
        public object Block([FromBody]ReportModel model)
        {
        try
            { 
                _customerService.Block(UserId, model.Id, model.Reason);
                return Ok(CreateResponse(_localizer["HasNoResponse"].Value, success: true));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Place Report.
        /// Input: ReportModel Object ; Output: success = true/false
        /// </summary>
        /// <typeparam name = "model"> ReportModel </typeparam>
        /// <returns> success = true/false </returns>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        [Authorize]
        [HttpPost]
        [Route("report")]
        public object Report([FromBody]ReportModel model)
        {
            try
            { 
                var me = _customerService.GetUserById(UserId);
                var other = _customerService.GetUserById(model.Id);
                _customerService.SendReport(me.NickName, model.Reason, other.NickName);
                return Ok(CreateResponse(_localizer["HasNoResponse"].Value, success: true));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Unblock User.
        /// Input: User ID; Output: success = true/false
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <returns> success = true/false </returns>
        /// <exception cref = "ApplicationError"> Application Error </exception>
        [Authorize]
        [HttpDelete]
        [Route("{id}/block")]
        public object BlockDelete(int id)
        {
            try
            { 
                string result = _customerService.Unblock(UserId, id);
                if (result == "Success")
                    return Ok(CreateResponse(_localizer["HasNoResponse"].Value, success: true));
                else
                    return Ok(CreateResponse(null, false, new Error[1] { new Error("NotDeleted", result) }));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Get user who follows me (mode = 0) or users im following (mode = 1).
        /// Input: User ID; Output: List(UserModel)
        /// </summary>
        /// <typeparam name = "mode"> int </typeparam>
        /// <returns> List(UserModel) </returns>
        [Authorize]
        [HttpGet]
        [Route("BlaBlog")]
        public object BlaBlog(int mode = 0)
        {
            try
            { 
                IList<User> result = new List<User>();
                if (mode == 0) // user who follows me (ofer kpoker)
                    result = _customerService.MyBlaBlog(UserId);
                else // users im following (ofer kpoker)
                    result = _customerService.ImTailing(UserId);
                var model = result.Select(u => new UserModel(u)).ToList();
                return CreateResponse(model);
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }
        #endregion

        /// <summary>
        /// Post User Message.
        /// Input: id (receiver user ID), MessageModel; Output: success = true/false
        /// </summary>
        /// <typeparam name = "id"> int </typeparam>
        /// <typeparam name = "message"> MessageModel </typeparam>
        /// <returns> success = true/false </returns>
        /// <exception cref = "ApplicationError"> DataBase Error </exception>
        [Authorize]
        [HttpPost]
        [Route("{id}/message")]
        public object Message([FromRoute]int id, [FromBody]MessageModel message)
        {
            try
            { 
                _customerService.SendMessage(new MessageUser
                {
                    LeftId = UserId,
                    RightId = id,
                    MessageDate = DateTime.Now,
                    Message = message.Message
                });
                return Ok(CreateResponse(_localizer["HasNoResponse"].Value, success: true));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Get Blocked Users.
        /// Input: No parameters; Output: IList(BlockedUser)
        /// </summary>
        /// <returns> IList(BlockedUser) </returns>
        [Authorize]
        [HttpGet]
        [Route("blocked")]
        public object GetBlocked()
        {
            try
            { 
                var blockedUsers = _customerService.GetBlockedUsers(UserId);
                List<BlockedUserModel> model = (from x in blockedUsers select new BlockedUserModel(x, _customerService, _localizer, _configuration)).ToList();
                return CreateResponse(model);
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Get Hidden Users.
        /// Input: No parameters; Output: IList(HiddenUserModel)
        /// </summary>
        /// <returns> IList(HidenUser) </returns>
        [Authorize]
        [HttpGet]
        [Route("hidden")]
        public object GetHidden()
        {
            try
            {
                var hidenUsers = _customerService.GetHiddenUsers(UserId);
                var isAllUsersExist = true;
                var uID = -1;
                hidenUsers?.ToList().ForEach(u =>
                {
                    if (u.OUser == null)
                    {
                        isAllUsersExist = false;
                        uID = u.RightId;
                    }
                });
                if (!isAllUsersExist)
                    return Ok(CreateResponse(null, false, new Error[1] { new Error("UserIsNotExistInDB", _localizer["UserIsNotExistInDB"] + ". UserID=" + uID.ToString()) }));
                List<HiddenUserModel> oUser = (from x in hidenUsers select new HiddenUserModel(x.OUser, _customerService)).ToList();
                return CreateResponse(oUser);
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }

        }

        /// <summary>
        /// Deactivate Logged Users.
        /// Input: No parameters; Output: success = true/false
        /// </summary>
        /// <returns> success = true/false </returns>
        [Authorize]
        [HttpPost]
        [Route("deactivate")]
        public object Deactivate()
        {
            try
            { 
                _customerService.Deactivate(UserId);
                return Ok(CreateResponse(_localizer["HasNoResponse"].Value, success: true));
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Search User By Nick.
        /// Input: string name; Output: UserModel Object
        /// </summary>
        /// <typeparam name = "name"> string </typeparam>
        /// <returns> UserModel Object </returns>
        /// <exception cref = "NicknameNotFound"> Nickname Not Found </exception>
        //[Authorize]
        [HttpGet]
        [Route("hiddenSearchByNick/{name}")]
        public object SearchByNick([FromRoute]string name)
        {
            try
            { 
                var customer = _customerService.FindByNickName(name);
                if (customer != null)
                {
                    return CreateResponse(new UserModel(customer));
                }
                return CreateResponse(null, false, new Error[1] { new Error("NicknameNotFound", _localizer["NicknameNotFound"]) });
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Search User By Email.
        /// Input: string email; Output: UserModel Object
        /// </summary>
        /// <typeparam name = "email"> string </typeparam>
        /// <returns> UserModel Object </returns>
        /// <exception cref = "EmailNotFound"> Email Not Found </exception>
        //[Authorize]
        [HttpGet]
        [Route("hiddenSearchByEmail/{email}")]
        public object SearchByEmail([FromRoute]string email)
        {
            try
            { 
                var customer = _customerService.FindByEmail(email);
                if (customer != null)
                {
                    return CreateResponse(new UserModel(customer));
                }
                return CreateResponse(null, false, new Error[1] { new Error("EmailNotFound", _localizer["EmailNotFound"]) });
            }
            catch (Exception ex)
            {
                return Ok(CreateResponse(null, false, new Error[1] { new Error("ApplicationError", _localizer["ApplicationError"] + " " + ex.Message) }));
            }
        }

        /// <summary>
        /// Change Culture. (Not used - for tests)
        /// Input: string culture [en, he]; Output:  success = true/false 
        /// </summary>
        /// <typeparam name = "culture"> string </typeparam>
        /// <returns>  success = true/false  </returns>
        [Authorize]
        [HttpPost]
        [Route("ChangeCulture")] 
        // Is not using for now 
        // AcceptLanguageHeaderRequestCultureProvider извлекает код культуры из заголовка Accept-Language, который передается в запросе
        public /*object*/ void ChangeCulture(/*[FromHeader] string lang, */string culture)
        {
            //var culture = Cultures.CD.GetCulture(lang);
            //Cultures.CD.SetThreadCulture(culture);

            //Microsoft.Extensions.Primitives.StringValues cult = String.Empty;
            //Request.Headers.TryGetValue("lang", out cult);
            //ChangeCulture(lang);

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );


            if (!String.IsNullOrEmpty(culture))
            {
                CultureInfo.CurrentCulture = new CultureInfo(culture);
                CultureInfo.CurrentUICulture = new CultureInfo(culture);
            }

            //return Ok();
        }


        private async Task<bool> SendEmailAsync(User user, int application, string contentFileName)
        {
            var messageBuilder = new MessageBuilder(Environment.CurrentDirectory + "/Content/" + contentFileName + ".txt");
            if (contentFileName == "RegistrationApproval")
            {
                messageBuilder.AddToken(new MessageToken("NickName", user.NickName));
                messageBuilder.AddToken(new MessageToken("Link", CreateApproveLink(user, application)));
            }
            else if (contentFileName == "PasswordReminder")
                messageBuilder.AddToken(new MessageToken("Password", user.Password));
            var body = await messageBuilder.BuildAsync();
            bool success = await _notificationService
              .SendNotificationAsync(new EmailNotification(
                _configuration["Smtp:from"],
                user.Email,
                _localizer[contentFileName],
                body));

            return success;
        }

        private string CreateApproveLink(User user, int application)
        {
            string hostApiAddress = String.Empty;
            if (application == (int)ApplicationType.Web)
            {
                hostApiAddress = _configuration["ApiKeys:HostApi"];
                return hostApiAddress + $@"Clients/{user.ID}/EmailApprove?token={user.EmailConfirmToken}&application={application}";
            }
            else
            {
                hostApiAddress = _configuration["ApiKeys:Host"];
                return hostApiAddress + $@"verification?token={user.EmailConfirmToken}&userId={user.ID}&application={application}";
            }
        }
        //[Route("Captcha")]
        //[HttpGet]
        //public async Task<object> CaptchaAsync()
        //{
        //    string code = new Random(DateTime.Now.Millisecond).Next(11111, 99999).ToString();
        //    code += new Random(DateTime.Now.Second).Next(11111, 99999).ToString();
        //    string encripted = _customerService.EncryptString(code, _configuration["CryptKey"]);
        //    Response.Headers.Add("CaptchaCode", encripted);
        //    //HttpContext.Session.SetString("CaptchaCode", code);
        //    CaptchaImage captcha = new CaptchaImage(code, 250, 50);
        //    using (var stream = new MemoryStream())
        //    {
        //        captcha.Image.Save(stream, ImageFormat.Png);
        //        return await Task.Run(() => File(stream.ToArray(), "image/jpeg"));
        //    }
        //}

        //tests:
        //Note: Test.
        //[HttpPost]
        //[Route("test1")]
        //public object TestSendMail([FromBody] User user)
        //{
        //  bool res = false;
        //  string errorMessage = "";
        //  try
        //  {
        //    res = SendPostRegisterEmailAsync(user).Result;
        //  }
        //  catch (Exception ex)
        //  {
        //    errorMessage = ex.ToString();
        //  }
        //  if (res)
        //  {
        //    return CreateResponse(success: true);
        //  }
        //  else
        //  {
        //    return CreateResponse(success: false, errors: new List<Error>() { new Error("default", errorMessage) });
        //  }
        //}

        ///// <summary>
        ///// Involve Socket IO Task. Api ll be called from Admin APP on change story/post status event
        ///// Input: sId int (post/story ID); Output: run Socket IO Task
        ///// </summary>
        ///// <typeparam name = "sId"> int </typeparam>
        ///// <returns> token </returns>
        //[Route("AdminAppSocket")]
        //[HttpPost]
        //public async void AdminAppChangeStoryStatusAsync(int sId)
        //{
        //    //await Hubs.ChatHub.SendAdminApp();
        //}

    }
}