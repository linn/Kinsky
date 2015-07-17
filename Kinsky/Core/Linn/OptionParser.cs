using System;
using System.Text;
using System.Collections.Generic;

namespace Linn
{
    public class OptionParser
    {
        public class OptionParserError : System.Exception
        {
            public OptionParserError(string aMsg)
                : base(aMsg)
            { }
        }

        public abstract class Option
        {
            public Option(string aShortName, string aLongName)
            {
                Assert.Check(aShortName != null || aLongName != null);
                if (aShortName != null)
                {
                    // short names must be of form "-(letter)" e.g "-d"
                    Assert.Check(aShortName.StartsWith("-") == true);
                    Assert.Check(aShortName.StartsWith("--") == false);
                    Assert.Check(aShortName.Length == 2);
                    iShortName = aShortName;
                }
                if (aLongName != null)
                {
                    // long names must be of form "--(word)" e.g "--word"
                    Assert.Check(aLongName.StartsWith("--") == true);
                    Assert.Check(aLongName.Length > 2);
                    iLongName = aLongName;
                }
            }

            public bool Match(string aName)
            {
                return (aName == iShortName || aName == iLongName);
            }

            public abstract void Process(string[] aOptArgs);
            public abstract void Reset();
            public abstract int ExpectedArgCount();
            public abstract void AppendHelp(OptionHelp aHelp);

            public string ShortName
            {
                get { return iShortName; }
            }

            public string LongName
            {
                get { return iLongName; }
            }

            protected string iShortName = null;
            protected string iLongName = null;
        }

        public class OptionString : Option
        {
            public OptionString(string aShortName, string aLongName, string aDefault, string aHelpDesc, string aHelpMetaVar)
                : base(aShortName, aLongName)
            {
                Assert.Check(aHelpDesc != null);
                Assert.Check(aHelpMetaVar != null);
                iValue = aDefault;
                iDefault = aDefault;
                iHelpDesc = aHelpDesc;
                iHelpMetaVar = aHelpMetaVar;
            }

            public override void Process(string[] aOptArgs)
            {
                Assert.Check(aOptArgs.Length == 1);
                iValue = aOptArgs[0];
            }

            public override void Reset()
            {
                iValue = iDefault;
            }

            public override int ExpectedArgCount()
            {
                return 1;
            }

            public override void AppendHelp(OptionHelp aHelp)
            {
                aHelp.Append(iShortName, iLongName, iHelpDesc, iHelpMetaVar);
            }

            public string Value
            {
                get { return iValue; }
            }

            private string iValue;
            private string iDefault;
            private string iHelpDesc;
            private string iHelpMetaVar;
        }

        public class OptionInt : Option
        {
            public OptionInt(string aShortName, string aLongName, int aDefault, string aHelpDesc, string aHelpMetaVar)
                : base(aShortName, aLongName)
            {
                Assert.Check(aHelpDesc != null);
                Assert.Check(aHelpMetaVar != null);
                iValue = aDefault;
                iDefault = aDefault;
                iHelpDesc = aHelpDesc;
                iHelpMetaVar = aHelpMetaVar;
            }

            public override void Process(string[] aOptArgs)
            {
                Assert.Check(aOptArgs.Length == 1);
                try
                {
                    iValue = Convert.ToInt32(aOptArgs[0]);
                }
                catch (System.FormatException)
                {
                    if (iShortName == null)
                    {
                        throw new OptionParserError("OptionInt " + iLongName + " has a non-integer value " + aOptArgs[0]);
                    }
                    else if (iLongName == null)
                    {
                        throw new OptionParserError("OptionInt " + iShortName + " has a non-integer value " + aOptArgs[0]);
                    }
                    else
                    {
                        throw new OptionParserError("OptionInt " + iShortName + "/" + iLongName + " has a non-integer value " + aOptArgs[0]);
                    }
                }
            }

            public override void Reset()
            {
                iValue = iDefault;
            }

            public override int ExpectedArgCount()
            {
                return 1;
            }

            public override void AppendHelp(OptionHelp aHelp)
            {
                aHelp.Append(iShortName, iLongName, iHelpDesc, iHelpMetaVar);
            }

