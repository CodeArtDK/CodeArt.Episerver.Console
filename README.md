# CodeArt.Episerver.Console

## Introduction
When you look at a typical Episerver installation, you will far too often find a lot of small Scheduled Job commands, only meant to be run manually - or a similar amount of initialization scripts that only run once and then is disabled. In a few rare cases some of these scripts/tools will even have made their way to a controller that is exposed on a secret url and will run some kind of code when called.
All of these components are due to the fact that developers and webmasters needs small custom pieces of code they can easily write and then run in an Episerver context.

The CodeArt Developer Console for Episerver, is an easy way for developers to run small console commands against the context of an Episerver instance. The concept is pretty simple - much like a scheduled job, you specify your commands/jobs as a class with an Execute method - then, all you have to do to run them is to type the command in a console exposed in the UI backend of Episerver and it will execute. 
But - unlike scheduled jobs - these console commands gives you a lot of advantages:
- You can supply *parameters* for them. And the parameters are simply properties of the Command class.
- You can chain (pipe) them together when you call them in the command line - So the output of one command becomes the input of the next.
- There are already a lot of readily available out-of-the-box commands - and that number will keep increasing.
- You can decide if you want to run them async or sync.
- Soon, you will be able to store chain of commands as Macros.
- You can use the CLI to even run the command prompts locally

## About the project
The code is still in a very early stage, and changes both can and will occur.
It is currently *not* considered production ready - so use on your own risk.
However - if you like the project and get inspired, feel free to help out. Either by doing pull requests, or by adding feature request under "Issues".

This project is not part of the Episerver Developer Tools project - but it will nest itself in the same menu structure as it is a tool for developers. It will happily co-exist with Episerver Developer Tools - but can also run independently.


## Installation
Just install the nuget package from the Episerver feed and you are ready to start using the Developer Console.
When you are logged in as an Administrator / WebAdmin, it can be found in menu under "Developer|Console".

## First use
When you see the console you can type in commands.
A command consist of the keyword for the command, followed by the parameters.
The parameters can either be a number of independent parameters, if the command supports it - or it can be named parameters, specified first by a '-' followed by the name, and afterwards the value. Values can be wrapped in quotes, should they contain spaces.

Try for example the HelloWorld command:

```
hello -Name Allan

or

hello Allan
```

The first example is using the named parameter to set the property "name" to Allan - the second is just passing "Allan" as a parameter to the command.
Both will result in the message "Hello [name]" being returned.

You can also try to use the command ```help``` to list all the available commands. If you call ```help [name of command]``` you get details of how a specific command should be used.

## Piping commands
Some commands can produce an output that can be 'piped' (passed along) to other commands.
One example of this would be the "loadcontent [content-id]" command. 
This command will load the content object with the provider content reference and pass it along. If you run it on it's own it will not produce any output - only if you pipe the output to another command can you use it.

Try running ```loadcontent 5|dump```. The "dump" command will simply try to output any object passed to it to the log.
So this will produce an output much like this:
```
10:14 AM DumpCommand> Content Object: Start
10:14 AM DumpCommand>   Kind: Page
10:14 AM DumpCommand>   Parent: 1
10:14 AM DumpCommand>   PageLink: 5
10:14 AM DumpCommand>   PageTypeID: 19
10:14 AM DumpCommand>   PageParentLink: 1
10:14 AM DumpCommand>   PagePendingPublish: False
10:14 AM DumpCommand>   PageWorkStatus: 4
10:14 AM DumpCommand>   PageDeleted: False
10:14 AM DumpCommand>   PageSaved: 6/19/2020 8:15:28 PM
10:14 AM DumpCommand>   PageTypeName: StartPage
10:14 AM DumpCommand>   PageChanged: 6/19/2020 8:15:28 PM
10:14 AM DumpCommand>   PageCreatedBy:
10:14 AM DumpCommand>   PageChangedBy:
10:14 AM DumpCommand>   PageMasterLanguageBranch: en
10:14 AM DumpCommand>   PageLanguageBranch: en
...
```
You can of course pipe multiple commands together - so we could expand the above with a "select" command to only pick one property of the content object:
```
loadcontent 5|select -Property PageName|dump
```
To only have the PageName written back to the console.

## Downloading files
It is possible to have command output returned as downloaded files - both in the CLI and in the browser based console.
Simply pipe the output to the "download" command and provide the filename as a parameter.
This command will for example download a csv file with all the pages that are descendents to page with ID 5:
```
listdescendents 5|select PageName PageTypeName PageCreated PageCreatedBy|tocsv|download -filename "pages.csv"
```

## Uploading files
NOTE: This feature currently only works in the CLI, not in the browser.
By starting with the "upload" command (and provide a file path as a parameter) you can upload a file and pass it in the pipe to other commands.


## Async commands
If you have a command that you expect will take a long time to execute you might want to run it in a separate thread. To accomplish this, use the keyword "start" before the command line.

