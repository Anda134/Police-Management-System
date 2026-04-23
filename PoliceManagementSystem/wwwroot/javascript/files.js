async function loadFiles() {
    const response = await fetch(`${API_BASE_URL}/CriminalFiles`);
    const files = await response.json();

    const tableBody = document.getElementById("filesTableBody");
    tableBody.innerHTML = "";

    files.forEach(file => {
        const row = `
            <tr>
                <td>${file.id}</td>
                <td>${file.title}</td>
                <td>${file.category}</td>
                <td>${file.status}</td>
                <td>${file.policeStationId}</td>
                <td>${file.agentId}</td>
            </tr>
        `;
        tableBody.innerHTML += row;
    });
}

window.onload = loadFiles;