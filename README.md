# qtools - .net Data Modelling tools for Q
[![GitHub license](https://img.shields.io/badge/license-LGPL%20v3-blue.svg)](https://github.com/machonky/qtools/blob/master/LICENSE)
[![Nuget](https://img.shields.io/nuget/v/QTools.svg)](https://www.nuget.org/packages/qtools)
[![NuGet](https://img.shields.io/nuget/dt/QTools.svg)](https://www.nuget.org/packages/qtools)
[![GitHub issues](https://img.shields.io/github/issues/machonky/Qtools.svg)](https://github.com/machonky/QTools/issues)
[![GitHub forks](https://img.shields.io/github/forks/machonky/QTools.svg?style=social&label=Fork)](https://github.com/machonky/QTools/network)
[![GitHub stars](https://img.shields.io/github/stars/machonky/QTools.svg?style=social&label=Star)](https://github.com/machonky/QTools/stargazers)

## Introduction
QTools permits a developer to annotate a C# data model to help automatically define table schema for use in the kdb+ database.

## Getting Started
Install the [NuGet package](https://www.nuget.org/packages/qtools).

> ### DISCLAIMER
> **IMPORTANT:** The current state of this toolkit is **PRE-ALPHA/Development**. Please consider it version a foundational version. Many areas could be improved and change significantly while refactoring current code and implementing new features. 

## Basic Example

We'll build a basic security model to restrict access to specific resources in our system.

First we'll create some classes to represent the authentication aspect of our security model.

```cs
namespace Auth
{
    public class User
    {
        [Key]
        public string Id { get; set; }
        [Unique]
        public string Login { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class LoginInfo
    {
        [Key]
        public string Id { get; set; }
        [Unique, ForeignKey(typeof(User))]
        public string User { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
    }
}
```

Lets add some classes to support implementation of our Access Control List

```cs
namespace Acl
{
    public class Principal
    {
        [Key]
        public string Id { get; set; }
        [Unique]
        public string Name { get; set; }
        public string Explanation { get; set; }
    }

    public class UserPrincipal
    {
        [ForeignKey(typeof(Auth.User))]
        public string User { get; set; }
        [ForeignKey(typeof(Principal))]
        public string Principal { get; set; }
    }
    
    public class Operation
    {
        [Key]
        public string Id { get; set; }
        [Unique]
        public string Name { get; set; }
        public string Explanation { get; set; }
    }

    public class Resource
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ResourceGrantAcl
    {
        [ForeignKey(typeof(Resource))]
        public string Resource { get; set; }
        [ForeignKey(typeof(Principal))]
        public string Principal { get; set; }
        [ForeignKey(typeof(Operation))]
        public string Operation { get; set; }
    }

    public class ResourceDenyAcl
    {
        [ForeignKey(typeof(Resource))]
        public string Resource { get; set; }
        [ForeignKey(typeof(Principal))]
        public string Principal { get; set; }
        [ForeignKey(typeof(Operation))]
        public string Operation { get; set; }
    }
}
```

Next we define a collection of the types involved in the entire schema as shown:
```cs
var types = new[] 
{
	typeof(Auth.User),
	typeof(Auth.LoginInfo),
	typeof(Acl.Principal),
	typeof(Acl.UserPrincipal),
	typeof(Acl.Operation),
	typeof(Acl.Resource),
	typeof(Acl.ResourceGrantAcl),
	typeof(Acl.ResourceDenyAcl),
};
```

After this we can supply the collection above to the schema builder which will generate the entire script for an empty kdb+ database including foreign key relationships.

```cs
var fullAclSchema = SchemaBuilder.DeclareEmptySchema(types);
```

This will yield a script in 'q' syntax for the empty database as shown:
```
.auth.user:([id:`symbol$()]`u#login:`symbol$();name:`symbol$();description:`symbol$())
.acl.principal:([id:`symbol$()]`u#name:`symbol$();explanation:`symbol$())
.acl.operation:([id:`symbol$()]`u#name:`symbol$();explanation:`symbol$())
.acl.resource:([id:`symbol$()]name:`symbol$();description:`symbol$())
.auth.loginInfo:([id:`symbol$()]`u#user:`symbol$();hash:`symbol$();salt:`symbol$())
.acl.userPrincipal:([]user:`symbol$();principal:`symbol$())
.acl.resourceGrantAcl:([]resource:`symbol$();principal:`symbol$();operation:`symbol$())
.acl.resourceDenyAcl:([]resource:`symbol$();principal:`symbol$();operation:`symbol$())
update user:`.auth.user$() from `.auth.loginInfo
update user:`.auth.user$() from `.acl.userPrincipal
update principal:`.acl.principal$() from `.acl.userPrincipal
update resource:`.acl.resource$() from `.acl.resourceGrantAcl
update principal:`.acl.principal$() from `.acl.resourceGrantAcl
update operation:`.acl.operation$() from `.acl.resourceGrantAcl
update resource:`.acl.resource$() from `.acl.resourceDenyAcl
update principal:`.acl.principal$() from `.acl.resourceDenyAcl
update operation:`.acl.operation$() from `.acl.resourceDenyAcl
```

Using the 'q' ```meta``` function will verify all the types and relationships.

## Notes

To populate the tables easily from .net objects take a look at [Qapper](https://github.com/machonky/Qapper)
