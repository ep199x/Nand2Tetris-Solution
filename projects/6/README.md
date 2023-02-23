# 6th Project - Nand2Tetris Solution

This is the solution for project 6 of the Nand2Tetris series in python. The goal of this project is to build an Assembler, which is a program that takes as input an Assembly code file and generates the corresponding binary machine code.

## Requirements

Before you begin, you'll need to have the following software installed on your computer:

- [Java](https://www.java.com/download) (to run the HardwareSimulator for testing)
- [Python 3](https://www.python.org/downloads/)
- [CPUEmulator](https://www.nand2tetris.org/software)

## How to run my solution

1. Clone the repository to your local machine

```shell

git clone https://github.com/ep199x/Nand2Tetris-Solution/

```

2. Navigate to project 6.

```shell

cd ".\projects\6\"

```

3. Compile the test files. 

```shell

python .\HackAssembler\HackAssembler.py <.\tests\filename.asm>

```

## Test generated files in the hardware simulator

To run the generated .hack file in the Hardware Simulator, follow these steps:

1. Open the CPU Emulator.
2. Set "Animate" to "No animation".
3. Load the generated .hack file.
4. Press the "Run" button to execute the code on the computer hardware built in previous projects of the Nand2Tetris series.

You can also load .asm files into the CPUEmulator.
