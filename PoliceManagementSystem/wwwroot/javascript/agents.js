async function loadAgents() {
    const response = await fetch(`${API_BASE_URL}/Agents`);
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
                <td>${agent.policeStationId}</td>
            </tr>
        `;
        tableBody.innerHTML += row;
    });
}

window.onload = loadAgents;