const retrievedState = localStorage.getItem('filterStateData');
const retrievedStateData = (JSON.parse(retrievedState));
const retrievedDist = localStorage.getItem('districtData');
const retrievedDistData = (JSON.parse(retrievedDist));
window.onload = () => {
    const request = new XMLHttpRequest();
    request.open("GET", "https://api.covid19india.org/data.json");
    request.send();
    request.onload = () => {
        const { response = {} } = request;
        const data = (JSON.parse(response));
        const { statewise = [] } = data;
        const filterStateData = statewise.map(data => {
            const { state = '', deltaconfirmed = '', deltarecovered = '', deltadeaths = '', active = '', recovered = '', deaths = '' } = data;
            return {
                state,
                deltaconfirmed,
                deltarecovered,
                deltadeaths,
                active,
                recovered,
                deaths
            };
        });
        localStorage.setItem('filterStateData', JSON.stringify(filterStateData));
        parentElement = document.getElementById('allData');
        if (request.status === 200) {
            // console.log(JSON.parse(request.response));
            const table = document.createElement("table");
            table.className = "zxTable";
            const thead = document.createElement("thead");
            thead.className = 'head_row';
            const tbody = document.createElement("tbody");
            const headRow = document.createElement("tr");
            headRow.className = 'head_row';
            ['State', 'Today Total', 'Today Recovered', 'Today Death', 'Total Active', 'Total Recovered', 'Total Death'].forEach(function (el) {
                const th = document.createElement("th");
                th.className = 'column_head'
                th.appendChild(document.createTextNode(el));
                headRow.appendChild(th);
            });
            thead.appendChild(headRow);
            table.appendChild(thead);

            filterStateData.forEach(function (el) {
                const tr = document.createElement("tr");
                tr.className = 'table_row';
                for (let o in el) {
                    const td = document.createElement("td");
                    td.className = 'table_column';
                    td.appendChild(document.createTextNode(el[o]))
                    tr.appendChild(td);
                }
                tbody.appendChild(tr);
            });
            table.appendChild(tbody);
            appendChildElement = parentElement.appendChild(table)
            openPopup();
        }
        else {
            parentElement.innerHTML = 'Server down'
            console.log(`error ${request.status} ${request.statusText}`);
        }
    };

    const districtRequest = new XMLHttpRequest();
    districtRequest.open("GET", "https://api.covid19india.org/state_district_wise.json");
    districtRequest.send();
    districtRequest.onload = () => {
        const { response = {} } = districtRequest;
        const distData = (JSON.parse(response));
        if (request.status === 200) {
            localStorage.setItem('districtData', JSON.stringify(distData));
        };
    };

    const stateSelect = document.getElementById('state');
    retrievedStateData.map(stateData => {
        const { state = '' } = stateData;
        let stateOption = document.createElement('option');
        stateOption.value = state;
        stateOption.text = state;
        stateSelect.appendChild(stateOption);
    });
    stateSelect.options[0].text = '----------------------------Select State----------------------------';
};

function getDist(event) {
    document.getElementById('emptyState').style.display = 'none';
    const distSelect = document.getElementById('district');
    distSelect.style.display = 'block';
    const selectedState = event.target.value;
    const selectedDistData = retrievedDistData[selectedState] || '';
    const { districtData = {} } = selectedDistData;
    distSelect.innerHTML = null;
    Object.keys(districtData).map((item) => {
        let distOption = document.createElement('option');
        distOption.value = item;
        distOption.text = item;
        distSelect.appendChild(distOption);
    });
};

document.getElementById('closePopup').addEventListener('click', closePopup);
function closePopup() {
    document.getElementById('statePopup').style.display = 'none';
};

function openPopup() {
    document.getElementById('statePopup').style.display = 'flex';
};
document.getElementById('subsBell').addEventListener('click', openPopup);
