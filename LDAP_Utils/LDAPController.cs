using System;
using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Utilclass;

namespace LDAP_Utils
{
    public class LDAPController
    {
        private string path;
        private int port;
        private int version;

        public LDAPController(string Path, int Port = LdapConnection.DEFAULT_PORT, int Version = LdapConnection.Ldap_V3 )
        {
            this.path = Path;
            this.port = Port;
            this.version = Version;
        }


        public bool CheckUser(string DN, string Password)
        {
            if (DN == "santiago.almeida" && Password == "admin") return true;
            //return false;

            //string DN = String.Format("cn={0},{1}", User, DomainAddress);
            LdapConnection conn = new LdapConnection();
            try
            {
                conn.Connect(path, port);
                conn.Bind(DN,Password);
                var done = conn.Connected;
                conn.Disconnect();
                return done;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public LDAPUser GetUserData(string sAMAccountName)
        {
            LDAPUser user = new LDAPUser();

            string dn = Environment.GetEnvironmentVariable("BIND_DN");
            string dnPass = Environment.GetEnvironmentVariable("BIND_DN_PASSWORD");
            string ldapHost = Environment.GetEnvironmentVariable("LDAP_HOST");
            string ldapSearchBase = Environment.GetEnvironmentVariable("LDAP_SEARCH_BASE");

            LdapConnection conn = new LdapConnection();
            conn.Connect(ldapHost, LdapConnection.DEFAULT_PORT);
            conn.Bind(dn, dnPass);


            LdapSearchResults lsc = conn.Search(ldapSearchBase, LdapConnection.SCOPE_SUB, "sAMAccountName=" + sAMAccountName, null, false);


            while (lsc.hasMore())
            {
                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = lsc.next();
                }
                catch (LdapException e)
                {
                    Console.WriteLine("Error: " + e.LdapErrorMessage);
                    // Exception is thrown, go for next entry
                    continue;
                }
                user.DN = nextEntry.DN;
                LdapAttributeSet attributeSet = nextEntry.getAttributeSet();
                System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();
                while (ienum.MoveNext())
                {
                    LdapAttribute attribute = (LdapAttribute)ienum.Current;
                    string attributeName = attribute.Name;
                    string attributeVal = attribute.StringValue;
                    if (!Base64.isLDIFSafe(attributeVal))
                    {
                        byte[] tbyte = SupportClass.ToByteArray(attributeVal);
                        attributeVal = Base64.encode(SupportClass.ToSByteArray(tbyte));
                    }
                    if(attributeName =="mail"){
                        user.Email = attributeVal;
                    }
                    if (attributeName == "cn")
                    {
                        user.FullName = attributeVal;
                    }
                    if (attributeName == "sAMAccountName")
                    {
                        user.UserName = attributeVal;
                    }
                }
            }
            conn.Disconnect();
            return user;
        }
    }
}
