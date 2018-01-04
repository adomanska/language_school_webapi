﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LanguageSchool.Model;
using LanguageSchool.BusinessLogic;
using LanguageSchool.DataAccess;
using System.Web;
using LanguageSchool.Shared.Dtos;
using Microsoft.AspNet.Identity;

namespace LanguageSchool.WebApi.Controllers
{
    public class ClassesController : BaseController
    {
        IClassBLL _classService;
        IStudentBLL _studentService;

        public ClassesController(IStudentBLL studentService, IClassBLL classService)
        {
            _studentService = studentService;
            _classService = classService;
        }

        public IHttpActionResult Get()
        {
            var classes = _classService.GetAll();
            if(classes!=null)
                return Ok(classes);
            else
                return NotFound();
        }

        [Route("api/classes/{id:int}"), HttpGet]
        public IHttpActionResult Get(int id)
        {
            var _class = _classService.GetByID(id);
            if (_class == null)
                return NotFound();
            else
                return Ok(_class);
        }

        public IHttpActionResult Get(string language, string languageLevel)
        {
            var classes = _classService.GetClasses(language, languageLevel);
            if (classes == null)
                return NotFound();
            return Ok(classes);
        }

        [Route("api/classes/top/{count:int}"),HttpGet]
        public IHttpActionResult GetTop(int count)
        {
            if (count <= 0)
                return BadRequest("Invalid argument: count has to be > 0");

            List<ClassBasicDataDto> topClasses;
            try
            {
                topClasses = _classService.GetTopClasses(count);
            }
            catch(Exception e)
            {
                if (e is ArgumentException)
                    return BadRequest(e.Message);
                else
                    return NotFound();
            }
            if (topClasses == null)
                return NotFound();
            return Ok(topClasses);
        }

        [Authorize]
        [Route("api/classes/suggested"), HttpGet]
        public IHttpActionResult GetSuggested()
        {
            List<ClassBasicDataDto> suggestedClasses;
            try
            {
                suggestedClasses = _classService.GetSuggestedClasses(CurrentUserId(), 1);
            }
            catch(Exception e)
            {
                if (e is ArgumentException)
                    return BadRequest(e.Message);
                else
                    return NotFound();
            }
            return Ok(suggestedClasses);
        }

        [Authorize]
        [Route("api/classes/{id:int}"), HttpPost]
        public IHttpActionResult SignFor(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid class id");

            var error = _studentService.SignForClass(CurrentUserId(), id);
            if (error != null)
                return BadRequest(error);

            return Ok();
        }

        [Authorize]
        [Route("api/classes/{id:int}"), HttpDelete]
        public IHttpActionResult DeleteSubscription(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid class id");

            var error = _studentService.UnsubscribeFromClass(CurrentUserId(), id);
            if (error != null)
                return BadRequest(error);

            return Ok();
        }

        [Route("api/classes/search"), HttpPost]
        public IHttpActionResult Search(ClassesSearchingModel searchingModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ClassFilter filter = new ClassFilter()
            {
                ClassName = searchingModel.ClassName,
                LanguageId = searchingModel.LanguageId,
                LanguageLevelId = searchingModel.LanguageLevelId,
                PageNumber = searchingModel.PageNumber,
                PageSize = searchingModel.PageSize
            };

            (var classes, int pageCount) = _classService.Search(filter);
            if (classes != null)
                return Ok(classes);
            else
                return NotFound();
        }

        [Authorize]
        [Route("api/classes/{id:int}/students"), HttpGet]
        public IHttpActionResult GetStudents(int id)
        {
            List<StudentBasicDataDto> students;
            try
            {
                students = _classService.GetStudents(id, CurrentUserId());
            }
            catch (Exception e)
            {
                return NotFound();
            }

            if (students == null)
                return BadRequest("You have to be signed for class to show students list");
            return Ok(students);
        }
    }
}
