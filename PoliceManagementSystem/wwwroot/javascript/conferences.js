/**
 * conferences.js
 * Handles all conference UI operations.
 * REQ-33: Create one-to-one communication sessions.
 * REQ-34: Create multi-station communication sessions.
 * REQ-35: Add multiple participants to a session.
 * REQ-36: Display list of connected participants.
 * REQ-40: Store basic conference metadata.
 * REQ-41: Allow users to submit a conference request.
 * REQ-42: Allow users to specify reason for conference.
 * REQ-43: Allow users to specify a personal callsign.
 * REQ-44: Allow Chief Inspector to specify date and time.
 * REQ-45: Assign highest priority to Chief Inspector requests.
 * REQ-46: Resolve scheduling conflicts based on priority.
 * REQ-47: Notify participants of scheduled conferences.
 * REQ-48: Automatically start conference at scheduled time.
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
 * Loads and displays all conferences in the table.
 * REQ-40: Shows conference metadata.
 * REQ-47: Shows scheduled conferences to participants.
 */
async function loadConferences() {
    try {
        const response = await apiFetch(`${API_BASE_URL}/Conference`);
        if (!response) return;

        const conferences = await response.json();
        const tableBody = document.getElementById("conferencesTableBody");
        tableBody.innerHTML = "";

        if (conferences.length === 0) {
            tableBody.innerHTML = `<tr><td colspan="6" class="text-center text-muted">No conferences found.</td></tr>`;
            return;
        }

        conferences.forEach(conference => {
            // REQ-44: Display scheduled time
            const scheduledTime = conference.scheduledAt
                ? new Date(conference.scheduledAt).toLocaleString()
                : "-";

            // REQ-48: Check if conference is upcoming or past
            const isUpcoming = conference.scheduledAt
                && new Date(conference.scheduledAt) > new Date();

            const row = `
                <tr>
                    <td>${conference.id}</td>
                    <td>${conference.reason ?? "-"}</td>
                    <td>${conference.organizerName ?? conference.organizerId}</td>
                    <td>${scheduledTime}</td>
                    <td>
                        <!-- REQ-48: Show if conference is upcoming or past -->
                        <span class="badge ${isUpcoming ? 'bg-success' : 'bg-secondary'}">
                            ${isUpcoming ? 'Upcoming' : 'Past'}
                        </span>
                    </td>
                    <td>
                        <button class="btn btn-sm btn-info me-1" onclick="viewParticipants(${conference.id})">
                            Participants
                        </button>
                        <button class="btn btn-sm btn-danger" onclick="deleteConference(${conference.id})">
                            Delete
                        </button>
                    </td>
                </tr>
            `;
            tableBody.innerHTML += row;
        });
    } catch (err) {
        showAlert("Failed to load conferences.", "danger");
    }
}

/**
 * Creates a new conference from form inputs.
 * REQ-41: Submit a conference request.
 * REQ-42: Specify reason for conference.
 * REQ-43: Specify personal callsign.
 * REQ-44: Chief Inspector specifies date and time.
 * REQ-45: Chief Inspector requests get highest priority.
 */
async function createConference() {
    const reason = document.getElementById("conferenceReason").value.trim();
    const callsign = document.getElementById("conferenceCallsign").value.trim();
    const scheduledAt = document.getElementById("conferenceTime").value;
    const organizerId = parseInt(document.getElementById("conferenceOrganizer").value);

    if (!reason || !scheduledAt || !organizerId) {
        showAlert("Reason, scheduled time and organizer are required.", "warning");
        return;
    }

    if (!callsign) {
        showAlert("Callsign is required.", "warning");
        return;
    }

    const conference = {
        reason,
        callsign,
        scheduledAt: new Date(scheduledAt).toISOString(),
        organizerId,
        priority: 1,
        participantIds: []
    };

    try {
        const response = await apiFetch(`${API_BASE_URL}/Conference`, {
            method: "POST",
            body: JSON.stringify(conference)
        });

        if (!response) return;

        if (response.ok) {
            clearForm();
            loadConferences();
            showAlert("Conference created successfully.", "success");
        } else {
            const error = await response.text();
            showAlert(error || "Failed to create conference.", "danger");
        }
    } catch (err) {
        showAlert("Failed to create conference.", "danger");
    }
}

/**
 * Views participants for a conference.
 * REQ-36: Display list of connected participants.
 * @param {number} conferenceId - The conference ID.
 */
async function viewParticipants(conferenceId) {
    try {
        const response = await apiFetch(`${API_BASE_URL}/Conference/${conferenceId}`);
        if (!response) return;

        const conference = await response.json();
        const participantsList = document.getElementById("participantsList");
        participantsList.innerHTML = "";

        if (!conference.participantNames || conference.participantNames.length === 0) {
            participantsList.innerHTML = `<li class="list-group-item text-muted">No participants yet.</li>`;
        } else {
            // REQ-36: Display all participants
            conference.participantNames.forEach(name => {
                participantsList.innerHTML += `
                    <li class="list-group-item">
                        ?? ${name}
                    </li>
                `;
            });
        }

        document.getElementById("participantsModalTitle").textContent =
            `Participants - ${conference.reason}`;

        const modal = new bootstrap.Modal(document.getElementById("participantsModal"));
        modal.show();
    } catch (err) {
        showAlert("Failed to load participants.", "danger");
    }
}

/**
 * Deletes a conference by ID.
 * @param {number} id - The conference ID to delete.
 */
async function deleteConference(id) {
    if (!confirm("Are you sure you want to delete this conference?")) return;

    try {
        const response = await apiFetch(`${API_BASE_URL}/Conference/${id}`, {
            method: "DELETE"
        });

        if (!response) return;

        if (response.ok) {
            loadConferences();
            showAlert("Conference deleted successfully.", "success");
        } else {
            showAlert("Failed to delete conference.", "danger");
        }
    } catch (err) {
        showAlert("Failed to delete conference.", "danger");
    }
}

/**
 * Loads agents into the organizer dropdown.
 * REQ-44: Chief Inspector can specify conference time.
 * REQ-45: Chief Inspector gets highest priority.
 */
async function loadOrganizers() {
    try {
        const response = await apiFetch(`${API_BASE_URL}/Agent`);
        if (!response) return;

        const agents = await response.json();
        const select = document.getElementById("conferenceOrganizer");
        select.innerHTML = '<option value="">Select organizer...</option>';

        agents.forEach(a => {
            select.innerHTML += `<option value="${a.id}">${a.firstName} ${a.lastName} (${a.role})</option>`;
        });
    } catch (err) {
        showAlert("Failed to load organizers.", "danger");
    }
}

/**
 * Clears the create conference form inputs.
 */
function clearForm() {
    document.getElementById("conferenceReason").value = "";
    document.getElementById("conferenceCallsign").value = "";
    document.getElementById("conferenceTime").value = "";
    document.getElementById("conferenceOrganizer").value = "";
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
    loadConferences();
    loadOrganizers();
};