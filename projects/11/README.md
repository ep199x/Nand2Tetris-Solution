# 11th Project - Nand2Tetris Solution

This is the solution for the 11th project of the Nand2Tetris course, written in C#. The project involves implementing a Jack Compiler that can translate Jack code into VM code.

## Requirements

To run and test this solution, you need to have the following software installed on your system:

- [Java](https://www.java.com/download) (for running the VM Emulator)
- [VMEmulator](https://www.nand2tetris.org/software)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/)
- [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## How to Run

To compile the Jack code into VM code, follow these steps:

1. Clone or download this repository to your local system.
2. Open the solution in Visual Studio from the JackCompiler directory.
3. Build the solution by selecting "Build" > "Build Solution" from the menu.
4. Go to the bin folder within the project and find the executable JackCompiler.exe.
5. Run the executable with the name of a Jack file or a directory containing Jack files as a command-line argument. The compiled Jack code will be saved in the same directory, with a .vm extension.

## Testing the Generated Files in the VMEmulator

To run the generated .vm file in the VMEmulator, follow these steps:

1. Open the VMEmulator.
2. Set "Animate" to "No animation".
3. Load the generated .vm file.
4. Press the "Run" button to execute the code.

## Documentation

The solution is well-documented in the source code, which makes use of regular expressions for tokenizing the Jack code and an object-oriented approach for organizing the code.
