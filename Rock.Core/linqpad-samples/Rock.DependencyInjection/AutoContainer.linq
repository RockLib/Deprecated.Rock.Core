<Query Kind="Program">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.DependencyInjection</Namespace>
</Query>

void Main()
{
    // The AutoContainer class is a simple, lightweight DI container that implements
    // the Rock.DependencyInjection.IResolver interface. AutoContainer is:
    //
    // - Useful when you don't need all the bells and whistles of a full-featured DI
    //   container.
    // - Helpful when you need to construct an instance of a class but you don't know
    //   anything about its constructor(s) and also you have some potential dependencies
    //   that can be available for a constructor if required.
    // - Fast.
    // - Cheap to construct.
    
    // Pass dependencies with "singleton scope" to AutoContainer's constructor.
    // These instances will be available for AutoContainer when resolving dependencies.
    AutoContainer autoContainer = new AutoContainer(new Bar(), new Baz());
    
    // Dependencies with "transient scope" are set with the SetBinding method.
    // New instances of the implementation type will be created by AutoContainer
    // when resolving dependencies.
    autoContainer.SetBinding(typeof(IQux), typeof(Qux));
    
    // To resolve a type, call the Get method.
    autoContainer.Get<Foo>().Dump("autoContainer.Get<Foo>()");
    
    // Can also use the non-generic Get method.
    autoContainer.Get(typeof(Foo)).Dump("autoContainer.Get(typeof(Foo))");
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

public class Bar : IBar
{
    private static int _seed;
    private readonly int _value = _seed++;
    public int Number { get { return _value; } }
}

public class Baz : IBaz
{
    private static int _seed;
    private readonly int _value = _seed++;
    public int Number { get { return _value; } }
}

public class Qux : IQux
{
    private static int _seed;
    private readonly int _value = _seed++;
    public int Number { get { return _value; } }
}
