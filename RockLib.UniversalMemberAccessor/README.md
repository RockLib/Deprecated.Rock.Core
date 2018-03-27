# RockLib.UniversalMemeberAccessor

*Defines mechanisms that enable easy access to non-public members of a type. Very dangerous - use with caution.*

```powershell
PM> Install-Package RockLib.UniversalMemeberAccessor
```

## Overview

Sometimes, you need to access the non-public members of a class. Maybe you needed to test some complex logic of class's private method. Or perhaps you need to access an internal class from a library that you don't own. Or change the value of a readonly field. None of these things are safe to do. But all of them have been possible in .NET for a very, very long time. But that reflection API is cumbersome. And slow.

`RockLib.UniversalMemberAccessor` makes these kinds of unsafe activities fast and effortless.

### `Unlock` Extension Method

When adding `RockLib.Dynamic` to a file's using directives, the `Unlock` extension method becomes available for any object. Its return type is `dynamic`. This "proxy object" allows you to access any member of the object by its name, as if it were public (there is no intellisense of course, because it's dynamic).

Given this simple class with a private field and a private method:

```c#
public class Foo
{
    private readonly int _bar = -1;    
    private void PrintBar() => Console.WriteLine(_bar);
}
```

What if we wanted to change the value of the readonly field and then call the private method? No problem:

```c#
Foo f = new Foo();
dynamic foo = f.Unlock();
foo._bar = 123;
foo.PrintBar(); // prints "123"
```

### `New` Extension Method

If you have a `Type` and you need to create an instance of it with in inaccessible constructor, use the `New` extension method. Pass the extension method the constructor arguments that the constructor needs, and it will return the same kind of dynamic object that the `Unlock` extension method returns.

```c#
public class Foo
{
    private Foo(int bar, string baz) => Console.WriteLine($"bar: {bar}, baz: {baz}");
}

Foo foo = typeof(Foo).New(123, "abc"); // prints "bar: 123, baz: abc"
```

### Static Access

You can access any static member through an instance of the target type.

```c#
public class Foo
{
    private static void Bar(int baz) => Console.WriteLine(baz);
}

Foo foo = new Foo().Unlock();
foo.Bar(123); // prints "123"
```

*TODO: talk about the `GetStatic` extension methods.*

### Backing Object

You can get the actual backing object using several mechanisms. *Note that for pseudo-properties, if a real property or field exists by the same name, then the real property or field value is returned.*

```c#
dynamic foo = new Foo().Unlock();

Foo f1 = (Foo)foo; // Can cast to a type.
Foo f2 = foo; // Can implicitly convert to a type.
Foo f3 = foo.Instance; // Can access the 'Instance' pseudo-property.
Foo f4 = foo.Object; // Can access the 'Object' pseudo-property.
Foo f5 = foo.Value; // Can access the 'Value' pseudo-property.
```
