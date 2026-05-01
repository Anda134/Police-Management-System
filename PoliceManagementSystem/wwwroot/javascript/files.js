/**
 * files.js
 * Handles all criminal file UI operations.
 * REQ-49: Create a new criminal activity file.
 * REQ-50: Store files grouped by category.
 * REQ-51: View file content.
 * REQ-52: Edit file content.
 * REQ-53: Update file status.
 * REQ-54: Display current status of each file.
 * REQ-55: Prevent deletion without authorization.
 * REQ-56: Maintain version history for files.
 * REQ-57: Search files by file name.
 * REQ-58: Search files by agent name.
 * REQ-59: Search files by criminal activity category.
 */

/**
 * REQ-73: Verify authentication on page load.
 * Redirects to login if no token found.
 */
requireAuth();

/**
 * REQ-75: Display current user info in navbar.
 */
document.getElementById("navUsername").textContent = localStorage.getItem("username");
document.getElementById("navRole").textContent = localStorage.getItem("role");

/**
 * Loads and displays all criminal files in the table.
 * REQ-51: View file content.
 * REQ-54: Display current status of each file.
 */
async function loadFiles() {
    try {
        const response = await apiFetch(`${API_BASE_URL}/CriminalFile`);
        if (!response) return;

        const files = await response.json();
        renderFiles(files);
    } catch (err) {
        showAlert("Failed to load criminal files.", "danger");
    }
}

/**
 * Renders criminal files into the table.
 * @param {Array} files - Array of criminal file objects.
 */
function renderFiles(files) {
    const tableBody = document.getElementById("filesTableBody");
    tableBody.innerHTML = "";

    if (files.length === 0) {
        tableBody.innerHTML = `<tr><td colspan="7" class="text-center text-muted">No files found.</td></tr>`;
        return;
    }

    files.forEach(file => {
        const row = `
            <tr>
                <td>${file.id}</td>
                <td>${file.title ?? "-"}</td>
                <td>
                    <!-- REQ-50: Files grouped by category -->
                    <span class="badge bg-info text-dark">${file.category ?? "-"}</span>
                </td>
                <td>
                    <!-- REQ-54: Display current status -->
                    <span class="badge ${getStatusBadge(file.status)}">${file.status ?? "-"}</span>
                </td>
                <td>${file.policeStationName ?? file.policeStationId}</td>
                <td>${file.agentName ?? "-"}</td>
                <td>
                    <!-- REQ-56: View version history -->
                    <button class="btn btn-sm btn-info me-1" onclick="viewHistory(${file.id})">History</button>
                    <!-- REQ-55: Delete requires authorization - Admin only -->
                    <button class="btn btn-sm btn-danger" onclick="deleteFile(${file.id})">Delete</button>
                </td>
            </tr>
        `;
        tableBody.innerHTML += row;
    });
}

/**
 * Returns Bootstrap badge class based on file status.
 * @param {string} status - The file status.
 * @returns {string} Bootstrap badge class.
 */
function getStatusBadge(status) {
    const statusMap = {
        "Open": "bg-success",
        "Closed": "bg-secondary",
        "Pending": "bg-warning text-dark",
        "Archived": "bg-dark"
    };
    return statusMap[status] ?? "bg-secondary";
}

/**
 * REQ-57, REQ-58, REQ-59: Search files by name, agent or category.
 * Filters the displayed files based on search input.
 */
async function searchFiles() {
    const searchTerm = document.getElementById("searchInput").value.trim().toLowerCase();
    const categoryFilter = document.getElementById("categoryFilter").value;

    try {
        const response = await apiFetch(`${API_BASE_URL}/CriminalFile`);
        if (!response) return;

        let files = await response.json();

        // REQ-57: Filter by file name
        // REQ-58: Filter by agent name
        if (searchTerm) {
            files = files.filter(f =>
                (f.title ?? "").toLowerCase().includes(searchTerm) ||
                (f.agentName ?? "").toLowerCase().includes(searchTerm)
            );
        }

        // REQ-59: Filter by category
        if (categoryFilter) {
            files = files.filter(f => f.category === categoryFilter);
        }

        renderFiles(files);
    } catch (err) {
        showAlert("Failed to search files.", "danger");
    }
}

/**
 * Creates a new criminal file from form inputs.
 * REQ-49: Create a new criminal activity file.
 */
