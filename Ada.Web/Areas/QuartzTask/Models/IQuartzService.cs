using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ada.Core;
using Ada.Core.Domain.QuartzTask;

namespace QuartzTask.Models
{
   public interface IQuartzService:ISingleDependency
   {
       void Start(Job entity);
       void Pause(Job entity);
       void Resume(Job entity);
       void Stop(Job entity);
   }
}
