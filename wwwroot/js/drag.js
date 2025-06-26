// JWT Token yönetimi
// let authToken = localStorage.getItem('authToken'); // Bunu kaldırıyoruz

// API base URL
const API_BASE = '/api';

// API çağrıları için yardımcı fonksiyon
async function apiCall(endpoint, options = {}) {
    const url = `${API_BASE}${endpoint}`;
    const authToken = localStorage.getItem('authToken');
    const config = {
        headers: {
            'Content-Type': 'application/json',
            ...(authToken && { 'Authorization': `Bearer ${authToken}` })
        },
        ...options
    };

    try {
        const response = await fetch(url, config);
        if (response.status === 401) {
            localStorage.removeItem('authToken');
            window.location.href = '/Account/Login';
            return;
        }
        const text = await response.text();
        if (!text || !text.trim()) return null;
        try {
            return JSON.parse(text);
        } catch (err) {
            console.warn('JSON parse hatası:', err, text);
            return null;
        }
    } catch (error) {
        console.error('API Error:', error);
        throw error;
    }
}

// Sayfa yüklendiğinde çalışacak ana fonksiyon
document.addEventListener('DOMContentLoaded', function () {
    const authToken = localStorage.getItem('authToken');
    if (!authToken) {
        // Token yoksa ana sayfaya yönlendir
        window.location.href = '/';
        return;
    }

    loadTodos();
    loadCategories();
    setupEventListeners();

    // Modal kapandığında odağı ana butona 300ms gecikmeyle taşı
    const todoModal = document.getElementById('todoModal');
    if (todoModal) {
        todoModal.addEventListener('hidden.bs.modal', function () {
            setTimeout(() => {
                const btn = document.getElementById('add-todo-btn');
                if (btn) btn.focus();
            }, 300);
        });
    }
    const categoryModal = document.getElementById('categoryModal');
    if (categoryModal) {
        categoryModal.addEventListener('hidden.bs.modal', function () {
            setTimeout(() => {
                const btn = document.getElementById('add-category-btn');
                if (btn) btn.focus();
            }, 300);
        });
    }
});

// Modal açık mı kontrolü için yardımcı fonksiyonlar
function isTodoModalOpen() {
    const modal = document.getElementById('todoModal');
    return modal && modal.classList.contains('show');
}
function isCategoryModalOpen() {
    const modal = document.getElementById('categoryModal');
    return modal && modal.classList.contains('show');
}

// Yapılacakları yükle
async function loadTodos() {
    if (isTodoModalOpen()) return; // Modal açıkken reload etme
    try {
        const todos = await apiCall('/TodoApi');
        renderTodos(todos);
    } catch (error) {
        console.error('Todos yüklenirken hata:', error);
    }
}

// Kategorileri yükle
async function loadCategories() {
    if (isCategoryModalOpen()) return; // Modal açıkken reload etme
    try {
        const categories = await apiCall('/category');
        renderCategories(categories);
        updateCategorySelect(categories);
    } catch (error) {
        console.error('Kategoriler yüklenirken hata:', error);
    }
}

// Yapılacakları render et
function renderTodos(todos) {
    const board = document.getElementById('todo-board');
    board.innerHTML = '';

    // Kategorilere göre grupla
    const grouped = {};
    todos.forEach(todo => {
        const key = todo.categoryName || 'Kategorisiz';
        if (!grouped[key]) grouped[key] = [];
        grouped[key].push(todo);
    });

    let dragged = null;

    Object.keys(grouped).forEach(categoryName => {
        // Kategori başlığı
        const header = document.createElement('div');
        header.className = 'todo-category-header';
        // Kategori rengi
        const firstTodo = grouped[categoryName][0];
        if (firstTodo && firstTodo.categoryColor) {
            header.style.color = firstTodo.categoryColor;
        }
        header.textContent = categoryName;
        board.appendChild(header);

        grouped[categoryName].forEach((todo, i) => {
            const row = createTodoRow(todo);
            row.draggable = true;
            row.dataset.orderIndex = todo.orderIndex;

            row.addEventListener('dragstart', function (e) {
                dragged = row;
                row.classList.add('dragging');
                row.style.opacity = '0.5';
            });
            row.addEventListener('dragend', function (e) {
                row.classList.remove('dragging');
                row.style.opacity = '1';
                dragged = null;
            });
            row.addEventListener('dragover', function (e) {
                e.preventDefault();
                if (row !== dragged) row.classList.add('drag-over');
            });
            row.addEventListener('dragleave', function (e) {
                row.classList.remove('drag-over');
            });
            row.addEventListener('drop', async function (e) {
                e.preventDefault();
                row.classList.remove('drag-over');
                if (dragged && dragged !== row) {
                    if (dragged.compareDocumentPosition(row) & Node.DOCUMENT_POSITION_FOLLOWING) {
                        board.insertBefore(dragged, row);
                    } else {
                        board.insertBefore(dragged, row.nextSibling);
                    }
                    dragged.classList.add('dropped');
                    setTimeout(() => dragged.classList.remove('dropped'), 600);
                    await updateTodoOrderOnBackend();
                    showToast('Yapılacak sıralaması güncellendi');
                }
            });
            board.appendChild(row);
        });
    });
}

