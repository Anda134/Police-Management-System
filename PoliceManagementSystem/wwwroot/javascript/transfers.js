async function loadTransfers() {
    const response = await fetch(`${API_BASE_URL}/AgentTransfers`);
    const transfers = await response.json();

    const tableBody = document.getElementById("transfersTableBody");
    tableBody.innerHTML = "";

    transfers.forEach(transfer => {
        const row = `
            <tr>
                <td>${transfer.id}</td>
                <td>${transfer.agentId}</td>
                <td>${transfer.fromStationId}</td>
                <td>${transfer.toStationId}</td>
                <td>${transfer.isPermanent}</td>
                <td>${transfer.isApproved}</td>
            </tr>
        `;
        tableBody.innerHTML += row;
    });
}

window.onload = loadTransfers;