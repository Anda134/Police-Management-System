/**
 * transfers.js
 * Handles all agent transfer UI operations.
 * REQ-65: Initiate a temporary transfer of an agent.
 * REQ-66: Initiate a permanent transfer of an agent.
 * REQ-67: Require approval for permanent transfers.
 * REQ-68: Specify start date of a temporary transfer.
 * REQ-69: Specify end date of a temporary transfer.
 * REQ-70: Automatically revert agent after temporary transfer expires.
 * REQ-71: Record the reason for each transfer.
 * REQ-72: Maintain history of all agent transfers.
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
 * Loads and displays all agent transfers in the table.
 * REQ-72: Shows full transfer history.
 */
async function loadTransfers() {
    try {
        const response = await apiFetch(`${API_BASE_URL}/AgentTransfer`);
        if (!response) return;

        const transfers = await response.json();
        const tableBody = document.getElementById("transfersTableBody");
        tableBody.innerHTML = "";

        if (transfers.length === 0) {
            tableBody.innerHTML = `<tr><td colspan="8" class="text-center text-muted">No transfers found.</td></tr>`;
            return;
        }

        transfers.forEach(transfer => {
            const row = `
                <tr>
                    <td>${transfer.id}</td>
                    <td>${transfer.agentName ?? transfer.agentId}</td>
                    <td>${transfer.fromStationName ?? transfer.fromStationId}</td>
                    <td>${transfer.toStationName ?? transfer.toStationId}</td>
                    <td>
                        <!-- REQ-65, REQ-66: Show transfer type -->
                        <span class="badge ${transfer.isPermanent ? 'bg-danger' : 'bg-warning text-dark'}">
                            ${transfer.isPermanent ? 'Permanent' : 'Temporary'}
                        </span>
                    </td>
                    <td>
                        <!-- REQ-67: Show approval status -->
                        <span class="badge ${transfer.isApproved ? 'bg-success' : 'bg-secondary'}">
                            ${transfer.isApproved ? 'Approved' : 'Pending'}
                        </span>
                    </td>
                    <!-- REQ-68, REQ-69: Show start and end dates -->
                    <td>${transfer.startDate ? new Date(transfer.startDate).toLocaleDateString() : "-"}</td>
                    <td>${transfer.endDate ? new Date(transfer.endDate).toLocaleDateString() : "-"}</td>
                    <td>${transfer.reason ?? "-"}</td>
                    <td>
                        <!-- REQ-67: Approve button - ChiefInspector only for permanent transfers -->
                        ${!transfer.isApproved ? `
                            <button class="btn btn-sm btn-success me-1" onclick="approveTransfer(${transfer.id})">
                                Approve
                            </button>` : ""}
                        <button class="btn btn-sm btn-danger" onclick="deleteTransfer(${transfer.id})">
                            Delete
                        </button>
                    </td>
                </tr>
            `;
            tableBody.innerHTML += row;
        });
    } catch (err) {
        showAlert("Failed to load transfers.", "danger");
    }
}

/**
 * Creates a new agent transfer from form inputs.
 * REQ-65: Temporary transfer with start and end date.
 * REQ-66: Permanent transfer requiring approval.
 * REQ-71: Records reason for transfer.
 */
async function createTransfer() {
    const agentId = parseInt(document.getElementById("transferAgent").value);
    const fromStationId = parseInt(document.getElementById("transferFromStation").value);
    const toStationId = parseInt(document.getElementById("transferToStation").value);
    const isPermanent = document.getElementById("transferType").value === "true";
    const startDate = document.getElementById("transferStartDate").value;
    const endDate = document.getElementById("transferEndDate").value;
    const reason = document.getElementById("transferReason").value.trim();

    if (!agentId || !fromStationId || !toStationId) {
        showAlert("Agent, from station and to station are required.", "warning");
        return;
    }

    if (fromStationId === toStationId) {
        showAlert("From station and to station must be different.", "warning");
        return;
    }

    // REQ-68, REQ-69: Temporary transfers require start and end date
    if (!isPermanent && (!startDate || !endDate)) {
        showAlert("Temporary transfers require both start and end date.", "warning");
        return;
    }

    const transfer = {
        agentId,
        fromStationId,
        toStationId,
        isPermanent,
        startDate: startDate || null,
        endDate: endDate || null,
        reason
    };

    try {
        const response = await apiFetch(`${API_BASE_URL}/AgentTransfer`, {
            method: "POST",
            body: JSON.stringify(transfer)
        });

        if (!response) return;

        if (response.ok) {
            clearForm();
            loadTransfers();
            showAlert("Transfer created successfully.", "success");
        } else {
            const error = await response.text();
            showAlert(error || "Failed to create transfer.", "danger");
        }
    } catch (err) {
        showAlert("Failed to create transfer.", "danger");
    }
}