function createTodoRow(todo) {
    const row = document.createElement('div');
    row.className = 'todo-row';
    row.dataset.id = todo.id;
    row.innerHTML = `
        <div>
            <span class="todo-title fw-semibold">${todo.title}</span>
            ${todo.description ? `<span class="todo-description text-muted small">${todo.description}</span>` : ''}
        </div>
    `;
    // Satıra tıklama ile detay modalı aç
    row.addEventListener('click', async function (e) {
        // Sürükle-bırak sırasında tıklama tetiklenmesin
        if (row.classList.contains('dragging')) return;
        await editTodo(todo.id, true); // true: detay için
    });
    return row;
}

// Kategorileri render et
function renderCategories(categories) {
    const list = document.getElementById('categories-list');
    list.innerHTML = '';

    // Drop zone oluşturucu
    function createDropZone(index) {
        const dz = document.createElement('div');
        dz.className = 'category-dropzone';
        dz.dataset.index = index;
        dz.addEventListener('dragover', function (e) {
            e.preventDefault();
            dz.classList.add('active');
        });
        dz.addEventListener('dragleave', function (e) {
            dz.classList.remove('active');
        });
        dz.addEventListener('drop', async function (e) {
            e.preventDefault();
            dz.classList.remove('active');
            if (window._draggedCategory) {
                const items = Array.from(list.querySelectorAll('.category-item'));
                if (items.length === 0 || dz.dataset.index == 0) {
                    list.insertBefore(window._draggedCategory, list.querySelector('.category-dropzone'));
                } else if (dz.dataset.index >= items.length) {
                    list.appendChild(window._draggedCategory);
                } else {
                    list.insertBefore(window._draggedCategory, items[dz.dataset.index]);
                }
                await updateCategoryOrderOnBackend();
                showToast('Kategori sıralaması güncellendi');
                loadCategories();
            }
        });
        return dz;
    }

    // Başa drop zone ekle
    list.appendChild(createDropZone(0));
    categories.forEach((category, i) => {
        const item = createCategoryItem(category);
        item.draggable = true;
        item.dataset.orderIndex = category.orderIndex;
        item.addEventListener('dragstart', function (e) {
            window._draggedCategory = item;
            item.classList.add('dragging');
            item.style.opacity = '0.5';
        });
        item.addEventListener('dragend', function (e) {
            item.classList.remove('dragging');
            item.style.opacity = '1';
            window._draggedCategory = null;
        });
        list.appendChild(item);
        // Her kategori arasına drop zone ekle
        list.appendChild(createDropZone(i + 1));
    });
}

// Todo kartı oluştur
function createTodoCard(todo) {
    const card = document.createElement('div');
    card.className = 'todo-card';
    card.draggable = true;
    card.dataset.id = todo.id;
    card.dataset.orderIndex = todo.orderIndex;

    const categoryColor = todo.categoryColor || '#007bff';
    card.style.borderLeftColor = categoryColor;

    card.innerHTML = `
        <div class="todo-content">
            <h6 class="todo-title">${todo.title}</h6>
            ${todo.description ? `<p class="todo-description">${todo.description}</p>` : ''}
            ${todo.categoryName ? `<span class="badge" style="background-color: ${categoryColor}">${todo.categoryName}</span>` : ''}
        </div>
        <div class="todo-actions">
            <button class="btn btn-sm btn-outline-primary edit-todo" data-id="${todo.id}">Düzenle</button>
            <button class="btn btn-sm btn-outline-danger delete-todo" data-id="${todo.id}">Sil</button>
        </div>
    `;

    return card;
}

