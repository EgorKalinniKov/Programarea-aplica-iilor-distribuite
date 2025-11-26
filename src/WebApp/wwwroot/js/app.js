const API_BASE_URL = 'http://localhost:8000/api';

// Загрузка расходов
async function loadExpenses() {
    try {
        const response = await fetch(`${API_BASE_URL}/expenses`);
        const expenses = await response.json();
        
        const tbody = document.getElementById('expensesTableBody');
        tbody.innerHTML = '';
        
        if (expenses.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" class="text-center">Нет данных</td></tr>';
            return;
        }
        
        expenses.forEach(expense => {
            const row = `
                <tr>
                    <td>${expense.id}</td>
                    <td>${expense.category}</td>
                    <td>${expense.amount.toFixed(2)} ₽</td>
                    <td>${new Date(expense.date).toLocaleDateString('ru-RU')}</td>
                    <td>${expense.description || '-'}</td>
                    <td>
                        <button class="btn btn-sm btn-danger" onclick="deleteExpense(${expense.id})">
                            Удалить
                        </button>
                    </td>
                </tr>
            `;
            tbody.innerHTML += row;
        });
    } catch (error) {
        console.error('Ошибка загрузки расходов:', error);
        alert('Ошибка загрузки расходов');
    }
}

// Добавление расхода
async function addExpense() {
    const category = document.getElementById('expenseCategory').value;
    const amount = parseFloat(document.getElementById('expenseAmount').value);
    const date = document.getElementById('expenseDate').value;
    const description = document.getElementById('expenseDescription').value;
    
    if (!category || !amount || !date) {
        alert('Заполните все обязательные поля');
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/expenses`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                category,
                amount,
                date: new Date(date).toISOString(),
                description
            })
        });
        
        if (response.ok) {
            bootstrap.Modal.getInstance(document.getElementById('addExpenseModal')).hide();
            document.getElementById('addExpenseForm').reset();
            loadExpenses();
        } else {
            alert('Ошибка добавления расхода');
        }
    } catch (error) {
        console.error('Ошибка:', error);
        alert('Ошибка добавления расхода');
    }
}

// Удаление расхода
async function deleteExpense(id) {
    if (!confirm('Удалить этот расход?')) {
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/expenses/${id}`, {
            method: 'DELETE'
        });
        
        if (response.ok) {
            loadExpenses();
        } else {
            alert('Ошибка удаления расхода');
        }
    } catch (error) {
        console.error('Ошибка:', error);
        alert('Ошибка удаления расхода');
    }
}

// Загрузка категорий
async function loadCategories() {
    try {
        const response = await fetch(`${API_BASE_URL}/categories`);
        const categories = await response.json();
        
        // Обновление таблицы категорий
        const tbody = document.getElementById('categoriesTableBody');
        tbody.innerHTML = '';
        
        if (categories.length === 0) {
            tbody.innerHTML = '<tr><td colspan="3" class="text-center">Нет данных</td></tr>';
        } else {
            categories.forEach(category => {
                const row = `
                    <tr>
                        <td>${category.id}</td>
                        <td>${category.name}</td>
                        <td>
                            <button class="btn btn-sm btn-danger" onclick="deleteCategory('${category.name}')">
                                Удалить
                            </button>
                        </td>
                    </tr>
                `;
                tbody.innerHTML += row;
            });
        }
        
        // Обновление выпадающего списка в форме добавления расхода
        const select = document.getElementById('expenseCategory');
        select.innerHTML = '<option value="">Выберите категорию...</option>';
        categories.forEach(category => {
            select.innerHTML += `<option value="${category.name}">${category.name}</option>`;
        });
    } catch (error) {
        console.error('Ошибка загрузки категорий:', error);
        alert('Ошибка загрузки категорий');
    }
}

