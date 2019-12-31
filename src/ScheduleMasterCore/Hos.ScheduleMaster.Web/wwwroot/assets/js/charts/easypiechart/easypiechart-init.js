
var gridbordercolor = "#eee";

var InitiateEasyPieChart = function () {
    return {
        init: function () {
            var easypiecharts = $('[data-toggle=easypiechart]');
            $.each(easypiecharts, function () {
                var barColor = getcolor($(this).data('barcolor')) || themeprimary,
                    trackColor = getcolor($(this).data('trackcolor')) || false,
                    scaleColor = getcolor($(this).data('scalecolor')) || false,
                    lineCap = $(this).data('linecap') || "round",
                    lineWidth = $(this).data('linewidth') || 3,
                    size = $(this).data('size') || 110,
                    animate = $(this).data('animate') || false;

                $(this).easyPieChart({
                    barColor: barColor,
                    trackColor: trackColor,
                    scaleColor: scaleColor,
                    lineCap: lineCap,
                    lineWidth: lineWidth,
                    size: size,
                    animate : animate
                });
            });
        }
    };
}();
