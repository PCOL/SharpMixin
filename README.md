# Mixins

A .Net library that adds runtime interface based mixin support.

## Installation

[SharpMixin is available on Nuget](https://www.nuget.org/packages/SharpMixin)

## Examples

````c#
public class TypeA
{
    public string Value { get; set; }
}

public class TypeB
{
    public bool Method(string paremeter1, bool paraemeter2, int parameter3)
    {
        ...
    }
}

public interface IMixin
{
    string Value { get; set; }

    bool Method(string parameter1, bool parameter2, int parameter3);
}

var typeA = new TypeA() { Value = "Hello World" };
var typeB = new TypeB();

var mixin = new object[] { typeA, typeB }.CreateAdapter<IMixin>();

Console.WriteLine(mixin.Value);
mixin.Method("Parm", true, 10);

````