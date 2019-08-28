# Overview

This project is a basic template that provides the following capabilities out of the box:
* Authentication
* Database Setup
* API configuration

## Authentication

The template is configured for two types of authentication.

1. Cookie-Based Authentication (for standard MVC websites) - Relies on Session
2. Basic Authentication (for APIs) - Relies on `Authorization` header.

Both configurations are set up with a users table in the database. See the database section for more information.

### 1. Cookie-Based Authentication

Cookie-Based Authentication is used if you are developing a standard MVC website. An `IAuthProvider` is provided in 
`WebApplication.Web.Providers.Auth` and supports the following methods that may be used from parts of your application:

- `bool IsLoggedIn { get; }`  - Indicates if the user is currently logged in.
- `User GetCurrentUser()` - Returns the current user.
- `bool SignIn(string username, string password)` - Attempts to authenticate a user.
- `void LogOff()` - Logs the user out of the system.
- `ChangePassword(string existingPassword, string newPassword)` - Changes the user's password.
- `void Register(string username, string password, string role)` - Register's a new user.
- `bool UserHasRole(string[] roles)` - Confirms if a user has one of the roles provided.

An implementation has already been made for this interface: `SessionAuthProvider`. Should you wish to use it, keep the following
dependencies mapped in your `Startup.cs` file:

```
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddScoped<IAuthProvider, SessionAuthProvider>();
services.AddTransient<IUserDAL>(m => new UserSqlDAL(@"Data Source=.\SQLEXPRESS;Initial Catalog=DemoDB;Integrated Security=True"));
```

**An `AccountController` is provided enabling all of the above functions leaving the implementation of the View up to you.**

#### Limiting Actions to Authenticated Users

If you have any actions in your MVC application that need to be restricted to authenticated users (or users with proper permission),
you can use the following attribute above your controller actions:

* `[AuthorizationFilter]` - Restricts usage to users that are logged in.
* `[AuthorizationFilter("Admin")]` - Restricts usage to users that only have the *Admin* role.
* `[AuthorizationFilter("Role 1", "Role 2", ...)]` - Restricts usage to users that only have the *Role 1* role OR the *Role 2* role, etc.


### 2. Basic Authentication

For applications developing REST-based APIs for support with a front-end, the Cookie-Based authentication approach will not work.
Instead every request that is sent to the API must provide an `Authorization` header with the credentials. This form is called 
*Basic Authentication*. Each request will effectively authenticate the user ensuring that they are allowed access to the data.

#### Server-Side Usage

To restrict actions within your API to require this level of authentication, add the following attribute above your actions:

* `[BasicAuthorization]` - Restricts API usage to authenticated users.
* `[BasicAuthorization("Admin")]` - Restricts API usage to users that only have the *Admin* role.
* `[BasicAuthorization("Role 1", "Role 2", ...)]` - Restricts API usage to users that only have the *Role 1* role OR the *Role 2* role, etc.

The only dependency configuration that you need to keep in your `Startup.cs` file is:

```
services.AddTransient<IUserDAL>(m => new UserSqlDAL(@"Data Source=.\SQLEXPRESS;Initial Catalog=DemoDB;Integrated Security=True"));
```

**An `AuthenticationController` is provided supporting user registration and retrieving the user.**

#### Client-Side USage

To make a `fetch` call that requires the `Authorization` header, your fetch request will need to be modified to include the user's credentials.

Example Request Getting the Current User
```javascript
// base-64 encode the username:password
const header = btoa(`${username}:${password}`);
fetch('https://localhost:44391/api/authentication/user', {
    method: 'GET',            
    headers: {
        'Content-Type': 'application/json',               
        'Authorization': 'Basic ' + header
    }
})
.then(response => {
	// Check to see if the request authenticated.
    if (response.status >= 400 && response.status < 500) {        
        return {message: "Error authenticating"};
    }
    return response.json();
})
.then(json => { 
	// Log the output
    console.log(json);                
});
```

The call to create a new user does not require authentication. It supports anonymous requests. 

Example Request Creating a User
```javascript
fetch('https://localhost:44391/api/authentication/register', {
    method: 'POST',
    body: JSON.stringify({
        username: username,
        password: password
    }),
    headers: {
        'Content-Type': 'application/json',               
    }
})
.then(response => {
    if (response.status === 200) {                
		// User was created
        return {message: 'authenticated'};
    }
    else { 
		// Something went wrong (user exists, params missing)
        return response.json();
    }
})
.then(json => { 
	// Log the output
    console.log(json);                
})
```

### Removing Authentication Code

**NOTE**: If you do not require authentication in your application you should remove the following files:

* `Providers/Auth`
* `Models/Account`, `Models/Authentication`, and `Models/User.cs`
* `DAL/IUserDAL.cs` and `DAL/UserSqlDAL.cs`
* `Controllers/AccountController.cs` and `Controllers/AuthenticationController.cs`
* `ViewComponents/NavBarViewComponent.cs`
* `Shared/Components/NavBar`
* `Tests/Providers`
* `Tests/DAL/UserSqlDALTests.cs`

Also be sure to remove the dependencies defined in `Startup.cs` and the reference to `NavBarComponent` in the `_Layout.cshtml` file.

-----

## Database

A sql script (`schema.sql`) is provided that can be used to create your database. Define the structure of your database here.

* **NOTE**: If you need additional data beyond the existing user table, you may add to it but know that you will also need to update all references in the code accordingly where the `User` object is leveraged.
* **NOTE 2**: If you do not need user data, be sure to remove references to it from `schema.sql`


Another sql script (`data.sql`) is provided for you to use if you need to reset the data or stage data in your database.

-----

## API Configuration

Lastly the project has been configured to support a REST API. The following was added:

1. CORS-Enabled to allow cross-origin requests. Requests are allowed from a pre-defined set of addresses (:5500 and :8080) but can be modified if necessary.
2. Swagger support added to generate your REST API documentation. **Be sure to comment your classes and methods.**