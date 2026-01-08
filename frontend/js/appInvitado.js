msjInvitado = document.getElementById("msjInvitado");

async function verificarRol() {
    const responseInvitado = await fetch("http://localhost:5022/invitado", {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem("token")}`
        }
    });
    if (!responseInvitado.ok){
        msjInvitado.textContent = "Acceso denegado a la sección de Invitado.";
        msjInvitado.style.color = "red";
        window.location.href = "index.html";
    }
    const responseData = await fetch("http://localhost:5022/perfil", {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem("token")}`
        }
    });
    const data = await responseData.json();
    msjInvitado.textContent = `Bienvenido a la sección de Invitado ${data.usuario}.`;
    msjInvitado.style.color = "green";
}
verificarRol();
async function logout() {
    localStorage.clear();
    window.location.href = "index.html";
}
document.getElementById("logoutButtonInvitado").addEventListener("click", logout);