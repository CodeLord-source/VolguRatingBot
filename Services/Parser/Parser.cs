using HtmlAgilityPack;

namespace RatingBot.Services.Parser
{
    public class Parser : IParser<string>
    {
        public string Parse(HtmlDocument document, int? number)
        {   
            var rowValues = new List<List<string>>();
            var messageString = "<code>";
            var balls = 0; 
            var rows = document
                .GetElementbyId($"tab-content-0")
                .SelectSingleNode($"//div[@id='tab-0-{number-1}']/table[@class='table table-striped table-hover grade-table']")
                .Descendants("tr")
                .ToList();
             
            foreach (var row in rows)
            {
                var currentRowValues = new List<string>();

                foreach (var column in row.ChildNodes)
                {
                    currentRowValues.Add(column.InnerText);
                }

                rowValues.Add(currentRowValues);
            }

            rowValues.RemoveAt(0);
             
            foreach (var sbj in rowValues)
            {
                var a = sbj.ToArray();

                for (int i = 0; i < a.Length; i++)
                {
                    if(a[i].Contains("("))
                    {
                        a[i].Replace("(", " ");
                    }

                    if (a[i].Contains(")"))
                    {
                        a[i].Replace(")", " ");
                    }

                    if (i % 2 != 0 && i < 5)
                    {
                        messageString += " " + a[i];
                    }
                    else if (i == 5)
                    {
                        messageString += $" ({a[i]}) ";
                    }
                    else if (i == 7 || i == 9 || i == 11 || i == 15 || i == 17 || i == 19)
                    {
                        try
                        {
                            balls += Convert.ToInt32(a[i]);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

                messageString += $" :{balls} \n";
                balls = 0;
            }
             
            return messageString+"</code>"; 
        }
    }
}
