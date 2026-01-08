msjAdmin = document.getElementById("msjAdmin");

async function verificarRol() {
    const responseAdmin = await fetch("http://localhost:5022/admin", {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem("token")}`
        }
    });

    if (!responseAdmin.ok){
        msjAdmin.textContent = "Acceso denegado a la sección de Admin.";
        msjAdmin.style.color = "red";
        window.location.href = "index.html";
    }
    const responseData = await fetch("http://localhost:5022/perfil", {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem("token")}`
        }
    });
    const data = await responseData.json();
    msjAdmin.textContent = `Bienvenido a la sección de Admin ${data.usuario}.`;
    msjAdmin.style.color = "green";
}
verificarRol();
async function logout() {
    localStorage.clear();
    window.location.href = "index.html";
}
document.getElementById("logoutButtonAdmin").addEventListener("click", logout);