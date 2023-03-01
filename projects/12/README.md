# NAND2Tetris Project 12: Operating System
This project involves implementing the Jack OS for the HACK computer built in the previous projects of the [NAND2Tetris](https://www.nand2tetris.org/) course. 
The operating system is responsible for managing system resources such as memory and input/output devices.

## Prerequisites

Before you can test the Jack OS implementation, you need to download and install the Nand2Tetris software from the [NAND2Tetris website](https://www.nand2tetris.org/software).

## How to test my files

1. Clone this repository and navigate to projects/12/JackOS.
2. To test individual .jack files, copy them into their corresponding test folder (e.g. 'ArrayTest' or 'MemoryTest'). For the final test, copy all .jack files into the 'FinalTest/Pong' folder.
3. Compile the directory containing .jack files in your terminal using the JackCompiler, which can be found in the tools provided with the Nand2Tetris software. For example:

```shell
.\JackCompiler.bat <projects\12\JackOS\ArrayTest>
```

4. Load the directory of the compiled .jack files into the VMEmulator.
5. If there is a .tst file in the test directory, load it as a script into the VMEmulator.
6. Set the animation to "No animation".
7. Run the program.
8. Compare the results. If a .tst file was present, the VMEmulator will indicate that the test was successful in the lower-left corner. If there is no .tst file, open the image in the test directory and compare it with the screen of the VMEmulator.

Note: The steps for running the JackCompiler assume that you are using the command prompt or terminal window included with your operating system. If you are using a different command prompt or terminal window, the commands and file paths may be different.
