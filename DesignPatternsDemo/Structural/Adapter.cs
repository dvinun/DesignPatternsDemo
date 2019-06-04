using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dvinun.DesignPatterns.Structural
{
    class Adapter
    {
        // In this demo, we will try use adapter pattern by making use of third party 
        // payroll module AdamPayroll and implement a new module Strawberry Payroll Service.
        public static void PlayDemo()
        {
            StrawberryPayrollService strawberryPayrollService = new StrawberryPayrollService();
            double salary = strawberryPayrollService.CalculatePayroll("287UBU123");
            Console.WriteLine("Salary: " + salary);
        }

        // Adapter interface
        interface IStrawberryPayrollService
        {
            double CalculatePayroll(string employeeId);
        }

        // Adaptee
        class AdamMoversPayroll
        {
            protected double CalculateMedianSalary(int yearsOfExperience)
            {
                return yearsOfExperience * 1000;
            }

            protected double CalculateEmployeeMedianSalary(string employeeId)
            {
                int yearsOfExperiene = 8; // Get the employee's experience somehow
                return CalculateMedianSalary(yearsOfExperiene);
            }
        }

        // Adapter concrete
        class StrawberryPayrollService : AdamMoversPayroll, IStrawberryPayrollService
        {
            public double CalculatePayroll(string employeeId)
            {
                int yearsOfExperiene = 9; // Get the employee's experience somehow

                return CalculateMedianSalary(yearsOfExperiene) / 2;
            }
        }
    }
}