// Kategori öğesi oluştur
function createCategoryItem(category) {
    const item = document.createElement('div');
    item.className = 'category-item';
    item.dataset.id = category.id;

    item.innerHTML = `
        <div class="d-flex justify-content-between align-items-center mb-2">
            <span class="category-name" style="color: ${category.color}">${category.name}</span>
            <div>
                <button class="btn btn-sm btn-outline-warning edit-category" data-id="${category.id}">Düzenle</button>
                <button class="btn btn-sm btn-outline-danger delete-category" data-id="${category.id}">Sil</button>
            </div>
        </div>
    `;

    return item;
}

// Kategori select'ini güncelle
function updateCategorySelect(categories) {
    const select = document.getElementById('todoCategory');
    select.innerHTML = '<option value="">Kategori Seçin</option>';
    
    categories.forEach(category => {
        const option = document.createElement('option');
        option.value = category.id;
        option.textContent = category.name;
        select.appendChild(option);
    });
}

// Event listener'ları ayarla
function setupEventListeners() {
    // Yeni todo ekleme
    document.getElementById('add-todo-btn').addEventListener('click', () => {
        document.getElementById('todoModalTitle').textContent = 'Yeni Yapılacak';
        document.getElementById('todoForm').reset();
        document.getElementById('saveTodoBtn').dataset.id = '';
        new bootstrap.Modal(document.getElementById('todoModal')).show();
    });

    // Yeni kategori ekleme
    document.getElementById('add-category-btn').addEventListener('click', () => {
        document.getElementById('categoryForm').reset();
        document.getElementById('saveCategoryBtn').dataset.id = '';
        new bootstrap.Modal(document.getElementById('categoryModal')).show();
    });

    // Todo kaydetme
    document.getElementById('saveTodoBtn').addEventListener('click', saveTodo);
    document.getElementById('saveCategoryBtn').addEventListener('click', saveCategory);

    // Todo ve kategori düzenleme/silme (delegated events)
    document.addEventListener('click', async (e) => {
        if (e.target.classList.contains('edit-todo')) {
            const id = e.target.dataset.id;
            await editTodo(id);
        } else if (e.target.classList.contains('delete-todo')) {
            const id = e.target.dataset.id;
            if (confirm('Bu yapılacak öğesini silmek istediğinizden emin misiniz?')) {
                await deleteTodo(id);
            }
        } else if (e.target.classList.contains('edit-category')) {
            const id = e.target.dataset.id;
            await editCategory(id);
        } else if (e.target.classList.contains('delete-category')) {
            const id = e.target.dataset.id;
            if (confirm('Bu kategoriyi silmek istediğinizden emin misiniz?')) {
                await deleteCategory(id);
            }
        }
    });

    // Modal kapanınca form ve id temizle
    const todoModal = document.getElementById('todoModal');
    if (todoModal) {
        todoModal.addEventListener('hidden.bs.modal', function () {
            document.getElementById('todoForm').reset();
            document.getElementById('saveTodoBtn').dataset.id = '';
        });
    }
    const categoryModal = document.getElementById('categoryModal');
    if (categoryModal) {
        categoryModal.addEventListener('hidden.bs.modal', function () {
            document.getElementById('categoryForm').reset();
            document.getElementById('saveCategoryBtn').dataset.id = '';
        });
    }
}

