# da.hub

## adfs setup - authentication & authorization working, problem with redirection
* Open AD FS Management
* create new application group
* native application - https://localhost:xxxx
   * copy client id in appsettings.json
* web application -
   * Identifier tab: replying 3rd party is client-id
   * notes: blank
   * access control policy: select required policy. allow all or mfa everytime etc
   * Issuance Transform Rules: 
      * create Rule
         * send Ldap attributes as claim
         * Attribute store: Active Directory
         * SamAccountName - outgoing name
         * Token-Group unqualified name -  Role
   * Client Permissions: select openid
# App 
## login
* display indivdual user details
* get all direct reports given manager
* deactivate users without activity in 2 weeks
* add users to group depending on job title
* logon script by job title
* OU by job title
* which user has logged into which computer. last 365 days??

