# Quick start

![image](https://github.com/VojtaSara/EDVR/assets/46105170/2fb74798-7021-438e-a008-6a1db2acf264)

To quickly start the labeling process an OpenXR compatible headset, preferably Valve Index needs to be connected to the computer. After that, the program VR Labeling.exe can be run, but without loaded point cloud data, there isn’t much to do. To load the data, it is necessary to go to the folder
```
C:\Users\*username*\AppData\LocalLow\DefaultCompany\VR_player\TrainingDataFolder
```
And input the point cloud data for labeling. The required format is a .txt file with one row for each point of the point cloud, where each row has the following simple structure:
```
x y z r g b
```
All stored as floats separated by a space. The r g b parameters are all 0-1 and denote the
prior color of the point - this is great for colored point clouds, but if the user doesn’t have color data available, they can input the any color, typically better for visibility are lighter shades. 

An example point cloud for labeling could look like this:
```
12.9333259195 22.8721282363 -0.9658880251 0.7450980392 0.6000000000 0.6000000000
12.5940314859 22.0696380019 -0.7958822267 0.4000000000 0.4000000000 0.6117647059
…
```
The loaded point clouds shouldn’t be bigger than 500 000 points , if you have data larger than that it is strongly advised to split the cloud into multiple files, this will cause that they won’t be later loaded at once freezing the user’s view. The 500 000 point limit was with a GEFORCE GTX 1660 SUPER on a desktop PC - if you have lower or higher specifications, you can experiment with different amounts of points.

Aside the point cloud, it is necessary to load the label set. This is another .txt file located in:
```
C:\Users\*username*\AppData\LocalLow\DefaultCompany\VR_player\Config\labels.txt
```
Inside it, define the categories that will then appear as options for the labeling, separated by a semicolon in the format:
label name ; label id ; (r, g, b)

Label id’s should be distinct and the colors for different objects sufficiently differentiated. Example contents of the labels.txt file:
```
'road';  7 ;(128, 64,128)
'sidewalk';  8 ;(244, 35,232)
'building'; 11 ;( 70, 70, 70)
```
The example files and further information is located on the projects Github https://github.com/VojtaSara/VR_labeler
# User documentation

Once the user correctly loads the data into the program as is explained in Quick start, they can start labeling. The left hand is used for holding the menu and the label palette, the right hand is the label brush. On top of that, there are two hand interactions and gestures that will be explained further.


# Tool overview

## Brush
A sphere attached to the right controller, which can be enlarged / shrunk down by the right controller joystick. By pressing down the trigger and painting over the scene, the current selected label is assigned to the affected points. The color-label of the brush is changed by selecting in the left hand label palette.

## BBox
A bounding box defined by two hands, the right hand primary button adds a box in the current position, the secondary button is an “undo” button. The added labels are automatically saved to the application folder in the background. The color-label of the bbox is changed by selecting in the left hand label palette.
## Hand
The hand tool gives the user ability to pan around and scale the scene they are currently viewing. The left hand is always passively in the hand tool mode, the right hand needs to be selected via the left hand menu. One hand enables panning, two hands enable scaling by “pinching” the scene, similar to two fingers scaling pictures on a touchscreen. 

## Saving & Loading
Saving and loading is performed automatically - the labeling persists even after an application crash / restart. The saved labels for each scene can be found in the application folder under
```
C:\Users\*username*\AppData\LocalLow\DefaultCompany\VR_player\SavedLabelsSemantic
```
For the semantic labels, and
```
C:\Users\*username*\AppData\LocalLow\DefaultCompany\VR_player\SavedLabels
```
for the bounding boxes.

## Next / previous scene
These buttons navigate between the different scenes - different point clouds in the 
```
C:\Users\*username*\AppData\LocalLow\DefaultCompany\VR_player\TrainingDataFolder
```
folder.


# Program documentation

![image](https://github.com/VojtaSara/EDVR/assets/46105170/1817ee24-8325-4ad1-aa0f-7422eb74d673)


The picture above sums up the architecture of the solution. Through iteration, the application became simpler, not more complex, which is quite unusual, but thanks to that, the resulting program is rather uncomplicated.

The root of the project is the Unity scene. In it, the different GameObjects handle different functionality.


The “Demo Environment” contains the light settings of the scene. The “Environment” and “PresentationRoom” prefabs contain the 3D models and settings for the environment around the user.

“Complete XR Origin Set Up” contains the OpenXR imported bindings and settings for the VR headset and the controllers. It includes various event handlers for pointing and clicking. 


The objects “LabelBrush” “HandTools” and “brushSphere” contain the various functionalities of the tools for Bounding box, Hand and Brush modes respectively. The “Canvas” element holds the left hand UI and the “Particle System” takes care of loading and rendering the point cloud using Unity’s particle system.

“ConfigLoader” loads the correct label config as is explained above and the “PositionLogger” is an extra object, which can be disabled, that logs the controller positions every second to a log file. 

This concludes a brief overview of the Unity project, now a short description of each of the objects and the C# scripts attached to them will be provided. The environment objects and the XR Origin Set up won’t be commented as the environment is just a collection of mesh renderers with the correct polygon models attached to them and the XR Origin Set up is explained in detail in the Unity OpenXR plugin documentation ( https://docs.unity3d.com/Manual/com.unity.xr.openxr.html )


## Label brush
The LabelBrush GameObject contains the logic for tool selection. The LabelBrush object contains a LabelBrush.cs script that directs which tool is selected in it’s Update() method and forwards the update to the currently selected tool:

- UpdateBboxMode() checks whether any of the buttons on the right controller were pressed and if so, it adds a label to the labelDatabase object or removes the last one added. Before the label arrives to the labelDatabase, the size, position and rotation of the label gets calculated from the properties of the controllers - the logic for this is handled using a helper class BoxPoints. The class LabelDatabase handled the rendering of the BBoxes by programmatically adding GameObjects with LineRenderers attached. This logic can be seen in the public void AddLabel(Vector3[] points, string type, Color labelColor) method of the LabelDatabase class. There, the calculated corners of the bbox are inputted via the points variable and based on the name of the label “type” and color “labelColor” a linerenderer winding through the 8 corners in a way to create all edges of the box is initialized and added to the private List<GameObject>[] labels; array of labels. The function public void RemoveLastLabel() simply pops the last label from this array. 

- UpdateSphereBrushMode() calls the pointCloudRenderer to color affected points on a trigger press. The brush size and controller position is passed. Inside the pointCloudRenderer, the point cloud is iterated through and affected points are selected. Their color is changed via a Texture2D variable which is utilized for it’s best compatibility with the Unity HDRP vfx graph particle system. 

The “hand” mode’s function is handled differently - via a “DualGrab” class component. This is for the reason that if we potentially wanted to add more grabbable objects in the future, this script could be easily attached to them in Unity without needing to code anything. So while in hand mode, the LabelBrush doesn’t perform any extra logic. The Particle System containing the opened point cloud checks if it is being grabbed by the left hand permanently, but when the right hand is also in hand mode, it performs scaling after both hands are triggered. 

## Point Cloud
The point cloud hides under the GameObject “Particle System” and contains the class PointCloudLoader and PointCloudRenderer. The loader handles loading the point cloud from a correctly formatted file, which is commented above. The renderer handles passing the point data into the HDRP VFX graph which looks like this (next page):



This guarantees that updates of the point cloud will be reasonably efficient thanks to handling them in parallel on the GPU.

The information about position and color of each particle is held inside Texture2D objects Texture2D texColor; Texture2D texPosScale;

Besides updating the point cloud when the user colors it, the renderer also contains a coroutine for saving the currently labeled area to a file. Thanks to using a coroutine, the application doesn’t hang while the saving is being performed and for this reason I chose to add an autosave feature that calls this coroutine every 10 seconds. This period can be modified.

As the pointcloud can be generally scaled and translated relative to the user’s controller, alignment of the vector spaces has to be performed using float GetScaleRelativeToWorld() and Unity’s this.transform.InverseTransformPoint(). 

## Discussion
Having to upload the files to specific directories is not very user friendly and a better loading method is badly needed. Using the Unity persistent path has some positives in terms of security and reliability - but having to manually dig through the file system to find these folders and potentially have to create them if they don’t exist 



