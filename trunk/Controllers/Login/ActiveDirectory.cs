/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System;
using System.DirectoryServices;
using System.Collections.Generic;

namespace LoginController
{
    public class ActiveDirectory
    {
        private readonly string _domainName;

        public ActiveDirectory(string domainName)
        {
            _domainName = domainName;
        }

        private DirectoryEntry CreateDirectoryEntry(string userName, string pwd)
        {
            //Create an entry to Active Directory
            DirectoryEntry dirEntry = new DirectoryEntry
            {
                Path = "LDAP://" + _domainName,
                Username = userName,
                Password = pwd
            };

            return dirEntry;
        }

        private DirectoryEntry CreateDirectoryEntry()
        {
            //Create an entry to Active Directory
            DirectoryEntry dirEntry = new DirectoryEntry {Path = "LDAP://" + _domainName};

            return dirEntry;
        }

        private static List<string> GetMemberGroupsFromSearchResult(SearchResult searchResult)
        {
            List<string> memberGroups = new List<string>();

            // loop through groups
            foreach (object properties in searchResult.Properties["memberOf"])
            {
                string property = properties.ToString();
                // The following is used to remove the cn= etc...
                int equalsIndex = property.IndexOf("=", 1);
                int commaIndex = property.IndexOf(",", 1);
                if (equalsIndex == -1)
                {
                    return null;
                }

                if (equalsIndex > -1)
                {
                    //New user group name found in Active Directory: " + userGroupName
                    //Get group name
                    string userGroupName = property.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1);
                    memberGroups.Add(userGroupName);
                }
            }
            if (memberGroups.Count == 0)
            {
                //No user group names found in Active Directory. 
                //The client may not have permission to read member groups for the specified user.
            }
            return memberGroups;
        }


        public bool Authenticate(string userName, string pwd)
        {
            //Get an entry to Active Directory
            using (DirectoryEntry dirEntry = CreateDirectoryEntry(userName, pwd))
            {
                //Instansiate a new Active Directory searcher, set filter and properties
                using (DirectorySearcher dirSearcher = new DirectorySearcher(dirEntry))
                {
                    //Set search filter
                    dirSearcher.Filter = "(sAMAccountName=" + userName + ")";

                    SearchResult searchResult;
                    try
                    {
                        searchResult = dirSearcher.FindOne();
                    }
                    catch (Exception err)
                    {
                        //The domain is not available or the client do not have permission to do the search.
                        //Check userName and/or passWord.
                        return false;
                    }

                    if (searchResult != null)
                    {
                        //User exist in Active Directory.
                        return true;
                    }
                    //User does not exist in Active Directory.
                    return false;
                }
            }
        }

        public List<string> GetMemberGroups(string userName)
        {
            //Get an entry to Active Directory
            using (DirectoryEntry dirEntry = CreateDirectoryEntry())
            {
                //Instansiate a new Active Directory searcher, set filter and properties
                using (DirectorySearcher dirSearcher = new DirectorySearcher(dirEntry))
                {
                    dirSearcher.Filter = string.Format("(sAMAccountName=" + userName + ")");
                    dirSearcher.PropertiesToLoad.Add("memberOf");

                    SearchResult searchResult = dirSearcher.FindOne();

                    return searchResult == null ? 
                        null : 
                        GetMemberGroupsFromSearchResult(searchResult);
                }
            }
        }
    }
}