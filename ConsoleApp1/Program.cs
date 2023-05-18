using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream fstream = File.OpenWrite("outtab.csv");
            string outstring = $"Begin: {DateTime.Now.ToString()}\n";
            fstream.Write(Encoding.UTF8.GetBytes(outstring), 0, Encoding.UTF8.GetBytes(outstring).Length);
            outstring = "repository_type\t"
                + "course_title\t"
                + "course_summery\t"
                + "unit_title\t"
                + "lesson_title\t"
                + "lesson_url\t"
                + "Type\n";
            fstream.Write(Encoding.UTF8.GetBytes(outstring), 0, Encoding.UTF8.GetBytes(outstring).Length);


            string base_url = "https://www.ixl.com/";
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            IWebDriver driver = new ChromeDriver(service, options, TimeSpan.FromMinutes(3));
            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
            driver.Url = base_url;
            var elements = driver.FindElements(By.ClassName("grade-module"));
            foreach(IWebElement elem in elements)
            {
                 
                var types = elem.FindElements(By.ClassName("grade-box-tab"));
                string repository_type = elem.FindElement(By.ClassName("grade-box-tab")).GetAttribute("innerHTML"); //repository_type
                string course_title = elem.FindElement(By.ClassName("grade-box-long-name")).GetAttribute("innerHTML"); //course_title
                string course_summary = elem.FindElement(By.ClassName("grade-description")).GetAttribute("innerHTML"); //course_summery
                Console.WriteLine($"  Processing course summary: {repository_type} > {course_title} > {course_summary}");
                foreach (IWebElement type in types)                
                {
                    IWebElement pare = type.FindElement(By.XPath(".."));
                    IWebDriver driver1 = new ChromeDriver(service, options, TimeSpan.FromMinutes(3));
                    driver1.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(30));
                    driver1.Url = pare.GetAttribute("href");
                    var courses = driver1.FindElements(By.ClassName("skill-tree-category"));
                    foreach (IWebElement course in courses)
                    {
                         
                        string unit_title = course.FindElement(By.ClassName("category-name")).GetAttribute("innerHTML"); //unit_title
                        //Console.WriteLine(unit_title);
                        var lessons = course.FindElements(By.ClassName("skill-tree-skill-node"));
                        foreach (IWebElement lesson in lessons)
                        {
                            string lesson_title = lesson.FindElement(By.ClassName("skill-tree-skill-name")).GetAttribute("innerHTML"); //lesson_title
                            //Console.WriteLine(lesson_title);
                            var lessonURLs = lesson.FindElements(By.ClassName("skill-tree-skill-name"));
                            foreach (IWebElement lessonURL in lessonURLs)
                            {
                                IWebElement paren = lessonURL.FindElement(By.XPath(".."));
                                string lesson_url = paren.GetAttribute("href"); //web location
                                //Console.WriteLine(lesson_url);
                                string out_line_string = repository_type + "\t";
                                out_line_string += course_title + "\t";
                                out_line_string += course_summary + "\t";
                                out_line_string += unit_title + "\t";
                                out_line_string += lesson_title + "\t";
                                out_line_string += lesson_url + "\n";                                
                                fstream.Write(Encoding.UTF8.GetBytes(out_line_string), 0, Encoding.UTF8.GetBytes(out_line_string).Length);
                                Console.WriteLine($"  Processing course summary: {repository_type} > {course_title} > {course_summary} > {unit_title} > {lesson_title} > {lesson_url}");
                
                            }                            
                        }                 
                    }
                    driver1.Close();
                }
            }

            driver.Close();

            outstring = "End: " + DateTime.Now.ToString();
            fstream.Write(Encoding.UTF8.GetBytes(outstring), 0, Encoding.UTF8.GetBytes(outstring).Length);
            fstream.Close();
        }
    }
}
