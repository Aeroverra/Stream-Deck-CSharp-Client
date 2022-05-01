# Aerove C# Stream Deck Client
[![Build](https://github.com/Aeroverra/Stream-Deck-C-Client/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/Aeroverra/Stream-Deck-C-Client/actions/workflows/dotnet.yml)[![NuGet](https://img.shields.io/nuget/v/Tech.Aerove.StreamDeck.Client.svg?style=flat)](https://www.nuget.org/packages/Tech.Aerove.StreamDeck.Client)[![NuGet](https://img.shields.io/nuget/v/Tech.Aerove.StreamDeck.Template.svg?style=flat)](https://www.nuget.org/packages/Tech.Aerove.StreamDeck.Template)

An easy to use C# Client library for making Elgato Stream Deck plugins taking advantage of the .NET Core service environment with cross platform plugin support for both Windows and Mac OSX.


## Purpose
The Elgato Stream Deck is used by many people mostly in the streaming industry. I purchased one myself to help speed up my development workflow and was surprised to see the lack of options when it came to the plugin store. Naturally I wanted to see how I could change that and contribute some of my own work and found out the only official SDKs are for C++ and Objective-C which may explain the lack of options since these are harder languages for people to start learning in.

I did find other decent options for developing in C# but after trying some of them out I ultimately decided to create my own. The biggest reason being that these SDK's could still be challenging for a beginner and I felt I could offer an easier solution to jump into. These solutions were also built on .NET Framework and even though nothing in them seemed Windows specific, it likely attributes to the lack of Mac plugins. 



## Requirements

 - [NET Core 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) 
 - [Visual Studio (It's Free)](https://visualstudio.microsoft.com/vs/community/) or other IDE

## Getting Started

Install the plugin templates by opening up CMD or Powershell

    dotnet new -i Tech.Aerove.StreamDeck.Template
Make sure you have the latest version

    dotnet new --update-apply

You will now see the following projects available in Visual Studio
| Project | Description |
|--|--|
| Beginner Stream Deck Plugin | Contains everything the normal one does and lots of comments and information perfect for a new developer or someone new to this client. |
| Stream Deck Plugin | Contains your first action and an example service  |

After selecting one of these templates you will need to fill in information about your plugin like your website, name and [UUID](https://developer.elgato.com/documentation/stream-deck/sdk/manifest/).

Once your finished you can run  the plugin and the "DevDebug" feature will automatically install and takeover your plugin giving you access to full debug mode.

![enter image description here](https://i.imgur.com/V27HDhy.png)

Thats it! You can read over the comments and go from there.
Please reference the [Elgato Documentation](https://developer.elgato.com/documentation/stream-deck/sdk/overview/) to understand how the Stream Deck works.

## Publishing for Windows and Mac
Publishing is simple in Visual Studio.

 1. Select your project.
 2. Select Build > Publish Selection
 3. Select Folder and click Finish
 4. Select Actions > More Options >  Edit
 5. Change Deployment Mode to "Self Contained"
 6. Change Target Runtime to "win-x64" or "osx-64"
 7. Expand the File Publish Options and Select "Produce Single File" and "Trim unused code"
 8. Save and click Publish
 9. Select "New" and repeat these steps for the opposite runtime
 10. Combine the two output folder contents and your done!
 11. You can find further information on how to publish to the store on the [Elgato Website](https://developer.elgato.com/documentation/stream-deck/sdk/packaging/)

## Special Features

### DevDebug
A unique system to automatically handle changes to your plugin without the need to restart the Stream Deck application while also giving you access to the built in visual studio debugger. No longer do you need to mess around with powershell scripts and log file debugging to get around the poor design of the Stream Deck, let DevDebug handle it for you.  In most cases it can take less than 4 seconds to launch debug and takeover the stream deck connection!

### Cross Platform Support
Easily develop a cross platform plugin.

### Service Architecture 
Built on top of the .NET Core service architecture you can easily Inject services into your actions and async support giving you the ability to fire off background tasks as needed.
I even provide a way to inject middleware into the pipeline between the Client and your actions. (Docs coming soon) 
