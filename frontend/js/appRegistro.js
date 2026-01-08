//registro variables-elementos
const registroButton = document.getElementById('registroButton');
const registroMensaje = document.getElementById('msjRegistro');

async function registrarUsuario() {
    const nombre = document.getElementById('usuarioRegistro').value;
    const email = document.getElementById('emailRegistro').value;
    const password = document.getElementById('passwordRegistro').value;
    const edad = document.getElementById('edadRegistro').value;

    if (nombre == "" || email == "" || password == "" || edad == "") {
        registroMensaje.textContent = "Por favor complete todos los campos.";
        registroMensaje.style.color = "red";
        return;
    }
    
    try{
        const response = await fetch("http://localhost:5022/registro",{
            method: "POST",
            headers:{
                "content-type": "application/json",
            },
            body: JSON.stringify({Nombre: nombre, Email: email, Edad: parseInt(edad) ,Password: password}),
        });
        

        if (!response.ok) {
            const data = await response.json();
            registroMensaje.textContent = data.error || "Error en el registro.";
            registroMensaje.style.color = "red";
            return;
        }
        registroMensaje.textContent = "Registro exitoso! Ahora puedes iniciar sesión.";
        registroMensaje.style.color = "green";
        window.location.href = "index.html";
        } catch (error) {
            registroMensaje.textContent = "Error conectando con la API";
            registroMensaje.style.color = "red";
            console.error("Error:", error);
        }
    }

registroButton.addEventListener("click", registrarUsuario);