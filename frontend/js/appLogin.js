//login variables-elementos
const loginButton = document.getElementById('loginButton');
const mensaje = document.getElementById('mensaje');

//funcion para login
async function loginUser() {
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    if (email == "" || password == "") {
        mensaje.textContent = "Por favor complete todos los campos.";
        mensaje.style.color = "red";
        return;
    }

    try {
        const response = await fetch("http://localhost:5022/login",{
            method: "POST",
            headers:{
                "content-type": "application/json"
            },
            body: JSON.stringify({email: email, password: password})
        });
        const data = await response.json();

        if (!response.ok) {
            mensaje.textContent = data.error;
            mensaje.style.color = "red";
            return;
        }
        mensaje.textContent = `Login exitoso!`;
        mensaje.style.color = "green";

        localStorage.setItem("token", data.token);
        localStorage.setItem("rol", data.rol);
        const token = localStorage.getItem("token");
        if (!token) {
                mensaje.textContent = "Por favor inicie sesión.";
                mensaje.style.color = "red";
                return;
            }
        if (data.rol === "Admin") {
            window.location.href = "admin.html";

        } else if (data.rol === "Invitado") {
            window.location.href = "invitado.html";
        } else{
            window.location.href = "empleado.html";
        }
        
    } catch (error) {
        mensaje.textContent = "Error conectando con la API";
        mensaje.style.color = "red";
        console.error("Error:", error);
    }
}

loginButton.addEventListener("click", loginUser);