// WebSocket для real-time статистики
let connection = null;

async function initWebSocket() {
    if (connection) return;
    
    connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:8003/statisticsHub")
        .build();

    connection.on("StatisticsUpdated", (data) => {
        updateDailyStats(data);
    });

    await connection.start();
}

function updateDailyStats(stats) {
    let html = '<table class="table"><thead><tr><th>Дата</th><th>Сумма</th><th>Количество</th></tr></thead><tbody>';
    stats.forEach(s => {
        html += `<tr><td>${s.date}</td><td>${s.total} ₽</td><td>${s.count}</td></tr>`;
    });
    html += '</tbody></table>';
    document.getElementById('dailyStats').innerHTML = html;
}

async function loadStatisticsWithPeriod() {
    const start = document.getElementById('startDate')?.value;
    const end = document.getElementById('endDate')?.value;
    
    let url = `${API_BASE_URL}/statistics/daily?`;
    if (start) url += `startDate=${start}&`;
    if (end) url += `endDate=${end}`;
    
    const response = await fetch(url);
    const data = await response.json();
    updateDailyStats(data);
    
    // Загружаем категории с тем же периодом
    url = `${API_BASE_URL}/statistics/by-category?`;
    if (start) url += `startDate=${start}&`;
    if (end) url += `endDate=${end}`;
    
    const catResponse = await fetch(url);
    const catStats = await catResponse.json();
    
    let html = '<table class="table"><thead><tr><th>Категория</th><th>Сумма</th><th>%</th></tr></thead><tbody>';
    catStats.forEach(s => {
        html += `<tr><td>${s.category}</td><td>${s.total} ₽</td><td>${s.percentage}%</td></tr>`;
    });
    html += '</tbody></table>';
    document.getElementById('categoryStats').innerHTML = html;
}
