# VRTK-OVRAvatar-UNet-Demo
This is a demo of how to use VRTK, OVRAvatar, and UNet together in Unity. It currently only works on Gear VR, but since I used VRTK getting it to work on other platforms shouldn’t be too hard. All the code written by me is in the [App] directory. Other directories at the root of the Assets folder are imported packages. The [App] directory contains the directories [Demos], [Dev], [System], and VRTK. The [Demos] directory contains this demo. The [Dev] directory contains resources created during development and are temporary. The [System] directory contains a library of application independent resources. All other directories under [App] contain resources pulled from imported resources and modified. Modified resources maintain the file structure they were found in. A note is added to the class documentation describing the modifications made.

This app starts the user in a room with a menu in front. Users can move around using the DPad of the controller. The player is represented by an Oculus Avatar. To the right of the player is a second avatar; a twin avatar. This was done simply for test and demonstration purposes.

Using the right controller, the user can select either to host an internet game or a local game. Once selected, a capsule will appear below the player. This was done simply to have a visual indication of the match being created. The menu also changes to allow the user to quit hosting.

Other devices can join the match. Once a match is created, other devices will see the name of the match in the list in the middle of the view. When a match is clicked, the user joins the match and now visible among other players. Movement is synchronized across devices. The menu also changes to allow the client to quit. Additionally, a user can pull a trigger to send a simple message (the number 8) to the other devices.

## Known Bugs ##
Gaze input works only if a controller (perhaps only the right controller) is connected.
