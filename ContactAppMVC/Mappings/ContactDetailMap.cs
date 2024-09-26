using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ContactAppMVC.Models;
using FluentNHibernate.Mapping;

namespace ContactAppMVC.Mappings
{
    public class ContactDetailMap:ClassMap<ContactDetail>
    {
        public ContactDetailMap()
        {
            Table("ContactDetails");
            Id(cd=>cd.Id).GeneratedBy.Identity();
            Map(cd => cd.Type);
            Map(cd=>cd.Value);
            References(c=>c.Contact).Column("ContactId").Cascade.None().Nullable();
        }
    }
}