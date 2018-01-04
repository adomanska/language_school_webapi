﻿using LanguageSchool.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageSchool.Shared.Dtos;

namespace LanguageSchool.BusinessLogic
{
    public interface IClassBLL
    {
        List<ClassDataDto> GetAll();
        ClassDataDto GetByID(int ID);
        List<ClassBasicDataDto> GetClasses(string language, string level);
        (List<ClassBasicDataDto> classes, int pageCount) Search(ClassFilter filter);
        List<ClassBasicDataDto> GetTopClasses(int count);
        List<ClassBasicDataDto> GetSuggestedClasses(string studentID, int count);
    }
}
