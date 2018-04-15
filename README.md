# qtools - .net Data Modelling tools for Q
[![GitHub license](https://img.shields.io/badge/license-LGPL%20v3-blue.svg)](https://github.com/machonky/qtools/blob/master/LICENSE)
[![Nuget](https://img.shields.io/nuget/v/QTools.svg)](https://www.nuget.org/packages/qtools)
[![NuGet](https://img.shields.io/nuget/dt/QTools.svg)](https://www.nuget.org/packages/qtools)
[![GitHub issues](https://img.shields.io/github/issues/machonky/Qtools.svg)](https://github.com/machonky/QTools/issues)
[![GitHub forks](https://img.shields.io/github/forks/machonky/QTools.svg?style=social&label=Fork)](https://github.com/machonky/QTools/network)
[![GitHub stars](https://img.shields.io/github/stars/machonky/QTools.svg?style=social&label=Star)](https://github.com/machonky/QTools/stargazers)

> ### DISCLAIMER
> **IMPORTANT:** The current state of this toolkit is **PRE-ALPHA/Development**. Please consider it version a foundational version. Many areas could be improved and change significantly while refactoring current code and implementing new features. 

## Introduction
QTools permits a developer to annotate a C# data model to help automatically define table schema for use in the kdb+ database. 

A developer can rapidly develop and prototype an entire schema declaratively from the .net environment. Without QTools a developer must develop the schema twice - once in the .net environment and again in the kdb+ 'q' language.

## Getting Started
Install the [NuGet package](https://www.nuget.org/packages/qtools).

## Basic Example

We'll build a basic security model to restrict access to specific resources in our system.

Next we'll create some classes to represent the authentication aspect of our security model. We'll be using some attributes declared in the ```QTools.Schema``` namespace to declare metadata of our property to our schema builder - as shown:

```cs
using QTools.Schema;
```

Note the attributes applied to properties of classes declared below.

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
	
        // We'll store the various roles when we deserialize
        [Ignore] public string[] PrincipalIds { get; set; }	
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

Lets add some classes to support implementation of our Access Control List.

```cs
namespace Acl
{
    public class Principal
    {
        [Key]
        public string Id { get; set; }
        [Unique]
        public string Name { get; set; }
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
    }

    public class Resource
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ResourceAcl
    {
        [ForeignKey(typeof(Resource))]
        public string Resource { get; set; }
        [ForeignKey(typeof(Principal))]
        public string Principal { get; set; }
        [ForeignKey(typeof(Operation))]
        public string Operation { get; set; }
    }

    public class GrantResourceAcl : ResourceAcl
    { }

    public class DenyResourceAcl : ResourceAcl
    { }
}
```

Next we define a collection of the types involved in the entire schema as shown:
```cs
// input types can be added in any order. They are sorted by dependency later
var types = new[] 
{
	typeof(Auth.User),
	typeof(Auth.LoginInfo),
	typeof(Acl.Principal),
	typeof(Acl.UserPrincipal),
	typeof(Acl.Operation),
	typeof(Acl.Resource),
	typeof(Acl.GrantResourceAcl),
	typeof(Acl.DenyResourceAcl),
};
```

After this we can supply the collection above to the schema builder which will generate the entire script for an empty kdb+ database including foreign key relationships.

```cs
string fullAclSchema = SchemaBuilder.DeclareEmptySchema(types);
```

The variable ```fullAclSchema``` will contain a script in 'q' syntax for the empty database as shown:

```
.auth.user:([id:`symbol$()]`u#login:`symbol$();name:`symbol$();description:`symbol$())
.acl.principal:([id:`symbol$()]`u#name:`symbol$())
.acl.operation:([id:`symbol$()]`u#name:`symbol$())
.acl.resource:([id:`symbol$()]name:`symbol$())
.auth.loginInfo:([id:`symbol$()]`u#user:`symbol$();hash:`symbol$();salt:`symbol$())
.acl.userPrincipal:([]user:`symbol$();principal:`symbol$())
.acl.grantResourceAcl:([]resource:`symbol$();principal:`symbol$();operation:`symbol$())
.acl.denyResourceAcl:([]resource:`symbol$();principal:`symbol$();operation:`symbol$())
update user:`.auth.user$() from `.auth.loginInfo
update user:`.auth.user$() from `.acl.userPrincipal
update principal:`.acl.principal$() from `.acl.userPrincipal
update resource:`.acl.resource$() from `.acl.grantResourceAcl
update principal:`.acl.principal$() from `.acl.grantResourceAcl
update operation:`.acl.operation$() from `.acl.grantResourceAcl
update resource:`.acl.resource$() from `.acl.denyResourceAcl
update principal:`.acl.principal$() from `.acl.denyResourceAcl
update operation:`.acl.operation$() from `.acl.denyResourceAcl
```

If the script above is copied and pasted at the 'q' prompt, an empty database will be fully defined.

## Notes
- Namespaces are preserved in lowercase. 
- Table names correspond to class names with a leading lowercase letter.
- In the script above, the ```update``` statements establish the foreign key relationships.
- Tables are declared in dependency order.
- Only simple foreign keys are supported

Using the 'q' ```meta``` function will verify all the types and relationships.

## Next Steps

To populate and retreive data from the tables from .net objects take a look at [Qapper](https://github.com/machonky/Qapper)
