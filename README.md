# Minesweeper Game

**Author:** Gali Polyak  
**Date:** April 28, 2023

---

## Description

Welcome to the Minesweeper game project! This simple Minesweeper game is built in C# using the XNA framework and is designed to run on Windows. The project provides a basic implementation of the classic Minesweeper game, where players can uncover tiles to find mines and strategically place flags to mark potential mine locations.

---

## Features

- Classic Minesweeper gameplay with multiple difficulty levels (Easy, Medium, Hard).
- Timed gameplay with the ability to restart the timer.
- Sound effects and background music.
- High score tracking.
- A graphical user interface created using XNA.

---

## Setup Instructions

To get the Minesweeper project running with MonoGame and Visual Studio 2022, follow these steps:

### 1. Install MonoGame Setup Files

The repository includes a folder called [`setup_files/`](./setup_files) containing three zip files:

- **`MGCB_ContentPipeline.zip`**  
  This is the **Pipeline Tool** used to manage and build game assets.  
  ➤ **Unzip** this file to any location on your system.

- **`MonoGameDesktopTemplate.zip`**  
  A project template to allow MonoGame projects in Visual Studio.  
  ➤ **Do not unzip.** Move this file to:  
  `Documents\Visual Studio 2022\Templates\ProjectTemplates\`

- **`MonoGame Project.zip`** *(optional)*  
  A slightly updated MonoGame template. Use this only if the other one doesn’t work.  
  ➤ Follow the same instructions as above.

### 2. Test MonoGame Setup

- Open Visual Studio 2022.
- Create a new project using the **MonoGame Desktop Application** template.
- Make sure it runs properly to confirm setup is complete.

---

## Installation

1. Clone or download this repository.
2. Open the solution file (`.sln`) in Visual Studio.
3. Ensure the `MGCB_ContentPipeline` tool is unzipped and available if you need to build or modify game assets.
4. Build the solution (`Ctrl+Shift+B`) to ensure everything compiles correctly.

---

## Usage

1. In Visual Studio, **build** the solution.
2. Run the game (`F5` or `Ctrl+F5`).
3. Play Minesweeper by clicking on tiles and placing flags!

---

## Controls

- **Left-click**: Uncover tile
- **Right-click**: Place/remove flag
- **Level Button**: Change game difficulty
- **Play Again**: Restart game
- **Speaker Icon**: Toggle sound
- **Exit**: Quit game

---

## Game Rules

1. Uncover all non-mine tiles to win.
2. Numbered tiles indicate how many adjacent mines there are.
3. Use flags to mark suspected mines.
4. Avoid clicking on a mine tile!

---

## Troubleshooting

- If the game doesn’t build, double-check that the MonoGame templates are correctly installed.
- If audio doesn’t play, ensure your system audio is enabled and functioning.
- Still stuck? Try recreating the project using the MonoGame template and copying over the game files.

---

## Credits

This project was created by me!

---

## Access to Setup Files

All necessary setup files are available in the [`setup_files/`](./setup_files) folder of this repository. If you encounter any issues or need help setting them up, feel free to reach out to me directly.
