using Common.Models.Database.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessment.Common.Configurations
{
    public class MappingProfiles : AutoMapper.Profile
    {
        public MappingProfiles()
        {

            CreateMap<Models.Response.AuthenticateResponse, User>();
            CreateMap<Models.Request.SignUpRequest, User>();

        }
    }
}
