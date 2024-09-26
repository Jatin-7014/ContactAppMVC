using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContactAppMVC.Models
{
    public class Contact
    {
        public virtual int Id { get; set; }
        public virtual string FName {  get; set; }
        public virtual string LName { get; set; }
        public virtual bool IsActive { get; set; } = true;
        public virtual User User { get; set; } = new User();
        public virtual IList<ContactDetail> ContactDetails { get; set; }=new List<ContactDetail>();

    }
}