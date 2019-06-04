using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dvinun.DesignPatterns.Behavioral
{
    partial class Interpreter
    {
        interface IExpression
        {
            bool Interpret(Instruction instruction);
        }

        class ExpressionUtils
        {
            public delegate void RegexMatchCallback(Instruction instruction, string value);

            public static bool RegexMatch(string pattern, Instruction instruction, RegexMatchCallback callback = null)
            {
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                string currentCommand = instruction.GetCurrentCommand();
                Match match = regex.Match(currentCommand);
                if (match.Success)
                {
                    instruction.SetCurrentCommand(Utils.Trim(instruction.GetCurrentCommand(), match.Value));
                    callback?.Invoke(instruction, match.Value);
                }
                Utils.PrintRegexMatch(match, System.Reflection.MethodBase.GetCurrentMethod());
                return match.Success;
            }
        }

        // Expression
        class MainExpression : IExpression
        {
            GreetingExp greetingExp = new GreetingExp();
            ActionExp actionExp = new ActionExp();

            public bool Interpret(Instruction instruction)
            {
                bool matchFound = greetingExp.Interpret(instruction);
                if (matchFound) matchFound = actionExp.Interpret(instruction);
                return matchFound;
            }
        }

        class GreetingExp : IExpression
        {
            InterjectionExp interjectionExp = null;
            VirtualAssistantExp virtualAssistantExp = null;
            PunctuationExp punctuationExp = null;

            public GreetingExp()
            {
                interjectionExp = new InterjectionExp();
                virtualAssistantExp = new VirtualAssistantExp();
                punctuationExp = new PunctuationExp();
            }

            public bool Interpret(Instruction instruction)
            {
                // we dont care about interjection but just log
                interjectionExp.Interpret(instruction);

                // try to match for virtual assistant. 
                bool matchFound = virtualAssistantExp.Interpret(instruction);

                // match punctuation only if the virtual assistant is found.
                if (matchFound) matchFound = punctuationExp.Interpret(instruction);

                return matchFound;
            }
        }

        class InterjectionExp : IExpression
        {
            const string pattern = @"^(hi|hey|hello)";

            public bool Interpret(Instruction instruction)
            {
                return ExpressionUtils.RegexMatch(pattern, instruction, regexMatchCallback);
            }

            void regexMatchCallback(Instruction instruction, string value)
            {
                instruction.SetGreetingInterjection(value);
            }
        }

        class PunctuationExp : IExpression
        {
            const string pattern = @"^(!|,|;|\.)";

            public bool Interpret(Instruction instruction)
            {
                return ExpressionUtils.RegexMatch(pattern, instruction);
            }
        }

        // Terminal Expression
        class VirtualAssistantExp : IExpression
        {
            const string pattern = @"^(alexa|google home|siri|cortana)";

            public bool Interpret(Instruction instruction)
            {
                return ExpressionUtils.RegexMatch(pattern, instruction, regexMatchCallback);
            }

            void regexMatchCallback(Instruction instruction, string value)
            {
                instruction.SetVirtualAssistant(value);
            }
        }

        class ActionExp : IExpression
        {
            ActionVerbExp actionVerbExp = new ActionVerbExp();
            ArticleExp articleExp = new ArticleExp();
            ApplianceExp applianceExp = new ApplianceExp();
            PunctuationExp punctuationExp = new PunctuationExp();
            PrepositionExp prepositionExp = new PrepositionExp();
            ActionParamsExp actionParamsExp = new ActionParamsExp();

            public bool Interpret(Instruction instruction)
            {
                bool matchFound = actionVerbExp.Interpret(instruction);
                // continue only if the match found
                if (!matchFound) return false;

                matchFound = articleExp.Interpret(instruction);
                // dont care if the article is not used in the instruction
                // if (!matchFound) return false

                matchFound = applianceExp.Interpret(instruction);
                if (!matchFound) return false;

                matchFound = punctuationExp.Interpret(instruction);
                // if the punctuation doesnt exist, then we got to parse action params
                if (!matchFound)
                {
                    bool actionParamsmatchFound = prepositionExp.Interpret(instruction);

                    if (actionParamsmatchFound) actionParamsmatchFound = actionParamsExp.Interpret(instruction);

                    actionParamsmatchFound = punctuationExp.Interpret(instruction);
                    matchFound = actionParamsmatchFound;
                }

                return matchFound;
            }
        }

        class PrepositionExp : IExpression
        {
            const string pattern = @"^(at|by|for|from|in|into|on|to|with)";

            public bool Interpret(Instruction instruction)
            {
                return ExpressionUtils.RegexMatch(pattern, instruction);
            }
        }

        class ActionVerbExp : IExpression
        {
            const string pattern = @"^(turn-on|turn-off|prepare|close|feed|set)";

            public bool Interpret(Instruction instruction)
            {
                return ExpressionUtils.RegexMatch(pattern, instruction, regexMatchCallback);
            }

            void regexMatchCallback(Instruction instruction, string value)
            {
                instruction.SetActionType(GetActionType(value));
            }

            ActionType GetActionType(string actionType)
            {
                if (actionType.Equals("turn-on", StringComparison.OrdinalIgnoreCase))
                {
                    return ActionType.TurnOn;
                }
                else if (actionType.Equals("turn-off", StringComparison.OrdinalIgnoreCase))
                {
                    return ActionType.TurnOff;
                }
                else if (actionType.Equals("prepare", StringComparison.OrdinalIgnoreCase))
                {
                    return ActionType.Prepare;
                }
                else if (actionType.Equals("close", StringComparison.OrdinalIgnoreCase))
                {
                    return ActionType.Close;
                }
                else if (actionType.Equals("feed", StringComparison.OrdinalIgnoreCase))
                {
                    return ActionType.Feed;
                }
                else if (actionType.Equals("set", StringComparison.OrdinalIgnoreCase))
                {
                    return ActionType.Set;
                }
                else return ActionType.None;
            }
        }

        class ArticleExp : IExpression
        {
            const string pattern = @"^(a|an|the|my)";

            public bool Interpret(Instruction instruction)
            {
                return ExpressionUtils.RegexMatch(pattern, instruction);
            }
        }

        // Terminal Expression
        class ApplianceExp : IExpression
        {
            public bool Interpret(Instruction instruction)
            {
                StringBuilder patternStringBuilder = new StringBuilder();
                patternStringBuilder.Append("^(");
                patternStringBuilder.Append(String.Join("|", instruction.GetAppliances().Select(item => item.GetNickName()).ToArray()));
                patternStringBuilder.Append(")");
                string pattern = patternStringBuilder.ToString();
                return ExpressionUtils.RegexMatch(pattern, instruction, regexMatchCallback);
            }

            void regexMatchCallback(Instruction instruction, string value)
            {
                instruction.SetApplianceName(value);
                instruction.SetApplianceType(GetApplianceType(instruction.GetAppliances(), value));
            }

            ApplianceType GetApplianceType(Appliance[] appliances, string applianceName)
            {
                return appliances.Single(item => item.GetApplianceNickName().ToLower() == applianceName.ToLower()).GetApplianceType();
            }
        }

        class ActionParamsExp : IExpression
        {
            DurationExp durationExp = new DurationExp();
            TimePeriodExp timePeriodExp = new TimePeriodExp();

            public bool Interpret(Instruction instruction)
            {
                // durationExp or timePeriodExp but not both of them
                bool matchFound = durationExp.Interpret(instruction);

                if (!matchFound) matchFound = timePeriodExp.Interpret(instruction);

                return matchFound;
            }
        }

        class DurationExp : IExpression
        {
            string pattern = "^[1-5][0-9]{0,2} (mins?|hours?)";

            public bool Interpret(Instruction instruction)
            {
                return ExpressionUtils.RegexMatch(pattern, instruction, regexMatchCallback);
            }

            void regexMatchCallback(Instruction instruction, string value)
            {
                ActionParams actionParams = new ActionParams();
                actionParams.Duration = value;
                instruction.SetActionParams(actionParams);
            }
        }

        class TimePeriodExp : IExpression
        {
            string pattern = "^(tomorrow|today) ((([1-9]|0[1-9]|1[0-2])((.)(00|15|30|45))? ?(am|pm))|morning|evening|night|noon)";

            public bool Interpret(Instruction instruction)
            {
                return ExpressionUtils.RegexMatch(pattern, instruction, regexMatchCallback);
            }

            void regexMatchCallback(Instruction instruction, string value)
            {
                ActionParams actionParams = new ActionParams();
                actionParams.TimePeriod = value;
                instruction.SetActionParams(actionParams);
            }

        }
    }
}
