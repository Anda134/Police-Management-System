/**
 * stations.js
 * Handles all police station UI operations.
 * REQ-1: Create a new police station.
 * REQ-2: Edit an existing police station.
 * REQ-3: Delete a police station.
 * REQ-4: Specify geographical coordinates.
 * REQ-6: Validate unique station identifier.
 * REQ-7: Validate geographical position before saving.
 * REQ-8: Store name, address and geographical position.
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
 * Loads and displays all police stations in the table.
 * REQ-9: All stations displayed on the page.
 */
async function loadStations() {
    try {
        const response = await apiFetch(`${API_BASE_URL}/PoliceStation`);
        if (!response) return;

        const stations = await response.json();
        const tableBody = document.getElementById("stationsTableBody");
        tableBody.innerHTML = "";

        stations.forEach(station => {
            const row = `
                <tr>
                    <td>${station.id}</td>
                    <td>${station.name}</td>
                    <td>${station.address ?? "-"}</td>
                    <td>${station.latitude}</td>
                    <td>${station.longitude}</td>
                    <td>
                        <button class="btn btn-sm btn-danger" onclick="deleteStation(${station.id})">Delete</button>
                    </td>
                </tr>
            `;
            tableBody.innerHTML += row;
        });
    } catch (err) {
        showAlert("Failed to load stations.", "danger");
    }
}

/**
 * Creates a new police station from the form inputs.
 * REQ-1: Chief Inspector can create a new station.
 * REQ-7: Validates that coordinates are provided before saving.
 * REQ-8: Stores name, address and geographical position.
 */
async function createStation() {
    const name = document.getElementById("stationName").value.trim();
    const address = document.getElementById("stationAddress").value.trim();
    const latitude = document.getElementById("stationLatitude").value.trim();
    const longitude = document.getElementById("stationLongitude").value.trim();

    // REQ-7: Validate geographical position before saving
    if (!name || !latitude || !longitude) {
        showAlert("Name, latitude and longitude are required.", "warning");
        return;
    }

    const station = {
        name: name,
        address: address,
        latitude: parseFloat(latitude),
        longitude: parseFloat(longitude)
    };

    try {
        const response = await apiFetch(`${API_BASE_URL}/PoliceStation`, {
            method: "POST",
            body: JSON.stringify(station)
        });

        if (!response) return;

        if (response.ok) {
            clearForm();
            loadStations();
            showAlert("Station created successfully.", "success");
        } else {
            const error = await response.text();
            showAlert(error || "Failed to create station.", "danger");
        }
    } catch (err) {
        showAlert("Failed to create station.", "danger");
    }
}

/**
 * Deletes a police station by ID.
 * REQ-3: Chief Inspector can delete a station.
 * @param {number} id - The station ID to delete.
 */
async function deleteStation(id) {
    if (!confirm("Are you sure you want to delete this station?")) return;

    try {
        const response = await apiFetch(`${API_BASE_URL}/PoliceStation/${id}`, {
            method: "DELETE"
        });

        if (!response) return;

        if (response.ok) {
            loadStations();
            showAlert("Station deleted successfully.", "success");
        } else {
            showAlert("Failed to delete station.", "danger");
        }
    } catch (err) {
        showAlert("Failed to delete station.", "danger");
    }
}

/**
 * Clears the create station form inputs.
 */
function clearForm() {
    document.getElementById("stationName").value = "";
    document.getElementById("stationAddress").value = "";
    document.getElementById("stationLatitude").value = "";
    document.getElementById("stationLongitude").value = "";
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
    loadStations();
};