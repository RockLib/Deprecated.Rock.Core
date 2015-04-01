<Query Kind="Program">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.DependencyInjection</Namespace>
</Query>

void Main()
{
    // A feature of Dependency Injection in Rock Framework is the ability to
    // "Merge" two instances of the IResolver interface.
    //
    // It is useful when you have a container that has "almost all" of the dependencies
    // it needs, and you also have access to a "supplimentary" container.

    // The primary container lacks the IBaz dependency.
    AutoContainer primaryContainer = new AutoContainer(new Bar());
    primaryContainer.SetBinding(typeof(IQux), typeof(Qux));
    
    // The secondary conatiner has the IBaz dependency.
    AutoContainer secondaryContainer = new AutoContainer(new Baz());
    
    // The merged container will use the secondary container when the primary container
    // cannot resolve the IBaz dependency.
    AutoContainer mergedContainer = primaryContainer.MergeWith(secondaryContainer);
    
    mergedContainer.Get<Foo>().Dump();
}

public interface IFoo
{
    IBar Bar { get; }
    IBaz Baz { get; }
    IQux Qux { get; }
}

public class Foo : IFoo
{
    private readonly IBar _bar;
    private readonly IBaz _baz;
    private readonly IQux _qux;
    
    public Foo(IBar bar, IBaz baz, IQux qux)
    {
        _bar = bar;
        _baz = baz;
        _qux = qux;
    }
    
    public IBar Bar { get { return _bar; } }
    public IBaz Baz { get { return _baz; } }
    public IQux Qux { get { return _qux; } }
}

public interface IBar {}
public interface IBaz {}
public interface IQux {}

public class Bar : IBar {}
public class Baz : IBaz {}
public class Qux : IQux {}