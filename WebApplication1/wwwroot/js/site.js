document.addEventListener("DOMContentLoaded", () => {
    const toastElement = document.getElementById("demoToast");
    const toastBody = toastElement?.querySelector(".toast-body");
    const toast = toastElement && window.bootstrap ? new bootstrap.Toast(toastElement) : null;

    document.querySelectorAll("[data-demo-action]").forEach((button) => {
        button.addEventListener("click", () => {
            if (toastBody) toastBody.textContent = button.dataset.demoAction;
            toast?.show();
        });
    });

    document.querySelector("[data-sidebar-toggle]")?.addEventListener("click", () => {
        document.getElementById("sidebar")?.classList.toggle("is-open");
    });

    const catalogSearch = document.querySelector("[data-catalog-search]");
    const genreSelect = document.querySelector("[data-catalog-genre]");
    const cards = [...document.querySelectorAll("[data-book-card]")];
    const count = document.querySelector("[data-book-count]");
    const emptyState = document.querySelector("[data-empty-catalog]");
    const filterCatalog = () => {
        const query = catalogSearch?.value.trim().toLocaleLowerCase("lt-LT") ?? "";
        const genre = genreSelect?.value ?? "";
        let visible = 0;
        cards.forEach((card) => {
            const matches = card.dataset.search.toLocaleLowerCase("lt-LT").includes(query) && (!genre || card.dataset.genre === genre);
            card.classList.toggle("d-none", !matches);
            if (matches) visible++;
        });
        if (count) count.textContent = visible;
        emptyState?.classList.toggle("d-none", visible !== 0);
    };
    catalogSearch?.addEventListener("input", filterCatalog);
    genreSelect?.addEventListener("change", filterCatalog);

    document.querySelectorAll("[data-table-search]").forEach((input) => {
        input.addEventListener("input", () => {
            const query = input.value.toLocaleLowerCase("lt-LT");
            document.querySelectorAll("[data-searchable]").forEach((row) => {
                row.classList.toggle("d-none", !row.textContent.toLocaleLowerCase("lt-LT").includes(query));
            });
        });
    });
});
