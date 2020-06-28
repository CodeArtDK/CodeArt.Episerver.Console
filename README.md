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
Just install the nuget package and you are ready to start using the Developer Console.
When you are logged in as an Administrator / WebAdmin, it can be found in menu under "Developer|Console".


## First use


## Examples of useful commands


## Integration points
- CLI
- UI access
- Slack bot


## Getting started with Commands


## Building your first command