            public int Value
            {
                get { return iValue; }
            }

            private int iValue;
            private int iDefault;
            private string iHelpDesc;
            private string iHelpMetaVar;
        }

        public class OptionUint : Option
        {
            public OptionUint(string aShortName, string aLongName, uint aDefault, string aHelpDesc, string aHelpMetaVar)
                : base(aShortName, aLongName)
            {
                Assert.Check(aHelpDesc != null);
                Assert.Check(aHelpMetaVar != null);
                iValue = aDefault;
                iDefault = aDefault;
                iHelpDesc = aHelpDesc;
                iHelpMetaVar = aHelpMetaVar;
            }

            public override void Process(string[] aOptArgs)
            {
                Assert.Check(aOptArgs.Length == 1);
                try
                {
                    iValue = Convert.ToUInt32(aOptArgs[0]);
                }
                catch (System.FormatException)
                {
                    if (iShortName == null)
                    {
                        throw new OptionParserError("OptionInt " + iLongName + " has a non-integer value " + aOptArgs[0]);
                    }
                    else if (iLongName == null)
                    {
                        throw new OptionParserError("OptionInt " + iShortName + " has a non-integer value " + aOptArgs[0]);
                    }
                    else
                    {
                        throw new OptionParserError("OptionInt " + iShortName + "/" + iLongName + " has a non-integer value " + aOptArgs[0]);
                    }
                }
            }

            public override void Reset()
            {
                iValue = iDefault;
            }

            public override int ExpectedArgCount()
            {
                return 1;
            }

            public override void AppendHelp(OptionHelp aHelp)
            {
                aHelp.Append(iShortName, iLongName, iHelpDesc, iHelpMetaVar);
            }

            public uint Value
            {
                get { return iValue; }
            }

            private uint iValue;
            private uint iDefault;
            private string iHelpDesc;
            private string iHelpMetaVar;
        }

        public class OptionBool : Option
        {
            public OptionBool(string aShortName, string aLongName, string aHelpDesc)
                : base(aShortName, aLongName)
            {
                Assert.Check(aHelpDesc != null);
                iValue = false;
                iHelpDesc = aHelpDesc;
            }

            public override void Process(string[] aOptArgs)
            {
                Assert.Check(aOptArgs.Length == 0);
                iValue = true;
            }

            public override void Reset()
            {
                iValue = false;
            }

            public override int ExpectedArgCount()
            {
                return 0;
            }

            public override void AppendHelp(OptionHelp aHelp)
            {
                aHelp.Append(iShortName, iLongName, iHelpDesc);
            }

            public bool Value
            {
                get { return iValue; }
            }

            private bool iValue;
            private string iHelpDesc;
        }

        public class OptionHelp
        {
            public static int kMaxNameLength = 22;

            public OptionHelp()
            {
                iText = "options:\n";
            }

            public void Append(string aShortName, string aLongName, string aDesc)
            {
                string name = ConstructName(aShortName, aLongName);
                iText += Construct(name, aDesc);
            }

            public void Append(string aShortName, string aLongName, string aDesc, string aMetaVar)
            {
                string name = ConstructName(aShortName, aLongName, aMetaVar);
                iText += Construct(name, aDesc);
            }

            public override string ToString()
            {
                return iText;
            }

            private string Construct(string aName, string aDesc)
            {
                StringBuilder help = new StringBuilder(aName);
                if (aName.Length > kMaxNameLength)
                {
                    help.Append("\n");
                    help.Append(' ', kMaxNameLength + 2);
                }
                else
                {
                    help.Append(' ', kMaxNameLength + 2 - aName.Length);
                }
                help.Append(aDesc);
                help.Append("\n");
                return help.ToString();
            }

            private string ConstructName(string aShortName, string aLongName, string aMetaVar)
            {
                Assert.Check(aShortName != null || aLongName != null);
                Assert.Check(aMetaVar != null);
                string help = "  ";
                if (aShortName != null && aLongName != null)
                {
                    help = "  " + aShortName + " " + aMetaVar + ", " + aLongName + "=" + aMetaVar;
                }
                else if (aShortName != null)
                {
                    help = "  " + aShortName + " " + aMetaVar;
                }
                else
                {
                    help = "  " + aLongName + "=" + aMetaVar;
                }
                return help;
            }

