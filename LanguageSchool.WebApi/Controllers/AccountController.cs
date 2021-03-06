﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using LanguageSchool.BusinessLogic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net.Mail;

namespace LanguageSchool.WebApi.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private AuthRepository _repo = null;
        private IStudentBLL _studentService;
        public AccountController(IStudentBLL studentService)
        {
            _repo = new AuthRepository();
            _studentService = studentService;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserRegistrationModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string error = null;
            if(!Validator.IsFirstNameValid(userModel.FirstName, ref error))
                return BadRequest(error);
            if (!Validator.IsLastNameValid(userModel.LastName, ref error))
                return BadRequest(error);
            if (!Validator.IsEmailValid(userModel.Email, ref error))
                return BadRequest(error);
            if (!Validator.IsPhoneNumberValid(userModel.PhoneNumber, ref error))
                return BadRequest(error);

            IdentityResult result = await _repo.RegisterUser(userModel);

            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            IdentityUser user = await _repo.FindUser(userModel.UserName, userModel.Password);
            _studentService.Add(user.Id, userModel.FirstName, userModel.LastName, userModel.Email, userModel.PhoneNumber);
            SendConfimationEmail(userModel.Email, userModel.FirstName);
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private void SendConfimationEmail(string emailAddress, string name)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.IsBodyHtml = true;
            mail.From = new MailAddress("languageschool.net@gmail.com");
            mail.To.Add(emailAddress);
            mail.Subject = "Welcome!";
            mail.Body = "<h2>Welcome "+name+"!</h2><p>Thanks for registering to our Language School. We hope you'll be satisfied with our lessons. Now you can manage your account and sign for classes chosen from wide offer. Go to login page.</p>"+
                "<p>Best regards,</p><p>Language School Team</p>";

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("languageschool.net", "psiakostka");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
        }
    }
}