// Добавление категории
async function addCategory() {
    const name = document.getElementById('categoryName').value.trim();
    
    if (!name) {
        alert('Введите название категории');
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/categories`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ name })
        });
        
        if (response.ok) {
            bootstrap.Modal.getInstance(document.getElementById('addCategoryModal')).hide();
            document.getElementById('addCategoryForm').reset();
            loadCategories();
        } else {
            const error = await response.json();
            alert(error.message || 'Ошибка добавления категории');
        }
    } catch (error) {
        console.error('Ошибка:', error);
        alert('Ошибка добавления категории');
    }
}

// Удаление категории
async function deleteCategory(name) {
    if (!confirm(`Удалить категорию "${name}"?`)) {
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/categories/${encodeURIComponent(name)}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            loadCategories();
        } else {
            alert('Ошибка удаления категории');
        }
    } catch (error) {
        console.error('Ошибка:', error);
        alert('Ошибка удаления категории');
    }
}

// Загрузка статистики
async function loadStatistics() {
    try {
        // Общая статистика
        const summaryResponse = await fetch(`${API_BASE_URL}/statistics/summary`);
        const summary = await summaryResponse.json();

        document.getElementById('summaryStats').innerHTML = `
            <div class="row">
                <div class="col-md-3">
                    <div class="card">
                        <div class="card-body">
                            <h5 class="card-title">Всего расходов</h5>
                            <p class="card-text fs-4">${summary.totalExpenses}</p>
                        </div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="card">
                        <div class="card-body">
                            <h5 class="card-title">Общая сумма</h5>
                            <p class="card-text fs-4">${summary.totalAmount.toFixed(2)} ₽</p>
                        </div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="card">
                        <div class="card-body">
                            <h5 class="card-title">Средний расход</h5>
                            <p class="card-text fs-4">${summary.averageAmount.toFixed(2)} ₽</p>
                        </div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="card">
                        <div class="card-body">
                            <h5 class="card-title">Макс/Мин</h5>
                            <p class="card-text fs-4">${summary.maxExpense.toFixed(2)} / ${summary.minExpense.toFixed(2)} ₽</p>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Статистика по категориям
        const categoryResponse = await fetch(`${API_BASE_URL}/statistics/by-category`);
        const categoryStats = await categoryResponse.json();

        let categoryHtml = '<table class="table table-striped"><thead><tr><th>Категория</th><th>Сумма</th><th>Количество</th><th>Процент</th></tr></thead><tbody>';

        categoryStats.forEach(stat => {
            categoryHtml += `
                <tr>
                    <td>${stat.category}</td>
                    <td>${stat.total.toFixed(2)} ₽</td>
                    <td>${stat.count}</td>
                    <td>${stat.percentage.toFixed(2)}%</td>
                </tr>
            `;
        });

        categoryHtml += '</tbody></table>';
        document.getElementById('categoryStats').innerHTML = categoryHtml;

    } catch (error) {
        console.error('Ошибка загрузки статистики:', error);
        alert('Ошибка загрузки статистики');
    }
}

// Экспорт в CSV
async function exportCSV() {
    try {
        const response = await fetch(`${API_BASE_URL}/import-export/export/csv`);
        const data = await response.json();

        const blob = new Blob([data.csvContent], { type: 'text/csv' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = data.filename;
        a.click();
        window.URL.revokeObjectURL(url);
    } catch (error) {
        console.error('Ошибка экспорта:', error);
        alert('Ошибка экспорта данных');
    }
}

// Импорт из CSV
async function importCSV() {
    const fileInput = document.getElementById('csvFile');
    const file = fileInput.files[0];

    if (!file) {
        alert('Выберите файл');
        return;
    }

    try {
        const text = await file.text();

        const response = await fetch(`${API_BASE_URL}/import-export/import/csv`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ csvContent: text })
        });

        const result = await response.json();

        let resultHtml = `<div class="alert alert-info">
            <strong>Импортировано:</strong> ${result.importedCount} записей
        </div>`;

        if (result.errors && result.errors.length > 0) {
            resultHtml += '<div class="alert alert-warning"><strong>Ошибки:</strong><ul>';
            result.errors.forEach(error => {
                resultHtml += `<li>${error}</li>`;
            });
            resultHtml += '</ul></div>';
        }

        document.getElementById('importResult').innerHTML = resultHtml;
        loadExpenses();
    } catch (error) {
        console.error('Ошибка импорта:', error);
        alert('Ошибка импорта данных');
    }
}

// Инициализация при загрузке страницы
document.addEventListener('DOMContentLoaded', function() {
    loadExpenses();
    loadCategories();

    // Установка текущей даты по умолчанию
    document.getElementById('expenseDate').valueAsDate = new Date();

    // Загрузка статистики при переключении на вкладку
    document.getElementById('statistics-tab').addEventListener('click', loadStatistics);
});

