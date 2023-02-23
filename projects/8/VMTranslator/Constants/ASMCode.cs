namespace Constants
{
    /// <summary>
    /// This class provides constants of code patterns to translate .vm files into .asm files.
    /// </summary>
    internal class ASMCode
    {
        private const string SP = "@SP\n";
        private const string IncreaseMAndPoint = "AM=M+1\n";
        private const string DecreaseMAndPoint = "AM=M-1\n";
        private const string StoreDInM = "M=D\n";
        private const string StoreAInD = "D=A\n";
        private const string StoreMInD = "D=M\n";

        private const string StoreSumOfAAndDInA = "A=D+A\n";
        private const string StoreSumOfAAndDInD = "D=D+A\n";

        private const string PointToM = "A=M\n";
        private const string PointToDecreasedM = "A=M-1\n";

        private const string LCL = "@LCL\n";
        private const string ARG = "@ARG\n";
        private const string THIS = "@THIS\n";
        private const string THAT = "@THAT\n";

        private const string IncreaseSP = SP + "M=M+1\n";
        private const string IncreaseSPAndPoint = SP + IncreaseMAndPoint;
        private const string DecreaseSPAndPoint = SP + DecreaseMAndPoint;
        private const string PushD = SP + PointToM + StoreDInM;

        private const string R13 = "@R13\n";
        private const string SetAddressToR13 = R13 + StoreDInM;
        private const string PointToR13 = R13 + "A=M\n";
        private const string DecreaseR13AndStorePointingValueToD = R13 + DecreaseMAndPoint + StoreMInD;
        private const string R14 = "@R14\n";

        private const string LoadLabelOrBaseAddressPlaceholder = "@{0}\n";
        private const string LoadVarOffsetOrNumberOfArgumentsPlaceholder = "@{1}\n";

        private const string UnconditionalJump = "0;JMP\n";

        public static readonly string SetSPTo256 = "@256\n" + StoreAInD + SP + StoreDInM;

        #region Push And Pop
        public static readonly string PushConstant = "@{0}\n" + StoreAInD + PushD + IncreaseSP;

        public static readonly string PushLocalArgThisThat =
            LoadLabelOrBaseAddressPlaceholder +
            StoreMInD +
            LoadVarOffsetOrNumberOfArgumentsPlaceholder +
            StoreSumOfAAndDInA +
            StoreMInD +
            PushD +
            IncreaseSP;

        public static readonly string PopLocalArgThisThat =
            LoadLabelOrBaseAddressPlaceholder +
            StoreMInD +
            LoadVarOffsetOrNumberOfArgumentsPlaceholder +
            StoreSumOfAAndDInD +
            SetAddressToR13 +
            DecreaseSPAndPoint +
            StoreMInD +
            PointToR13 +
            StoreDInM;

        public static readonly string PushTemp =
            LoadLabelOrBaseAddressPlaceholder +
            StoreAInD +
            LoadVarOffsetOrNumberOfArgumentsPlaceholder +
            StoreSumOfAAndDInA +
            StoreMInD +
            PushD +
            IncreaseSPAndPoint;

        public static readonly string PopTemp =
            LoadLabelOrBaseAddressPlaceholder +
            StoreAInD +
            LoadVarOffsetOrNumberOfArgumentsPlaceholder +
            StoreSumOfAAndDInD +
            SetAddressToR13 +
            DecreaseSPAndPoint +
            StoreMInD +
            PointToR13 +
            StoreDInM;

        public static readonly string PushStatic =
            LoadLabelOrBaseAddressPlaceholder +
            StoreMInD +
            PushD +
            IncreaseSPAndPoint;

        public static readonly string PopStatic =
            LoadLabelOrBaseAddressPlaceholder +
            StoreAInD +
            SetAddressToR13 +
            DecreaseSPAndPoint +
            StoreMInD +
            PointToR13 +
            StoreDInM;

        public static readonly string PushPointer =
            LoadLabelOrBaseAddressPlaceholder +
            StoreMInD +
            PushD +
            IncreaseSP;

        public static readonly string PopPointer =
            DecreaseSPAndPoint +
            StoreMInD +
            LoadLabelOrBaseAddressPlaceholder +
            StoreDInM;
        #endregion

        #region Arithmetic Operations
        public static readonly string UnaryOperation =
            SP +
            PointToDecreasedM +
            "M={0}M\n";

        private const string PrepareBinaryOperation =
            SP +
            DecreaseSPAndPoint +
            StoreMInD +
            "A=A-1\n";

        public static readonly string BinaryOperation =
            PrepareBinaryOperation +
            "M=M{0}D\n";

        public static readonly string Compare =
            PrepareBinaryOperation +
            "D=D-M\n" +
            "@COMPARE_TRUE_{1}\n" +
            "D;{0}\n" +
            SP +
            PointToDecreasedM +
            "M=-1\n" +
            "@COMPARE_END_{1}\n" +
            UnconditionalJump +
            "(COMPARE_TRUE_{1})\n" +
            SP +
            PointToDecreasedM +
            "M=0\n" +
            "(COMPARE_END_{1})\n";
        #endregion

        #region Branching
        public static readonly string Label =
            "({0})\n";

        public static readonly string Goto =
            LoadLabelOrBaseAddressPlaceholder +
            UnconditionalJump;

        public static readonly string If =
            DecreaseSPAndPoint +
            StoreMInD +
            LoadLabelOrBaseAddressPlaceholder +
            "D;JNE\n";
        #endregion

        #region Functions
        public static readonly string Function =
            // Function label
            "({0})\n" +
            // The code generated here initializes the local variables of the function.
            // This code is generated in the caller's code and is only inserted here.
            "{1}\n";

        public const string InitializeLocalVarOfFunction =
            SP +
            PointToM +
            "M=0\n" +
            IncreaseSPAndPoint;

        public static readonly string Call =
            // Push return address onto the stack 
            LoadLabelOrBaseAddressPlaceholder +
            StoreAInD +
            PushD +
            IncreaseSPAndPoint +
            // Push LCL pointer onto the stack
            LCL +
            StoreMInD +
            PushD +
            IncreaseSPAndPoint +
            // Push ARG pointer onto the stack
            ARG +
            StoreMInD +
            PushD +
            IncreaseSPAndPoint +
            // Push THIS pointer onto the stack
            THIS +
            StoreMInD +
            PushD +
            IncreaseSPAndPoint +
            // Push THAT pointer onto the stack
            THAT +
            StoreMInD +
            PushD +
            IncreaseSPAndPoint +
            // Set LCL pointer to SP
            SP +
            StoreMInD +
            LCL +
            StoreDInM +
            // Set ARG pointer to start of arguments in current frame
            "@5\n" +
            "D=D-A\n" +
            LoadVarOffsetOrNumberOfArgumentsPlaceholder +
            "D=D-A\n" +
            ARG +
            StoreDInM +
            // Loads the address of function 
            "@{2}\n" +
            UnconditionalJump +
            "({0})";

        public const string Return =
            // Copy LCL to R13
            LCL +
            StoreMInD +
            SetAddressToR13 +
            // Calculate return address + store it in R14
            "@5\n" +
            "A=D-A\n" +
            StoreMInD +
            R14 +
            StoreDInM +
            // Move result to beginning of the ARG segment
            DecreaseSPAndPoint +
            StoreMInD +
            ARG +
            PointToM +
            StoreDInM +
            // Move SP to one after ARG
            ARG +
            StoreMInD +
            SP +
            "M=D+1\n" +
            // Restore THAT
            DecreaseR13AndStorePointingValueToD +
            THAT +
            StoreDInM +
            // Restore THIS
            DecreaseR13AndStorePointingValueToD +
            THIS +
            StoreDInM +
            // Restore ARG
            DecreaseR13AndStorePointingValueToD +
            ARG +
            StoreDInM +
            // Restore LCL
            DecreaseR13AndStorePointingValueToD +
            LCL +
            StoreDInM +
            // Jump to return address
            R14 +
            PointToM +
            UnconditionalJump;
        #endregion
    }
}
