function addRow() {
  // Get the table by ID
  var table = document.getElementById("table");
  const className = document.getElementById("className").value;
  const numberOfPeople = document.getElementById("numberOfPeople").value;
  const description = document.getElementById("description").value;

  var row = table.insertRow(-1);
  var cell1 = row.insertCell(0);
  var cell2 = row.insertCell(1);
  var cell3 = row.insertCell(2);
    var cell4 = row.insertCell(3);
    
  cell1.innerHTML = className;
  cell2.innerHTML = numberOfPeople;
  cell3.innerHTML = description;
  cell4.innerHTML = "Delete";
  cell4.className = "delete-btn";

  // Clear the input fields
  document.getElementById("className").value = "";
  document.getElementById("numberOfPeople").value = "";
  document.getElementById("description").value = "";

  // Hover effect
  row.addEventListener("mouseover", function () {
    row.style.backgroundColor = "rgba(255, 130, 0, 0.2)";
  });

  row.addEventListener("mouseout", function () {
    row.style.backgroundColor = "";
  });

  row.addEventListener("dblclick", function () {
    row.remove();
  });

  // click on delete button
  cell4.addEventListener("click", function () {
    row.remove();
  });
}
