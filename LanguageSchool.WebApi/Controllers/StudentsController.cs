﻿using LanguageSchool.BusinessLogic;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;



namespace LanguageSchool.WebApi.Controllers
{
    public class StudentsController : BaseController
    {
        IStudentBLL _studentService;
        IClassBLL _classService;

        public StudentsController(IStudentBLL studentService, IClassBLL classService)
        {
            _studentService = studentService;
            _classService = classService;
        }

        [Authorize]
        [Route("api/student/classes"), HttpGet]
        public IHttpActionResult GetClasses()
        {
            var classesIDs = _studentService.GetClasses(CurrentUserId());
            if (classesIDs == null)
                return NotFound();
            return Ok(classesIDs.Select(x => _classService.GetByID(x)).ToList());
        }

        [Authorize]
        [Route("api/student/info"), HttpGet]
        public IHttpActionResult GetInformations()
        {
            var studentInfo = _studentService.GetById(CurrentUserId());
            if(studentInfo == null)
                return NotFound();

            studentInfo.UserName = CurrentUserName();
            return Ok(studentInfo);
        }

        [Authorize]
        [Route("api/student/info"), HttpPut]
        public IHttpActionResult PutInformations(EditProfileModel editModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string error = _studentService.Update(CurrentUserId(), editModel.FirstName, editModel.LastName, editModel.Email, editModel.PhoneNumber);

            if (error != null)
                return BadRequest(error);

            return Ok();
        }

        [Authorize]
        [Route("api/student/charge"), HttpGet]
        public IHttpActionResult GetTotalCharge()
        {
            var charge = _studentService.GetTotalCharge(CurrentUserId());
            if (!charge.HasValue)
                return NotFound();
            else
                return Ok(String.Format("{0:0.00} PLN",charge.Value));
        }

    }
}
