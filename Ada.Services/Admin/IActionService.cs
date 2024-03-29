﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;

namespace Ada.Services.Admin
{
   public interface IActionService:IDependency
   {
       void Add(Core.Domain.Admin.Action action, bool isCurd = false);
       void Update(Core.Domain.Admin.Action action);
       void Delete(Core.Domain.Admin.Action action);
   }
}
