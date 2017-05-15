A Useful, Interactive, and Extensible .NET Console Application Template for Development and Testing
===================================================================================================

This application can serve as a template of framework into which you can easily plug in commands related tothe code you want to demo, excercise, or which forms the business layer of an actual Console application. The "Interactive" part of this Console is already in place - all you need to do is plug command classes and methods into the Commands namespace and you're ready to go. 

The goal here was not to emulate a fully-functioning Shell or terminal. I wanted to be able to:

* Run the program, be greeted with a prompt (customizable, in this case), and then enter commands corresponding to various methods defined in a specific area of the application. 
* Receive feedback (where appropriate), error messages, and such 
* Easily add/remove commands 
* Generally be able to quickly put together a functioning console application, with predictable and familiar interactive behavior, without re-inventing the wheel every time.  

For more detailed information about how this works, see the Blog post at: [C#: Building a Useful, Extensible .NET Console Application Template for Development and Testing](http://typecastexception.com/post/2014/09/07/C-Building-a-Useful-Extensible-NET-Console-Application-Template-for-Development-and-Testing.aspx)

Assumptions
-----------

As it is currently configured, this application makes a few assumptions about architecture. You can easily adapt things to suit your purposes, but out-of-the-box, the following "rules" are assumed:

* Methods representing Console commands will be defined as `public static` methods which always return a `string`, and are defined on `public static` classes. 
* Classes containing methods representing Console commands will be located in the `Commands` namespace, and in the *Commands* folder. 
* There will always be a static class named `DefaultCommands` from which methods may be invoked from the Console directly by name. For many console projects, this will likely be sufficient. 
* Commands defined on classes other than DefaultCommands will be invoked from the console using the familiar dot syntax: ClassName.CommandName. 

Defining Commands
-----------------

If you were to define the following (trival example-style) commands in your `DefaultCommands` class, you will be able to execute these from the Console when you run the application. The DefaultCommands class must be present in the project, and must be within the `Commands` namespace (note that the methods must be `static` in order to be available to the console as commands, and the project assumes a `string` return type), 

```csharp
public static string DoSomething(int id, string data)
{
    return string.Format(ConsoleFormatting.Indent(2) + 
        "I did something to the record Id {0} and saved the data '{1}'", id, data);
}


public static string DoSomethingElse(DateTime date)
{
    return string.Format(ConsoleFormatting.Indent(2) + "I did something else on {0}", date);
}


public static string DoSomethingOptional(int id, string data = "No Data Provided")
{
    var result = string.Format(ConsoleFormatting.Indent(2) + 
        "I did something to the record Id {0} and saved the data {1}", id, data);

    if(data == "No Data Provided")
    {
        result = string.Format(ConsoleFormatting.Indent(2) + 
        "I did something to the record Id {0} but the optinal parameter "
        + "was not provided, so I saved the value '{1}'", id, data);
    }
    return result;
}
```

Executing Commands
------------------

The commands above can be executed when you run the application with the following syntax:

Execute the `DoSomething` command:

```
console> DoSomething 55 "My Data"
```

Execute the `DoSomethingElse` command:

```
console> DoSomethingElse 7/4/2014
```
The console recognizes and deals with optional method parameters. 

Execute the `DoSomethingOptional` command inluding optional parameters:

```
console> DoSomethingOptional 212 "This is my optional data"
```

OR, you could omit the last argument, and the default value defined on the method will be used:

```
console> DoSomethingOptional 212
```

I'm happy to take pull requests, suggestions, and ideas. 
