$(document).ready(function () {
    const $searchInput = $('#liveSearch');
    const $productList = $('#productList');
    const originalHtml = $productList.html();

    $searchInput.on('input', function () {
        const query = $(this).val().trim().toLowerCase();

        if (query.length < 2) {
            $productList.html(originalHtml);
            return;
        }

        $.get('/Products/LiveSearch', { query: query })
            .done(function (data) {
                if (!data || data.length === 0) {
                    $productList.html('<p class="text-center">Keine Produkte gefunden</p>');
                    return;
                }

                let html = '';

                data.forEach(product => {
                    html += `
                        <div class="col">
                            <div class="card h-100 shadow-sm">
                                <img src="${product.imageUrl}" class="card-img-top" alt="${product.name}" style="height: 200px; object-fit: cover;">
                                <div class="card-body">
                                    <h5 class="card-title">${product.name}</h5>
                                    <p class="card-text text-muted">${product.description}</p>
                                    <p class="card-text fw-bold">${product.price.toFixed(2)} €</p>
                                    <p class="badge bg-primary">${product.categoryName}</p>
                                </div>
                                <div class="card-footer d-flex justify-content-between">
                                    <a href="/Products/Details/${product.id}" class="btn btn-sm btn-outline-primary">Details</a>
                                    <a href="/Products/Edit/${product.id}" class="btn btn-sm btn-outline-warning">Bearbeiten</a>
                                    <a href="/Products/Delete/${product.id}" class="btn btn-sm btn-outline-danger">Löschen</a>
                                </div>
                            </div>
                        </div>
                    `;
                });

                $productList.html(html);
            })
            .fail(function () {
                $productList.html('<p class="text-danger">Fehler beim Laden der Produkte</p>');
            });
    });
});
