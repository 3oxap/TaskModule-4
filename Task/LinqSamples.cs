// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
	[Title("LINQ Module")]
	[Prefix("Linq")]
	public class LinqSamples : SampleHarness
	{

		private DataSource dataSource = new DataSource();

		[Category("Restriction Operators")]
		[Title("Where - Task 1")]
		[Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
		public void Linq1()
		{
			int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

			var lowNums =
				from num in numbers
				where num < 5
				select num;

			Console.WriteLine("Numbers < 5:");
			foreach (var x in lowNums)
			{
				Console.WriteLine(x);
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 2")]
		[Description("This sample return return all presented in market products")]

		public void Linq2()
		{
			var products =
				from p in dataSource.Products
				where p.UnitsInStock > 0
				select p;

			foreach (var p in products)
			{
				ObjectDumper.Write(p);
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 3")]
		[Description("Returns all customers whose total turnover exceeds a certain value!")]
		//возвращает всех клиентов, чей суммарный оборот (сумма всех заказов) превосходит некоторую величину X!
		public void Linq3()
        {
			var customer =
				from p in dataSource.Customers
				where p.Orders.Sum(s=>s.Total)>7800
				select p;
			
            foreach (var item in customer)
            {
				ObjectDumper.Write(item);
            }
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 4")]
		[Description("List of suppliers in the same country and the same city!")]
		//список поставщиков в той же стране и том же городе
		public void Linq4()
		{
			var listSupplierJoin =
				from p in dataSource.Customers
				join s in dataSource.Suppliers on p.Country equals s.Country
				select new { CompanyName= p.CompanyName, CountryCompany = p.Country, SupplierName =s.SupplierName, CountrySupplier = s.Country };

			var listSupplier =
				from c in dataSource.Customers
				from s in dataSource.Suppliers
				where c.Country == s.Country
				select new { CompanyName = c.CompanyName, CountryCompany = c.Country, SupplierName = s.SupplierName, CountrySupplier = s.Country };
            foreach (var item in listSupplier)
            {
				ObjectDumper.Write(item);
            }
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 5")]
		[Description("All customers who had orders exceeding the amount!")]
		//все клиентов, у которых были заказы, превосходящие по сумме величину
		public void Linq5()
		{
			var customer =
				from c in dataSource.Customers
				where c.Orders.Any(g=>g.Total>1500)
				select new { CompanyName = c.CompanyName, Orders=c.Orders};

			foreach (var item in customer)
            {	
				ObjectDumper.Write(item.CompanyName);
				ObjectDumper.Write(item.Orders.Select(v=>v));
				
            }
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 6")]
		[Description("List of clients indicating from which month of which year they became clients!")]
		//Cписок клиентов с указанием, начиная с какого месяца какого года они стали клиентами
		public void Linq6()
		{
			DateTime date = new DateTime(1996,2,12);
			var customer = 
            from c in dataSource.Customers
            select new { order = c.Orders.Where(t => t.OrderDate > date).FirstOrDefault(), companyName = c.CompanyName };


            foreach (var item in customer)
            {
                ObjectDumper.Write(item.companyName);
                ObjectDumper.Write(item.order);
            }
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 7")]
		[Description("Previous task, but issue a list sorted by year, month, customer turnover!")]
		//Предыдущее задание, но выдайте список отсортированным по году, месяцу, оборотам клиента и имени
		public void Linq7()
		{

            var customer = dataSource.Customers.Select(o => new
            {
				o.CompanyName,
                orders = from p in o.Orders
                         orderby p.OrderDate.Year descending,p.OrderDate.Month descending, p.Total descending
                         select p
            }
            ).OrderByDescending(o=>o.CompanyName);
            foreach (var item in customer)
            {
				ObjectDumper.Write(item.CompanyName);
				ObjectDumper.Write(item.orders);
            }
		
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 8")]
		[Description("Indicate all customers who have a non-digital postal code or an empty region or an operator code is not specified on the phone!")]
		//Укажите всех клиентов, у которых указан нецифровой почтовый код или не заполнен регион или в телефоне не указан код оператора
		public void Linq8()
		{
			Regex regexPostalCode = new Regex(@"\D{2}");
			Regex regexPhone = new Regex(@"[()]");
			var customer =
				from c in dataSource.Customers
				where c.PostalCode!=null
				where c.Region==null
				select c;

			foreach (var item in customer)
			{
				if (regexPostalCode.IsMatch(item.PostalCode)&& !regexPhone.IsMatch(item.Phone))
                    {
						ObjectDumper.Write(item);
					}
			}

            
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 9")]
		[Description("Group all products by category, inside - by availability in stock, inside the last group, sort by cost!")]
		//Сгруппируйте все продукты по категориям, внутри – по наличию на складе, внутри последней группы отсортируйте по стоимости
		public void Linq9()
		{
			var product =
				from p in dataSource.Products
				group p by new { p.Category, p.UnitsInStock, p.UnitPrice}; 

            foreach (var item in product)
            {
				ObjectDumper.Write(item);
            }
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 10")]
		[Description("Group the products into groups cheap, average price,expensive. Set the boundaries of each group yourself!")]
		//Сгруппируйте товары по группам «дешевые», «средняя цена», «дорогие». Границы каждой группы задайте сами
		public void Linq10()
		{
			var product =
				from p in dataSource.Products
				let cheap = p.UnitPrice < 20
				let averge=p.UnitPrice<45
				let expensive = p.UnitPrice>45 
				
				group p by new { expensive, averge, cheap };

            foreach (var item in product)
            {
				ObjectDumper.Write(item);
            }
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 11")]
		[Description("Calculate the average profitability of each city and the average intensity!")]
		//Рассчитайте среднюю прибыльность каждого города и среднюю интенсивность
		public void Linq11()
		{

			var profit = from p in dataSource.Customers
						 group p by p.City into g
						 select new { name = g.Key, 
							 avergeTotal = g.Where(t => t.Orders.Count() > 0).Select(t => t.Orders.Average(m => m.Total)).Average(),
						 avergeOrders=g.Where(t=>t.Orders.Count()>0).Select(t=>t.Orders.Count()).Average()};

			foreach (var item in profit)
			{
				ObjectDumper.Write(item.name);
				ObjectDumper.Write(item.avergeTotal);
				ObjectDumper.Write(item.avergeOrders);
				ObjectDumper.Write("");
				
			}
        }

		[Category("Restriction Operators")]
		[Title("Where - Task 12")]
		[Description("Make average annual statistics of customer activity by month statistics by years, years and months!")]
		//Сделайте среднегодовую статистику активности клиентов по месяцам статистику по годам, по годам и месяцам
		public void Linq12()//доделать
		{
			var monthActivity =
				from i in dataSource.Customers
				group i by i into g

				select new
				{



					//g.Key.Orders.Where(t => g.Key.Orders.Count() > 0).GroupBy(t => t.OrderDate.Month).Select(t => t.Count())

					
                    month =g.Key.Orders.SelectMany(t=>g.Key.Orders.Where(q=>g.Key.Orders.Count()>0).GroupBy(y=>y.OrderDate.Month)),
                    years = g.Key.Orders.Where(t => g.Key.Orders.Count() > 0).GroupBy(t => t.OrderDate.Year).Select(t => t.Count())

                };

			

            foreach (var item in monthActivity)
            {
				
				ObjectDumper.Write(item.month.LastOrDefault());
			
            }
		}

	}
}
