/**
 * agents.js
 * Handles all agent-related UI operations.
 * REQ-17: Assign agent to station.
 * REQ-18: Remove agent from station.
 * REQ-19: Define role for each agent.
 * REQ-21: Modify hierarchy relations.
 * REQ-23: Prevent circular subordination.
 * REQ-24: Max one station head per station.
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
 * Loads and displays all agents in the table.
 * REQ-17: Shows agent-station assignments.
 */
async function loadAgents() {
    try {
        const response = await apiFetch(`${API_BASE_URL}/Agent`);
        if (!response) return;

        const agents = await response.json();
        const tableBody = document.getElementById("agentsTableBody");
        tableBody.innerHTML = "";

        agents.forEach(agent => {
            const row = `
                <tr>
                    <td>${agent.id}</td>
                    <td>${agent.firstName}</td>
                    <td>${agent.lastName}</td>
                    <td>${agent.badge}</td>
                    <td>${agent.role}</td>
                    <td>${agent.policeStationName ?? agent.policeStationId}</td>
                    <td>${agent.superiorName ?? "-"}</td>
                    <td>
                        <button class="btn btn-sm btn-danger" onclick="deleteAgent(${agent.id})">Delete</button>
                    </td>
                </tr>
            `;
            tableBody.innerHTML += row;
        });
    } catch (err) {
        showAlert("Failed to load agents.", "danger");
    }
}

/**
 * Loads all stations into the station dropdown for the create form.
 */
async function loadStationsDropdown() {
    try {
        const response = await apiFetch(`${API_BASE_URL}/PoliceStation`);
        if (!response) return;

        const stations = await response.json();
        const select = document.getElementById("agentStation");
        select.innerHTML = '<option value="">Select station...</option>';

        stations.forEach(station => {
            select.innerHTML += `<option value="${station.id}">${station.name}</option>`;
        });
    } catch (err) {
        showAlert("Failed to load stations.", "danger");
    }
}

/**
 * Creates a new agent from the form inputs.
 * REQ-17: Assigns agent to a station.
 * REQ-19: Sets role for the agent.
 * REQ-24: Server validates max one station head per station.
 */
async function createAgent() {
    const agent = {
        firstName: document.getElementById("agentFirstName").value.trim(),
        lastName: document.getElementById("agentLastName").value.trim(),
        badge: document.getElementById("agentBadge").value.trim(),
        role: document.getElementById("agentRole").value,
        policeStationId: parseInt(document.getElementById("agentStation").value)
    };

    if (!agent.firstName || !agent.badge || !agent.policeStationId) {
        showAlert("First name, badge and station are required.", "warning");
        return;
    }

    try {
        const response = await apiFetch(`${API_BASE_URL}/Agent`, {
            method: "POST",
            body: JSON.stringify(agent)
        });

        if (!response) return;

        if (response.ok) {
            clearForm();
            loadAgents();
            showAlert("Agent created successfully.", "success");
        } else {
            const error = await response.text();
            showAlert(error || "Failed to create agent.", "danger");
        }
    } catch (err) {
        showAlert("Failed to create agent.", "danger");
    }
}

/**
 * Deletes an agent by ID.
 * REQ-18: Removes agent from station.
 * @param {number} id - The agent ID to delete.
 */
async function deleteAgent(id) {
    if (!confirm("Are you sure you want to delete this agent?")) return;

    try {
        const response = await apiFetch(`${API_BASE_URL}/Agent/${id}`, {
            method: "DELETE"
        });

        if (!response) return;

        if (response.ok) {
            loadAgents();
            showAlert("Agent deleted successfully.", "success");
        } else {
            showAlert("Failed to delete agent.", "danger");
        }
    } catch (err) {
        showAlert("Failed to delete agent.", "danger");
    }
}

/**
 * Clears the create agent form inputs.
 */
function clearForm() {
    document.getElementById("agentFirstName").value = "";
    document.getElementById("agentLastName").value = "";
    document.getElementById("agentBadge").value = "";
    document.getElementById("agentRole").value = "Agent";
    document.getElementById("agentStation").value = "";
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
    loadAgents();
    loadStationsDropdown();
};