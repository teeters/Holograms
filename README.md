# Holograms
Holograms is an Augmented Reality viewing app developed in Unity and designed for the iPhone. It identifies image targets using the Vuforia 
cloud reco system and dynamically loads 3d enhancements from a webserver.

# Installation
In order to build Holograms for the iPhone you will need 3 things:

1. Unity version 5.3.0. IMPORTANT: The Vuforia library is not compatible with higher versions of Unity, 
so do not get the latest. Download [here](https://unity3d.com/get-unity/download/archive)
2. A Mac with XCode (and an iPhone).
3. (optionally) A Vuforia developer account. If you want to use your own database of image targets, you will
need to create your own account with Vuforia. Registration is free and pretty simple, you can sign up [here](https://developer.vuforia.com/)
Note that if you do this, you will also need to set up a webserver to host your 3d models. (More on this in a moment).

## Setup in Unity
When you install Unity, it should create a directory in your home folder called Unity, where it will look for your future projects.
When you clone the source code, you should place it under the Unity directory.

After downloading, you will need to make a few configuration changes in Unity to prepare the project for deployment to iOS.

1. In the Assets folder, open the file viewHologram.unity. This will take you to the main development scene for the app.
2. You should see an empty-looking scene in Unity with some objects in the Hierarchy window: A Cloud Reco object, an ARCamera
object, and an ImageTarget object.
3. (optional) If you want to use your own Vuforia database, you will need to reconfigure the Vuforia objects. Select the 
ARCamera and paste your App Key into the Inspector window, then select the Cloud Reco object and paste in the Client Key and 
Client Secret for your database.
4. Go to File->Build Settings.
5. In the Build Settings Window, select iOS from the list of platforms and be sure to hit the Change Platform button at 
the bottom.
6. Click the Player Settings button and look at the list of options that opens up in the Inspector window.
7. At the top of the window, enter SamTeeter as the company name and Holograms as the product
8. Under Other Settings, make sure that the bundle identifier matches - it should read com.SamTeeter.Holograms
9. IMPORTANT: Uncheck the box that says Auto Graphics API, and select OpenGLES2 from the drop-down that opens. Vuforia
is not compatible with the Metal API.
10. Back in the Build Settings window, check the box that says Development Build and then hit Build.

## Setup in XCode
Building the project should take a while. Grab a Coke and come back. When Unity finishes, you should have a new subdirectory
in your project folder that contains the auto-generated XCode project. Select the Unity-iPhone.xcodeproj file and open it up in XCode.

There are just a few more things to take care of before the project can be deployed.

1. In the navigator window, select the project file at the very top. 
2. In the general window, select your code signing identity for the app. XCode can help create one for you if you don't have one.
3. Under the Build Settings tab, scroll down until you see a line that says Resealse: DWARF with dSYM file. Change it to just,
DWARF. (This is optional, but the dSYM file takes a very long time to generate. Unless you plan on using the debugger a lot,
I'd skip it).
4. Plug in your iPhone and hit the play button in XCode.
5. Pick up a book or flip over to Netflix; this will probably take a while.

##(Optional) Using Your Own Server
At the moment your iPhone camera detects an image in Holograms, two things happen. First, the Vuforia Cloud Reco object
sends the image data to Vuforia where it is compared to the image targets in your database. Vuforia replies with the image
target's metadata package--a simple file that can contain anything at all, up to a certain size. In the case of Holograms,
it simply contains a word--the name of the 3d model associated with that image target. In its second stage of operation,
Holograms searches its own server for a Unity assetbundle with the same name, downloads it, and displays it.

If you want to use your own imagetargets, you will need to upload them to your Vuforia database and attach metadata files
with the appropriate model names. You  will then need to prepare an assetbundle for each 3d model you wish to use, make sure
it has the same name as the one in the metadata file, and upload it to your server. Finally, you will need to modify the settings
in the CloudHandler script in Unity. 

Go back to the viewHologram scene and select the CloudReco object. You should see a script attached in the Inspector window
called CloudHandler. This script is the main nerve center of Holograms. You'll notice it has two public variables: BaseURL and 
username. Holograms is designed with multiple users in mind, so it looks for assetbundles at the location http://BaseURL/username/modelname.
The modelname will be filled in dynamically when Holograms recognizes an imagetarget; you need to set BaseURL to the 
directory on your server where the username folders are stored, and username to the subdirectory that contains the 3d models
you want to use. In the future the app should have a user login flow which automates all of this.