// Todo kaydet (ekle/güncelle)
async function saveTodo() {
    const title = document.getElementById('todoTitle').value;
    const description = document.getElementById('todoDescription').value;
    const categoryId = document.getElementById('todoCategory').value;
    const id = document.getElementById('saveTodoBtn').dataset.id;

    if (!title) {
        alert('Başlık gereklidir!');
        return;
    }

    const todo = {
        title,
        description,
        categoryId: categoryId || null,
        isCompleted: false,
        orderIndex: 0
    };

    try {
        if (id) {
            // Güncelleme
            await apiCall(`/TodoApi/${id}`, {
                method: 'PUT',
                body: JSON.stringify({ ...todo, id: parseInt(id) })
            });
        } else {
            // Ekleme
            await apiCall('/TodoApi', {
                method: 'POST',
                body: JSON.stringify(todo)
            });
        }
        bootstrap.Modal.getInstance(document.getElementById('todoModal')).hide();
        loadTodos();
    } catch (error) {
        alert('Yapılacak kaydedilirken hata oluştu!');
    }
}

// Kategori düzenle
async function editCategory(id) {
    try {
        const category = await apiCall(`/category/${id}`);
        document.getElementById('categoryName').value = category.name;
        document.getElementById('categoryColor').value = category.color;
        document.getElementById('saveCategoryBtn').dataset.id = id;
        new bootstrap.Modal(document.getElementById('categoryModal')).show();
    } catch (error) {
        alert('Kategori yüklenirken hata oluştu!');
    }
}

// Kategori kaydet (ekle/güncelle)
async function saveCategory() {
    const name = document.getElementById('categoryName').value;
    let color = document.getElementById('categoryColor').value;
    const id = document.getElementById('saveCategoryBtn').dataset.id;

    if (!name) {
        alert('Kategori adı gereklidir!');
        return;
    }

    // Color alanını 7 karakterli hex'e tamamla
    if (color.length === 4) {
        color = '#' + color[1] + color[1] + color[2] + color[2] + color[3] + color[3];
    }

    const category = {
        name,
        color,
        orderIndex: 0
    };

    try {
        let response;
        if (id) {
            // Güncelleme
            response = await apiCall(`/category/${id}`, {
                method: 'PUT',
                body: JSON.stringify({ ...category, id: parseInt(id) })
            });
        } else {
            // Ekleme
            response = await apiCall('/category', {
                method: 'POST',
                body: JSON.stringify(category)
            });
        }

        if (response && response.errors) {
            alert('Kategori kaydedilemedi: ' + Object.values(response.errors).flat().join(', '));
            return;
        }

        bootstrap.Modal.getInstance(document.getElementById('categoryModal')).hide();
        loadCategories();
    } catch (error) {
        alert('Kategori kaydedilirken hata oluştu!');
    }
}

// Todo sil
async function deleteTodo(id) {
    try {
        await apiCall(`/TodoApi/${id}`, { method: 'DELETE' });
    } catch (error) {
        // Hata olsa bile alert gösterme
    }
    // Eğer modal açıksa önce kapat, sonra listeyi güncelle
    const todoModal = document.getElementById('todoModal');
    if (todoModal && todoModal.classList.contains('show')) {
        bootstrap.Modal.getInstance(todoModal).hide();
        setTimeout(() => loadTodos(), 350); // Modal animasyonu için küçük gecikme
    } else {
        loadTodos();
    }
}

// Kategori sil
async function deleteCategory(id) {
    try {
        await apiCall(`/category/${id}`, { method: 'DELETE' });
    } catch (error) {
        // Hata olsa bile alert gösterme
    }
    loadCategories();
}

// Todo düzenle
async function editTodo(id, showDelete) {
    try {
        const todo = await apiCall(`/TodoApi/${id}`);
        document.getElementById('todoModalTitle').textContent = 'Yapılacak Detay';
        document.getElementById('todoTitle').value = todo.title;
        document.getElementById('todoDescription').value = todo.description || '';
        document.getElementById('todoCategory').value = todo.categoryId || '';
        document.getElementById('saveTodoBtn').dataset.id = id;
        // Sil butonunu göster/gizle
        let deleteBtn = document.getElementById('deleteTodoBtn');
        if (!deleteBtn) {
            deleteBtn = document.createElement('button');
            deleteBtn.id = 'deleteTodoBtn';
            deleteBtn.className = 'btn btn-danger';
            deleteBtn.textContent = 'Sil';
            deleteBtn.style.marginRight = 'auto';
            deleteBtn.addEventListener('click', async function () {
                if (confirm('Bu yapılacak silinsin mi?')) {
                    await deleteTodo(id);
                    bootstrap.Modal.getInstance(document.getElementById('todoModal')).hide();
                }
            });
            // Modal footer'a ekle
            const footer = document.querySelector('#todoModal .modal-footer');
            footer.insertBefore(deleteBtn, footer.firstChild);
        }
        deleteBtn.style.display = showDelete ? '' : 'none';
        new bootstrap.Modal(document.getElementById('todoModal')).show();
    } catch (error) {
        alert('Yapılacak yüklenirken hata oluştu!');
    }
}

