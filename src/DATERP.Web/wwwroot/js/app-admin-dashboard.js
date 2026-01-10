/**
 * Admin Dashboard
 */

'use strict';

(function () {
    let cardColor, headingColor, labelColor, borderColor, legendColor;

    if (isDarkStyle) {
        cardColor = config.colors_dark.cardColor;
        headingColor = config.colors_dark.headingColor;
        labelColor = config.colors_dark.textMuted;
        legendColor = config.colors_dark.bodyColor;
        borderColor = config.colors_dark.borderColor;
    } else {
        cardColor = config.colors.cardColor;
        headingColor = config.colors.headingColor;
        labelColor = config.colors.textMuted;
        legendColor = config.colors.bodyColor;
        borderColor = config.colors.borderColor;
    }

    // System Uptime Chart
    // --------------------------------------------------------------------
    const systemUptimeChartEl = document.querySelector('#systemUptimeChart');
    const systemUptimeChartConfig = {
        chart: {
            height: 100,
            width: 130,
            type: 'donut',
            toolbar: {
                show: false
            }
        },
        labels: ['Uptime', 'Downtime'],
        series: [99.8, 0.2],
        colors: [config.colors.success, config.colors.secondary],
        stroke: {
            width: 0
        },
        dataLabels: {
            enabled: false
        },
        legend: {
            show: false
        },
        grid: {
            padding: {
                top: 0,
                bottom: 0,
                right: 0,
                left: 0
            }
        },
        plotOptions: {
            pie: {
                donut: {
                    size: '70%',
                    labels: {
                        show: true,
                        value: {
                            fontSize: '1.5rem',
                            fontFamily: 'Public Sans',
                            color: headingColor,
                            fontWeight: 500,
                            offsetY: -15,
                            formatter: function (val) {
                                return parseInt(val) + '%';
                            }
                        },
                        name: {
                            offsetY: 20,
                            fontFamily: 'Public Sans'
                        },
                        total: {
                            show: true,
                            fontSize: '0.9rem',
                            label: 'Uptime',
                            color: labelColor,
                            formatter: function (w) {
                                return '99%';
                            }
                        }
                    }
                }
            }
        }
    };
    if (typeof systemUptimeChartEl !== undefined && systemUptimeChartEl !== null) {
        const systemUptimeChart = new ApexCharts(systemUptimeChartEl, systemUptimeChartConfig);
        systemUptimeChart.render();
    }

    // Horizontal Bar Chart
    // --------------------------------------------------------------------
    const horizontalBarChartEl = document.querySelector('#horizontalBarChart');
    const horizontalBarChartConfig = {
        chart: {
            height: 270,
            type: 'bar',
            toolbar: {
                show: false
            }
        },
        plotOptions: {
            bar: {
                horizontal: true,
                barHeight: '70%',
                distributed: true,
                startingShape: 'rounded',
                borderRadius: 7
            }
        },
        colors: [
            config.colors.primary,
            config.colors.success,
            config.colors.warning,
            config.colors.info,
            config.colors.danger
        ],
        series: [
            {
                data: [710, 350, 290, 450, 150]
            }
        ],
        xaxis: {
            categories: ['Người dùng', 'Hoạt động', 'Hoàn thành', 'Truy cập', 'Lỗi'],
            axisBorder: {
                show: false
            },
            axisTicks: {
                show: false
            },
            labels: {
                style: {
                    colors: labelColor,
                    fontSize: '13px'
                },
                formatter: function (val) {
                    return `${val}`;
                }
            }
        },
        yaxis: {
            labels: {
                style: {
                    colors: labelColor,
                    fontSize: '13px',
                    fontFamily: 'Public Sans'
                }
            }
        },
        grid: {
            borderColor: borderColor,
            xaxis: {
                lines: {
                    show: true
                }
            },
            yaxis: {
                lines: {
                    show: false
                }
            },
            padding: {
                top: 0,
                right: 0,
                bottom: 0,
                left: 10
            }
        },
        legend: {
            show: false
        },
        tooltip: {
            theme: false
        }
    };
    if (typeof horizontalBarChartEl !== undefined && horizontalBarChartEl !== null) {
        const horizontalBarChart = new ApexCharts(horizontalBarChartEl, horizontalBarChartConfig);
        horizontalBarChart.render();
    }

    // System Data Table
    // --------------------------------------------------------------------
    var dt_system_table = $('.datatables-admin-system');

    if (dt_system_table.length) {
        var dt_system = dt_system_table.DataTable({
            dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6 d-flex justify-content-center justify-content-md-end"f>>t<"row"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
            displayLength: 7,
            lengthMenu: [7, 10, 25, 50, 75, 100],
            language: {
                search: '',
                searchPlaceholder: 'Tìm kiếm dịch vụ...'
            }
        });
    }
})();
