window.addEventListener('DOMContentLoaded', event => {

    console.log("site.js carregado com sucesso");

    // Navbar shrink function
    var navbarShrink = function () {
        const navbarCollapsible = document.body.querySelector('#mainNav');
        if (!navbarCollapsible) {
            return;
        }
        if (window.scrollY === 0) {
            navbarCollapsible.classList.remove('navbar-shrink');
        } else {
            navbarCollapsible.classList.add('navbar-shrink');
        }
    };

    // Verificar e ocultar o footer nas páginas que não são a home
    const path = window.location.pathname;
    if (path !== '/' && !path.includes('Index') && path !== '/OqueDoar') {
        var footer = document.getElementById('footer');
        if (footer) {
            footer.style.display = 'none';
        }
    }

    // Redirecionar ao clicar em "Entrar"
    const btnLogin = document.getElementById('btnGeralLogin');
    if (btnLogin) {
        btnLogin.addEventListener('click', function () {
            window.location.href = '/Login';
        });
    }

    // Redirecionar ao clicar em "Cadastrar"
    const btnCadastro = document.getElementById('btnGeralCadastro');
    if (btnCadastro) {
        btnCadastro.addEventListener('click', function () {
            window.location.href = '/TelaCadastro1';
        });
    }

    // Fecha o modal atual ao clicar em links dentro dele
    document.querySelectorAll('.link-geral').forEach(link => {
        link.addEventListener('click', function () {
            var currentModal = bootstrap.Modal.getInstance(document.querySelector('.modal.show'));
            if (currentModal) {
                currentModal.hide();
            }
        });
    });

    // ===== Chat mobile: ativar conversa e voltar =====

    // Abre o chat (ao clicar em um contato)
    document.querySelectorAll(".contato").forEach(function (contato) {
        contato.addEventListener("click", function () {
            document.getElementById("chat-container").classList.add("chat-ativo");
        });
    });

    // Botão de voltar (caso tenha o botão na página)
    const voltarBtn = document.querySelector("[onclick='voltarParaLista()']");
    if (voltarBtn) {
        voltarBtn.addEventListener("click", function () {
            document.getElementById("chat-container").classList.remove("chat-ativo");
        });
    }

});
