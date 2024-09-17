// Use raw timestamps directly and apply formatting manually
function formatDate(timestamp) {
    const date = new Date(timestamp);
    const year = date.getUTCFullYear();
    const month = (date.getUTCMonth() + 1).toString().padStart(2, '0');  // Months are 0-indexed
    const day = date.getUTCDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;  // Return formatted date in UTC (YYYY-MM-DD)
}

// External tooltip handler for custom HTML tooltip
function externalTooltipHandler(context) {
    let tooltipEl = document.getElementById('chartjs-tooltip');
    if (!tooltipEl) {
        tooltipEl = document.createElement('div');
        tooltipEl.id = 'chartjs-tooltip';
        tooltipEl.innerHTML = '<table></table>';
        document.body.appendChild(tooltipEl);
    }
    const tooltipModel = context.tooltip;
    if (tooltipModel.opacity === 0) {
        tooltipEl.style.opacity = 0;
        return;
    }
    if (tooltipModel.body) {
        const bodyLines = tooltipModel.body.map(bodyItem => bodyItem.lines);
        const innerHtml = '<tbody>' + bodyLines.map((line) => `<tr><td>${line}</td></tr>`).join('') + '</tbody>';
        const tableRoot = tooltipEl.querySelector('table');
        tableRoot.innerHTML = innerHtml;
    }
    const { offsetLeft: positionX, offsetTop: positionY } = context.chart.canvas;
    tooltipEl.style.opacity = 1;
    tooltipEl.style.position = 'absolute';
    tooltipEl.style.left = positionX + tooltipModel.caretX + 'px';
    tooltipEl.style.top = positionY + tooltipModel.caretY + 'px';
    tooltipEl.style.pointerEvents = 'none';
    tooltipEl.style.backgroundColor = 'rgba(0, 0, 0, 0.7)';
    tooltipEl.style.color = 'white';
    tooltipEl.style.borderRadius = '5px';
    tooltipEl.style.padding = '10px';
    tooltipEl.style.fontSize = '12px';
}

fetch('/price_actual.json')
    .then(response => response.json())
    .then(actualData => {
        const actualPrices = actualData.prices.slice(-21).map(item => item[1]);
        const actualDates = actualData.prices.slice(-21).map(item => formatDate(item[0]));

        fetch('/price_predicted.json')
            .then(response => response.json())
            .then(predictedData => {
                const predictedPrices = predictedData.prices.map(item => item[1]);
                const predictedDates = predictedData.prices.map(item => formatDate(item[0]));

                const allDates = actualDates.concat(predictedDates);
                const allPrices = actualPrices.concat(predictedPrices);

                const ctx = document.getElementById('cryptoChart').getContext('2d');

                new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: allDates,  // Use manually formatted UTC dates
                        datasets: [
                            {
                                label: 'Price (USD)',
                                data: allPrices,
                                borderColor: 'rgb(255, 99, 132)',
                                backgroundColor: function(context) {
                                    const index = context.dataIndex;
                                    const numActual = actualPrices.length;
                                    return index >= numActual ? 'rgba(75, 192, 192, 0.2)' : 'rgba(255, 99, 132, 0.2)';
                                },
                                fill: true
                            }
                        ]
                    },
                    options: {
                        scales: {
                            x: {
                                type: 'category',  // Treat x-axis as categories instead of time
                                title: {
                                    display: true,
                                    text: 'Date (UTC)'
                                }
                            },
                            y: {
                                title: {
                                    display: true,
                                    text: 'Price (USD)'
                                },
                                beginAtZero: false
                            }
                        },
                        responsive: true,
                        plugins: {
                            legend: {
                                display: false
                            },
                            tooltip: {
                                enabled: false,
                                external: externalTooltipHandler,
                                callbacks: {
                                    label: function(tooltipItem) {
                                        const currentPrice = tooltipItem.raw;
                                        const index = tooltipItem.dataIndex;
                                        const previousPrice = allPrices[index - 1];
                                        let priceDiff = '';
                                        let colorClass = '';
                                        let arrow = '';
                                        if (index > 0) {
                                            const diff = currentPrice - previousPrice;
                                            arrow = diff > 0 ? '↑' : '↓';
                                            colorClass = diff > 0 ? 'green-text' : 'red-text';
                                            priceDiff = `<span class="${colorClass}">${arrow}</span>$${Math.abs(diff).toFixed(2)}`;
                                        }
                                        return `Price: $${currentPrice.toFixed(2)}<br>${priceDiff}`;
                                    }
                                }
                            }
                        }
                    },
                    plugins: [{
                        beforeDraw: function(chart) {
                            const ctx = chart.ctx;
                            const numActual = actualPrices.length;
                            const xAxis = chart.scales['x'];
                            const yAxis = chart.scales['y'];
                            ctx.save();
                            ctx.fillStyle = 'rgba(75, 192, 192, 0.4)';
                            const startX = xAxis.getPixelForValue(allDates[numActual]);
                            const endX = xAxis.right;
                            const topY = yAxis.top;
                            const bottomY = yAxis.bottom;
                            ctx.fillRect(startX, topY, endX - startX, bottomY - topY);
                            ctx.restore();
                        }
                    }]
                });
            })
            .catch(error => {
                console.error("Error fetching predicted data:", error);
            });
    })
    .catch(error => {
        console.error("Error fetching actual market data:", error);
    });
