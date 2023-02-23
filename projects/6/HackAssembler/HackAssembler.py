import sys
import datetime
import Parser
import SymbolTable
import Code

class HackAssembler:
    def __init__(self, input_stream, output_path):
        self.symbol_table = SymbolTable.SymbolTable()
        self.next_variable_address = 16
        self.parser = Parser.Parser(input_stream.readlines())
        input_stream.close()
            
        self.first_pass()
        self.second_pass(output_path)        

    def first_pass(self):
        """
        Reads the program lines, one by one focusing only on (label) declarations.
        Adds the found labels to the symbol table.

        Args:
        input_stream -- Stream of input .asm file.
        """
        instructions_line_number = 0

        while self.parser.has_more_lines():
            self.parser.advance()
            instruction_type = self.parser.instruction_type()

            if instruction_type == Parser.A_INSTRUCTION or instruction_type == Parser.C_INSTRUCTION:
                instructions_line_number += 1
            elif instruction_type == Parser.L_INSTRUCTION:
                self.symbol_table.add_entry(self.parser.symbol(), instructions_line_number)

    def second_pass(self, output_path):
        """
        Reads the program lines again and generate the corresponding instructions.

        Args:
        input_stream -- Stream of input .asm file.
        output_path -- String representation of the path where the code will generated.
        """
        self.parser.reset_instruction_index() # Starts again from the beginning of the file.
        code = Code.Code()

        with open(output_path, 'w', encoding='utf8') as output_stream:
            while self.parser.has_more_lines():
                self.parser.advance() # Gets the next instruction

                if self.parser.instruction_type() == Parser.A_INSTRUCTION: # If the instruction is @ symbol
                    symbol = self.parser.symbol()

                    if symbol.isnumeric(): # If the value of the A-instruction is numeric
                        output_stream.write(code.generate_a_instruction(int(symbol)))
                    else: # If the value of the A-instruction is a symbol
                        if self.symbol_table.__contains__(symbol):
                            output_stream.write(code.generate_a_instruction(self.symbol_table.get_address(symbol))) # Translates the symbol to its binary value
                        else: # If symbol is not in the symbol table, adds it
                            self.symbol_table.add_entry(symbol, int(self.next_variable_address))
                            output_stream.write(code.generate_a_instruction(self.next_variable_address)) # Translates the symbol to its binary value
                            self.next_variable_address += 1
                elif self.parser.instruction_type() == Parser.C_INSTRUCTION: # If the instruction is dest=comp;jump
                    output_stream.write(code.generate_c_instruction(self.parser.comp(), self.parser.dest(), self.parser.jump())) # Translates each of the three fields into its binary value


if __name__ == '__main__':
    filename = sys.argv[1] if len(sys.argv) > 1 else ""
    file_extension = ".asm"

    while not filename.endswith(file_extension):
        filename = input("Please enter a file path with the correct extension ({}): ".format(file_extension))
    try:
        with open(filename, 'r') as input_stream:
            print("The file was successfully opened!")
            print("Translating...")
            start_time = datetime.datetime.now()
            HackAssembler(input_stream, filename.replace(".asm", ".hack"))
            duration = (datetime.datetime.now() - start_time).total_seconds()
            print("Translation finished successfully in {} seconds.".format(duration))
    except FileNotFoundError:
        print("The file was not found. Please try again.")

