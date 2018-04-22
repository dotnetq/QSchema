# QSchema - .net Data Modelling tools for Q
[![GitHub license](https://img.shields.io/badge/license-LGPL%20v3-blue.svg)](https://github.com/dotnetq/qschema/blob/master/LICENSE)
[![Nuget](https://img.shields.io/nuget/v/QSchema.SchemaBuilder.svg)](https://www.nuget.org/packages/QSchema.SchemaBuilder)
[![NuGet](https://img.shields.io/nuget/dt/QSchema.SchemaBuilder.svg)](https://www.nuget.org/packages/QSchema.SchemaBuilder)
[![GitHub issues](https://img.shields.io/github/issues/dotnetq/qschema.svg)](https://github.com/dotnetq/qschema/issues)
[![GitHub forks](https://img.shields.io/github/forks/dotnetq/qschema.svg?style=social&label=Fork)](https://github.com/dotnetq/qschema/network)
[![GitHub stars](https://img.shields.io/github/stars/dotnetq/qschema.svg?style=social&label=Star)](https://github.com/dotnetq/qschema/stargazers)

> ### DISCLAIMER
> **IMPORTANT:** The current state of this toolkit is **PRE-ALPHA/Development**. Please consider it version a foundational version. Many areas could be improved and change significantly while refactoring current code and implementing new features. 

## Introduction
qschema permits a developer to annotate a C# data model to help automatically define table schema for use in the kdb+ database. 

A developer can rapidly develop and prototype an entire schema declaratively from the .net environment. Without qschema a developer must develop the schema twice - once in the .net environment and again in the kdb+ 'q' language.

## Getting Started
Install the [NuGet package](https://www.nuget.org/packages/QSchema.SchemaBuilder).

## Basic Example

We'll build a basic security model to restrict access to specific resources in our system.

Next we'll create some classes to represent the authentication aspect of our security model. We'll be using some attributes declared in the ```DotnetQ.QSchema.Attributes``` namespace to declare metadata of our property to our schema builder - as shown:

```cs
using DotnetQ.QSchema.Attributes;
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
using DotnetQ.QSchema.SchemaBuilder;
...
string fullAclSchema = SchemaBuilder.DeclareEmptySchema(types);
```

The variable ```fullAclSchema``` will contain a script in 'q' syntax for the empty database as shown:

```
.auth.user:([id:`symbol$()]`u#login:`symbol$();name:`symbol$();description:`symbol$())
.acl.principal:([id:`symbol$()]`u#name:`symbol$())
.acl.operation:([id:`symbol$()]`u#name:`symbol$())
.acl.resource:([id:`symbol$()]name:`symbol$())
.auth.loginInfo:([id:`symbol$()]user:`.auth.user$();hash:`symbol$();salt:`symbol$())
.acl.userPrincipal:([]user:`.auth.user$();principal:`.acl.principal$())
.acl.grantResourceAcl:([]resource:`.acl.resource$();principal:`.acl.principal$();operation:`.acl.operation$())
.acl.denyResourceAcl:([]resource:`.acl.resource$();principal:`.acl.principal$();operation:`.acl.operation$())
```

If the script above is input into a kdb+ session, an empty database will be fully defined, including foreign keys.

## Notes
- Namespaces are preserved in lowercase. 
- Table names correspond to class names with a leading lowercase letter.
- In the script above, the ```update``` statements establish the foreign key relationships.
- Tables are declared in dependency order. As long as table dependencies are directed and acyclic, a schema can be calculated.
- Only simple foreign keys are supported

Using the 'q' ```meta``` function will verify all the types and relationships.

## Next Steps

To populate and retreive data from the tables from .net objects take a look at [Qapper](https://github.com/dotnetq/Qapper)
