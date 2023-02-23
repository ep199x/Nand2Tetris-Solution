import os

A_INSTRUCTION = 0
C_INSTRUCTION = 1
L_INSTRUCTION = 2


class Parser:
    def __init__(self, instructions):
        """
        Initializes a new instance of the Parser class with the specified input stream.

        Args:
            input_stream: Stream of the current input .asm file.
        """
        # Store the instructions and current state variables.
        self.instructions = instructions
        self.current_line_number = -1
        self.current_instruction = ""
        self.total_line_numbers = len(self.instructions) - 1


    def has_more_lines(self):
        return self.current_line_number < self.total_line_numbers

    def advance(self):
        self.current_line_number += 1
        self.current_instruction = self.instructions[self.current_line_number].strip()
        
        if self.current_instruction.startswith("//") or self.current_instruction == "":
            self.advance()

    def instruction_type(self):
        if self.current_instruction.startswith('@'):
            return A_INSTRUCTION
        elif self.current_instruction.startswith('(') and self.current_instruction.endswith(')'):
            return L_INSTRUCTION
        elif len(self.current_instruction.split('=')) == 2 or len(self.current_instruction.split(';')) == 2:
            return C_INSTRUCTION

    def symbol(self):
        if self.instruction_type() == L_INSTRUCTION:
            return self.current_instruction[1:-1]
        elif self.instruction_type() == A_INSTRUCTION:
            return self.current_instruction[1:]

    def dest(self):
        current_instruction_parts = self.current_instruction.split('=')

        if len(current_instruction_parts) == 2:
            return current_instruction_parts[0]
        else:
            return "null"

    def comp(self):
        current_instruction_parts = self.current_instruction.split('=')

        if len(current_instruction_parts) == 2:
            return current_instruction_parts[1]
        else:
            return self.current_instruction.split(';')[0]

    def jump(self):
        current_instruction_parts = self.current_instruction.split(';')

        if len(current_instruction_parts) == 2:
            return current_instruction_parts[1]
        else:
            return "null"

    def reset_instruction_index(self):
        self.current_line_number = -1
