# 8th Project - Nand2Tetris Solution

This is the solution for the 8th project of the Nand2Tetris course, written in C#. The project involves implementing a VM translator that can translate VM code into Assembly code for the hack computer.

## Requirements

To run and test this solution, you need to have the following software installed on your system:

- [Java](https://www.java.com/download) (for running the CPUEmulator)
- [CPUEmulator](https://www.nand2tetris.org/software)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/)
- [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## How to Run

To compile the VM code into Assembly code, follow these steps:

1. Clone or download this repository to your local system.
2. Open the solution in Visual Studio from the VMTranslator directory.
3. Build the solution by selecting "Build" > "Build Solution" from the menu.
4. Go to the bin folder within the project and find the executable JackCompiler.exe.
5. Run the executable with the name of a .vm file or a directory containing .vm files as a command-line argument. The compiled VM code will be saved in the same directory, with a .asm extension.

## Testing the Generated Files in the CPUEmulator

To run the generated .vm file in the CPUEmulator, follow these steps:

1. Open the CPUEmulator.
2. Set "Animate" to "No animation".
3. Load the generated .asm file.
4. Press the "Run" button to execute the code.
