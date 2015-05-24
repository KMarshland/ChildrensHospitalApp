# SWIPE
This app is designed to help navigation around Children's Mercy Hospital. It is written in C# with Unity 5. 

# Map Generation
It takes image files for each floor (configured in MapMaker.cs), then creates a bool[,] for every floor based on the grayscale values. For a given pixel, if it's more dark than light, that value is marked as true. Then, each place where it's true gets a cube placed in it. (These cubes do get merged together for rendering speed reasons)

These 3d models are cached in Prefabs/Floors. To use a new version, just delete that prefab and resave it after generation.

Additionally, these 3d models are only actually loaded the first time someone goes to a floor. When they leave that floor, the model remains in memory but not rendered, so that it is much faster if and when they return to that floor.

# GUI
Most of the UI is written in html, in Resources/HTML/All.html. Because of the HTML rendering system, all the html is in this one file, with different sections in different divs which get marked as hidden or visible depending on the occassion. 

All the logic and listeners are in Scripts/MapCameraControl.cs.

# Markers
The marker save is just a string that gets updated from the server when it has internet access. Then it loads the markers from that string. It's in a pipe delimited format, and might be worth changing to JSON at some point. 

These markers are then rendered on the map if they are marked as important or if they are actively involved in a path.

The server side code for placing the markers and spitting back the marker save is not currently included in the repository, but will be soon. 

# Pathing
Pathing is done using a node based graph and A*. Most of this is done using a third party library, but the placement of nodes is done by us. Nodes are placed on both inner and outer corners, and where markers are. These node maps are cached in Resources/PathCaches. With a new map, these caches should be regenerated. 

When the A* library completes a path, we smooth it to remove redundant segments. 

We also use our own A* to figure out which elevators to take. Then we call the beefier library to path each segment from markers to elevators.

# Legal peculiarities
The A* library and the HTML rendering library are both one license per seat. Luckily they're fairly cheap.
