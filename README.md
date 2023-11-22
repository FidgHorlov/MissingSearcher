Hi there, my name is Andrii Horlov. <br></br>
Those tools have been made for [Tsukat](www.tsukat.com), to simplify some work routines.
Below, you can read about each of the tools.

List of tools:
1. Find missing objects in scenes. ([info](https://github.com/FidgHorlov/SeparateTools/tree/main#find-missing-objects))
<br> `https://github.com/FidgHorlov/SeparateTools.git#missingElements` </br> 
2. Scene parameters ([url](https://github.com/FidgHorlov/SeparateTools/tree/main#scene-parameters))
<br> `https://github.com/FidgHorlov/SeparateTools.git#sceneParameters` </br>
3. Shortcuts to add some useful tools ([info](https://github.com/FidgHorlov/SeparateTools/tree/main#package-hub))
<br> `https://github.com/FidgHorlov/SeparateTools.git#tsukatHub` </br>
4. Log to file ([info](https://github.com/FidgHorlov/SeparateTools/tree/main#logger))
<br> `https://github.com/FidgHorlov/SeparateTools.git#logger` </br>

---
# How to use
1. Copy the link of the liked package.
2. Open Package Manager in opened project.
3. Press `+`, and select `Add package from git URL...` <br></br>
![image](https://github.com/FidgHorlov/SeparateTools/assets/110767790/f3cef339-278d-4320-a360-231251b51f24)
4. Paste the copied link and press `Add` <br></br>
![image](https://github.com/FidgHorlov/SeparateTools/assets/110767790/6d16e824-489d-4045-b4b3-1522c0245fe1)

---

# Logger
Simply, keep your logs in the filesystem. 
<br>To open the Logs configuration please follow the toolbar</br>
![image](https://github.com/FidgHorlov/SeparateTools/assets/110767790/10e05e2c-8b16-4a09-95ce-367e443bc0dc)
<br>You can see a few different settings:</br>
![image](https://github.com/FidgHorlov/SeparateTools/assets/110767790/646ee467-30d2-4974-9ca6-cf816fafcdcb)
1. Log capacity could be `Full` or `Short`: 
- Full - keeps the logs stack trace;
- Short - shows only messages.

2. File writing could be `One File`, `One Big File`, `Separate files`
- One file - every run, the log file will be overridden;
- One big file - all logs are kept inside one file;
- Separate files - all logs are kept in different files. Likewise, if you select this one, you will be able to write the amount of the saved files.

All logs are kept inside of the `Persistent data path`.

# Find missing objects
This tool helps you to detect a gameobjects, prefabs, renderers - with some problems.

What you have to do is:

1. In the menu tab - select "Tsukat/Find.../"
2. Select what you would like to find.
<br></br>
![image](https://github.com/FidgHorlov/SeparateTools/assets/110767790/acac7221-3f2d-4ab6-95c0-2b2ea5b7a0d0)

_The tool simply gets all the scenes inside of the "Assets/" folder and searches for the missing scripts, missing prefabs, or materials on the scene. As a result - it gave you the message in the Unity console._

# Package hub
Currently, this tool contains two packages you will be able to use:
* Assets Usage Detector;
* DoTween;

Other packages are owned by Tsukat Studio, and it's not possible to use them. In the future, you won't be able to see them.
If you have any other useful packages, suggest them on the `Issues` page

# Scene parameters
This tool helps you to work with the scenes, prepare scenes to build, etc.
Imagine you have a project that works with a few different platforms/devices: Android Meta Quest, Android Pico, Windows, WebGL, etc.
And for each platform, you have to build its own scenes. So, with this tool, it is much easier.

![image](https://github.com/FidgHorlov/SeparateTools/assets/110767790/7b84753d-acaf-4ae4-a8b8-077c8a8d91b0)

## Scene Manager
Here you can see the list of all the scenes in the `Assets` folder.

* Select the `Add to build` and select the Target Platform.
* Select the `Add to Menu` to see your scene in the Toolbar.

![image](https://github.com/FidgHorlov/SeparateTools/assets/110767790/f9668d88-59ae-47c9-9423-d04ef0f1fd7e)

_*if you don't see any target platforms, please press the `Scene manager settings`._

## Add to menu?
This tool helps you to have all selected scenes in quick access from the toolbar:

![image](https://github.com/FidgHorlov/SeparateTools/assets/110767790/16f48064-cb81-407b-a185-4b34e1d984c2)


## Scene Manager settings

You can enable/disable Target Platforms which are supported by your platform.

![image](https://user-images.githubusercontent.com/110767790/219476378-51aec150-5400-42b8-ba52-f9630bbf9511.png)

_* In case you need to add scenes to a specific platform, that is not unsupported on your device, please enable `UnSupported platforms` via `ContextMenu`:_

![image](https://user-images.githubusercontent.com/110767790/219477110-7c287c9d-6274-4fd2-accf-e49b49465f68.png)

When you are ready, press "Apply" to add all selected platforms to the `SceneManager`.
_* To add more platforms, please install them through `UnityHub` or `Build Settings`_

## Prepare Build
Basically, this tool keeps all the scenes for the selected platforms. And you don't need to drag and drop every time you need to switch platforms.

![image](https://user-images.githubusercontent.com/110767790/219478080-e0aa2f84-e3f0-499a-b1da-2d6220c8cee3.png)

Here you can see all scenes which were selected by you in the `Scene Manager`. Also, in case you don't need some of them, you can disable them. 
After you press `Open Build Settings` you will see the Unity default window with all selected scenes (from the `Prepare window`).

If you have any questions please contact me:
ahorlov@tsukat.com
andreygorlovv@gmail.com

<i> Copyright (c) 2023. Andrii Horlov </i>
