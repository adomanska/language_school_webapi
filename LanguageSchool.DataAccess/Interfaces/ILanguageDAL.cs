﻿using LanguageSchool.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageSchool.DataAccess
{
    public interface ILanguageDAL
    {
        List<Language> GetAll();
        Language GetById(int Id);
    }
}