/**
 * Approves a permanent transfer.
 * REQ-67: Permanent transfers require approval from ChiefInspector.
 * @param {number} id - The transfer ID to approve.
 */
async function approveTransfer(id) {
    if (!confirm("Approve this transfer?")) return;

    try {
        const response = await apiFetch(`${API_BASE_URL}/AgentTransfer/${id}/approve`, {
            method: "PATCH"
        });

        if (!response) return;

        if (response.ok) {
            loadTransfers();
            showAlert("Transfer approved successfully.", "success");
        } else {
            showAlert("Failed to approve transfer. You may not have permission.", "danger");
        }
    } catch (err) {
        showAlert("Failed to approve transfer.", "danger");
    }
}

/**
 * Deletes a transfer by ID.
 * @param {number} id - The transfer ID to delete.
 */
async function deleteTransfer(id) {
    if (!confirm("Are you sure you want to delete this transfer?")) return;

    try {
        const response = await apiFetch(`${API_BASE_URL}/AgentTransfer/${id}`, {
            method: "DELETE"
        });

        if (!response) return;

        if (response.ok) {
            loadTransfers();
            showAlert("Transfer deleted successfully.", "success");
        } else {
            showAlert("Failed to delete transfer.", "danger");
        }
    } catch (err) {
        showAlert("Failed to delete transfer.", "danger");
    }
}

/**
 * Loads agents and stations into dropdowns for the create form.
 */
async function loadDropdowns() {
    try {
        const [agentsRes, stationsRes] = await Promise.all([
            apiFetch(`${API_BASE_URL}/Agent`),
            apiFetch(`${API_BASE_URL}/PoliceStation`)
        ]);

        if (agentsRes) {
            const agents = await agentsRes.json();
            const agentSelect = document.getElementById("transferAgent");
            agentSelect.innerHTML = '<option value="">Select agent...</option>';
            agents.forEach(a => {
                agentSelect.innerHTML += `<option value="${a.id}">${a.firstName} ${a.lastName}</option>`;
            });
        }

        if (stationsRes) {
            const stations = await stationsRes.json();
            ["transferFromStation", "transferToStation"].forEach(selectId => {
                const select = document.getElementById(selectId);
                select.innerHTML = '<option value="">Select station...</option>';
                stations.forEach(s => {
                    select.innerHTML += `<option value="${s.id}">${s.name}</option>`;
                });
            });
        }
    } catch (err) {
        showAlert("Failed to load dropdowns.", "danger");
    }
}

/**
 * Shows or hides date fields based on transfer type.
 * REQ-68, REQ-69: Date fields only required for temporary transfers.
 */
function onTransferTypeChange() {
    const isPermanent = document.getElementById("transferType").value === "true";
    const dateFields = document.getElementById("dateFields");
    dateFields.style.display = isPermanent ? "none" : "flex";
}

/**
 * Clears the create transfer form inputs.
 */
function clearForm() {
    document.getElementById("transferAgent").value = "";
    document.getElementById("transferFromStation").value = "";
    document.getElementById("transferToStation").value = "";
    document.getElementById("transferType").value = "false";
    document.getElementById("transferStartDate").value = "";
    document.getElementById("transferEndDate").value = "";
    document.getElementById("transferReason").value = "";
    onTransferTypeChange();
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
    loadTransfers();
    loadDropdowns();
    onTransferTypeChange();
};