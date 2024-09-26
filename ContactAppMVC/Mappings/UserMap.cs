using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContactAppMVC.Models;
using FluentNHibernate.Mapping;

namespace ContactAppMVC.Mappings
{
    public class UserMap:ClassMap<User>
    {
        public UserMap()
        {
            Table("Users");
            Id(u => u.Id).GeneratedBy.GuidComb();
            Map(u => u.Password);
            Map(u=>u.FName);
            Map(u=>u.LName);
            Map(u=>u.IsActive);
            Map(u=>u.IsAdmin);
            HasMany(u=>u.Contacts).Inverse().Cascade.All();
            HasOne(u=>u.Role).PropertyRef(u=>u.User).Cascade.All().Constrained();

        }
    }
}