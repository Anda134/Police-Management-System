async function loadStations() {
    const response = await fetch(`${API_BASE_URL}/PoliceStations`);
    const stations = await response.json();

    const tableBody = document.getElementById("stationsTableBody");
    tableBody.innerHTML = "";

    stations.forEach(station => {
        const row = `
            <tr>
                <td>${station.id}</td>
                <td>${station.name}</td>
                <td>${station.address ?? ""}</td>
                <td>${station.latitude}</td>
                <td>${station.longitude}</td>
            </tr>
        `;
        tableBody.innerHTML += row;
    });
}

async function createStation() {
    const station = {
        name: document.getElementById("stationName").value,
        address: document.getElementById("stationAddress").value,
        latitude: parseFloat(document.getElementById("stationLatitude").value),
        longitude: parseFloat(document.getElementById("stationLongitude").value)
    };

    const response = await fetch(`${API_BASE_URL}/PoliceStations`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(station)
    });

    if (response.ok) {
        document.getElementById("stationName").value = "";
        document.getElementById("stationAddress").value = "";
        document.getElementById("stationLatitude").value = "";
        document.getElementById("stationLongitude").value = "";
        loadStations();
    } else {
        alert("Failed to create station.");
    }
}

window.onload = loadStations;