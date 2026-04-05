# Penalty Shootout Game

This project is a 2D Unity penalty shootout game where the player aims a shot, times the kick using a moving timing bar, and tries to score against a goalkeeper. The game includes multiple levels with increasing difficulty, tutorial guidance, score or streak goals, win and restart conditions, sound effects, pause and settings menus, and Firebase-based account and progress saving. The gameplay is designed to feel simple and arcade-like while still requiring timing and accuracy. In later levels, the goalkeeper can also react based on saved player shot patterns.

## Third-Party Plugins Used

The project uses the following third-party plugins and services:
- **Firebase Auth** – used for account creation, login, logout, and password reset.
- **Firebase Firestore** – used for saving usernames, unlocked levels, level completion progress, and shot pattern data.

## Steps to Run the Project

1. Open the project in **Unity** using a minumum version of 6.3 for development.
2. Let Unity load and reimport the project files if needed.
3. Make sure the required scenes are included in **Build Settings**.
4. Check that Firebase is properly set up in the project, including the required configuration files and dependencies.
5. Open the starting scene, which is the **Main Menu**.
6. Press **Play** in the Unity Editor to test the game.
7. The player can then log in or sign up, choose unlocked levels, and play through the penalty shootout system.
8. For the iOS build, go to file -> build profile, and install the iOS platform.
9. Switch to that iOS platform after installation and build -> clean build in a new folder.
10. Access that folder, find the **Unity-iPhone.xcworkspace** and open it using xcode.
11. Run the build while connected to your iPhone using a cable.
