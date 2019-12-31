var gridbordercolor = "#eee";

var InitiateChartJS = function () {
    return {
        init: function () {

            var doughnutData = [
                    {
                        value: 30,
                        color: themeprimary
                    },
                    {
                        value: 50,
                        color: themesecondary
                    },
                    {
                        value: 100,
                        color: themethirdcolor
                    },
                    {
                        value: 40,
                        color: themefourthcolor
                    },
                    {
                        value: 120,
                        color: themefifthcolor
                    }

            ];
            var lineChartData = {
                labels: ["", "", "", "", "", "", ""],
                datasets: [
                    {
                        fillColor: "rgba(93, 178, 255,.4)",
                        strokeColor: "rgba(93, 178, 255,.7)",
                        pointColor: "rgba(93, 178, 255,.7)",
                        pointStrokeColor: "#fff",
                        data: [65, 59, 90, 81, 56, 55, 40]
                    },
                    {
                        fillColor: "rgba(215, 61, 50,.4)",
                        strokeColor: "rgba(215, 61, 50,.6)",
                        pointColor: "rgba(215, 61, 50,.6)",
                        pointStrokeColor: "#fff",
                        data: [28, 48, 40, 19, 96, 27, 100]
                    }
                ]

            };
            var pieData = [
                    {
                        value: 30,
                        color: themeprimary
                    },
                    {
                        value: 50,
                        color: themesecondary
                    },
                    {
                        value: 100,
                        color: themefourthcolor
                    }

            ];
            var barChartData = {
                labels: ["January", "February", "March", "April", "May", "June", "July"],
                datasets: [
                    {
                        fillColor: themeprimary,
                        strokeColor: themeprimary,
                        data: [65, 59, 90, 81, 56, 55, 40]
                    },
                    {
                        fillColor: themethirdcolor,
                        strokeColor: themethirdcolor,
                        data: [28, 48, 40, 19, 96, 27, 100]
                    }
                ]

            };
            var chartData = [
                    {
                        value: Math.random(),
                        color: themeprimary
                    },
                    {
                        value: Math.random(),
                        color: themesecondary
                    },
                    {
                        value: Math.random(),
                        color: themethirdcolor
                    },
                    {
                        value: Math.random(),
                        color: themefourthcolor
                    },
                    {
                        value: Math.random(),
                        color: themefifthcolor
                    },
                    {
                        value: Math.random(),
                        color: "#ed4e2a"
                    }
            ];
            var radarChartData = {
                labels: ["", "", "", "", "", "", ""],
                datasets: [
                    {
                        fillColor: "rgba(140,196,116,0.5)",
                        strokeColor: "rgba(140,196,116,.7)",
                        pointColor: "rgba(140,196,116,.7)",
                        pointStrokeColor: "#fff",
                        data: [65, 59, 90, 81, 56, 55, 40]
                    },
                    {
                        fillColor: "rgba(215,61,50,0.5)",
                        strokeColor: "rgba(215,61,50,.7)",
                        pointColor: "rgba(215,61,50,.7)",
                        pointStrokeColor: "#fff",
                        data: [28, 48, 40, 19, 96, 27, 100]
                    }
                ]

            };
            new Chart(document.getElementById("doughnut").getContext("2d")).Doughnut(doughnutData);
            new Chart(document.getElementById("line").getContext("2d")).Line(lineChartData);
            new Chart(document.getElementById("radar").getContext("2d")).Radar(radarChartData);
            new Chart(document.getElementById("polarArea").getContext("2d")).PolarArea(chartData);
            new Chart(document.getElementById("bar").getContext("2d")).Bar(barChartData);
            new Chart(document.getElementById("pie").getContext("2d")).Pie(pieData);

        }
    };
}();