## Examples of useful commands
- "memory" will return the amount of memory used by the Episerver application. Use "kb", "mb" or "gb" as parameters.
- "notify -Subject Hello" will send a notification to editors. There are several parameters, but at least the "- Subject" parameter is mandatory.
- "appsettings|dump" will list the appsettings
- "assemblies|dump" lists assemblies loaded in the app domain
- "listdescendents 5 |filter -property PageTypeName -operator equals -value ArticlePage|select -property PageName|dump" will list the pagenames of descendents below page 5, of the type ArticlePage.
- "createcontent -contenttypename StandardPage -parent 5|setproperty -name PageName -value "This is cool"|savecontent -action publish" will create a new standard page below 5, set the pagename property and save and publish it.
- "listen -event SavedContent|select Content|select PageName|dump" will listen for new content saved events and dump them to the console.
- "import-asset -url [url]" will download an image at that url and add it to the root of the media folder.


## Using the Browser based console
As you use the browser based console and start typing commands, you will notice how the hint-box below suggests commands based on your input. If there is just one command suggested you can hit CTRL+. to use it. You can use arrows (up and down) to navigate previously entered commands.

## Using the CLI
You can download the latest version of the CLI **[here](https://codeartdownload.z16.web.core.windows.net/CodeArt.Episerver.Console.CLI.zip)**
Note, it requires .NET Core 3.1 runtime on your machine. Unzip the file and open up a command prompt.
Before you can start using it, you have to go to the browser based developer console and create an access token.
You do this, by calling the command "createaccesstoken". It will create and register a new access token and return it to you in the console. Make sure you copy it, as this is the only chance you will have (only a hashed version of the token is stored in Episerver).
You can now start the CLI in your command prompt, by providing it with the endpoint and access token as parameters like this: 
```
DevConsoleCLI.exe http://localhost:61510/ 9nw1tZzURUyOYouCXCq0pQ==
```
Note: If you only want to run a single command, you can provide that command after the first two parameters.
If you later want to manage your access tokens, you can use the "listaccesstokens" command and the "removeaccesstoken" command.


## Getting started with Commands
You can easily extend with your own commands. All you have to do is implement one or more of these interfaces:
- IConsoleCommand. The basic command that executes and outputs one line to the log.
- IConsoleOutputCommand. A command that can send multiple lines back to the console.
- IInputCommand. A command that accepts input piped in.
- IOutputCommand. A command that produces output which can be piped to other commands.

You should also use the Command attribute on top of the class declation to indicate which keyword this command should be called with. Similarly, each property that should be exposed as parameters should have the CommandParameter attribute attached to them.


## Building your first command

The simplest kind of command would be something along the lines of the HelloWorldCommand:
```csharp
    [Command(Keyword ="hello", Description ="The classic hello world example command")]
    public class HelloWorldCommand : IConsoleCommand
    {
        
        [CommandParameter]
        public string Name { get; set; }

        /// <summary>
        /// Returns a string that will be returned as output.
        /// </summary>
        /// <returns></returns>
        public string Execute(params string[] parameters)
        {
            if (string.IsNullOrEmpty(Name) && parameters.Length == 1) Name = parameters.First();
            return "Hello " + Name;
        }
    }
```
Notice how it supports both a list of parameters for easy use - but also a named parameter.
The Execute method returns a string that will be sent to the console (unless it's null).

## Output Command
An example of a simple Output command is this one, that lists the loaded assemblies. In this case, it will check to see if it's output is being piped to another command. If it is not, it will instead output it to the console:
```csharp
   [Command(Keyword ="assemblies", Description = "List all assemblies loaded in the AppDomain")]
    public class AssembliesCommand : IOutputCommand, IConsoleOutputCommand
    {
        public event CommandOutput OnCommandOutput;
        public event OutputToConsoleHandler OutputToConsole;

        public string Execute(params string[] parameters)
        {
            int cnt = 0;
            if (OnCommandOutput == null) OutputToConsole.Invoke(this, "Assemblies loaded: ");
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                cnt++;
                if (OnCommandOutput != null)
                {
                    //Pass on
                    OnCommandOutput.Invoke(this, assembly);

                } else
                {
                    //Output
                    OutputToConsole.Invoke(this, "\t" + assembly.FullName);
                }
            }
            return $"{cnt} assemblies listed.";
        }
    }
```
Notice how input and output is handled using events. If the OnCommandOutput event is null, it means that there are no commands listening to it, in which case the OutputToConsole event is used. It is here because this class also implements IConsoleOutputCommand.
Another safe way of passing along data is to simply output it using "OnCommandOutput?.Invoke(this,data);".

## Input Command
Finally, here is a command that can accept incoming data (or data from the parameters) and write it to the Episerver log file.
```csharp
[Command(Keyword ="tolog",Description ="Sends content to log")]
    public class ToLogCommand : IConsoleCommand, IInputCommand
    {
        private static readonly ILogger log = LogManager.GetLogger();

        [CommandParameter]
        public Level Level { get; set; }

        [CommandParameter]
        public string Message { get; set; }

        public string Execute(params string[] parameters)
        {
            if (!string.IsNullOrEmpty(Message)) log.Log(Level, Message);
            return "Written to log";
        }

        public void Initialize(IOutputCommand Source, params string[] parameters)
        {
            Source.OnCommandOutput += Source_OnCommandOutput;
        }

        private void Source_OnCommandOutput(IOutputCommand sender, object output)
        {
            log.Log(Level, output.ToString());
        }

        public ToLogCommand()
        {
            Level = Level.Error;
        }
    }
```
Notice how with an IInputCommand you have an Initialize method to allow you to hook into the source commands OnCommandOutput event. The Initialize methods of every command in the command pipeline is always called before any of the Execute methods are called.