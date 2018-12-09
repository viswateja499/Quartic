using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace CodingChallenge
{
    public class Processor
    {
        public Processor()
        {
            DataSet = new Dictionary<string, List<Data>>();
        }

        public Dictionary<string, List<Data>> DataSet { get; set; }

        public RuleEngine RuleEngine { get; set; }

        public async void ReadAndSetFileAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("Given Path is Not Valid");
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
                        if (DataSet.ContainsKey(data.Signal))
                        {
                            List<Data> signalData;
                            DataSet.TryGetValue(data.Signal, out signalData);
                            signalData.Add(data);
                        }
                        else
                        {
                            var signalData = new List<Data>();
                            signalData.Add(data);
                            DataSet.Add(data.Signal, signalData);
                        }
                    }
                }
            }
        }

        public void ValidateData()
        {
            var invalidData = RuleEngine.ValidateData(DataSet);
            foreach (var invalid in invalidData)
            {
                Console.WriteLine("{0}", invalid.Signal);
            }
        }

        public void ReadRulesFromUser()
        {
            Console.WriteLine("Please enter custom rules to execute on data");
            Console.WriteLine("Rules Format");
            Console.WriteLine("1) Integer: Signal Condition(>, <, ==)  Value");
            Console.WriteLine("2) String: Signal Condition(==, !=) Value");
            Console.WriteLine("2) Datetime: Signal Condition(>, <, ==) Value(Year)");
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
