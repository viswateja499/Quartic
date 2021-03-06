﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace CodingChallenge
{
    public class Processor
    {
        public Processor()
        {
            DataSet = new List<Data>();
        }

        public List<Data> DataSet { get; set; }

        public RuleEngine RuleEngine { get; set; }

        public bool ReadAndSetFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("Given Path is Not Valid");
                return false;
            }

            if (File.Exists(path))
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    dynamic items = JsonConvert.DeserializeObject(json);

                    foreach (var obj in items)
                    {
                        Data data = CreateDataModel(obj);
                        DataSet.Add(data);

                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public void ValidateData()
        {
            var invalidData = RuleEngine.ValidateData(DataSet);
            if (invalidData.Any())
            {
                Console.WriteLine("Invalid Data:");
            }
            else
            {
                Console.WriteLine("No Invalid Data");
            }

            foreach (var invalid in invalidData)
            {
                if (invalid is Data<int>)
                {
                    var data = invalid as Data<int>;
                    Console.WriteLine("{0} {1}", data.Signal, data.Value);
                }
                else if (invalid is Data<DateTime>)
                {
                    var data = invalid as Data<DateTime>;
                    Console.WriteLine("{0} {1}", data.Signal, data.Value);
                }
                else if (invalid is Data<string>)
                {
                    var data = invalid as Data<string>;
                    Console.WriteLine("{0} {1}", data.Signal, data.Value);
                }
            }
        }

        public void ReadRulesFromUser()
        {
            Console.WriteLine("Please enter custom rules to execute on data");
            Console.WriteLine("Rules Format");
            Console.WriteLine("1) Integer: Signal Condition(>, <, ==)  Value");
            Console.WriteLine("2) String: Signal Condition(==, !=) Value");
            Console.WriteLine("2) Datetime: Signal Condition(In, NotIn) Value(Present, Future, Past)");
            List<string> rules = new List<string>();

            // first read input till there are nonempty items, means they are not null and not ""
            // also add read item to list do not need to read it again    
            string line;
            while ((line = Console.ReadLine()) != null && line != "")
            {
                rules.Add(line);
            }

            this.RuleEngine = new RuleEngine(rules);
            RuleEngine.ProcessRules();
        }

        private Data CreateDataModel(dynamic obj)
        {
            string s = obj.value_type;

            if (obj.value_type != null)
            {
                switch (s)
                {
                    case "Integer":
                        var model = new Data<int>();
                        model.Signal = obj.signal != null ? obj.signal : string.Empty;
                        string tempValue = obj.value;
                        model.Value = Convert.ToInt32(Math.Round(Convert.ToDouble(tempValue)));

                        return model;

                    case "String":
                        var stringModel = new Data<string>();
                        stringModel.Signal = obj.signal != null ? obj.signal : string.Empty;
                        stringModel.Value = Convert.ToString(obj.value);
                        return stringModel;

                    case "Datetime":
                        var dateModel = new Data<DateTime>();
                        dateModel.Signal = obj.signal != null ? obj.signal : string.Empty;
                        dateModel.Value = Convert.ToDateTime(obj.value);
                        return dateModel;

                    default:
                        var defaultModel = new Data<int>();
                        defaultModel.Signal = obj.signal != null ? obj.signal : string.Empty;
                        defaultModel.Value = -1;
                        return defaultModel;

                }
            }

            return null;
        }
    }
}
