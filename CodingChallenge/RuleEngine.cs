﻿using System;
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

        public List<Data> ValidateData(Dictionary<string, List<Data>> data)
        {
            var invalidData = new List<Data>();
            foreach (var rule in processedRules)
            {
                List<Data> dataSet;

                if (data.TryGetValue(rule.SignalName, out dataSet))
                {
                    if (dataSet.OfType<Data<int>>().Any())
                    {
                        double ruleValue = 0;
                        if (double.TryParse(rule.Value, out ruleValue))
                        {
                            var value = Convert.ToInt32(Math.Round(Convert.ToDouble(rule.Value)));

                            foreach (var validateDataSet in dataSet.OfType<Data<int>>())
                            {
                                if (!ValidateNumericData(rule, validateDataSet, value))
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