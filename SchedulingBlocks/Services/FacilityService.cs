using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using SchedulingBlocks.ContactsApi;
using SchedulingBlocks.Email;
using SchedulingBlocks.Models;
using SchedulingBlocks.Models.AppDb;
using Newtonsoft.Json;
using SchedulingBlocks.Interfaces;

namespace SchedulingBlocks.Services
{
    public class FacilityService
    {
        //Dependecy Injection
        private IAppDbRepository _appdb;

        public FacilityService()
        {
            _appdb = MvcApplication.Container.GetInstance<IAppDbRepository>();
        }

        public IEnumerable<Facility> GetAllFacilities()
        {
            return _appdb.GetAllFacilities();
        }
    }
}