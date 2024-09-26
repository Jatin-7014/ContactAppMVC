using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContactAppMVC.Models;
using FluentNHibernate.Mapping;

namespace ContactAppMVC.Mappings
{
    public class RoleMap:ClassMap<Role>
    {
        public RoleMap()
        {
            Table("Roles");
            Id(r => r.Id).GeneratedBy.Identity();
            Map(r => r.RoleName);
            References(r=>r.User).Column("UserId").Unique().Cascade.None();
        }
    }
}