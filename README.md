# godotOscSharp

A simple library for OpenSoundControl sending and receiving over a network, for use in Godot.

It's been lightly tested in Godot 4.1, invoked from a scene's script written in C#.

### How to Use

The easiest way to download this is to use one of the following commands from inside your project: `git submodule add https://github.com/cass-dlcm/godotOscSharp.git` or `git submodule add git@github.com:cass-dlcm/godotOscSharp.git`, depending on whether you connect to GitHub using HTTPS or SSH.

Then use:

```Bash
cd godotOscSharp
git checkout tags/v0.1.0 -b v0.1.0-branch
```

It'll put the files inside a directory in your project named `godotOscSharp`, and checkout the version listed in the tag.

The constructor for the OscReceiver takes in an `int` port as a mandatory argument.

The constructor for the OscSender takes in an `IPAddress` and a `int` port as mandatory arguments.

Here's an example of how to create and use a receiver.
```C#
var receiver = new godotOscSharp.OscReceiver(9000);
receiver.MessageReceived += (sender, e) =>
{
    GD.Print($"Received a message from {e.IPAddress}:{e.Port}");
    GD.Print(e.Message.ToString());
    };
receiver.ErrorReceived += (sender, e) =>
{
    GD.Print($"Error: {e.ErrorMessage}");
};
```