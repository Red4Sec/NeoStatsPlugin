using System.Globalization;
using System.Linq;
using System.Text;
using NeoStats.Core;

namespace NeoStats.Extensions
{
    public static class ChartExtensions
    {
        public static string DoTimePerBlockChart(this BlockStatCollection[] stats)
        {
            if (stats.Length == 0) return "";

            stats.FirstOrDefault().ComputeAverageTimePerBlock(out var minTimePeerBlock, out var maxTimePeerBlock, out var avgTimePeerBlock);

            var title = $"Min: {minTimePeerBlock.TotalSeconds.ToString("0.00's'")} - Max: {maxTimePeerBlock.TotalSeconds.ToString("0.00's'")} - Avg: {avgTimePeerBlock.TotalSeconds.ToString("0.00's'")}";

            // ['Block', 'v2.10'/*, 'v2.09'*/],

            var data = new StringBuilder();
            data.AppendLine($"['Block', '{string.Join(",", stats.Select(u => u.Title))}'],");

            foreach (var height in stats.FirstOrDefault().Blocks.Keys)
            {
                //['1', 8175000 /*, 8008000*/],

                var values = stats
                    .Where(u => u.Blocks.ContainsKey(height))
                    .Select(u => u.Blocks[height].ElapsedTime.TotalSeconds).ToArray();

                // All stats must contains the block

                if (values.Length != stats.Length)
                {
                    continue;
                }

                data.AppendLine($"['{height}',{string.Join(",", values.Select(u => u.ToString(CultureInfo.InvariantCulture)))}],");
            }

            return @"<html>
<head>
  <script type='text/javascript' src='https://www.gstatic.com/charts/loader.js'></script>
  <script type='text/javascript'>
    google.charts.load('current', {packages:['corechart']});
    google.charts.setOnLoadCallback(drawChart);
    function drawChart() {

      var data = google.visualization.arrayToDataTable
      ([
        " + data.ToString().TrimEnd(',') + @"
      ]);

      var options = {
        chartArea: {width: '70%'},
        hAxis: {
          title: '" + title + @"',
          minValue: 0,
          textStyle: {
            bold: true,
            fontSize: 12,
            color: '#4d4d4d'
          },
          titleTextStyle: {
            bold: true,
            fontSize: 18,
            color: '#4d4d4d'
          }
        },
        vAxis: {
          title: 'Seconds per block',
          textStyle: {
            fontSize: 14,
            bold: true,
            color: '#848484'
          },
          titleTextStyle: {
            fontSize: 14,
            bold: true,
            color: '#848484'
          }
        }
      };

      var chart_div = document.getElementById('chart_div');
      var chart = new google.visualization.ColumnChart(chart_div);

      // Wait for the chart to finish drawing before calling the getImageURI() method.
      google.visualization.events.addListener(chart, 'ready', function () 
      {
        chart_div.innerHTML = '<img src=\'' + chart.getImageURI() + '\'>';
        console.log(chart_div.innerHTML);
      });

      chart.draw(data, options);
  }
</script>
<div id='chart_div'></div>
</head>
</html>";
        }
    }
}