// Todo sıralamasını güncelle
async function updateTodoOrder(id, newIndex) {
    try {
        await apiCall(`/TodoApi/${id}/order`, {
            method: 'PUT',
            body: JSON.stringify(newIndex)
        });
    } catch (error) {
        console.error('Sıralama güncellenirken hata:', error);
    }
}

function setupCategoryDragAndDrop() {
    const list = document.getElementById('categories-list');
    let dragged = null;

    list.querySelectorAll('.category-item').forEach(item => {
        item.addEventListener('dragstart', function (e) {
            dragged = item;
            item.classList.add('dragging');
            item.style.opacity = '0.5';
        });
        item.addEventListener('dragend', function (e) {
            item.classList.remove('dragging');
            item.style.opacity = '1';
        });
        item.addEventListener('dragover', function (e) {
            e.preventDefault();
            item.classList.add('drag-over');
        });
        item.addEventListener('dragleave', function (e) {
            item.classList.remove('drag-over');
        });
        item.addEventListener('drop', async function (e) {
            e.preventDefault();
            item.classList.remove('drag-over');
            if (dragged && dragged !== item) {
                // DOM sırasını güncelle
                if (dragged.compareDocumentPosition(item) & Node.DOCUMENT_POSITION_FOLLOWING) {
                    list.insertBefore(dragged, item);
                } else {
                    list.insertBefore(dragged, item.nextSibling);
                }
                // Bırakılan item'a dropped class'ı ekle
                dragged.classList.add('dropped');
                setTimeout(() => dragged.classList.remove('dropped'), 600);
                // Backend sıralamasını güncelle
                await updateCategoryOrderOnBackend();
                showToast('Kategori sıralaması güncellendi');
            }
        });
    });
}

async function updateCategoryOrderOnBackend() {
    const list = document.getElementById('categories-list');
    const items = Array.from(list.querySelectorAll('.category-item'));
    const updates = items.map((item, i) => ({ id: parseInt(item.dataset.id), orderIndex: i }));
    try {
        await apiCall('/category/reorder', {
            method: 'PUT',
            body: JSON.stringify(updates)
        });
    } catch (error) {
        console.error('Kategori sırası toplu güncellenirken hata:', error);
    }
}

async function updateTodoOrderOnBackend() {
    const board = document.getElementById('todo-board');
    const items = Array.from(board.querySelectorAll('.todo-row'));
    const updates = items.map((item, i) => ({ id: parseInt(item.dataset.id), orderIndex: i }));
    try {
        await apiCall('/TodoApi/reorder', {
            method: 'PUT',
            body: JSON.stringify(updates)
        });
    } catch (error) {
        console.error('Yapılacak sırası toplu güncellenirken hata:', error);
    }
}

// Toast göster
function showToast(message) {
    let toast = document.getElementById('custom-toast');
    if (!toast) {
        toast = document.createElement('div');
        toast.id = 'custom-toast';
        toast.style.position = 'fixed';
        toast.style.bottom = '30px';
        toast.style.left = '50%';
        toast.style.transform = 'translateX(-50%)';
        toast.style.background = 'rgba(40,40,40,0.95)';
        toast.style.color = '#fff';
        toast.style.padding = '12px 32px';
        toast.style.borderRadius = '8px';
        toast.style.fontSize = '1rem';
        toast.style.zIndex = '9999';
        toast.style.boxShadow = '0 2px 12px rgba(0,0,0,0.2)';
        toast.style.opacity = '0';
        toast.style.transition = 'opacity 0.3s';
        document.body.appendChild(toast);
    }
    toast.textContent = message;
    toast.style.opacity = '1';
    setTimeout(() => {
        toast.style.opacity = '0';
    }, 1200);
} 