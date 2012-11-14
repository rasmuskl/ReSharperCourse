using System;
using System.Collections.Generic;
using System.Threading;
using Basic.Support.Proc;

namespace Basic.Context.Misc
{
    public class ContextClass
    {
        private int quantity;
        private string _name;

        public ContextClass(int quantity, string name, int max)
        {
            this.quantity = quantity;
            _name = name;
        }

        private void AddQuantity()
        {
            quantity++;
        }

        public void CreateMore()
        {
            string[] dataValues = new[] { "a", "b", "c" };

            if (_name != null && _name != "")
            {
                try
                {
                    foreach (var data in dataValues)
                    {
                        if (data != "a")
                        {
                            DataProcessor.DoProcessing(data)
                            quantity++;
                        }
                    }

                    string dataA = null;
                    foreach (var data in dataValues)
                    {
                        if (data == "a")
                        {
                            dataA = data;
                            break;
                        }
                    }

                    Console.Out.WriteLine(dataA);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void DoLess(string name)
        {
            Console.Out.WriteLine(nam);
        }
    }
}