async function createFile() {
    const title = document.getElementById("fileTitle").value.trim();
    const category = document.getElementById("fileCategory").value;
    const stationId = parseInt(document.getElementById("fileStation").value);
    const agentId = parseInt(document.getElementById("fileAgent").value);

    if (!title || !category || !stationId) {
        showAlert("Title, category and station are required.", "warning");
        return;
    }

    const file = { title, category, status: "Open", policeStationId: stationId, agentId };

    try {
        const response = await apiFetch(`${API_BASE_URL}/CriminalFile`, {
            method: "POST",
            body: JSON.stringify(file)
        });

        if (!response) return;

        if (response.ok) {
            clearForm();
            loadFiles();
            showAlert("Criminal file created successfully.", "success");
        } else {
            const error = await response.text();
            showAlert(error || "Failed to create file.", "danger");
        }
    } catch (err) {
        showAlert("Failed to create file.", "danger");
    }
}

/**
 * Loads version history for a criminal file.
 * REQ-56: Maintain version history for files.
 * @param {number} fileId - The criminal file ID.
 */
async function viewHistory(fileId) {
    try {
        const response = await apiFetch(`${API_BASE_URL}/CriminalFile/${fileId}/history`);
        if (!response) return;

        const history = await response.json();
        const historyBody = document.getElementById("historyTableBody");
        historyBody.innerHTML = "";

        if (history.length === 0) {
            historyBody.innerHTML = `<tr><td colspan="5" class="text-center text-muted">No history found.</td></tr>`;
        } else {
            history.forEach(entry => {
                historyBody.innerHTML += `
                    <tr>
                        <td>${new Date(entry.changedAt).toLocaleString()}</td>
                        <td>${entry.changeType}</td>
                        <td>${entry.status}</td>
                        <td>${entry.category}</td>
                        <td>${entry.changedByUsername}</td>
                    </tr>
                `;
            });
        }

        // Show history modal
        const modal = new bootstrap.Modal(document.getElementById("historyModal"));
        modal.show();
    } catch (err) {
        showAlert("Failed to load file history.", "danger");
    }
}

/**
 * Deletes a criminal file by ID.
 * REQ-55: Prevent deletion without authorization - server validates role.
 * @param {number} id - The file ID to delete.
 */
async function deleteFile(id) {
    if (!confirm("Are you sure you want to delete this file?")) return;

    try {
        const response = await apiFetch(`${API_BASE_URL}/CriminalFile/${id}`, {
            method: "DELETE"
        });

        if (!response) return;

        if (response.ok) {
            loadFiles();
            showAlert("File deleted successfully.", "success");
        } else {
            showAlert("Failed to delete file. You may not have permission.", "danger");
        }
    } catch (err) {
        showAlert("Failed to delete file.", "danger");
    }
}

/**
 * Loads stations and agents into dropdowns for the create form.
 */
async function loadDropdowns() {
    try {
        const [stationsRes, agentsRes] = await Promise.all([
            apiFetch(`${API_BASE_URL}/PoliceStation`),
            apiFetch(`${API_BASE_URL}/Agent`)
        ]);

        if (stationsRes) {
            const stations = await stationsRes.json();
            const stationSelect = document.getElementById("fileStation");
            stationSelect.innerHTML = '<option value="">Select station...</option>';
            stations.forEach(s => {
                stationSelect.innerHTML += `<option value="${s.id}">${s.name}</option>`;
            });
        }

        if (agentsRes) {
            const agents = await agentsRes.json();
            const agentSelect = document.getElementById("fileAgent");
            agentSelect.innerHTML = '<option value="">Select agent...</option>';
            agents.forEach(a => {
                agentSelect.innerHTML += `<option value="${a.id}">${a.firstName} ${a.lastName}</option>`;
            });
        }
    } catch (err) {
        showAlert("Failed to load dropdowns.", "danger");
    }
}

/**
 * Clears the create file form inputs.
 */
function clearForm() {
    document.getElementById("fileTitle").value = "";
    document.getElementById("fileCategory").value = "";
    document.getElementById("fileStation").value = "";
    document.getElementById("fileAgent").value = "";
}

/**
 * Displays a dismissible alert message on the page.
 * @param {string} message - The message to display.
 * @param {string} type - Bootstrap alert type (success, danger, warning).
 */
function showAlert(message, type) {
    const alertDiv = document.getElementById("alertBox");
    alertDiv.innerHTML = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
}

/** Initialize page on load. */
window.onload = function () {
    loadFiles();
    loadDropdowns();
};