-----------------------------------------------------------------------------
						yIRC: Unity IRC library
					Copyright © 2012-2015 Dennis Sitelew
							Version 0.1.0.2
						  suppyirc@gmail.com
-----------------------------------------------------------------------------
I.	Discription.
II. Changelog.
III. Common problems.
-----------------------------------------------------------------------------
I. Discription

Thank you for choosing yIRC library. This library will help you to create a
suitable in- or pre-game char for your game.

This library is provided with two usage examples:
1)  InGame NGUI example - shows how you can use this library to create in-game 
	chat using NGUI library. You can easily  separate users  from different 
	games  simply putting them in different channels.

	WARNING: Starting with v0.1.02 NGUI Library is no longer included 
	into yIRC library. In order to use NGUI example you need to have the 
	NGUI library (you have to setup sample first). 

2)  Chat Room OnGUI Example - shows you how to create simple chat room using 
	Unity OnGUI functions.

Both samples follows the same logic:
	1 - Crate a separate script for IRC-network interaction, 
		for example: "InGameChat".
	2 - subscribe on important messages.
	3 - use Update function to listen to new messages.

II. Changelog.
0.1.0.2 Windows Phone support.		
		Since NGUI DLL library is not supported anymore. NGUI library is no 
		longer included and unsupported.
		Updated OnGUI example.
0.1.0.1 Minor bug fixes in iOS library impementation
0.1.0.0 Initial library release.
-----------------------------------------------------------------------------
III. Common problems

To learn how to use yIRC library within your Windows Phone application, you 
have to 

In case if you have any problems with web-player including error messages like this:
Couldn't connect! Reason: Could not connect to: server_name:6667 
Unable to connect, as no valid crossdomain policy was found

I reccomend you to take a look at the "Security Sandbox of the Webplayer" documnet: 
http://docs.unity3d.com/Documentation/Manual/SecuritySandbox.html

It explains why you can receive this kind of errors and how can you fix it.

But if you are really lazy, here is a main points:

1 - For successful connecting from your web-based unity application you 
have to have a running policy server on your server.

2 - You can find windows exutable and source code for this server 
in your Unity3D installation folder: Editor\Data\Tools\SocketPolicyServer

Feel free to contact me if you have any library-related questions: suppyirc@gmail.com