This repository contains a basic B2B Blazor Web application integrated with Auth0 Organizations.

Read the article [Enable Self-Subscribing Model in Your Blazor B2B SaaS Application](https://auth0.com/blog/enable-self-subscription-in-blazor-b2b-saas-application) to learn more about it.

# Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) installed on your machine

# To run this application

1. Clone the repo with the following command:

   ```bash
   git clone --branch organization-via-code --single-branch https://github.com/andychiare/MyBlazorSaaS 
   ```

2. Move to the `MyBlazorSaaS\MyBlazorSaaS` folder.

3. [Register the web app with Auth0](https://auth0.com/docs/get-started/auth0-overview/create-applications/regular-web-apps) and add your Auth0 domain and client ID to the `appsettings.json` configuration file.

4. Also [register an M2M application](https://auth0.com/docs/get-started/auth0-overview/create-applications/machine-to-machine-apps) with access to the Management API and assign its client ID and client secret to the `ManagementClientId` and `ManagementClientSecret` keys in `appsettings.json`.

5. Assign the default connection ID you want to use to the `DefaultConnectionId` key in `appsettings.json` (see [here](https://auth0.com/blog/enable-self-subscription-in-blazor-b2b-saas-application/#Enable-the-Onboarding-Process) for more info).

6. Make sure you have [enabled Organization support for the application in your Auth0 tenant](https://auth0.com/docs/manage-users/organizations/login-flows-for-organizations#configure-your-application-to-use-organizations).

7. Type `dotnet run` in a terminal window to launch the application.

8. Point your browser to the [https://localhost:7187](https://localhost:7187) address. You should see a web page like the following:

![Welcome to MyBlazorSaaS](welcome-my-blazor-saas.png)