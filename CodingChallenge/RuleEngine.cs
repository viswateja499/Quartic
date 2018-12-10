using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodingChallenge
{
    public class RuleEngine
    {
        private List<string> rules;

        private List<Rule> processedRules;

        public RuleEngine(List<string> rules)
        {
            this.rules = rules;
            this.processedRules = new List<Rule>();
        }

        public void ProcessRules()
        {
            foreach (var rule in rules)
            {
                if (IsRuleValid(rule))
                {
                    var format = rule.Split(' ');
                    var processedRule = new Rule()
                    {
                        SignalName = format[0],
                        Condition = SetCondition(format[1]),
                        Value = format[2]
                    };

                    this.processedRules.Add(processedRule);
                }
                else
                {
                    Console.WriteLine("{0} -- Not Valid", rule);
                }
            }
        }

        public List<Data> ValidateData(List<Data> data)
        {
            var invalidData = new List<Data>();
            foreach (var rule in processedRules)
            {
                if (data.Any(d => d.Signal.ToLower() == rule.SignalName.ToLower()))
                {
                    if (data.OfType<Data<int>>().Any(d => d.Signal.ToLower() == rule.SignalName.ToLower()))
                    {
                        double ruleValue = 0;
                        if (double.TryParse(rule.Value, out ruleValue))
                        {
                            var value = Convert.ToInt32(Math.Round(Convert.ToDouble(rule.Value)));

                            foreach (var validateDataSet in data.OfType<Data<int>>())
                            {
                                if (!ValidateNumericData(rule, validateDataSet, value))
                                {
                                    invalidData.Add(validateDataSet);
                                }
                            }

                        }
                    }

                    if (data.OfType<Data<DateTime>>().Any(d => d.Signal.ToLower() == rule.SignalName.ToLower()))
                    {
                        if (rule.Value.ToLower() == "future" || rule.Value.ToLower() == "present" || rule.Value.ToLower() == "past")
                        {
                            foreach (var validateDataSet in data.OfType<Data<DateTime>>())
                            {
                                if (!ValidateDateTimeData(rule, validateDataSet))
                                {
                                    invalidData.Add(validateDataSet);
                                }
                            }
                        }
                    }

                    if (data.OfType<Data<string>>().Any(d => d.Signal.ToLower() == rule.SignalName.ToLower()))
                    {
                        if (rule.Value.ToLower() == "low" || rule.Value.ToLower() == "high")
                        {
                            foreach (var validateDataSet in data.OfType<Data<string>>())
                            {
                                if (!ValidatestringData(rule, validateDataSet))
                                {
                                    invalidData.Add(validateDataSet);
                                }
                            }
                        }
                    }
                }
            }

            return invalidData;
        }

        private bool ValidatestringData(Rule rule, Data<string> validateDataSet)
        {
            if (Condition.Equals == rule.Condition)
            {
                return validateDataSet.Value.ToLower() == rule.Value.ToLower();
            }
            else if (Condition.NotEqual == rule.Condition)
            {
                return validateDataSet.Value.ToLower() != rule.Value.ToLower();
            }

            return false;
        }

        private bool ValidateDateTimeData(Rule rule, Data<DateTime> validateDataSet)
        {
            var currentYear = DateTime.Now.Year;

            if (Condition.In == rule.Condition)
            {
                if (rule.Value == "Future")
                {
                   return validateDataSet.Value.Year > currentYear;
                }
                else if (rule.Value == "Present")
                {
                    return validateDataSet.Value.Year == currentYear;
                }
                else if (rule.Value == "Past")
                {
                    return validateDataSet.Value.Year < currentYear;
                }
            }
            else if (Condition.NotIn == rule.Condition)
            {
                if (rule.Value == "Future")
                {
                    return validateDataSet.Value.Year < currentYear;
                }
                else if (rule.Value == "Present")
                {
                    return validateDataSet.Value.Year != currentYear;
                }
                else if (rule.Value == "Past")
                {
                    return validateDataSet.Value.Year > currentYear;
                }
            }

            return false;
        }

        private bool ValidateNumericData(Rule rule, Data<int> validateDataSet, int value)
        {
            if (rule.Condition == Condition.Greater)
            {
                return validateDataSet.Value > value;
            }
            else if (rule.Condition == Condition.Equals)
            {
                return validateDataSet.Value == value;
            }
            else if (rule.Condition == Condition.Lesser)
            {
                return validateDataSet.Value < value;
            }
            else if (rule.Condition == Condition.NotEqual)
            {
                return validateDataSet.Value != value;
            }

            return false;
        }

        private Condition SetCondition(string condition)
        {
            Condition outPut;
            switch (condition)
            {
                case ">":
                    outPut = Condition.Greater;
                    break;

                case "<":
                    outPut = Condition.Lesser;
                    break;

                case "==":
                    outPut = Condition.Equals;
                    break;

                case "!=":
                    outPut = Condition.NotEqual;
                    break;

                case "In":
                    outPut = Condition.In;
                    break;

                case "NotIn":
                    outPut = Condition.NotIn;
                    break;

                default:
                    outPut = Condition.None;
                    break;
            }

            return outPut;
        }

        private bool IsRuleValid(string rule)
        {
            if (string.IsNullOrEmpty(rule) || string.IsNullOrWhiteSpace(rule))
            {
                return false;
            }

            rule = this.RemoveExtraSpaces(rule);
            var format = rule.Split(' ');

            if (format.Length != 3)
            {
                return false;
            }

            return true;
        }

        private string RemoveExtraSpaces(string rule)
        {
            rule = rule.Trim();

            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            rule = regex.Replace(rule, " ");

            return rule;
        }
    }
}
