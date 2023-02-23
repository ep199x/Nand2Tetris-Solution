comp_instructions = {
    "0":    "0101010",
    "1":    "0111111",
    "-1":   "0111010",
    "D":    "0001100",
    "A":    "0110000",
    "M":    "1110000",
    "!D":   "0001101",
    "!A":   "0110001",
    "!M":   "1110001",
    "-D":   "0001111",
    "-A":   "0110011",
    "-M":   "1110011",
    "D+1":  "0011111",
    "A+1":  "0110111",
    "M+1":  "1110111",
    "D-1":  "0001110",
    "A-1":  "0110010",
    "M-1":  "1110010",
    "D+A":  "0000010",
    "D+M":  "1000010",
    "D-A":  "0010011",
    "D-M":  "1010011",
    "A-D":  "0000111",
    "M-D":  "1000111",
    "D&A":  "0000000",
    "D&M":  "1000000",
    "D|A":  "0010101",
    "D|M":  "1010101"
}

dest_instructions = {
    "null": "000",
    "M":    "001",
    "D":    "010",
    "MD":   "011",
    "A":    "100",
    "AM":   "101",
    "AD":   "110",
    "ADM":  "111"
}

jump_instructions = {
    "null": "000",
    "JGT":  "001",
    "JEQ":  "010",
    "JGE":  "011",
    "JLT":  "100",
    "JNE":  "101",
    "JLE":  "110",
    "JMP":  "111"
}

class Code:
    def __init__(self):
        pass

    def generate_a_instruction(self, symbol):
        """
        Generate the 16-bit A-instruction binary string for the given symbol.

        Args:
            symbol (str): The symbol to convert to binary.

        Returns:
            str: The 16-bit binary string for the A-instruction, padded with leading zeros if necessary.
        """
        return format(symbol, 'b').zfill(16) + "\n"

    def generate_c_instruction(self, cx, dx, jx):
        """
        Generate the 16-bit C-instruction binary string for the given comp, dest, and jump instructions.

        Args:
            cx (str): The comp instruction.
            dx (str): The dest instruction.
            jx (str): The jump instruction.

        Returns:
            str: The 16-bit binary string for the C-instruction in the format "111cccccccdddjjj\n".
        """
        return "111{}{}{}\n".format(comp_instructions[cx], dest_instructions[dx], jump_instructions[jx])

