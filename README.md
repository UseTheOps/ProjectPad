# ProjectPad


## How to build

*Secrets are not included* so you'll have to create your own Azure-AD backed application.

You must create an application with a dekstop/mobile authorization scheme and add https://login.microsoftonline.com/common/oauth2/nativeclient as a valid redirect uri.

Required scopes are (in delegated access) :

* User.Read
* email
* Files.ReadWrite.AppFolder
* offline_access
* Azure Devops : user_impersonation

You then have to create a secrets.json file in the `ProjectPadUWP` project :

```json
{
  "MSALSettings": {
    "clientId": "... your app's client id ..."
  }
}

```