            private string ConstructName(string aShortName, string aLongName)
            {
                Assert.Check(aShortName != null || aLongName != null);
                string help = "  ";
                if (aShortName != null && aLongName != null)
                {
                    help = "  " + aShortName + ", " + aLongName;
                }
                else if (aShortName != null)
                {
                    help = "  " + aShortName;
                }
                else
                {
                    help = "  " + aLongName;
                }
                return help;
            }

            private string iText;
        }

        public OptionParser()
        {
            iOptions = new List<Option>();
            iPosArgs = new List<string>();
            iHelpOption = new OptionBool("-h", "--help", "show this help message and exit");
            AddOption(iHelpOption);
        }

        public OptionParser(string[] aArgs)
            : this()
        {
            iArgs = aArgs;
        }

        public void AddOption(Option aOption)
        {
            Assert.Check(Find(aOption.ShortName) == null);
            Assert.Check(Find(aOption.LongName) == null);
            iOptions.Add(aOption);
        }

        public void Parse()
        {
            Parse(iArgs);
        }

        public void Parse(string[] aArgs)
        {
            iPosArgs.Clear();
            foreach (Option opt in iOptions)
            {
                opt.Reset();
            }

            try
            {
                int i = 0;
                while (i < aArgs.Length)
                {
                    Option opt = Find(aArgs[i]);
                    if (opt == null)
                    {
                        // this is not an option - positional argument
                        if (aArgs[i].StartsWith("-"))
                        {
                            // handle argument MonoDevelop adds to commandline
                            if(!aArgs[i].StartsWith("-psn_"))
                            {
                                // this is an unspecified option
                                throw new OptionParserError("No such option: " + aArgs[i]);
                            }
                        }
                        else
                        {
                            iPosArgs.Add(iArgs[i]);
                        }
                        i++;
                    }
                    else
                    {
                        // build an array of the number of arguments this option
                        // is expecting
                        string[] optArgList = new string[opt.ExpectedArgCount()];
                        try
                        {
                            Array.Copy(aArgs, i + 1, optArgList, 0, optArgList.Length);
                        }
                        catch (ArgumentException)
                        {
                            throw new OptionParserError("Option " + aArgs[i] + " has incorrect arguments");
                        }

                        // check if any of the optArgs are actual options
                        foreach (string arg in optArgList)
                        {
                            if (Find(arg) != null)
                            {
                                throw new OptionParserError("Option " + aArgs[i] + " has incorrect arguments");
                            }
                        }

                        // process this option
                        opt.Process(optArgList);
                        i += 1 + optArgList.Length;
                    }
                }
            }
            catch (Exception aExc)
            {
                iPosArgs.Clear();
                foreach (Option opt in iOptions)
                {
                    opt.Reset();
                }
                throw aExc;
            }

            // Check if help option has been set
            if (iHelpOption.Value)
            {
                DisplayHelp();
            }
        }

        public bool HelpSpecified()
        {
            return (iHelpOption.Value);
        }

        public string Help()
        {
            OptionHelp help = new OptionHelp();
            foreach (Option opt in iOptions)
            {
                opt.AppendHelp(help);
            }
            return iUsage + "\n" + help.ToString();
        }

        public void DisplayHelp()
        {
            Console.WriteLine(Help());

            Environment.Exit(1);
        }

        public string Usage
        {
            get { return iUsage; }
            set { iUsage = value; }
        }
        public List<string> PosArgs
        {
            get { return iPosArgs; }
        }

        private Option Find(string aName)
        {
            if (aName == null)
            {
                return null;
            }
            foreach (Option opt in iOptions)
            {
                if (opt.Match(aName) == true)
                {
                    return opt;
                }
            }
            return null;
        }

        private string[] iArgs;
        private List<Option> iOptions;
        private List<string> iPosArgs;
        private OptionBool iHelpOption;
        private string iUsage = "usage:";
    }
}  // namespace Linn


