<div align="center">
  <img width=120 src="./Assets/Art/Textures/Ship_Color.png">
   <h2>Lunar-Lander-ML-Agents</h2>
</div>

# Getting Started

## Prerequisites
This application was developed and tested on Windows.

### Python
Download and install [Python 3.10.11 (64-bit)](https://www.python.org/downloads/release/python-31011/). [Direct download](https://www.python.org/ftp/python/3.10.11/python-3.10.11-amd64.exe)
> [!NOTE]
> Please make sure that python is added to your system path.

### Unity (optional)
Only necessary if you want to develop or run the game in the Unity editor.<br/>
Download and install [Unity](https://unity.com/download). [Direct download](https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup.exe)

### Git (optional)
Only necessary if you want to clone the repo and develop.<br/>
Download and install [Git (64-Bit)](https://git-scm.com/download/win).

## Installation

### Download the project

#### Without Git (includes the standalone application and a pretrained model)
Download and unpack the application from the [Release Page](https://github.com/kemmejer/Lunar-Lander-ML-Agents/releases).

#### Using Git (for development)
Open a command line in your desired target directory and execute the following line:
```vb
git clone https://github.com/kemmejer/Lunar-Lander-ML-Agents.git
```

### Setup

Execute the file **setup.bat** and wait until the installation has finished.<br/>
> [!NOTE] 
> The setup of the Python virtual environment and the installation of the dependencies may take a while.
> If the installation has finished, the window will either automatically close or show the following message:
> "The setup has finished. You may now close this window."

## Running the application

### Using Unity
1. Launch the Unity Hub.
2. Click the **Add** button.
3. Select the folder of the Lunar-Lander-ML-Agents project.
4. Launch the project.
5. Open the MainScene in the **Project explorer** by double-clicking it under: Assets->Scenes->MainScene.
6. Press the **Play** button of the editor.
7. If you start the training for the first time, you may be prompted to install TMP Essentials. Stop the editor by pressing the play button and click on **Import TMP Essentials**.
8. Press the **Play** button in the editor again. Everything should now be working.
   
### Standalone

Open the **Build** folder inside the project folder and launch the **Lunar Lander ML-Agents.exe**
> [!NOTE]
> You must have downloaded the release build of this project in order to have an included standalone application.

# Usage

## Manual play

To spawn a user controllable ship, use the button "Spawn Player Ship" in the controls header.

### Controls
| Action       | Shortcut                                          |
|--------------|---------------------------------------------------|
| Thrust       | <kbd>&uarr;</kbd>, <kbd>W</kbd>, <kbd>Space</kbd> |
| Rotate left  | <kbd>&larr;</kbd>, <kbd>A</kbd>                   |
| Rotate right | <kbd>&rarr;</kbd>, <kbd>D</kbd>                   |


## Starting the training

To start the training of the agents, use the "Start Training" button in the controls header.

## Configuration

When you start the training, the currently selected config in the controls header will be used.<br/>

The ship and training parameters can be adjusted and randomized.
Every randomizable parameter has two values:<br/>
The first value is the base value. The second value is the random deviation applied to the base value.<br/>
The values are calculated as follows:
```
value = max(BaseValue +- Deviation, 0)
```

## Parameter

### Ship Parameter

| Parameter           | Explanation                                                          |
|---------------------|----------------------------------------------------------------------|
| Max Fuel            | The starting fuel amount of the ship                                 |
| Fuel Consumption    | The fuel consumption per thrust                                      |
| Rotation Speed      | Angle in degrees to rotate the ship per rotation                     |
| Thrust Amount       | Amount of force to be applied to the ship while thrusting            |
| Max Velocity        | The maximum allowed velocity on landing                              |
| Max Angle           | The maximum allowed angle between the ground and the ship on landing |
| Mass                | The mass of the ship for physics simulation                          |
| Drag                | The drag of the ship for physics simulation                          |
| Gravity Scale       | The gravity of the environment for physics simulation                |
| Horizontal Velocity | The horizontal spawning velocity of the ship                         |


### Ground Generator

| Parameter                        | Explanation                                                                           |
|----------------------------------|---------------------------------------------------------------------------------------|
| Noise Height                     | Height modifier for the hilly top of the ground                                       |
| Base Height                      | Base height of the ground                                                             |
| Noise Scale                      | Scale of the hilly top of the ground. Higher values result in more peaks and valleys  |
| Resolution                       | The amount of subdivisions of the ground. Higher values result in smoother landscapes |
| Seed                             | The seed used for random generation                                                   |
| Regenerate Ground while Training | Controls whether the ground should be regenerated every nth iteration while training  |
| Regenerate Interval              | Adjusts the interval in which the ground should be regenerated                        |

### Machine Learning

| Parameter               | Explanation                                                                                                                                        |
|-------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------|
| Draw Rays               | Toggles whether the raycasts should be visualized                                                                                                  |
| Rays per Direction      | The amount of raycasts in each direction. If the value is 2, the agents send two rays angled to the right, one down and two angled to the left     |
| Angle                   | Angle of the down facing cone in which the rays are be evenly distributed in                                                                       |
| Horizontal Distribution | Horizontal distribution of the starting point of the raycast. A value of zero results in all rays starting from a single point underneath the ship |
| Ship Count              | The amount of agents used for parallel training                                                                                                    |
| Decision Interval       | The amount of steps a agent can move until the next decision is requested from the model                                                           |
