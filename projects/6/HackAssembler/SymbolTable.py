class SymbolTable(dict):
    def __init__(self):
        super().__init__()
        self["SCREEN"] = 16384
        self["KBD"] = 24576
        self["SP"] = 0
        self["LCL"] = 1
        self["ARG"] = 2
        self["THIS"] = 3
        self["THAT"] = 4

        for i in range(0, 16):
            self['R' + str(i)] = i

    def add_entry(self, symbol, address):
        self.__setitem__(symbol, address)

    def get_address(self, symbol):
        return self.get(symbol)
