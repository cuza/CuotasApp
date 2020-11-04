using System;
namespace LDAP_Utils
{
    public class LDAPUser
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string DN { get; set; }
        public LDAPUser()
        {
            
        }
    }
}
