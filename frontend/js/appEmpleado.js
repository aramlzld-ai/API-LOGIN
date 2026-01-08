msjEmpleado = document.getElementById("msjEmpleado");

async function verificarRol() {
    const responseEmpleado = await fetch("http://localhost:5022/empleado", {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem("token")}`
        }
    });
    if (!responseEmpleado.ok){
        msjEmpleado.textContent = "Acceso denegado a la sección de Empleado.";
        msjEmpleado.style.color = "red";
        window.location.href = "index.html";
    }
    const responseData = await fetch("http://localhost:5022/perfil", {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem("token")}`
        }
    });
    const data = await responseData.json();
    msjEmpleado.textContent = `Bienvenido a la sección de Empleado ${data.usuario}.`;
    msjEmpleado.style.color = "green";
}
verificarRol();
async function logout() {
    localStorage.clear();
    window.location.href = "index.html";
}
document.getElementById("logoutButtonEmpleado").addEventListener("click", logout);