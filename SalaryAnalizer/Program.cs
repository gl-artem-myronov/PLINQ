using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SalaryAnalizer
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = @"company.csv";

            var src = File.ReadLines(filename, Encoding.Default)
                .AsParallel()
                .Skip(1)
                .Select(x => new Member(x))
                .ToList();

            var avgSalaryByDepartment =
                src
                    .AsParallel()
                    .GroupBy(
                        x => x.Department,
                        (dep, members) => new { Department = dep, AvgSalary = members.Average(x => x.Salary) }
                        )
                    .ToList();

            var groups = new[]{
                new {min = 17, max=30},
                new {min = 31, max=45},
                new {min = 46, max = int.MaxValue}
                };

            var maxSalaryByGroup =
                src
                    .AsParallel()
                    .GroupBy(
                        member => groups.First(group => group.min <= member.Age && group.max >= member.Age),
                        (x, y) => (x, y.Max(m => m.Salary))
                        )
                    .ToList();
        }
    }

    class Member
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public int Age { get; set; }
        public int Salary { get; set; }

        public Member(string src)
        {
            var details = src.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            FirstName = details[0];
            LastName = details[1];
            Department = details[2];
            Age = int.Parse(details[3]);
            Salary = int.Parse(details[4]);
        }
    }

}
