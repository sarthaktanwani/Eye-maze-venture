# Maze-Eye-Venture

This project introduces a method of EOG signal processing using a state machine requiring a low memory and processing footprint. The processed signals from the EOG waveform are interfaced with Unity to play the game "Maze Eye-venture". The game is a demonstration of using bio-signals to perform general-purpose tasks on a computer without the need for standard inputs such as a keyboard and a mouse. The gameplay is in real-time and only requires the user's eye movements to play the entire game.

`Eye Game` is an asset imported from the Unity Asset Store [Original here.](https://assetstore.unity.com/packages/tools/modeling/maze-generator-38689) and modified to take Serial input from arduino, classify them as eye movements i.e. left movt, right movt and blink, and use those as navigation for the ball in the maze.

`docs/` is a folder that includes the final report and a pdf of the final presentation

`max_min_interval_reset` include the firmware for Arduino written using vscode + PlatformIO environment which implements a simple Band Pass Filter and transfers raw ADC EOG values to the Unity Script using Serial connection (UART).

`game_demo.mp4` is a video demonstrating the final gameplay using just eye movements

`presentation.mp4` is a recording of the final presentation for the MS Capstone Project
