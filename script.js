document.getElementById("logindiv").addEventListener("submit", function (event) {
    event.preventDefault();
    let username = document.getElementById("pilotID").value;
    let password = document.getElementById("accessCode").value;
    let credentialArray = [username, password];
    console.log(credentialArray);
});

function updateClock() {
    const now = new Date();
    let hours = now.getHours();
    let minutes = now.getMinutes();
    let seconds = now.getSeconds();
    console.log(now);
    if (seconds < 10) {
        seconds = "0" + seconds;
    } // padding to seconds
    
    document.getElementById("clock").innerHTML = hours + ":" + minutes + ":" + seconds;
}
setInterval(updateClock, 1000);
updateClock();

document.addEventListener("keydown", function (event) {
    if (event.key === "h") {
        console.log(event.key);
        let forms = document.getElementsByTagName("form");
        for (let form of forms) {
            form.classList.toggle("hidden");
        }
    }
}
);


