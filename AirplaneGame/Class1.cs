/******************************************************************************

                            Online C# Compiler.
                Code, Compile, Run and Debug C# program online.
Write your code in this editor and press "Run" button to execute it.

*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HelloWorld
{

    public class test
    {
        public int id;
        public string name;
        public test(int i, string s)
        {
            id = i;
            name = s;
        }
    }

    public void setList(test te)
    {
        te.id = 5;
        te.name = "World";
    }

    public List<test> testList = new List<test>();

    static void Main()
    {
        Console.WriteLine("Hello World");
        testList.Add(new test(0, "Hello"));
        Console.WriteLine(testList[0]);
        setList(testList[0]);
        Console.WriteLine(testList[0]);
    }


}
